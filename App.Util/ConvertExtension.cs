using System;
using System.Text;
using System.Globalization;

namespace App.Util
{
    public class ConvertExtension
    {
        public static char ToChar(object obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            {
                return Convert.ToChar(obj.ToString().Trim());
            }

            return new char();
        }

        public static string ToString(object obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            {
                return obj.ToString().Trim();
            }

            return string.Empty;
        }

        public static int ToInteger(object obj)
        {
            int intHelper = 0;
            if (obj == null)
            {
                return 0;
            }

            if (int.TryParse(obj.ToString(), out intHelper))
            {
                return intHelper;
            }

            //SE AGREGA LA COMPATIBILIDAD DEL BOOLENA CON VALORES INT16
            if (string.Compare(ToString(obj), "true", true) == 0)
            {
                return 1;
            }

            return 0;
        }

        public static double ToDouble(object obj)
        {
            double dblHelper = 0;
            if (obj != null && double.TryParse(obj.ToString(), out dblHelper))
            {
                return dblHelper;
            }
            return 0;
        }

        public static decimal ToDecimal(object obj)
        {
            decimal decHelper = 0;
            if (obj != null && decimal.TryParse(obj.ToString(), out decHelper))
            {
                return decHelper;
            }
            return 0;
        }

        public static byte[] ToByteArray(string str)
        {
            string strHelper = ToString(str);
            if (strHelper != string.Empty)
            {
                try
                {
                    var enc = new UTF8Encoding();
                    byte[] binFile = enc.GetBytes(strHelper);
                    return binFile;
                }
                catch
                {
                }
            }

            return new byte[0];
        }

        public static bool ToBoolean(object obj)
        {
            bool bolHelper;
            if (obj == null)
            {
                return false;
            }
            if (bool.TryParse(obj.ToString().ToLower(), out bolHelper))
            {
                return bolHelper;
            }

            //SE AGREGA LA COMPATIBILIDAD DEL BOOLENA CON VALORES INT16
            if (ToString(obj) == "1")
            {
                return true;
            }

            return false;
        }

        public static DateTime ToDateTime(object obj)
        {
            CultureInfo ci = new CultureInfo("es-ES");
            DateTime dtHelper = DateTime.MinValue;
            if (obj != null && DateTime.TryParse(obj.ToString(), ci, DateTimeStyles.None, out dtHelper))
            {
                return dtHelper;
            }

            return DateTime.MinValue;
        }

        public static TimeSpan ToTimeSpam(object obj)
        {
            var tsmHelper = new TimeSpan();
            if (obj != null && TimeSpan.TryParse(obj.ToString(), out tsmHelper))
            {
                return tsmHelper;
            }
            return tsmHelper;
        }

        public static object ToDbSqlValue(object obj)
        {
            if (obj == null)
            {
                return DBNull.Value;
            }

            if (string.Compare("string", obj.GetType().Name, true) == 0)
            {
                if (string.IsNullOrEmpty(obj.ToString()) || obj.ToString().Trim() == "-1")
                {
                    return DBNull.Value;
                }
                return obj;
            }

            if (string.Compare("int32", obj.GetType().Name, true) == 0)
            {
                if (ToNullableInteger(obj) == null || ToNullableInteger(obj) == -1)
                {
                    return DBNull.Value;
                }
                return obj;
            }

            if (string.Compare("int16", obj.GetType().Name, true) == 0)
            {
                if (ToNullableInteger(obj) == null)
                {
                    return DBNull.Value;
                }
                return obj;
            }

            if (string.Compare("decimal", obj.GetType().Name, true) == 0)
            {
                if (ToNullableDecimal(obj) == null)
                {
                    return DBNull.Value;
                }
                return obj;
            }

            if (string.Compare("double", obj.GetType().Name, true) == 0)
            {
                if (ToNullableDouble(obj) == null)
                {
                    return DBNull.Value;
                }
                return obj;
            }

