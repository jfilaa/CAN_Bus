using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Globalization;
using System.IO;

namespace Acceleration_test
{
    class GPSReceiver
    {
        public SerialPort GPSPort;

        private int RychlostGPS;
        private DateTime CasGPS;
        private double RychlostGeoid;
        public byte RozdilProtiUTC = 0;
        private poloha Poloha;
        private kvalitaSignaluGPS KvalitaSignaluGPS;

        public enum kvalitaSignaluGPS
        {
            NeurcenaPozice = 0,
            ZjistenaPozice = 1,
            DiferencialiGPS = 3
        }

        public struct poloha
        {
            double Sirka;
            double Delka;
            bool Sever;
            bool Vychod;

            /*public poloha()
            {
                this.Sirka = 0;
                this.Delka = 0;
                this.Sever = true;
                this.Vychod = true;
            }*/

            public poloha(string Retezec) // vstupní formáty 4912.2526N 01635.0378E a N4912.2526 E01635.0378
            {
                if (Retezec.Substring(0, 1) == "N" || Retezec.Substring(0, 1) == "S") //
                {
                    this.Sirka = double.Parse(Retezec.Substring(1, Retezec.IndexOf(" ") - 1).Replace(".", ","));
                    this.Sever = (Retezec.Substring(0, 1)) == "N";
                    Retezec = Retezec.Substring(Retezec.IndexOf(" ") + 1);
                    this.Vychod = (Retezec.Substring(0, 1)) == "E";
                    Retezec = Retezec.Substring(1);
                    this.Delka = double.Parse(Retezec.Substring(1).Replace(".", ","));
                }
                else
                {
                    this.Sirka = double.Parse(Retezec.Substring(0, Retezec.IndexOf(" ") - 1).Replace(".", ","));
                    this.Sever = (Retezec.Substring(Retezec.IndexOf(" ") - 1, 1)) == "N";
                    Retezec = Retezec.Substring(Retezec.IndexOf(" ") + 1, Retezec.Length - (Retezec.IndexOf(" ") + 1));
                    this.Delka = double.Parse(Retezec.Substring(0, Retezec.Length - 1).Replace(".", ","));
                    this.Vychod = (Retezec.Substring(Retezec.Length - 1, 1)) == "E";
                }
            }

            override public string ToString()
            {
                string Retezec;
                if (this.Sever) Retezec="N";
                else  Retezec="S";
                Retezec += this.Sirka+" ";
                if (this.Vychod) Retezec+="E";
                else  Retezec+="W";
                Retezec +=this.Delka;
                return Retezec.Replace(",",".");
            }

            public static implicit operator string(poloha Poloha)
            {
                return Poloha.ToString();
            }
        }

        public void Init()
        {
            GPSPort.ReadTimeout = 1050;
            GPSPort.NewLine = "\r\n"; // "\r";
            // zde se nastartuje vlákno pro čtení dat z GPS
            StopVlaknoGPS = false;
            Thread Cteni = new Thread(VlaknoCteniGPS);
            Cteni.Start();
        }

        public int GetRychlost()
        {
            return RychlostGPS;
        }

        public double GetRychlostGeoid()
        {
            return RychlostGeoid;
        }

        public kvalitaSignaluGPS GetStavSignaluGPS()
        {
            return KvalitaSignaluGPS;
        }

        public poloha GetPoloha()
        {
            return Poloha;
        }

        public DateTime GetCas()
        {
            return CasGPS;
        }

        string PredchoziPozice = "";
        DateTime Zacatek;
        public bool StopVlaknoGPS = false;

