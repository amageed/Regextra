# Regextra

Regextra is a small library built to solve problems that are handily addressed by regular expressions.

Currently, the project provides a Passphrase Regex Builder. Future plans include simple template formatting, and methods for common string manipulation scenarios.

## How do I get started?

Check out the [wiki](https://github.com/amageed/Regextra/wiki) and visit the project's [demo site](http://softwareninjaneer.com/regextra) for more examples and a chance to try out some client-side validation. Also, the extensive [test suite](https://github.com/amageed/Regextra/tree/master/src/Tests) might be worth a look.

## Passphrase Regex Builder

A common question I've seen on StackOverflow is how to write code that enforces strong passphrase or password rules. Popular responses tend to tackle the problem by using a regex with look-aheads. I've seen this so much that I decided to have fun writing a solution that allowed people to produce regex patterns that would enforce such rules.

### Example usage

The following code generates a pattern to enforce a password of 8-25 characters that requires at least two lowercase letters in the range of `a-z` and numbers excluding those in the range of `0-4` (i.e., numbers in the `5-9` range are acceptable).

	var builder = PassphraseRegex.With.MinLength(8)
	                                  .MaxLength(25)
	                                  .IncludesRange('a', 'z')
	                                  .WithMinimumOccurrenceOf(2)
	                                  .ExcludesRange(0, 4);
	
	PassphraseRegexResult result = builder.ToRegex();
	
	if (result.IsValid)
	{
	    if (result.Regex.IsMatch(input))
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
    
## What this project is not

This project isn't an attempt to write a fluent API for regular expressions in general. Learning regex is a better option IMO :)

## Core Contributor

  - Ahmad Mageed ([@amageed](http://www.twitter.com/amageed))

<hr />

Regextra is Copyright © 2013-2014 [Ahmad Mageed](http://softwareninjaneer.com) under the [MIT license](https://github.com/amageed/Regextra/blob/master/LICENSE).
