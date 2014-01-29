Regextra
========

This is a new side-project for me and it's still in its infancy. Please watch the project and follow me on Twitter ([@amageed](http://www.twitter.com/amageed)).

Project Demo Site
-----------------
Visit the project's demo site for more examples and a chance to try out some client-side validation: http://softwareninjaneer.com/regextra

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
- maximum consecutive identical characters

Example usage
--------------
The following code generates a pattern to enforce a password of 8-25 characters that requires at least two lowercase letters in the range of `a-z` and numbers excluding those in the range of `0-4` (i.e., numbers in the `5-9` range are acceptable).

    var builder = PassphraseRegex.With.MinLength(8)
                                      .MaxLength(25)
                                      .IncludesRange('a', 'z')
                                      .WithMinimumOccurrenceOf(2)
                                      .ExcludesRange('0', '4');
    var result = builder.ToPattern();
    if (result.IsValid)
    {
        if (Regex.IsMatch(input, result.Pattern))
        {
            // passphrase meets requirements
        }
        else
        {
            // passphrase is no good
        }
    }
    else
    {
        // check the regex parse exception message for the generated pattern
        Console.WriteLine(result.Error);
    }
    
PassphraseRegex Class
----------------------

**Available Methods:**
- *MinLength(int length)*
- *MaxLength(int length)*
- *IncludesAnyCharacters(string characters)*
- *ExcludesCharacters(string characters)*
- *IncludesRange(char start, char end)*
- *ExcludesRange(char start, char end)*
- *MaxConsecutiveIdenticalCharacterOf(int length)* - prevents *n* identical characters, e.g., "aaabc" would fail if the max is 2
- *WithMinimumOccurrenceOf(int length)* - available for positive rules only (i.e., *IncludesAnyCharacters* and *IncludesRange*)
- *ToPattern()* - generates the regex pattern based on the specified rules and returns a *PatternResult*

**Initialization:**

Initialization is achieved via either of the following 3 properties so that you're free to construct the builder with code that flows nicely with the first method you decided to use:

- *That*: e.g. *PassphraseRegex.That.IncludesRange(...)*
- *Which*: e.g. *PassphraseRegex.Which.ExcludesCharacters(...)*
- *With*: e.g. *PassphraseRegex.With.MinLength(...)*

**Possible Exceptions Thrown:**

Certain methods will throw an exception when given invalid inputs.

- *IncludesAnyCharacters*: throws *ArgumentException* when input is null or empty
- *ExcludesCharacters*: same behavior as *IncludesAnyCharacters*
- *WithMinimumOccurrenceOf*: throws *ArgumentOutOfRangeException* when the length is less than 1
- *MaxConsecutiveIdenticalCharacterOf*: throws *ArgumentOutOfRangeException* when the length is less than 2
- *ToPattern*: throws *ArgumentException* if either of the specified min/max lengths are less than the number of rules specified

PatternResult Class Properties
------------------------------

Once you've specified the *PatternRegex* rules and call *ToPattern()*, a *PatternResult* will be returned with the following properties:

- *IsValid*: indicates whether the pattern was successfully generated or not
- *Error*: if the pattern was invalid, this contains the Regex class' exception message
- *Pattern*: the generated pattern to use with the Regex class

What this project is not
------------------------
This project isn't an attempt to write a fluent API for regular expressions in general. Learning regex is a better option IMO :)

Regextra is Copyright © 2013-2014 [Ahmad Mageed](http://softwareninjaneer.com) under the [MIT license](https://github.com/amageed/Regextra/blob/master/LICENSE).