        void VlaknoCteniGPS()
        {
            StreamWriter GPSSoubor = new StreamWriter("GPS.txt", true);
            string Radek = "";
            while (!StopVlaknoGPS)
            {
                try
                {
                    Radek = GPSPort.ReadLine();
                }
                catch (TimeoutException)
                {
                    continue;
                }
                catch
                {
                    break;
                }
                string[] Elementy = Radek.Split(',');// Parsuj(Radek,',');
                DateTime Cas = DateTime.Now;
                if (Elementy[0].Contains("$GP"))
                {
                    Elementy[0] = Elementy[0].Replace("$GP", "");
                    switch (Elementy[0])
                    {
                        case "GSA":
                            break;
                        case "RMC":
                            // reálný čas

                            byte Hod, Min, Sec = 0;
                            int mSec = 0;
                            string Retezec = Elementy[1];
                            Hod = (byte)(byte.Parse(Retezec.Substring(0, 2)) + RozdilProtiUTC);
                            Min = byte.Parse(Retezec.Substring(2, 2));
                            Sec = byte.Parse(Retezec.Substring(4, 2));
                            try
                            {
                                mSec = int.Parse(Retezec.Substring(7, 3));
                            }
                            catch (Exception ex)
                            { }
                            //textBox4.Text = Hod + ":" + Min + ":" + Sec + "," + mSec;
                            DateTime Dnes = DateTime.Now;
                            Retezec = Dnes.Year + "-" + ("" + Dnes.Month).PadLeft(2, '0') + "-" + ("" + Dnes.Day).PadLeft(2, '0') + "T" + ("" + Hod).PadLeft(2, '0') +
                                ":" + ("" + Min).PadLeft(2, '0') + ":" + ("" + Sec).PadLeft(2, '0') + "." + ("" + mSec).PadLeft(7, '0') + "Z";//((string)
                            CasGPS = DateTime.ParseExact(Retezec,
                                       "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.AssumeUniversal |
                                       DateTimeStyles.AdjustToUniversal);
                            // rychlost GPS
                            if (Elementy[7] != "")
                            {
                                RychlostGPS = (int)Math.Round(double.Parse(Elementy[7].Replace('.', ',')) * 1.852);
                            }
                            GPSSoubor.WriteLine(Cas.ToShortDateString() + " " + Cas.ToLongTimeString() + "\t" + Radek);

                            break;

                        case "GGA":
                            if (Elementy[2] != "")
                            {
                                string PolohaSTR = Elementy[3] + Elementy[2] + " " + Elementy[5] + Elementy[4];
                                // poloha
                                Poloha = new poloha(PolohaSTR);
                                //string debug = Poloha.ToString();
                                // platnost dat polohy

                                KvalitaSignaluGPS = (kvalitaSignaluGPS)int.Parse(Elementy[6]);
                                // výpočet rychlosti delty polohy

                                if (PredchoziPozice == "")
                                {
                                    PredchoziPozice = PolohaSTR;
                                    Zacatek = DateTime.Now;
                                }
                                else
                                {
                                    TimeSpan delka = DateTime.Now - Zacatek;
                                    string AktualniPozice = PolohaSTR;
                                    double Vzdalenost = VypocetVzdalenosti(PredchoziPozice, AktualniPozice);
                                    RychlostGeoid = (double)(Vzdalenost / delka.TotalSeconds);
                                    PredchoziPozice = PolohaSTR;
                                    Zacatek = DateTime.Now;
                                }
                            }
                            GPSSoubor.WriteLine(Cas.ToShortDateString() + " " + Cas.ToLongTimeString() + "\t" + Radek);
                            break;

                        case "GLL":
                            break;

                        case "GSV":

                            break;

                        case "VTG":

                            break;

                    }
//                    string Debug = Elementy[0];
                }
            }
            GPSSoubor.Flush();
            GPSSoubor.Close();
        }

