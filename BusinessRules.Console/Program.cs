using BusinessRules.Common;
using BusinessRules.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace BusinessRules.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            List<EntityFieldDefinition> definition = new List<EntityFieldDefinition>();
            definition.Add(new EntityFieldDefinition() { FieldName = "FirstName", FieldTypeStr = "System.String" });
            definition.Add(new EntityFieldDefinition() { FieldName = "Age", FieldTypeStr = "System.Int32" });
            definition.Add(new EntityFieldDefinition() { FieldName = "IncrementedAge", FieldTypeStr = "System.Int32" });

            IEntity obj = EntityFacade.GetType(new EntityDefinition() { EntityName = "Person", EntityFields = definition });

            //obj.GetType().GetProperty("FirstName").SetValue(obj, "Hello");
            obj.SetProperty("FirstName", "Hello");
            obj.SetProperty("Age", 32);
            obj.SetProperty("IncrementedAge", 5);
            System.Console.WriteLine(obj.GetProperty<string>("FirstName"));

            Type personType = AppDomain.CurrentDomain.GetAssemblies().First(a => a.IsDynamic && a.FullName == "Person, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
                .GetTypes().First(t => t.FullName == "Person");

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.FullName == "Person")
                        System.Console.WriteLine(t.FullName, t.AssemblyQualifiedName);
                }
            }

            definition.Add(new EntityFieldDefinition() { FieldName = "Person", FieldTypeStr = "Person" });

            IEntity obj1 = EntityFacade.GetType(new EntityDefinition() { EntityName = "Person1", EntityFields = definition });

            obj1.SetProperty("Person.FirstName", "Hello-0");
            obj1.SetProperty("FirstName", "Hello-1");
            System.Console.WriteLine(obj1.GetProperty<string>("FirstName"));

            Rule rule = new Rule();
            rule.RuleName = "Rule1";
            rule.RuleGroup = "Group1";
            rule.Priority = 1;
            rule.EntityName = "Person";
            rule.RuleCondition = new RuleCondition()
            {
                LogicalOperator = LogicalOperator.None,
                OperandLHS = new Operand()
                {
                    OperandType = OperandValueType.Property,
                    OperandValue = "Age"
                },
                OperandRHS = new Operand()
                {
                    OperandType = OperandValueType.Value,
                    OperandValue = 12,
                    Type = "System.Int32"
                },
                RelationalOperator = RelationalOperator.GreaterThan
            };

            rule.RuleExecution = new List<RuleExecution>();
            rule.RuleExecution.Add(new RuleExecution()
            {
                Order = 1,
                OperandLHS = new Operand()
                {
                    OperandValue = "Age",
                    OperandType = OperandValueType.Property
                },
                OperandRHS = new Operand()
                {
                    OperandType = OperandValueType.CustomMethod,
                    OperandValue = "[BusinessRules.BasicMethodsLibrary.dll][BusinessRules.BasicMethodsLibrary.BasicMethods][Add]"
                }
            });

            rule.RuleExecution[0].OperandRHS.MethodParameters = new List<Operand>();
            rule.RuleExecution[0].OperandRHS.MethodParameters.Add(new Operand()
            {
                OperandType = OperandValueType.Property,
                OperandValue = "IncrementedAge"
            });
            rule.RuleExecution[0].OperandRHS.MethodParameters.Add(new Operand()
            {
                OperandType = OperandValueType.Property,
                OperandValue = "Age"
            });

            rule.RuleExecution.Add(new RuleExecution()
            {
                Order = 2,
                OperandLHS = new Operand()
                {
                    OperandValue = "Age",
                    OperandType = OperandValueType.Property
                },
                OperandRHS = new Operand()
                {
                    OperandType = OperandValueType.CustomMethod,
                    OperandValue = "[BusinessRules.BasicMethodsLibrary.dll][BusinessRules.BasicMethodsLibrary.BasicMethods][Add]"
                }
            });

            rule.RuleExecution[1].OperandRHS.MethodParameters = new List<Operand>();
            rule.RuleExecution[1].OperandRHS.MethodParameters.Add(new Operand()
            {
                OperandType = OperandValueType.Property,
                OperandValue = "Age"
            });
            rule.RuleExecution[1].OperandRHS.MethodParameters.Add(new Operand()
            {
                OperandType = OperandValueType.Value,
                OperandValue = 1,
                Type = "System.Int32"
            });
            //bool b = RulesManager.EvaluateCondition(rule.RuleCondition, obj);
            //RulesManager.AddNewRule(rule);
            RulesManager.ExecuteRule(RulesManager.GetRuleByName("Rule1").Value, obj);
            System.Console.WriteLine(obj.GetProperty<int>("Age"));



            List<EntityFieldDefinition> definition1 = new List<EntityFieldDefinition>();
            definition1.Add(new EntityFieldDefinition() { FieldName = "FirstName", FieldTypeStr = "System.String" });
            definition1.Add(new EntityFieldDefinition() { FieldName = "Age", FieldTypeStr = "System.Int32" });
            definition1.Add(new EntityFieldDefinition() { FieldName = "IncrementedAge", FieldTypeStr = "RecursiveType" });

            IEntity obj_recusive = EntityFacade.GetType(new EntityDefinition() { EntityName = "RecursiveType", EntityFields = definition1 });
            obj_recusive.SetProperty("IncrementedAge.IncrementedAge.Age", 76);


            System.Console.ReadLine();

        }
    }
}
