Regextra
========

This is a new side-project for me and it's still in its infancy. Please watch the project and follow me on Twitter ([@amageed](http://www.twitter.com/amageed)).

Passphrase Regex Builder
------------------------

A common question I've seen on StackOverflow is how to write code that enforces strong passphrase or password rules. Popular responses tend to tackle the problem by using a regex with look-aheads. I've seen this so much that I decided to have fun writing a solution that allowed people to produce regex patterns that would enforce such rules.

Currently I'm working on the ***PassphraseRegex*** class, which provides a fluent API to generate the pattern. For example, the class can produce patterns to enforce:

- at least 1 lowercase letter
- at least 2 digits
- min/max length
- character ranges (i.e., `[a-z]`)
- excluded characters
- excluded ranges

Example usage
--------------
The following code generates a pattern to enforce a password of 8-25 characters that requires at least two lowercase letters in the range of `a-z` and numbers excluding those in the range of `0-4` (i.e., numbers in the `5-9` range are acceptable).

    var builder = PassphraseRegex.With.MinLength(8)
                                      .MaxLength(25)
                                      .IncludesRange('a', 'z')
                                      .WithMinimumOccurrenceOf(2)
                                      .ExcludesRange('0', '4');
    var pattern = builder.ToPattern();
    if (Regex.IsMatch(input, pattern))
    {
        // passphrase meets requirements
    }
    else
    {
        // passphrase is no good
    }

Available methods
-----------------

- *MinLength(int length)*
- *MaxLength(int length)*
- *ContainsCharacters(string characters)*
- *ExcludesCharacters(string characters)*
- *IncludesRange(char start, char end)*
- *ExcludesRange(char start, char end)*
- *WithMinimumOccurrenceOf(int length)* - available for positive rules only (i.e., *ContainsCharacters* and *IncludesRange*)
- *ToPattern()* - generates the regex pattern based on the specified rules
- *ToString()* - overridden to call ToPattern()

Initialization is achieved via either of the following 3 properties so that you're free to construct the builder with code that flows nicely with the first method you decided to use:

- *That* -> e.g. *PassphraseRegex.That.IncludesRange(...)*
- *Which* -> e.g. *PassphraseRegex.Which.ExcludesCharacters(...)*
- *With* -> e.g. *PassphraseRegex.With.MinLength(...)*

Coming Soon
-----------
There are more enhancements in mind... off the top of my head:
- overload the Range method to accept numbers (currently chars only), and/or specify ranges as a single string with a dash "0-9"
- providing access to the list of individual rules and their purpose in plain text to be able to intelligently inform a user of what rule they're not satisfying
- annotate the regex pattern with comments based on each rule's purpose (related to the above point) for use with the *RegexOptions.IgnorePatternWhitespace* option
- better error handling
- more tests
- samples and documentation, of course
- Nuget package

What this project is not
------------------------
This project isn't an attempt to write a fluent API for regular expressions in general. Learning regex is a better option IMO :)

Regextra is Copyright © 2013 [Ahmad Mageed](http://softwareninjaneer.com) under the [MIT license](https://github.com/amageed/Regextra/blob/master/LICENSE).
