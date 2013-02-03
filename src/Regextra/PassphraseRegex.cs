using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regextra
{
    public static class PassphraseRegex
    {
        public static IPassphraseRegex That
        {
            get { return new PassphraseRegexBuilder(); }
        }

        public static IPassphraseRegex Which
        {
            get { return new PassphraseRegexBuilder(); }
        }

        public static IPassphraseRegex With
        {
            get { return new PassphraseRegexBuilder(); }
        }
    }

    internal class PassphraseRegexBuilder : IPassphraseRegex, IPassphraseRegexOptions
    {
        private readonly Regex _dashMatcher = new Regex(@"\\?-");
        private IList<IRule> _rules = new List<IRule>();
        private string _error;
        private int _maxConsecutiveIdenticalCharacter;
        private int _minLength;
        private int _maxLength;

        public IPassphraseRegexOptions IncludesAnyCharacters(string characters)
        {
            CharactersRule<Rule>(characters, chars => new Rule(chars));
            return this;
        }

        public IPassphraseRegex ExcludesCharacters(string characters)
        {
            CharactersRule<NegativeRule>(characters, chars => new NegativeRule(chars));
            return this;
        }

        private void CharactersRule<T>(string characters, Func<string, IRule> rule) where T : IRule
        {
            if (String.IsNullOrEmpty(characters)) throw new ArgumentException("Characters should not be null or empty", "characters");
            SanitizeDashes(ref characters);
            _rules.Add(rule(String.Format("[{0}]", String.Join("", characters))));
        }

        public IPassphraseRegexOptions IncludesRange(char start, char end)
        {
            RangeRule<Rule>(start, end, range => new Rule(range));
            return this;
        }

        public IPassphraseRegex ExcludesRange(char start, char end)
        {
            RangeRule<NegativeRule>(start, end, range => new NegativeRule(range));
            return this;
        }

        private void RangeRule<T>(char start, char end, Func<string, IRule> rule) where T : IRule
        {
            // if start >= end it will be handled by the Regex check in ToPattern()
            _rules.Add(rule(String.Format("[{0}-{1}]", start, end)));
        }

        public IPassphraseRegex MaxConsecutiveIdenticalCharacterOf(int length)
        {
            if (length < 2) throw new ArgumentOutOfRangeException("length", "Maximum occurrence must be greater than one.");
            _maxConsecutiveIdenticalCharacter = length;
            return this;
        }

        public IPassphraseRegex MinLength(int length)
        {
            _minLength = length;
            return this;
        }

        public IPassphraseRegex MaxLength(int length)
        {
            _maxLength = length;
            return this;
        }

        public IPassphraseRegex WithMinimumOccurrenceOf(int length)
        {
            if (length < 1) throw new ArgumentOutOfRangeException("length", "Minimum occurrence must be greater than zero.");
            var lastRule = (Rule)_rules.Last();
            lastRule.MinimumOccurrence = length;
            return this;
        }

        public PatternResult ToPattern()
        {
            if (!_rules.Any())
            {
                return new PatternResult("", null);
            }

            ValidateLength(_minLength, "Minimum");
            ValidateLength(_maxLength, "Maximum");
            if (_minLength == 0)
            {
                _minLength = _maxLength > 1 ? 1 : _rules.Count;
            }

            var rules = String.Join("", _rules.Select(r => r.Requirement));
            string maxIdenticalCharPattern = GetMaxIdenticalConsecutiveCharPattern();
            string quantifier = GetPatternQuantifier();
            var pattern = String.Format("^{0}{1}.{2}$", rules, maxIdenticalCharPattern, quantifier);

            PatternResult result;
            try
            {
                // use the regex class to validate the pattern (exception will be thrown if invalid)
                new Regex(pattern);
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
            finally
            {
                result = new PatternResult(pattern, _error);
            }
            return result;
        }

        private string GetMaxIdenticalConsecutiveCharPattern()
        {
            if (_maxConsecutiveIdenticalCharacter == 0)
                return "";
            
            const string pattern = @"(?!.*?(.)\1{{{0}}})";
            return String.Format(pattern, _maxConsecutiveIdenticalCharacter);
        }

        private string GetPatternQuantifier()
        {
            string quantifier = "";
            if (_minLength <= 1 && _maxLength == 0)
            {
                quantifier = "+";
            }
            else if (_minLength > 1 || _maxLength > 1)
            {
                quantifier = String.Format("{{{0},{1}}}",
                                 _minLength,
                                 _maxLength > 0 ? _maxLength.ToString() : "");
            }
            return quantifier;
        }

        private bool ValidateLength(int length, string type)
        {
            if (length != 0 && length < _rules.Count) throw new ArgumentException(type + " length must be greater than or equal to the number of rules specified.");
            return true;
        }

        private void SanitizeDashes(ref string characters)
        {
            // remove all dashes and place one at the end to avoid an unintended range
            if (_dashMatcher.IsMatch(characters))
            {
                characters = _dashMatcher.Replace(characters, "") + "-";
            }
        }
    }
}
