using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {
        public static bool Contains(this string str, string contains, CompareOptions comparison)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(str, contains, comparison) >= 0;
        }

        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison, bool atStartOnly = false)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);

            if (index != 0 && atStartOnly)
                return str;

            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;

                if (atStartOnly)
                    break;

                index = str.IndexOf(oldValue, index, comparison);
            }

            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }
}
