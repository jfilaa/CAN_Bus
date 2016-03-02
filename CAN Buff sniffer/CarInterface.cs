using System;
using System.Threading;
using System.IO.Ports;
using System.IO;

class CarInterface
{
    protected DateTime Time;
    protected bool Run = true;
    protected Source source;
    protected Thread thread;

    public StreamWriter LogFile;

    public SerialPort port;
    public StreamReader file;
    protected string line;
    protected float speed;

    public struct Value
    {
        public DateTime date;
        public object data;
    }

    public enum Source
    {
        SerialPort,
        File,
    }

    public DateTime GetTime()
    {
        return Time;
    }

    public float GetSpeed()
    {
        return speed;
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

    public static Value GrepDataString(string Radek)
    {
        // 24.01.2016 8:48:02	0x0439	0B	31	00	00	00	00
        // 24.01.2016 8:48:02:123	0x0439	0B	31	00	00	00	00
        // 0x0439	0B	31	00	00	00	00
        // 16.01.2016 11:44:53	$GPGGA,104500.337,8960.000000,N,00000.000000,E,0,0,,137.000,M,13.000,M,,*44
        // 16.01.2016 11:44:53:123	$GPGGA,104500.337,8960.000000,N,00000.000000,E,0,0,,137.000,M,13.000,M,,*44
        // $GPGGA,104500.337,8960.000000,N,00000.000000,E,0,0,,137.000,M,13.000,M,,*44
        string timeString;
        Value value;
        value.date = new DateTime();
        
        if (Radek.Contains(":"))
        {
            timeString = Radek.Substring(0, Radek.IndexOf("\t"));
            Radek = Radek.Substring(Radek.IndexOf("\t") + 1);
            int ms = 0;
            string[] array = timeString.Split(':');
            if ((array.Length - 1) == 3)
            {
                timeString = timeString.Substring(0, timeString.LastIndexOf(":"));
                int.TryParse(array[3], out ms);
            }
            value.date = DateTime.Parse(timeString);
            value.date = value.date.AddMilliseconds(ms);
            //Time = value.date;
        }
        value.data = Radek;
        return value;
    }
}
