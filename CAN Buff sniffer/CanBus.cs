using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Diagnostics;

class CanBus
{
    public enum WinkerState
    {
        None = 0,
        LeftWinker = 1,
        RightWinker = 2,
        Both = 3
    };

    class Value
    {
        public DateTime date;
        public object data;

        public Value()
        {
        }

        public Value(DateTime date, object data)
        {
            this.date = date;
            this.data = data;
        }
    }

    enum Source
    {
        SerialPort,
        File,
    }

    string VIN = ""; // status string, now for VIN only
    bool VIN_Ready = false;
    DateTime Time = new DateTime();
    UInt32 Odometer = 0;
    UInt16 RPM;
    float BateryVoltage;
    float Speed;
    float CoolingTemp;
    WinkerState winkers = WinkerState.None;
    WinkerState winkersPrevious = WinkerState.None;
    bool Run = true;
    Source source;
    Thread thread;

    SerialPort port;
    public StreamReader file;

    Ramec ramec;
    string line;

    List<Ramec> listOfFrame = new List<Ramec>();
    List<Value> winkerChangeList = new List<Value>();

    public CanBus()
    {
    }

    public CanBus(StreamReader stream)
    {
        source = Source.File;
        file = stream;
        thread = new Thread(proc);
        thread.Start();
    }

    public CanBus(SerialPort port)
    {
        source = Source.SerialPort;
        thread = new Thread(proc);
        thread.Start();
    }

    public WinkerState GetWinkerStatus()
    {
        return winkers;
    }

    public string GetVIN()
    {
        if (VIN_Ready) return VIN;
        return "";
    }

    public string GetVIN_Blocking()
    {
        while (!VIN_Ready) { }
        return VIN;
    }

    public DateTime GetTime()
    {
        return Time;
    }

    public UInt32 GetOdo()
    {
        return Odometer;
    }

    public UInt16 GetRPM()
    {
        return RPM;
    }

    public float GetBateryVoltage()
    {
        return BateryVoltage;
    }

    public float GetCoolingTemperature()
    {
        return CoolingTemp;
    }

    /*public Value WaitForWinkerChange_Blocking()
    {
        WinkerState state;
        while ((state = GetWinkerStatus()) == WinkerState.None) { }
        return new Value(GetTime(), state);
    }*/

