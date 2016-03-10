using BusinessRules.Common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Reflection;

namespace BusinessRules.Core
{
    public static class RulesManager
    {
        #region private fields
        private static ConcurrentDictionary<string, Rule> rulesCache = new ConcurrentDictionary<string, Rule>();
        private static readonly object lockObj = new object();
        private static string rulesPath = "rules.xml";
        #endregion

        #region .ctor
        static RulesManager()
        {
            if (!File.Exists(rulesPath))
            {
                XDocument entitiesDoc = new XDocument(new XElement("root"));
                entitiesDoc.Save(rulesPath);
            }
            LoadRules();
        }
        #endregion

        #region public methods
        public static IEnumerable<KeyValuePair<string, Rule>> GetRulesForEntity(IEntity entity)
        {
            return rulesCache
                .Where(r => r.Value.EntityName == entity.GetProperty<string>("EntityName"))
                .AsEnumerable();
        }
        public static IEnumerable<KeyValuePair<string, Rule>> GetRulesInGroup(string groupName)
        {
            return rulesCache
                .Where(r => r.Value.RuleGroup == groupName)
                .AsEnumerable();
        }

        public static KeyValuePair<string, Rule> GetRuleByName(string ruleName)
        {
            return rulesCache
                .FirstOrDefault(r => r.Value.RuleName == ruleName);
        }

        // This method could be moved to extension as well, but then we would be writing all rule logic in extension,
        // which doesn't make any sense
        public static IEntity ExecuteRule(Rule rule, IEntity entity)
        {
            bool preConditionResult = EvaluateCondition(rule.RuleCondition, entity);
            if (preConditionResult)
            {
                foreach (RuleExecution execution in rule.RuleExecution.OrderBy(re => re.Order))
                {
                    // left hand side value should always be property in execution
                    entity.SetProperty(Convert.ToString(execution.OperandLHS.OperandValue),
                        GetOperandValue(execution.OperandRHS, entity));
                }
            }
            return entity;
        }

        public static void AddNewRule(Rule newRule)
        {
            AddRules(newRule);
        }

        public static void UpdateRule(Rule newRule)
        {
            UpdateRules(newRule);
        }

        public static void DeleteRule(string ruleName)
        {
            Delete(ruleName);
        }
        // Execute Rule /  Rules
        #endregion

        #region condition evaluation
        private static bool EvaluateCondition(RuleCondition condition, IEntity entity)
        {
            bool status = false;

            switch (condition.LogicalOperator)
            {
                // This is the last level of statement
                case LogicalOperator.None:
                    var lhsOperand = GetOperandValue(condition.OperandLHS, entity);
                    var rhsOperand = GetOperandValue(condition.OperandRHS, entity);

                    // types are not equal
                    if (lhsOperand.GetType() != rhsOperand.GetType())
                    {
                        return false;
                    }

                    switch (condition.RelationalOperator)
                    {
                        case RelationalOperator.Equals: return lhsOperand.Equals(rhsOperand);
                        case RelationalOperator.GreaterThan: return GreaterThan(lhsOperand, rhsOperand);
                        case RelationalOperator.GreaterThanOrEqualsTo: return GreaterThanEqualTo(lhsOperand, rhsOperand);
                        case RelationalOperator.LessThan: return LessThan(lhsOperand, rhsOperand);
                        case RelationalOperator.LessThanOrEqualsTo: return LessThanEqualTo(lhsOperand, rhsOperand);
                        case RelationalOperator.NotEquals: return !lhsOperand.Equals(rhsOperand);
                    }
                    return false;
                case LogicalOperator.And:
                    foreach (RuleCondition subCondition in condition.Conditions)
                    {
                        // fail if any one condition fails
                        if (!EvaluateCondition(subCondition, entity))
                            return false;
                    }
                    return true;
                case LogicalOperator.Or:
                    foreach (RuleCondition subCondition in condition.Conditions)
                    {
                        // pass if any one condition passes
                        if (EvaluateCondition(subCondition, entity))
                            return true;
                    }
                    return false;
            }

            return status;
        }

        #endregion

        #region private methods

