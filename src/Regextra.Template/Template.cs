using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regextra
{
    public static class Template
    {
        private readonly static string _templatePattern = "(?<StartDelimiter>{+)(?<Property>.+?)(?::(?<Format>.+?))?(?<EndDelimiter>}+)";
        private readonly static string _escapeTokenPattern = @"({|})\1";
        private readonly static Regex _templateRegex = new Regex(_templatePattern, RegexOptions.Compiled);
        private readonly static Regex _escapeTokenRegex = new Regex(_escapeTokenPattern, RegexOptions.Compiled);
        private readonly static char START_DELIMITER_CHAR = '{';
        private readonly static char END_DELIMITER_CHAR = '}';
        private readonly static string START_DELIMITER = "StartDelimiter";
        private readonly static string END_DELIMITER = "EndDelimiter";
        private readonly static string PROPERTY = "Property";
        private readonly static string FORMAT = "Format";

        public static string Format<T>(string template, T item) where T : class
        {
            var type = typeof(T);
            var result = _templateRegex.Replace(template, m =>
            {
                if (IsBalancedEscaped(m))
                    return FormatBalancedEscapedToken(m.Value);

                var propertyInfo = type.GetProperty(m.Groups[PROPERTY].Value);
                if (propertyInfo == null)
                    throw new MissingFieldException(type.Name, m.Groups[PROPERTY].Value);

                string property = null;
                if (m.Groups[FORMAT].Value != String.Empty)
                {
                    var format = "{0:" + m.Groups[FORMAT].Value + "}";
                    property = String.Format(format, propertyInfo.GetValue(item, null));
                }
                else
                {
                    property = propertyInfo.GetValue(item, null).ToString();
                }

                if (IsPartiallyEscaped(m))
                    property = FormatPartiallyEscapedToken(m, property);

                return property;
            });

            return result;
        }

        private static bool IsBalancedEscaped(Match m)
        {
            var result = m.Groups[START_DELIMITER].Length > 1
                && m.Groups[START_DELIMITER].Length % 2 == 0
                && m.Groups[START_DELIMITER].Length == m.Groups[END_DELIMITER].Length;
            return result;
        }

        private static bool IsPartiallyEscaped(Match m)
        {
            var result = (m.Groups[START_DELIMITER].Length > 1 && m.Groups[END_DELIMITER].Length % 2 != 0)
                || (m.Groups[END_DELIMITER].Length > 1 && m.Groups[START_DELIMITER].Length % 2 != 0);
            return result;
        }

        private static string FormatBalancedEscapedToken(string token)
        {
            var result = _escapeTokenRegex.Replace(token, "$1");
            return result;
        }

        private static string FormatPartiallyEscapedToken(Match m, string property)
        {
            var result = String.Concat(TrimPartialDelimiters(START_DELIMITER_CHAR, m.Groups[START_DELIMITER].Length),
                            property,
                            TrimPartialDelimiters(END_DELIMITER_CHAR, m.Groups[END_DELIMITER].Length));
            return result;
        }

        private static string TrimPartialDelimiters(char delimiterChar, int length)
        {
            if (length == 1)
                return String.Empty;

            /* integer division effectively trims odd/even number of delimiters
             * 2 / 2 = 1 delim
             * 3 / 2 = 1 delim
             * 5 / 2 = 2 delims
             */
            var result = new String(delimiterChar, length / 2);
            return result;
        }
    }
}