    public void fill()
    {
        long StopBytes = 0;
        var watch = Stopwatch.StartNew();
        long StartBytes = System.GC.GetTotalMemory(true);
        while (true) // work to end of times
        {
            line = file.ReadLine();
            if (line == null)
            {
                Run = false;
                break;
            }
            ramec = new Ramec(line);
            listOfFrame.Add(ramec);
        }
        StopBytes = System.GC.GetTotalMemory(true);
        GC.KeepAlive(listOfFrame);
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        MessageBox.Show("Loaded " + listOfFrame.Count() + " items\n"
            + "Size is " + Math.Round(((long)(StopBytes - StartBytes)) / 1024.0 / 1024.0, 2) + " MB\n"
            + "Loading time is " + elapsedMs / 1000 + " sec", "Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void Test()
    {
        while (true) // work to end of times
        {
            line = file.ReadLine();
            if (line == null)
            {
                Run = false;
                break;
            }
            ramec = new Ramec(line);
            ParseFrame(ramec);
        }
    }

    public void Start()
    {
        Run = true;
    }

    public void Abort()
    {
        Run = false;
        thread.Abort();
    }

    void proc()
    {
        long counter = 0;
        switch (source)
        {
            case Source.File:
                {
                    while (Run) // work to end of times
                    {
                        line = file.ReadLine();
                        if (line == null)
                        {
                            Run = false;
                            break;
                        }
                        ramec = new Ramec(line);
                        counter++;
                        ParseFrame(ramec);
                    }
                    MessageBox.Show("Data načtena. (celkem " + counter + " CAN bus rámců)", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
            case Source.SerialPort:
                {
                    port.NewLine = "\r\n"; // define string of end of line (\r and \n)
                    try
                    {
                        port.Open(); // try to open serial port
                    }
                    catch (Exception ex) // problem with opening serial port
                    {
                        throw new Exception("Problém s otevřením sériového portu. " + ex.Message);
                    }
                    while (Run) // work to end of times
                    {
                        ramec = new Ramec(port.ReadLine());
                        ParseFrame(ramec);
                    }
                    break;
                }
            default:
                break;
        }
        
    }

    public void ParseFrame(Ramec ramec) 
    {
        switch (ramec.ID)
        {
            case 0x470: // doors state
                        // 0x470 00 XX 00 00 00 – kontakty dveří
                {

                    break;
                }
            case 0x02C1: // winkers state
                         // 0x2C1 0X 00 00 00 04 – blinkry
                {
                    winkersPrevious = winkers;
                    if ((ramec.Data[0] & 1) > 1) winkers = WinkerState.LeftWinker;
                    else if ((ramec.Data[0] & 2) > 1) winkers = WinkerState.RightWinker;
                    else if (((ramec.Data[0] & 1) > 1) && ((ramec.Data[0] & 2) > 1)) winkers = WinkerState.Both;
                    else winkers = WinkerState.None;
                    if (winkersPrevious != winkers)
                    {
                        winkerChangeList.Add(new Value(ramec.GetTime(), winkers));
                    }
                    break;
                }
            case 0x0571: // Batery Voltage
                         // 0x571 XX 00 00 00 00 00 – napětí akumulátoru[V]
                {
                    BateryVoltage = (ramec.Data[0] / 2 + 50) / 10;
                    break;
                }
            case 0x351: // Speed
                        // 0x351 00 XX YY 00 00 00 00 00 - rychlost
                {
                    Speed = (ramec.Data[2] * 256 + ramec.Data[1]) / 201;
                    break;
                }
            case 0x359: // Speed
                        //0x359 00 XX YY 00 00 00 00 00 - rychlost
                {
                    Speed = (ramec.Data[2] * 256 + ramec.Data[1]) / 201;
                    break;
                }
            case 0x35B: // RPM, Cooling Temp
                        // 0x35B 00 XX YY ZZ 00 00 00 – otáčky motoru + teplota vody
                {
                    if (ramec.Data[0] != 0x07)
                    {
                        RPM = (UInt16)((ramec.Data[2] * 256 + ramec.Data[1]) / 4);
                        CoolingTemp = ramec.Data[3] - 10;
                    }
                    break;
                }
            case 0x65F: // mh to bude VIN (oh, it's VIN, it need's special work)
                {
                    byte[] pole;
                    switch (ramec.Data[0]) // first part of VIN
                    {
                        case 0:
                            VIN_Ready = false;
                            pole = ramec.Data.Skip(5).Take(3).ToArray(); // take 3 bytes from 6st byte
                            VIN = System.Text.Encoding.Default.GetString(pole);
                            break;
                        case 1:
                            if (VIN.Length >= 3)
                            {
                                pole = ramec.Data.Skip(1).Take(7).ToArray(); // take 7 bytes from firts byte
                                VIN = VIN.Substring(0, 3) + System.Text.Encoding.Default.GetString(pole); // replace midle part of VIN
                            }
                            break;
                        case 2:
                            if (VIN.Length >= (3 + 7))
                            {
                                pole = ramec.Data.Skip(1).Take(7).ToArray(); // the same
                                VIN = VIN.Substring(0, 3 + 7) + System.Text.Encoding.Default.GetString(pole); // replace last part of VIN
                                VIN_Ready = true;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                }
            case 0x065D: // oh, data from Odometer, date (sometimes) and time
                {
                    Odometer = (UInt32)(ramec.Data[1] + ramec.Data[2] * 256 + (ramec.Data[3] & 0x0F) * 256 * 256);
                    int Hour = (ramec.Data[5] & 0xF0) / 16 + (ramec.Data[6] & 1) * 16;
                    int Min = (ramec.Data[6] & 0x7E) / 2;
                    int Sec = (ramec.Data[7] & 0x1F) * 2 + (ramec.Data[6] & 0x80) / 128;
                    int Year = ramec.Data[3] / 128 + (ramec.Data[4] & 0x07) * 128;
                    int Month = (ramec.Data[4] & 0x78) / 8;
                    int Day = (ramec.Data[4] & 0x80) / 128 + (ramec.Data[5] & 0x0F) * 2;
                    
                    if (Year == 0) Year = DateTime.Now.Year; // if date not supported by car the fill today date
                    if (Month == 0) Month = DateTime.Now.Month;
                    if (Day == 0) Day = DateTime.Now.Day;
                    Time = new DateTime(Year, Month, Day, Hour, Min, Sec);
                    break;
                }
            default:
                break;
        }
    }
}
