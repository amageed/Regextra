Regextra
========

This is a new side-project for me and it's still in its infancy. Please watch the project and follow me on Twitter ([@amageed](http://www.twitter.com/amageed)).

Password Rules Builder
----------------------

A common question I've seen on StackOverflow is how to write code that enforces strong password rules. Popular responses tend to tackle the problem by using a regex with look-aheads. I've seen this so much that I decided to have fun writing a solution that allowed people to produce regex patterns that would enforce such rules.

Currently I'm working on the ***PasswordRulesBuilder*** class, which provides a fluent API to generate the pattern. For example, the class can produce patterns to enforce:

- 1 lowercase letter
- 1 digit
- min/max length
- character ranges (i.e., `[a-z]`)

Example usage
--------------
The following code generates a pattern to enforce a password of 8-25 characters that requires at least one lowercase letter in the range of `a-z` and at least one number in the range of `0-9`.

    var builder = new PasswordRulesBuilder().MinLength(8)
                                            .MaxLength(25)
                                            .Range('a', 'z')
                                            .Range('0', '9');
    var pattern = builder.ToPattern();
    if (Regex.IsMatch(input, pattern))
    {
        // password meets requirements
    }
    else
    {
        // password is no good
    }

Current rule methods
--------------------

- *MinLength(int length)*
- *MaxLength(int length)*
- *ContainsCharacters(string characters)*
- *ExcludesCharacters(string characters)*
- *IncludesRange(char start, char end)*
- *ExcludesRange(char start, char end)*
- *ToPattern()* - generates the regex pattern based on the specified rules
- *ToString()* - overloaded to call ToPattern()

There are more enhancements in mind... off the top of my head:
- overload the Range method to accept numbers (currently chars only), and/or specify ranges as a single string with a dash "0-9"
- more control over the minimum occurrences required for a rule (i.e., "at least 2 digits")
- providing access to the list of individual rules and their purpose in plain text to be able to intelligently inform a user of what rule they're not satisfying
- better error handling
- more tests
- samples
- documentation, of course
- Nuget package

What this project is not
------------------------
This project isn't an attempt to write a fluent API for regular expressions in general. Learning regex is a better option IMO :)

Regextra is Copyright © 2013 [Ahmad Mageed](http://softwareninjaneer.com) under the [MIT license](https://github.com/amageed/Regextra/blob/master/LICENSE).