        private static bool GreaterThan(object lhsOperand, object rhsOperand)
        {
            if (PrimitiveTypes.Test(lhsOperand.GetType()) && PrimitiveTypes.Test(rhsOperand.GetType()))
            {
                switch (lhsOperand.GetType().FullName.ToLower())
                {
                    case "system.int32": return Convert.ToInt32(lhsOperand) > Convert.ToInt32(rhsOperand);
                    case "system.datetime": return Convert.ToDateTime(lhsOperand) > Convert.ToDateTime(rhsOperand);
                    case "system.int64": return Convert.ToInt64(lhsOperand) > Convert.ToInt64(rhsOperand);
                    case "system.long": return Convert.ToInt64(lhsOperand) > Convert.ToInt64(rhsOperand);
                    case "system.double": return Convert.ToDouble(lhsOperand) > Convert.ToDouble(rhsOperand);
                    case "system.decimal": return Convert.ToDecimal(lhsOperand) > Convert.ToDecimal(rhsOperand);
                    case "system.char": return Convert.ToChar(lhsOperand) > Convert.ToChar(rhsOperand);
                    case "system.single": return Convert.ToSingle(lhsOperand) > Convert.ToSingle(rhsOperand);
                    case "system.sbyte": return Convert.ToSByte(lhsOperand) > Convert.ToSByte(rhsOperand); ;
                    case "system.uint16": return Convert.ToUInt16(lhsOperand) > Convert.ToUInt16(rhsOperand);
                    case "system.uint32": return Convert.ToUInt32(lhsOperand) > Convert.ToUInt32(rhsOperand);
                    case "system.uint64": return Convert.ToUInt64(lhsOperand) > Convert.ToUInt64(rhsOperand);
                }
                throw new Exception(string.Format("Type not compatibale for {0} comparision operation", "Greater than"));
            }
            else {
                return false;
            }
        }

        private static bool GreaterThanEqualTo(object lhsOperand, object rhsOperand)
        {
            if (PrimitiveTypes.Test(lhsOperand.GetType()) && PrimitiveTypes.Test(rhsOperand.GetType()))
            {
                switch (lhsOperand.GetType().FullName.ToLower())
                {
                    case "system.int32": return Convert.ToInt32(lhsOperand) >= Convert.ToInt32(rhsOperand);
                    case "system.datetime": return Convert.ToDateTime(lhsOperand) >= Convert.ToDateTime(rhsOperand);
                    case "system.int64": return Convert.ToInt64(lhsOperand) >= Convert.ToInt64(rhsOperand);
                    case "system.long": return Convert.ToInt64(lhsOperand) >= Convert.ToInt64(rhsOperand);
                    case "system.double": return Convert.ToDouble(lhsOperand) >= Convert.ToDouble(rhsOperand);
                    case "system.decimal": return Convert.ToDecimal(lhsOperand) >= Convert.ToDecimal(rhsOperand);
                    case "system.char": return Convert.ToChar(lhsOperand) >= Convert.ToChar(rhsOperand);
                    case "system.single": return Convert.ToSingle(lhsOperand) >= Convert.ToSingle(rhsOperand);
                    case "system.sbyte": return Convert.ToSByte(lhsOperand) >= Convert.ToSByte(rhsOperand); ;
                    case "system.uint16": return Convert.ToUInt16(lhsOperand) >= Convert.ToUInt16(rhsOperand);
                    case "system.uint32": return Convert.ToUInt32(lhsOperand) >= Convert.ToUInt32(rhsOperand);
                    case "system.uint64": return Convert.ToUInt64(lhsOperand) >= Convert.ToUInt64(rhsOperand);
                }
                throw new Exception(string.Format("Type not compatibale for {0} comparision operation", "Greater than"));
            }
            else {
                return false;
            }
        }

        private static bool LessThan(object lhsOperand, object rhsOperand)
        {
            if (PrimitiveTypes.Test(lhsOperand.GetType()) && PrimitiveTypes.Test(rhsOperand.GetType()))
            {
                switch (lhsOperand.GetType().FullName.ToLower())
                {
                    case "system.int32": return Convert.ToInt32(lhsOperand) < Convert.ToInt32(rhsOperand);
                    case "system.datetime": return Convert.ToDateTime(lhsOperand) < Convert.ToDateTime(rhsOperand);
                    case "system.int64": return Convert.ToInt64(lhsOperand) < Convert.ToInt64(rhsOperand);
                    case "system.long": return Convert.ToInt64(lhsOperand) < Convert.ToInt64(rhsOperand);
                    case "system.double": return Convert.ToDouble(lhsOperand) < Convert.ToDouble(rhsOperand);
                    case "system.decimal": return Convert.ToDecimal(lhsOperand) < Convert.ToDecimal(rhsOperand);
                    case "system.char": return Convert.ToChar(lhsOperand) < Convert.ToChar(rhsOperand);
                    case "system.single": return Convert.ToSingle(lhsOperand) < Convert.ToSingle(rhsOperand);
                    case "system.sbyte": return Convert.ToSByte(lhsOperand) < Convert.ToSByte(rhsOperand); ;
                    case "system.uint16": return Convert.ToUInt16(lhsOperand) < Convert.ToUInt16(rhsOperand);
                    case "system.uint32": return Convert.ToUInt32(lhsOperand) < Convert.ToUInt32(rhsOperand);
                    case "system.uint64": return Convert.ToUInt64(lhsOperand) < Convert.ToUInt64(rhsOperand);
                }
                throw new Exception(string.Format("Type not compatibale for {0} comparision operation", "Greater than"));
            }
            else {
                return false;
            }
        }

