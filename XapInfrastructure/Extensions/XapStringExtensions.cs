using System;
using System.Text;

namespace Xap.Infrastructure.Extensions {
    public static class XapStringExtensions {
        public static StringBuilder RemoveLast(this StringBuilder sb, string value) {
            if (sb.Length < 1) return sb;
            sb.Remove(sb.ToString().LastIndexOf(value), value.Length);
            return sb;
        }

        public static string Between(this string value, string a, string b) {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1) {
                return "";
            }
            if (posB == -1) {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB) {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a) {
            int posA = value.IndexOf(a);
            if (posA == -1) {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a) {
            int posA = value.LastIndexOf(a);
            if (posA == -1) {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length) {
                return "";
            }
            return value.Substring(adjustedPosA);
        }

        public static string FirstCharToLower(this string str) {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string FirstCharToUpper(this string str) {
            if (String.IsNullOrEmpty(str) || Char.IsUpper(str, 0))
                return str;

            return Char.ToUpperInvariant(str[0]) + str.Substring(1);
        }
    }
}
