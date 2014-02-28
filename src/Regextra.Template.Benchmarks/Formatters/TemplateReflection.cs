using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Regextra
{
    public static class TemplateReflection
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

        public static string TemplateFormat(this string template, object item)
        {
            return Format(template, item);
        }

        public static string Format(string template, object item)
        {
            var result = _templateRegex.Replace(template, m =>
            {
                if (IsBalancedDelimiterCountEven(m))
                {
                    return FormatEscapedToken(m.Value);
                }

                if (IsBalancedDelimiterCountOdd(m))
                {
                    string property = GetPropertyValue(item, m);
                    return FormatOddBalancedToken(m, property);
                }

                if (IsPartiallyDelimited(m))
                {
                    Func<string> propertyValue = () => GetPropertyValue(item, m);
                    return FormatPartiallyDelimitedToken(m, propertyValue);
                }

                return m.Value;
            });

            return result;
        }

        private static string GetPropertyValue(object item, Match m)
        {
            bool hasNestedProperties = m.Groups[PROPERTY].Value.Contains(".");
            return hasNestedProperties ? GetNestedPropertyValue(item, m) : GetSinglePropertyValue(item, m);
        }

        private static string GetSinglePropertyValue(object item, Match m)
        {
            var type = item.GetType();
            var propInfo = type.GetProperty(m.Groups[PROPERTY].Value);
            if (propInfo == null)
            {
                throw new MissingFieldException(type.Name, m.Groups[PROPERTY].Value);
            }

            string property = null;
            if (m.Groups[FORMAT].Value != String.Empty)
            {
                property = FormatProperty(item, m, propInfo);
            }
            else
            {
                property = propInfo.GetValue(item, null).ToString();
            }

            return property;
        }

        private static string GetNestedPropertyValue(object item, Match m)
        {
            string[] properties = m.Groups[PROPERTY].Value.Split('.');
            object parent = null;
            object current = item;
            PropertyInfo propInfo = null;

            for (int index = 0; index < properties.Length; index++)
            {
                parent = current;
                propInfo = current.GetType().GetProperty(properties[index]);

                if (propInfo == null)
                {
                    var chain = GetFailedPropertyChain(properties, index);
                    throw new MissingFieldException(chain.Item1, chain.Item2);
                }

                current = propInfo.GetValue(current, null);
            }

            if (m.Groups[FORMAT].Value != String.Empty)
            {
                string result = FormatProperty(parent, m, propInfo);
                return result;
            }
            else
            {
                return current.ToString();
            }
        }

        private static string FormatProperty(object item, Match m, PropertyInfo propertyInfo)
        {
            var format = String.Concat("{0:", m.Groups[FORMAT].Value, "}");
            var property = String.Format(format, propertyInfo.GetValue(item, null));
            return property;
        }

        private static Tuple<string, string> GetFailedPropertyChain(string[] properties, int index)
        {
            string parentProperty = index == 0 ? properties[0] : String.Join(".", properties.Take(index));
            string childProperty = index == 0 ? "" : properties[index];
            return Tuple.Create(parentProperty, childProperty);
        }

        private static bool IsBalancedDelimiterCountEven(Match m)
        {
            var isEven = m.Groups[START_DELIMITER].Length % 2 == 0
                && m.Groups[START_DELIMITER].Length == m.Groups[END_DELIMITER].Length;
            return isEven;
        }

        private static bool IsBalancedDelimiterCountOdd(Match m)
        {
            var isOdd = m.Groups[START_DELIMITER].Length % 2 != 0
                && m.Groups[START_DELIMITER].Length == m.Groups[END_DELIMITER].Length;
            return isOdd;
        }

        private static string FormatEscapedToken(string token)
        {
            var result = _escapeTokenRegex.Replace(token, "$1");
            return result;
        }

        private static string FormatOddBalancedToken(Match m, string property)
        {
            // integer division effectively reduces odd/even number of delimiters
            int delimiterCount = m.Groups[START_DELIMITER].Length / 2;

            if (delimiterCount == 0)
            {
                return property;
            }

            var result = String.Concat(new String(START_DELIMITER_CHAR, delimiterCount),
                property,
                new String(END_DELIMITER_CHAR, delimiterCount));

            return result;
        }

        private static bool IsPartiallyDelimited(Match m)
        {
            var isPartiallyDelimited = m.Groups[START_DELIMITER].Length != m.Groups[END_DELIMITER].Length;
            return isPartiallyDelimited;
        }

        private static string FormatPartiallyDelimitedToken(Match m, Func<string> propertyValue)
        {
            /* Logic for partial escaping depends on the delimiter pairs:
             * - even/odd pairs = Reduce even sets of delimiters, retain property name
             * e.g., {{Name} => {Name}
             * e.g., {Name}} => {Name}
             * 
             * - odd pairs = Reduce even sets of delimiters, use property value
             * e.g., {{{Name} => {Ahmad
             * e.g., {Name}}} => Ahmad}
             */

            var isOddPair = m.Groups[START_DELIMITER].Length % 2 != 0 && m.Groups[END_DELIMITER].Length % 2 != 0;

            var result = String.Concat(TrimPartialDelimiters(isOddPair, START_DELIMITER_CHAR, m.Groups[START_DELIMITER].Length),
                isOddPair ? propertyValue() : m.Groups[PROPERTY].Value,
                TrimPartialDelimiters(isOddPair, END_DELIMITER_CHAR, m.Groups[END_DELIMITER].Length));
            return result;
        }

        private static string TrimPartialDelimiters(bool isOddPair, char delimiterChar, int length)
        {
            if (length == 1)
            {
                return isOddPair ? String.Empty : delimiterChar.ToString();
            }

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
