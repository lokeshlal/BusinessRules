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
2. All custom methods should be part of class "BasicMethods" and should be in namespace "M". As this is the pattern used in "Core" to identify the custom methods. So new set of methods can be added to existing class or a new partial class file can be created.
3. HelperMethod attribute should be present with all custom methods for example
```
[HelperMethod("StringConcatenate", 2, "string, string")]
```
First parameter is dummy name, second is number of parameter this function accepts and third is type of parameters this method accepts.

Custom methods are useful for complicated formulas, database lookups, using data from REST APIs or any custom feature.

Such as to implement an Excel like "IF" method implement a method using following signature
```
T IF<T>(bool condition, T ActionIftrue, T ActionIfFalse);
```

# Rest End points
BusinessRules.Console project shows a sample usage of how to execute rules via rest call.
There are 2 end points to execute rules

1. api/async/executerule - To execute a single rule
2. api/async/executerulegroup - To execute set of rules with in a same rule group

# Rule Groups
All rules in a Rule Group should have same Facts associated.

# Writing Rules

Write rules in a similar way you would write a Linq query or regular conditions

For example, Lets assume we have fact Person
```
class Person
{
  public string Name { get; set; }
  public int Age { get; set; }
}
```

To write a rule condition where age is between 20 and 30
```
(Person.Age > 20 And Person.Age < 30)
```

Now to add the execution (Action) part

First assign a property to the action and its order of execution as well (because a single rule can have multiple actions)

Now lets say if above condition is true, then we have to multiple age by 2 and concatenate "Updated" in the Name.

So we will add 2 executions to rule.

First one would look like this

Property = Age, Order = 1 and rule execution should look like this
```
Person.Age * 2
```

Second would look like this

Property = Name, Order = 2 and rule execution should look like this
```
Person.Name + "Updated"
```

#### Rules example:

In below screenshot M.Multiply is custom method which accepts 2 int type parameter

![Alt text](https://raw.githubusercontent.com/lokeshlal/BusinessRules/master/rules_demo.png "rules")


#### Note

For client library usage, i.e. how to call rules from client application, please see BusinessRules.Client and BusinessRules.Console project.