        //*************************************************************************************************************************
        /// <summary>
        /// Vyzobne z řetězce elementy oddělené odělovacím znakem
        /// </summary>
        /// <param name="Retezec"> Řetězec ze kterého budeme vyzobávat elementy
        /// </param>
        /// <param name="OddelovaciZnak">Znak který jednotlivé elementy odděluje.
        /// </param>
        /// <returns>Vrací pole řetězců obsahující vyzobnuté elementy</returns>
        //*************************************************************************************************************************
        public string[] Parsuj(string Retezec, char OdedelovaciZnak)
        {
            int[] KonecElementu = new int[Retezec.Length];
            // získáme počet elementů (element;element) = počet znaků ;
            int PocetElementu = 0;
            for (int i = 0; i < Retezec.Length; i++)
            {
                if (Retezec[i] == OdedelovaciZnak)
                {
                    KonecElementu[PocetElementu] = i;
                    PocetElementu++;
                }
            }
            string[] Element = new string[PocetElementu + 1];
            // vyzobeme elementy do pole řetězců
            Element[0] = Retezec.Substring(0, KonecElementu[0]);
            for (int i = 1; i < PocetElementu; i++)
            {
                Element[i] = Retezec.Substring(KonecElementu[i - 1] + 1, KonecElementu[i] - KonecElementu[i - 1] - 1);
            };
            Element[PocetElementu] = Retezec.Substring(KonecElementu[PocetElementu - 1] + 1);
            return Element;
        }

        /*//*************************************************************************************************************************
        /// <summary>
        /// Vypočte vzdálenost mezi souřadnicemi GPS.
        /// </summary>
        /// <param name="Poloha1">GPS souřadnice prvníhp bodu. Formát: N=sever, S=jih ddmm.mmmm , 
        /// E=východ, W=západ dddm.mmmm příklad: "N4912.2526 E01635.0378"
        /// </param>
        /// <param name="Poloha2">GPS souřadnice druhého bodu.
        /// </param>
        /// <returns>Vrací vzdálenost mezi dvěma body v km.</returns>
        //*************************************************************************************************************************
        public double VypocetVzdalenosti(string Poloha1, string Poloha2)
        {
            int MinutyN1, MinutyN2, MinutyE1, MinutyE2 = 0;
            double VterinyN1, VterinyN2, VterinyE1, VterinyE2 = 0;
            MinutyN1 = int.Parse(Poloha1.Substring(1, 2).Replace('.', ','));
            MinutyE1 = int.Parse(Poloha1.Substring(Poloha1.IndexOf(" ") + 2, 3).Replace('.', ','));


            MinutyN2 = int.Parse(Poloha2.Substring(1, 2).Replace('.', ','));
            MinutyE2 = int.Parse(Poloha2.Substring(Poloha2.IndexOf(" ") + 2, 3).Replace('.', ','));


            VterinyN1 = double.Parse(Poloha1.Substring(3, Poloha1.IndexOf(" ") - 2).Replace('.', ','));
            string Debug = Poloha1.Substring(14, 7);
            VterinyE1 = double.Parse(Poloha1.Substring(Poloha1.IndexOf(" ") + 2).Replace('.', ','));

            VterinyN2 = double.Parse(Poloha2.Substring(3, Poloha1.IndexOf(" ")-2).Replace('.', ','));
            VterinyE2 = double.Parse(Poloha2.Substring(Poloha2.IndexOf(" ") + 2).Replace('.', ','));
            double N1 = MinutyN1 + (VterinyN1 / 60);
            double E1 = MinutyE1 + (VterinyE1 / 60);
            double N2 = MinutyN2 + (VterinyN2 / 60);
            double E2 = MinutyE2 + (VterinyE2 / 60);

            return 6378.05 * 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(((N1 * Math.PI / 180) - (N2 * Math.PI / 180)) / 2), 2)
                + Math.Cos((N1 * Math.PI / 180)) * Math.Cos((N2 * Math.PI / 180)) * Math.Pow(Math.Sin(((E1 * Math.PI / 180)
                - (E2 * Math.PI / 180)) / 2), 2)));

            //return 6378.05 * 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(((N1 * Math.PI / 180)
            //    - ((MinutyN2 + (VterinyN2 / 60)) * Math.PI / 180)) / 2), 2) + Math.Cos(((MinutyN1 + (VterinyN1 / 60))
            //    * Math.PI / 180)) * Math.Cos(((MinutyN2 + (VterinyN2 / 60)) * Math.PI / 180)) * Math.Pow(Math.Sin((((
            //    MinutyE1 + (VterinyE1 / 60)) * Math.PI / 180) - ((MinutyE2 + (VterinyE2 / 60)) * Math.PI / 180)) / 2), 2)));

        }*/

