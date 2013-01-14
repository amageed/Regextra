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

There are more enhancements in mind... off the top of my head:
- negation to disallow characters
- more control over the minimum occurrences required for a rule (i.e., "at least 2 digits")
- providing access to the list of individual rules and their purpose in plain text to be able to intelligently inform a user of what rule they're not satisfying
- better error handling
- more tests

What this project is not
------------------------
This project isn't an attempt to write a fluent API for regular expressions in general. Learning regex is a better option IMO :)