            if (string.Compare("datetime", obj.GetType().Name, true) == 0)
            {
                if (ToDateTime(obj) <= DateTime.MinValue)
                {
                    return DBNull.Value;
                }
                return obj;
            }

            if (string.Compare("timespan", obj.GetType().Name, true) == 0)
            {
                if (ToTimeSpam(obj) <= TimeSpan.MinValue)
                {
                    return DBNull.Value;
                }
                return obj;
            }

            if (string.Compare("boolean", obj.GetType().Name, true) == 0)
            {
                if (ToNullableBoolean(obj) == null)
                {
                    return DBNull.Value;
                }
                return obj;
            }

            return obj;
        }

        public static string ToNullableString(object obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            {
                return obj.ToString().Trim();
            }

            return null;
        }

        public static int? ToNullableInteger(object obj)
        {
            int intHelper = 0;
            if (obj == null)
            {
                return null;
            }

            if (int.TryParse(obj.ToString(), out intHelper))
            {
                return intHelper;
            }

            //SE AGREGA LA COMPATIBILIDAD DEL BOOLENA CON VALORES INT16
            if (ToString(obj).ToLower() == "true")
            {
                return 1;
            }
            if (ToString(obj).ToLower() == "false")
            {
                return 0;
            }

            return null;
        }

        public static double? ToNullableDouble(object obj)
        {
            double dblHelper = 0;
            if (obj != null && double.TryParse(obj.ToString(), out dblHelper))
            {
                return dblHelper;
            }
            return null;
        }

        public static decimal? ToNullableDecimal(object obj)
        {
            decimal decHelper = 0;
            if (obj != null && decimal.TryParse(obj.ToString(), out decHelper))
            {
                return decHelper;
            }
            return null;
        }

        public static bool? ToNullableBoolean(object obj)
        {
            bool bolHelper;
            if (obj == null)
            {
                return null;
            }

            if (bool.TryParse(obj.ToString().ToLower(), out bolHelper))
            {
                return bolHelper;
            }

            //SE AGREGA LA COMPATIBILIDAD DEL BOOLENA CON VALORES INT16
            if (ToString(obj) == "1")
            {
                return true;
            }
            if (ToString(obj) == "0")
            {
                return false;
            }

            return null;
        }

        public static DateTime? ToNullableDateTime(object obj)
        {
            CultureInfo ci = new CultureInfo("es-es");
            DateTime dtHelper = DateTime.MinValue;
            if (obj != null && DateTime.TryParse(obj.ToString(), ci, DateTimeStyles.None, out dtHelper))
            {
                return dtHelper;
            }

            return null;
        }

        public static DateTime? ToNullableDateTimeEnd(object obj)
        {
            CultureInfo ci = new CultureInfo("es-es");
            DateTime dtHelper = DateTime.MinValue;
            if (obj != null && !obj.ToString().Trim().Equals(string.Empty) && DateTime.TryParse(obj.ToString() + " 23:59:59", ci, DateTimeStyles.None, out dtHelper))
            {
                return dtHelper;
            }

            return null;
        }

        public static TimeSpan? ToNullableTimeSpam(object obj)
        {
            var tsmHelper = new TimeSpan();
            if (obj != null && TimeSpan.TryParse(obj.ToString(), out tsmHelper))
            {
                return tsmHelper;
            }
            return null;
        }

        public static string[] ToStringArray(string strValue, char chrSeparator)
        {
            if (string.IsNullOrEmpty(strValue))
            {
                return new string[0];
            }
            return strValue.Split(chrSeparator);
        }

        public static string ToStringBoolean(object obj)
        {
            bool bolHelper;
            if (obj != null && bool.TryParse(obj.ToString().ToLower(), out bolHelper))
            {
                if (bolHelper)
                {
                    return "Si";
                }
            }

            return "No";
        }

        public static string ToStringSplit(object obj, int indice, string separador)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            {
                var s = obj.ToString().Split(new string[] { separador }, StringSplitOptions.RemoveEmptyEntries);
                if (s.Length > indice)
                {
                    return s[indice];
                }
            }

            return string.Empty;
        }
    }
}
