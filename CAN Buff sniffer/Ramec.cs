using System;
using System.Text.RegularExpressions;

public class Ramec
{
    /*
    * Class for store CAN Bus packet
    */
    public UInt16 ID; // ID of CAN Bus packet
    byte CTL; // len od Data
    public byte[] Data; // Data
    byte CRC; // CRC, not used
    DateTime Cas; // Time of arrive packet

    public Ramec(string Radek) // parse string from format "0x0000 01 02 03 04\r\n"
    {
        // 24.01.2016 8:48:02	0x0439	0B	31	00	00	00	00
        // 24.01.2016 8:48:02:123	0x0439	0B	31	00	00	00	00
        // 0x0439	0B	31	00	00	00	00
        this.Cas = DateTime.Now; // Now arrived
        Radek = (string)(CarInterface.GrepDataString(Radek)).data;
        this.ID = Convert.ToUInt16(Radek.Substring(Radek.IndexOf("0x") + 2, 4), 16); // get ID, from hex
        Radek = Radek.Substring(7);
        byte i = 0;
        this.CTL = (byte)(Regex.Matches(Radek, " ").Count + 1); // get num of Data bytes
        if (this.CTL == 1) this.CTL = (byte)(Regex.Matches(Radek, "\t").Count + 1); // get num of Data bytes
        this.Data = new byte[this.CTL];
        for (; i < this.CTL; i++) // parse data Bytes from hex
        {
            this.Data[i] = Convert.ToByte(Radek.Substring(i * 3, 2), 16);
        }
    }

    public override string ToString() // override ToString to format "0x0000 XX ..."
    {
        string Retezec = "0x" + this.ID.ToString("X4") + "\t"; // ID of CAN Bus packet
        foreach (byte dato in this.Data) Retezec += dato.ToString("X2") + "\t"; // bin array to Hex (format "0A 0B 00")
        return Retezec.Substring(0, Retezec.Length - 1); // remove last space (tab) from foreach
    }

    public string ToStringTime() // the same as ToString but with time "03.01.2016 18:24:43	0x0571	6F  41  00  01  01  00"
    {
        string Retezec = this.Cas.ToShortDateString() + " " + this.Cas.ToLongTimeString() + "\t0x" + this.ID.ToString("X4") + "\t";
        foreach (byte dato in this.Data) Retezec += dato.ToString("X2") + "\t"; // bin array to Hex (format "0A 0B 00")
        return Retezec.Substring(0, Retezec.Length - 1); // remove last space (tab) from foreach
    }

    public string ToStringTimeMs() // the same as ToString but with time with ms "03.01.2016 18:24:43:122	0x0571	6F  41  00  01  01  00"
    {
        string Retezec = this.Cas.ToShortDateString() + " " + this.Cas.ToLongTimeString() + ":" + this.Cas.Millisecond.ToString("3") + "\t0x" + this.ID.ToString("X4") + "\t";
        foreach (byte dato in this.Data) Retezec += dato.ToString("X2") + "\t"; // bin array to Hex (format "0A 0B 00")
        return Retezec.Substring(0, Retezec.Length - 1); // remove last space (tab) from foreach
    }

    public DateTime GetTime()
    {
        return Cas;
    }
}