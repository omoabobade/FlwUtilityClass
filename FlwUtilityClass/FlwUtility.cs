using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace FlwUtilityClass
{
    public class FlwUtility
    {
        /** Data Type Conversion**/
        #region
        public static double ToDouble(object v)
        {
            double resp = 0;
            try
            {
                resp = Convert.ToDouble(v);
            }
            catch { }
            return resp;
        }
        public static decimal ToDecimal(string v)
        {
            decimal resp = 0;
            try
            {
                resp = Convert.ToDecimal(v);
            }
            catch { }
            return resp;
        }
        public static int ToInt32(object p)
        {
            int resp = 0;
            try
            {
                resp = Convert.ToInt32(p);
            }
            catch { }
            return resp;
        }

        public static long ToInt64(object p)
        {
            long resp = 0;
            try
            {
                resp = Convert.ToInt64(p);
            }
            catch { }
            return resp;
        }

        public static string ToMoney(double v)
        {
            return v.ToString("C",
                  CultureInfo.CreateSpecificCulture("en-Ng"));
        }
        #endregion


        /** IO **/


        public static bool FileExists(string destFile0)
        {
            return File.Exists(destFile0);
        }

        public static void FileCopy(string destFile0, string destFile1)
        {
            File.Copy(destFile0, destFile1);
        }

        public static void FileDelete(string destFile0)
        {

            File.Delete(destFile0);
        }



        /** Object Manipulation */

        public static string TimeStampCode(DateTime dtm, string prefix = "")
        {
            Thread.Sleep(1);
            string stamp = dtm.ToString("yyMMddHHmmssffffff");
            long num = long.Parse(stamp);
            return prefix + DecimalToArbitrarySystem(num, 36);
        }


        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

        public static string[] Explode(string txt, string delim = ",", int opt = 1)
        {
            string[] arr = null;
            txt = txt.Trim();
            if (txt == "") return arr;
            if (delim.Length != 1) return arr;
            char[] sep = delim.ToCharArray();
            try
            {
                if (opt == 1)
                    arr = txt.Split(sep, StringSplitOptions.None);
                else
                    arr = txt.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                //Log.Write("StringSplit: " + txt + " " + delim + " " + ex.Message);
            }
            return arr;
        }

        public static string ConvertToRegex(string bit)
        {
            //[D:11:11]
            //[R:100:50000]
            //[L:A:Z]

            char[] trima = { '[', ']' };
            var txt = bit.Trim(trima);
            char[] sep = { ':' };

            string[] bits = txt.ToUpper().Split(sep);
            if (bits.Length != 3)
            {
                throw new Exception("Regex build parts is not 3 characters");
            }
            var resp = "";
            var n1 = 0;
            var n2 = 0;
            switch (bits[0])
            {
                case "D":
                    n1 = ToInt32(bits[1]);
                    n2 = ToInt32(bits[2]);
                    resp = @"\d{" + n1 + "," + n2 + "}";
                    break;
                case "R":
                    break;
                case "A":
                    resp = @"\w{" + n1 + "," + n2 + "}";
                    break;
            }
            return resp;
        }

        public static DateTime DefaultDate()
        {
            return new DateTime(1970, 1, 1);
        }

        public static DateTime ToDateTime(string datetime)
        {
            var dtm = DateTime.Now;
            if (!DateTime.TryParse(datetime, out dtm))
            {
                dtm = DefaultDate();
            }
            return dtm;
        }

        public static DateTime CurrentDateTime()
        {
            return DateTime.UtcNow;
        }

        public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new string(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        public static T DeserializeJSON<T>(string objectData)
        {
            return JsonConvert.DeserializeObject<T>(objectData);
        }

        public static T DeserializeXML<T>(string objectData)
        {
            var serializer = new XmlSerializer(typeof(T));
            object result;
            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }
            return (T)result;

        }


    }
}
