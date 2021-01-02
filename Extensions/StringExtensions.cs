using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Extensions
{
    public static class StringExtensions
    {
        public enum TimestampType
        {
            Testing,
            Batch,
            VBS
        }


        public static string RemoveQuotes(this String str)
        {
            if (str.StartsWith(@"""") && str.EndsWith(@""""))
            {
                str = str.Remove(str.Length - 1);
                str = str.Substring(1);
            }

            return str;
        }

        public static string RemoveBeforeChar(this String str, Char character)
        {
            return str.Substring(str.IndexOf(character) + 1);
        }

        public static string ReplaceTimestamp(this String str, TimestampType timestampType = TimestampType.Testing)
        {
            switch (timestampType)
            {
                case (TimestampType.Batch):
                    return str.Replace("{TIMESTAMP}", "%datetime%");
                case (TimestampType.VBS):
                    return str.Replace("{TIMESTAMP}", @""" + sprintf(""{0:yyyy-MM-dd hh-mm-ss}"", Array(datetime)) + """);
                default:
                    return str.Replace("{TIMESTAMP}", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            }
        }

        public static string ReplaceDate(this String str)
        {
            return str.Replace("%datetime%", "{TIMESTAMP}").Replace(@""" + sprintf(""{0:yyyy-MM-dd hh-mm-ss}"", Array(datetime)) + """, "{TIMESTAMP}");
        }
    }
}
