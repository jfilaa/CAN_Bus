using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;

class XMLpoint
{
    struct GPS
    {
        public double Latitude;
        public double Longtitude;
        public Int16 Altitude;
    };

    DateTime time;
    GPS coordinates;

    static GPS ParsePosition(string Retezec)
    {
        //"15.911068 50.349347 0",
        GPS position;
        string[] Array = Retezec.Split(' ');
        position.Altitude = Int16.Parse(Array[2]);                          // nadmořská výška
        position.Latitude = double.Parse(Array[1].Replace(',', '.'), CultureInfo.InvariantCulture);        // zeměpisná šířka
        position.Longtitude = double.Parse(Array[0].Replace(',', '.'), CultureInfo.InvariantCulture);      // zeměpisná délka
        return position;
    }

    static DateTime ParseTime(string Retezec)
    {
        // "2016-01-24T08:07:50Z",
        byte Day = 0;
        byte Month = 0;
        Int16 Year = 0;

        byte Hour = 0;
        byte Minute = 0;
        byte Second = 0;

        Retezec = Retezec.Replace('T', '-').Replace("Z", "");
        string[] Array = Retezec.Split('-');

        Year = Int16.Parse(Array[0]);
        Month = byte.Parse(Array[1]);
        Day = byte.Parse(Array[2]);

        Array = Array[3].Split(':');

        Hour = byte.Parse(Array[0]);
        Minute = byte.Parse(Array[1]);
        Second = byte.Parse(Array[2]);
        return new DateTime(Year, Month, Day, Hour, Minute, Second);
    }

    public XMLpoint(DateTime time, long Latitude, long Longtitude, Int16 Altitude)
    {
        this.time = time;
        this.coordinates.Latitude = Latitude;
        this.coordinates.Longtitude = Longtitude;
        this.coordinates.Altitude = Altitude;
    }

    public XMLpoint(string Time, string Position)
    {
        /*this.time = time;
        this.Latitude = Latitude;
        this.Longtitude = Longtitude;
        this.Altitude = Altitude;*/
        this.time = XMLpoint.ParseTime(Time);
        this.coordinates = XMLpoint.ParsePosition(Position);
    }
}

class GoogleEarth
{

    string[] Casy1 =
        {
            "2016-01-24T08:07:50Z",
            "2016-01-24T08:07:51Z",
            "2016-01-24T08:07:52Z",
            "2016-01-24T08:07:53Z",
            "2016-01-24T08:07:54Z",
            "2016-01-24T08:07:55Z",
            "2016-01-24T08:07:56Z",
            "2016-01-24T08:07:57Z",
            "2016-01-24T08:07:58Z",
        };

    string[] Gps1 =
        {
            "15.911068 50.349347 0",
            "15.911159 50.349256 0",
            "15.911249 50.349167 0",
            "15.911346 50.349084 0",
            "15.911444 50.349004 0",
            "15.911552 50.348933 0",
            "15.911663 50.348865 0",
            "15.911773 50.34880199999999 0",
            "15.911871 50.348751 0",
        };

    string[] Casy2 =
        {
            "2016-01-24T08:28:12Z",
            "2016-01-24T08:28:13Z",
            "2016-01-24T08:28:14Z",
            "2016-01-24T08:28:15Z",
            "2016-01-24T08:28:16Z",
            "2016-01-24T08:28:17Z",
            "2016-01-24T08:28:18Z",
            "2016-01-24T08:28:19Z",
            "2016-01-24T08:28:20Z",
            "2016-01-24T08:28:21Z",
            "2016-01-24T08:28:22Z",
            "2016-01-24T08:28:23Z",
            "2016-01-24T08:28:24Z",
            "2016-01-24T08:28:25Z"
        };

    string[] Gps2 =
        {
            "15.84604 50.21292099999999",
            "15.846169 50.212798",
            "15.846301 50.21267400000001",
            "15.846429 50.21254999999999",
            "15.84655 50.212428",
            "15.84667 50.21231099999999",
            "15.846779 50.212195",
            "15.84688 50.212084",
            "15.846973 50.21197600000001",
            "15.847069 50.211874",
            "15.847154 50.211776",
            "15.847228 50.211681",
            "15.847292 50.21159099999999",
            "15.847352 50.211508",
        };

