# BusinessRules
Simple Business Rules

1. Create/update Facts from UI
2. Create/update rules for fact from UI
3. Assign a rule group name to rules, to execute set of rules against a Fact
4. Rest based services to execute the rules

Pending Items:

1. Add more test cases

# How to add custom methods

1. All custom methods should be added in BusinessRules.BasicMethodsLibrary class library project
2. All custom methods should be part of class "BasicMethods" and should be in namespace "M". As this is the pattern used in "Core" to identify the custom methods
3. HelperMethod attribute should be present with all custom methods for example
```c#
[HelperMethod("StringConcatenate", 2, "string, string")]
```
First parameter is dummy name, second is number of parameter this function accepts and third is type of parameters this method accepts.