        private static bool LessThanEqualTo(object lhsOperand, object rhsOperand)
        {
            if (PrimitiveTypes.Test(lhsOperand.GetType()) && PrimitiveTypes.Test(rhsOperand.GetType()))
            {
                switch (lhsOperand.GetType().FullName.ToLower())
                {
                    case "system.int32": return Convert.ToInt32(lhsOperand) <= Convert.ToInt32(rhsOperand);
                    case "system.datetime": return Convert.ToDateTime(lhsOperand) <= Convert.ToDateTime(rhsOperand);
                    case "system.int64": return Convert.ToInt64(lhsOperand) <= Convert.ToInt64(rhsOperand);
                    case "system.long": return Convert.ToInt64(lhsOperand) <= Convert.ToInt64(rhsOperand);
                    case "system.double": return Convert.ToDouble(lhsOperand) <= Convert.ToDouble(rhsOperand);
                    case "system.decimal": return Convert.ToDecimal(lhsOperand) <= Convert.ToDecimal(rhsOperand);
                    case "system.char": return Convert.ToChar(lhsOperand) <= Convert.ToChar(rhsOperand);
                    case "system.single": return Convert.ToSingle(lhsOperand) <= Convert.ToSingle(rhsOperand);
                    case "system.sbyte": return Convert.ToSByte(lhsOperand) <= Convert.ToSByte(rhsOperand); ;
                    case "system.uint16": return Convert.ToUInt16(lhsOperand) <= Convert.ToUInt16(rhsOperand);
                    case "system.uint32": return Convert.ToUInt32(lhsOperand) <= Convert.ToUInt32(rhsOperand);
                    case "system.uint64": return Convert.ToUInt64(lhsOperand) <= Convert.ToUInt64(rhsOperand);
                }
                throw new Exception(string.Format("Type not compatibale for {0} comparision operation", "Greater than"));
            }
            else {
                return false;
            }
        }

        private static object GetOperandValue(Operand operand, IEntity entity)
        {
            switch (operand.OperandType)
            {
                case OperandValueType.Constant:
                    return Constants.Instance.GetConstant(operand.OperandValue);
                case OperandValueType.Value:
                    return Convert.ChangeType(operand.OperandValue, TypeHandler.GetType(operand.Type));
                case OperandValueType.Property:
                    return entity.GetProperty(operand.OperandValue);
                case OperandValueType.CustomMethod:
                    MethodInfo methodInfo = BasicMethodsManager.GetMethod(operand.OperandValue);
                    return methodInfo.Invoke(null, new object[] { operand.MethodParameters, entity });
            }
            return null;
        }

        private static void AddRules(Rule rule)
        {
            XDocument xDoc = XDocument.Load(rulesPath, LoadOptions.None);
            XElement newElement = new XElement("Rule");
            newElement.SetAttributeValue("name", rule.RuleName);
            newElement.SetAttributeValue("definition", Serializer.Serialize(rule));
            xDoc.Element("root").Add(newElement);

            rulesCache.TryAdd(rule.RuleName, rule);

            SaveRules(xDoc);
        }

        private static void UpdateRules(Rule updatedRule)
        {
            XDocument xDoc = XDocument.Load(rulesPath, LoadOptions.None);

            XElement updatedElement = xDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == updatedRule.RuleName);
            updatedElement.SetAttributeValue("name", updatedRule.RuleName);
            updatedElement.SetAttributeValue("definition", Serializer.Serialize(updatedRule));

            //update cache
            rulesCache[updatedRule.RuleName] = updatedRule;

            SaveRules(xDoc);

            rulesCache.TryAdd(updatedRule.RuleName, updatedRule);

            SaveRules(xDoc);
        }

        private static void Delete(string ruleName)
        {
            XDocument xDoc = XDocument.Load(rulesPath, LoadOptions.None);

            xDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == ruleName).Remove();
            //update cache
            Rule deleteRule;
            rulesCache.TryRemove(ruleName, out deleteRule);

            SaveRules(xDoc);

        }
        private static void LoadRules()
        {
            XDocument xDoc = XDocument.Load(rulesPath, LoadOptions.None);

            foreach (XElement ruleElement in xDoc.Element("root").Descendants("Rule"))
            {
                Rule rule = Serializer.Deserialize<Rule>(ruleElement.Attribute("definition").Value);
                rulesCache.TryAdd(rule.RuleName, rule);
            }
        }
        private static void SaveRules(XDocument doc)
        {
            doc.Save(rulesPath);
        }
        #endregion

    }
}
