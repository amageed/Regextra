# Regextra

Regextra is a small library built to address problems that are handily solved by regular expressions.

Currently, the library offers a *Passphrase Regex Builder* and *Template Formatting*. Future plans include methods for common string manipulation scenarios.

## How do I get started?

Check out the [wiki](https://github.com/amageed/Regextra/wiki) and visit the project's [demo site](http://softwareninjaneer.com/regextra) for a chance to try out some client-side validation (using the patterns produced by the `PassphraseRegex` builder). Also, the extensive [test suite](https://github.com/amageed/Regextra/tree/master/src/Tests) is worth a glance.

Regextra is available via [NuGet](https://www.nuget.org/packages/Regextra/):

    PM> Install-Package Regextra 

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
    
## Template Formatting

Template formatting allows you to perform named formatting on a string template using an object's matching properties. It's available via the static `Template.Format` method and the string extension method, `FormatTemplate`. The formatter features:

  - Nested Properties
  - Standard/Custom string formatting
  - Escaping of properties
  - Detailed exception messages to pinpoint missing properties
  - Great performance (in part thanks to [FastMember](http://code.google.com/p/fast-member/))

### Example usage

	var order = new
	{
	    Description = "Widget",
	    OrderDate = DateTime.Now,
	    Details = new
	    {
	        UnitPrice = 1500
	    }
	};
	
	string template = "We just shipped your order of '{Description}', placed on {OrderDate:d}. Your {{credit}} card will be billed {Details.UnitPrice:C}.";
	
	string result = Template.Format(template, order);
	// or use the extension: template.FormatTemplate(order);

The result of the code is:

> We just shipped your order of 'Widget', placed on 2/28/2014. Your {credit} card will be billed $1,500.00.

## OSS Libraries Used

Regextra makes use of the following OSS libraries:

  - [FastMember](http://code.google.com/p/fast-member/) ([License](http://www.apache.org/licenses/LICENSE-2.0)). 

## Core Contributor

  - Ahmad Mageed ([@amageed](http://www.twitter.com/amageed))

----------

Regextra is Copyright © 2013-2014 [Ahmad Mageed](http://softwareninjaneer.com) under the [MIT license](https://github.com/amageed/Regextra/blob/master/LICENSE).
