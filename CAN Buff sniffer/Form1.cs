using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.IO;

namespace CAN_Buff_sniffer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Thread vlakno;
        bool MaBezet = false; // should thread run?
        //List<Ramec> Ramce = new List<Ramec>(); // list for store CAN Bus packets, not used

        UInt16[] Whitelist = new UInt16[] {  }; // array of whitelisted CAN Bus packets
        UInt16[] Blacklist = new UInt16[] { 0x065F }; // array of blacklisted CAN Bus packets

        private void button1_Click(object sender, EventArgs e)
        {
            if (!MaBezet) // is running?
            {
                Properties.Settings.Default.Port = textBox1.Text;
                Properties.Settings.Default.UseWhiteList = UseWhiteList.Checked;
                Properties.Settings.Default.UseBlackList = UseBlackList.Checked;
                Properties.Settings.Default.LogWithTime = LogWithTime.Checked;
                Properties.Settings.Default.LogWithMs = LogWithMs.Checked;
                Properties.Settings.Default.LogOnly = LogOnly.Checked;
                Properties.Settings.Default.Save(); // save all options to Settings
                MaBezet = true; // yes, it should run
                vlakno = new Thread(Vlakno);
                vlakno.Start(); // start thread
                OpenClose.Text = "Close"; // change state of button to "Close"
            }
            else
            {
                vlakno.Abort(); // kill thread
                OpenClose.Text = "Open"; // change state of button to "Close"
                MaBezet = false; // no, it should'n run
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // after start restore all optiong from Settings
            textBox1.Text = Properties.Settings.Default.Port;
            UseWhiteList.Checked = Properties.Settings.Default.UseWhiteList;
            UseBlackList.Checked = Properties.Settings.Default.UseBlackList;
            LogWithTime.Checked = Properties.Settings.Default.LogWithTime;
            LogWithMs.Checked = Properties.Settings.Default.LogWithMs;
            LogOnly.Checked = Properties.Settings.Default.LogOnly;
        }

        string VIN = ""; // status string, now for VIN only
        DateTime Time = new DateTime();
        UInt32 Odometer = 0;
        UInt16 RPM;
        float BateryVoltage;
        float Speed;
        float CoolingTemp;
        bool LeftWinker = false;
        bool RightWinker = false;

        bool FilterInsert(String Radek) // insert with filtering by WhiteList of BlackList if is enabled
        {
            Ramec ramec = new Ramec(Radek); // make new CAN Bus frame
            String retezec = "";
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
                        if ((ramec.Data[0] & 1) > 1) LeftWinker = true;
                        if ((ramec.Data[0] & 2) > 1) RightWinker = true;
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
                                pole = ramec.Data.Skip(5).Take(3).ToArray(); // take 3 bytes from 6st byte
                                VIN = System.Text.Encoding.Default.GetString(pole);
                                break;
                            case 1:
                                pole = ramec.Data.Skip(1).Take(7).ToArray(); // take 7 bytes from firts byte
                                VIN = VIN.Substring(0, 3) + System.Text.Encoding.Default.GetString(pole); // replace midle part of VIN
                                break;
                            case 2:
                                pole = ramec.Data.Skip(1).Take(7).ToArray(); // the same
                                VIN = VIN.Substring(0, 3 + 7) + System.Text.Encoding.Default.GetString(pole); // replace last part of VIN

                                Status.Invoke((MethodInvoker)delegate () // show VIN on GUI
                                {
                                    Status.Text = "VIN: " + VIN + ", odo: " + Odometer + " km, Time: " + Time.ToShortTimeString();
                                });
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
                        if (Year == 0) Year = 2000; // if date not supported by car the fill 01.01.2000
                        if (Month == 0) Month = 1;
                        if (Day == 0) Day = 1;
                        Time = new DateTime(Year, Month, Day, Hour, Min, Sec);
                        Status.Invoke((MethodInvoker)delegate () // show VIN on GUI
                        {
                            Status.Text = "VIN: " + VIN + ", odo: " + Odometer + " km, Time: " + Time.ToLongTimeString();
                        });
                        break;
                    }
                default:
                    break;
            }
            if (SouborRaw != null) // is file avaliable for writing?
            {
                // log all arrived CAN Bus data to file xxxRaw
                if (LogWithTime.Checked) retezec = (LogWithMs.Checked ? ramec.ToStringTimeMs() : ramec.ToStringTime()); // log to file, with time of without
                else retezec = Radek.Replace(" ", "\t");
                SouborRaw.WriteLine(retezec);
                SouborRaw.Flush();
            }
            if ((UseWhiteList.Checked && Whitelist.Contains(ramec.ID)) || // filter by WhiteList
                (UseBlackList.Checked && !Blacklist.Contains(ramec.ID)) || // filter by BlackList
                (!UseWhiteList.Checked && !UseBlackList.Checked)) // no filter selected
            {
                listBox1.Items.Add(ramec); // add item to listBox
                listBox1.SelectedIndex = listBox1.Items.Count - 1; // scroll to last item
                if (SouborFiltered != null) // is file avaliable for writing?
                {
                    // write filter data only
                    if (LogWithTime.Checked) retezec = (LogWithMs.Checked ? ramec.ToStringTimeMs() : ramec.ToStringTime());
                    else retezec = Radek.Replace(" ", "\t");
                    SouborFiltered.WriteLine(retezec);
                    SouborFiltered.Flush();
                }
            }
            return false;
        }

        SerialPort COM;
        StreamWriter SouborRaw;
        StreamWriter SouborFiltered;

        void OpenFiles(String Name) // open Files with user defined name (one for Raw data and second for Filtered data)
        {
            try
            {
                SouborRaw = new StreamWriter(Name + "Raw.csv", true); // append is enabled
            }
            catch (Exception ex)
            { }
            try
            {
                if (!LogOnly.Checked) SouborFiltered = new StreamWriter(Name + ".csv", true); // append is enabled
            }
            catch (Exception ex) // ops, something is wrong
            { }
        }

        void OpenFiles() // open Files with predefined name Data.csv and DataRaw.csv 
        {
            OpenFiles("Data");
        }

        void Vlakno()
        {
            // main work thread
            while (MaBezet)
            {
                try
                {
                    COM = new SerialPort(textBox1.Text); // get user defined COM port name
                    COM.NewLine = "\r\n"; // define string of end of line (\r and \n)
                    try
                    {
                        COM.Open(); // try to open serial port
                    }
                    catch (Exception ex) // problem with opening serial port
                    {
                        MessageBox.Show("Problém s otevřením sériového portu. " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    if (FileName.Text == "") OpenFiles(); // open output log files
                    else OpenFiles(FileName.Text);

                    while (MaBezet)
                    {
                        try
                        {
                            string Radek = COM.ReadLine();
                            if (!LogOnly.Checked)
                            {
                                //Ramce.Add(new Ramec(Radek));
                                listBox1.Invoke((MethodInvoker)delegate () // insert arrived data to GUI from thread
                                {
                                    FilterInsert(Radek);
                                });
                            }
                            else
                            {
                                if (SouborRaw != null) // is file avaliable for writing?
                                {
                                    DateTime now = DateTime.Now;
                                    // log all arrived CAN Bus data to file xxxRaw
                                    Radek = now.ToShortDateString() + " " + now.ToLongTimeString() + "\t" + Radek.Replace(" ", "\t");
                                    SouborRaw.WriteLine(Radek);
                                }
                            }
                        }
                        catch (TimeoutException ex) // no ne line, wait for next time
                        { }
                        catch (Exception ex) // another exception
                        { }
                    }
                }
                catch (ThreadAbortException) // end of thread
                { }
                //catch ()
                catch (Exception ex) // other error
                {
                    string Err = ex.Message + "\r\n\r\n" + ex.StackTrace; // show message with StackTrace
                }
            }
            OpenClose.Invoke((MethodInvoker)delegate () // change state of button
            {
                OpenClose.Text = "Open";
            });
            if ((COM != null) || COM.IsOpen) COM.Close(); // if is serial port open, than close it
            if (SouborRaw != null)
            {
                SouborRaw.Flush();
                SouborRaw.Close(); // if is file used, than close it
            }
            if (SouborFiltered != null)
            {
                SouborFiltered.Flush();
                SouborFiltered.Close(); // if is file used, than close it
                MaBezet = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog SouborName = new OpenFileDialog();
            SouborName.ShowDialog();
            CanBus can = new CanBus(new StreamReader(SouborName.FileName));
        }

        void Test()
        {
            // Few test CAN Bus packets
            String[] StrRamcu = new string[]
            {
                "0x02C3 11",
                "0x060E 08 01",
                "0xFFFF 00 01 02",
                "0x042B 19 01 00 00 00 00",
                "0x0000 00 01 02 03 04 05 06 07",
                "0x065F 00 00 00 00 00 54 4D 42",  // VIN, first part
                "0x065F 01 42 30 30 30 30 30 35", // next part of VIN
                "0x065F 02 32 30 30 30 30 30 30", // and last part of VIN
                "0x065D	F1 2E EE 02 00 D0 1C 09" // 192046 km, 13:14:18
                //"0x065D	5E 62 38 02 00 00 E7 1D"  // 16:51:59

            };
            if (FileName.Text == "") OpenFiles(); // open files
            else OpenFiles(FileName.Text);
            Whitelist = new UInt16[] { 0xFFFF }; // fill WhiteList by 0xFFFF (only 0xFFFF will by listed)
            Blacklist = new UInt16[] { 0x2C3 }; // fill BlackList by 0x2C3 (only 0x2C3 will be ignored)
            /*for (int i = 0; i < 10; i++) // test fill of data
            {*/
                foreach (string Radek in StrRamcu)
                {
                    FilterInsert(Radek);
                    Ramec ramec = new Ramec(Radek);
                    string debug = ramec.ToStringTime();
                    Thread.Sleep(100); // slower, for GUI test
                }
            //}//*/

            //listBox1.Items.Add(new Ramec(Radek));//Ramce.Add(new Ramec(Str));
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog SouborName = new OpenFileDialog();
            SouborName.ShowDialog();
            CanBus can = new CanBus();
            can.file = new StreamReader(SouborName.FileName);
            //can.Test();
            can.fill();
        }
    }
}