        //*************************************************************************************************************************
        /// <summary>
        /// Vypočte vzdálenost mezi souřadnicemi GPS.
        /// </summary>
        /// <param name="Poloha1">GPS souřadnice prvníhp bodu. Formát: N=sever, S=jih ddmm.mmmm , 
        /// E=východ, W=západ dddm.mmmm příklad: "N4912.2526 E01635.0378"
        /// </param>
        /// <param name="Poloha2">GPS souřadnice druhého bodu.
        /// </param>
        /// <returns>Vrací vzdálenost mezi dvěma body v km.</returns>
        //*************************************************************************************************************************
        public static double VypocetVzdalenosti(string Poloha1, string Poloha2)
        {
            int MinutyN1, MinutyN2, MinutyE1, MinutyE2 = 0;
            double VterinyN1, VterinyN2, VterinyE1, VterinyE2 = 0;
            MinutyN1 = int.Parse(Poloha1.Substring(1, 2).Replace('.', ','));
            MinutyE1 = int.Parse(Poloha1.Substring(Poloha1.IndexOf(" ") + 2, 3).Replace('.', ','));


            MinutyN2 = int.Parse(Poloha2.Substring(1, 2).Replace('.', ','));
            string MinutyE2String = Poloha2.Substring(Poloha2.IndexOf(" ") + 2, 3).Replace('.', ',');
            MinutyE2 = int.Parse(Poloha2.Substring(Poloha2.IndexOf(" ") + 2, 3).Replace('.', ','));


            VterinyN1 = double.Parse(Poloha1.Substring(3, Poloha1.IndexOf(" ") - 2).Replace('.', ','));
            VterinyE1 = double.Parse(Poloha1.Substring(Poloha1.IndexOf(" ") + 2).Replace('.', ','));

            VterinyN2 = double.Parse(Poloha2.Substring(3, Poloha1.IndexOf(" ") - 2).Replace('.', ','));
            VterinyE2 = double.Parse(Poloha2.Substring(Poloha2.IndexOf(" ") + 2).Replace('.', ','));
            double N1 = MinutyN1 + (VterinyN1 / 60);
            double E1 = MinutyE1 + (VterinyE1 / 60);
            double N2 = MinutyN2 + (VterinyN2 / 60);
            double E2 = MinutyE2 + (VterinyE2 / 60);

            return 6378.05 * 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(((N1 * Math.PI / 180) - (N2 * Math.PI / 180)) / 2), 2)
                + Math.Cos((N1 * Math.PI / 180)) * Math.Cos((N2 * Math.PI / 180)) * Math.Pow(Math.Sin(((E1 * Math.PI / 180)
                - (E2 * Math.PI / 180)) / 2), 2)));

            //return 6378.05 * 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(((N1 * Math.PI / 180)
            //    - ((MinutyN2 + (VterinyN2 / 60)) * Math.PI / 180)) / 2), 2) + Math.Cos(((MinutyN1 + (VterinyN1 / 60))
            //    * Math.PI / 180)) * Math.Cos(((MinutyN2 + (VterinyN2 / 60)) * Math.PI / 180)) * Math.Pow(Math.Sin((((
            //    MinutyE1 + (VterinyE1 / 60)) * Math.PI / 180) - ((MinutyE2 + (VterinyE2 / 60)) * Math.PI / 180)) / 2), 2)));

        }
    }
}
