using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Regextra
{
    public class PasswordRulesBuilder
    {
        private readonly Regex _dashMatcher = new Regex(@"\\?-");
        private IList<string> _rules = new List<string>();
        private int _minLength;
        private int _maxLength;

        public PasswordRulesBuilder ContainsCharacters(string characters)
        {
            // remove all dashes and place one at the end to avoid an unintended range
            if (_dashMatcher.IsMatch(characters))
            {
                characters = _dashMatcher.Replace(characters, "") + "-";
            }
            _rules.Add(String.Format("[{0}]", String.Join("", characters)));
            return this;
        }

        public PasswordRulesBuilder Range(char start, char end)
        {
            // TODO: decide whether to throw an exception if start >= end ... handled by Regex check for now
            _rules.Add(String.Format("[{0}-{1}]", start, end));
            return this;
        }

        public PasswordRulesBuilder MinLength(int length)
        {
            _minLength = length;
            return this;
        }

        public PasswordRulesBuilder MaxLength(int length)
        {
            _maxLength = length;
            return this;
        }

        public string ToPattern()
        {
            if (!_rules.Any())
            {
                return "";
            }

            if (ValidateLength(_minLength, "Minimum") && _minLength == 0)
            {
                _minLength = _rules.Count;
            }
            ValidateLength(_maxLength, "Maximum");

            var rules = String.Join("", _rules.Select(FormatRequirement));
            var builder = new StringBuilder("^" + rules);
            string range = String.Format("{0},{1}",
                               _minLength,
                               (_maxLength > 0 ? _maxLength.ToString() : ""));
            builder.AppendFormat(".{{{0}}}", range);
            builder.Append("$");

            var pattern = builder.ToString();
            // use the regex class to validate the pattern (exception will be thrown if invalid)
            new Regex(pattern);
            return pattern;
        }

        public override string ToString()
        {
            return ToPattern();
        }

        private string FormatRequirement(string input)
        {
            return "(?=.*" + input + ")";
        }

        private bool ValidateLength(int length, string type)
        {
            if (length != 0 && length < _rules.Count)
            {
                throw new ArgumentException(type + " length must be greater than or equal to the number of rules specified.");
            }

            return true;
        }
    }
}
