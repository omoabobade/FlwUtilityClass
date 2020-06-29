using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;
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

        public static string TimeStampCode(string prefix = "")
        {
            try
            {
                Thread.Sleep(1);
                string stamp = DateTime.Now.ToString("yyMMddHHmmssffffff");
                long num = long.Parse(stamp);
                return prefix + DecimalToArbitrarySystem(num, 36);
            }
            catch (Exception ex)
            {
                //Log.Announce(ex);
                return "";
            }
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


        public static string SerializeXML(object objectInstance)
        {
            string txt = "";
            try
            {
                var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var serializer = new XmlSerializer(objectInstance.GetType());
                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = false;
                settings.Encoding = new UTF8Encoding(false);
                settings.ConformanceLevel = ConformanceLevel.Document;
                var memoryStream = new MemoryStream();
                using (var writer = XmlWriter.Create(memoryStream, settings))
                {
                    serializer.Serialize(writer, objectInstance, emptyNamepsaces);
                    txt = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
               
            }
            return txt;
        }

        public static string SerializeJSON(object objectInstance)
        {
            return JsonConvert.SerializeObject(objectInstance);
        }


        public static string MobileTo234(string mobile)
        {
            var k = CleanMobile(mobile);
            if (k.StartsWith("234") && k.Length == 13)
            {

            }
            else if (k.StartsWith("0") && k.Length == 11)
            {
                k = "234" + k.Substring(1, 10);
            }
            return k;
        }
        public static string MobileFrom234(string customerMobile)
        {
            customerMobile = CleanMobile(customerMobile);

            if (customerMobile.StartsWith("234"))
            {
                if (customerMobile.Length == 13)
                {
                    customerMobile = "0" + customerMobile.Substring(3, 10);
                }
            }
            return customerMobile;
        }

        public static string CleanMobile(string mobile)
        {
            return mobile.Replace("+", "");
        }

        public static string Hash512(string text)
        {
            string hash = "";
            using (SHA512 alg = new SHA512Managed())
            {
                byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(text));
                hash = HexStrFromBytes(result).ToUpper();
            }
            return hash;
        }

        private static string HexStrFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