    public void WriteToFile(string FileName)
    {

        XMLpoint bod = new XMLpoint("2016-01-24T08:07:50Z", "15.911068 50.349347 0");

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.NewLineOnAttributes = true;
        XmlTextWriter writer = new XmlTextWriter(new StreamWriter("out.kml"));
        writer.Formatting = Formatting.Indented;
        writer.WriteStartDocument();
        writer.WriteStartElement("kml");
        writer.WriteAttributeString("xmlns", "http://www.opengis.net/kml/2.2");
        writer.WriteAttributeString("xmlns:gx", "http://www.google.com/kml/ext/2.2");
        writer.WriteAttributeString("xmlns:kml", "http://www.opengis.net/kml/2.2");
        writer.WriteAttributeString("xmlns:atom", "http://www.w3.org/2005/Atom");
        writer.WriteStartElement("Document");
        writer.WriteElementString("name", "Trasa.kml");
        writer.WriteElementString("open", "1");

        writer.WriteStartElement("Style");
        writer.WriteAttributeString("id", "multiTrack_n");
        writer.WriteStartElement("IconStyle");
        writer.WriteStartElement("Icon");
        writer.WriteElementString("href", "http://earth.google.com/images/kml-icons/track-directional/track-0.png");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteStartElement("LineStyle");
        writer.WriteElementString("color", "99ffac59");
        writer.WriteElementString("width", "6");
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteStartElement("Style");
        writer.WriteAttributeString("id", "multiTrack_h");
        writer.WriteStartElement("IconStyle");
        writer.WriteElementString("scale", "1.2");
        writer.WriteStartElement("Icon");
        writer.WriteElementString("href", "http://earth.google.com/images/kml-icons/track-directional/track-0.png");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteStartElement("LineStyle");
        writer.WriteElementString("color", "99ffac59");
        writer.WriteElementString("width", "8");
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteStartElement("Style");
        writer.WriteAttributeString("id", "multiTrack_h0");
        writer.WriteStartElement("IconStyle");
        writer.WriteElementString("scale", "1.2");
        writer.WriteStartElement("Icon");
        writer.WriteElementString("href", "http://earth.google.com/images/kml-icons/track-directional/track-0.png");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteStartElement("LineStyle");
        writer.WriteElementString("color", "990000ff");
        writer.WriteElementString("width", "8");
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteStartElement("StyleMap");
        writer.WriteAttributeString("id", "multiTrack");
        writer.WriteStartElement("Pair");
        writer.WriteElementString("key", "normal");
        writer.WriteElementString("styleUrl", "#multiTrack_n");
        writer.WriteEndElement();
        writer.WriteStartElement("Pair");
        writer.WriteElementString("key", "highlight");
        writer.WriteElementString("styleUrl", "#multiTrack_h");
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteStartElement("Style");
        writer.WriteAttributeString("id", "multiTrack_n0");
        writer.WriteStartElement("IconStyle");
        writer.WriteStartElement("Icon");
        writer.WriteElementString("href", "http://earth.google.com/images/kml-icons/track-directional/track-0.png");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteStartElement("LineStyle");
        writer.WriteElementString("color", "990000ff");
        writer.WriteElementString("width", "6");
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteStartElement("StyleMap");
        writer.WriteAttributeString("id", "multiTrack0");
        writer.WriteStartElement("Pair");
        writer.WriteElementString("key", "normal");
        writer.WriteElementString("styleUrl", "#multiTrack_n0");
        writer.WriteEndElement();
        writer.WriteStartElement("Pair");
        writer.WriteElementString("key", "highlight");
        writer.WriteElementString("styleUrl", "#multiTrack_h0");
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteStartElement("Placemark");
        writer.WriteElementString("name", "Pravá");
        writer.WriteElementString("styleUrl", "#multiTrack0");
        writer.WriteStartElement("gx:Track");
        //writer.WriteElementString("when", "2016-01-24T08:07:50Z");
        foreach (string Retezec in Casy1) writer.WriteElementString("when", Retezec);
        //writer.WriteElementString("gx:coord", "15.911068 50.349347 0");
        foreach (string Retezec in Gps1) writer.WriteElementString("gx:coord", Retezec);
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteStartElement("Placemark");
        writer.WriteElementString("name", "Levá");
        writer.WriteElementString("styleUrl", "#multiTrack");
        writer.WriteStartElement("gx:Track");
        //writer.WriteElementString("when", "2016-01-24T08:28:12Z");
        foreach (string Retezec in Casy2) writer.WriteElementString("when", Retezec);
        //writer.WriteElementString("gx:coord", "15.84604 50.21292099999999");
        foreach (string Retezec in Gps2) writer.WriteElementString("gx:coord", Retezec);
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.WriteString("\r\n");
        writer.Flush();
        writer.Close();
    }
}
