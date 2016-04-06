using BusinessRules.Common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq.Dynamic;

namespace BusinessRules.Core
{
    public static class RulesManager
    {
        #region private fields
        internal static ConcurrentDictionary<string, Rule> rulesCache = new ConcurrentDictionary<string, Rule>();
        private static readonly object lockObj = new object();
        private static string rulesPath = ConfigurationManager.Configuration.RulesPath;
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

        public static bool IsRuleExists(string ruleName)
        {
            if (rulesCache.ContainsKey(ruleName))
            {
                return true;
            }
            return false;
        }
        public static void AddorUpdateRule(Rule newRule)
        {
            if (IsRuleExists(newRule.RuleName))
            {
                UpdateRule(newRule);
            }
            else
            {
                AddRules(newRule);
            }
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

        #region rule evaluation

        public static IEntity ExecuteRule(string ruleName, object obj)
        {
            Rule rule = GetRuleByName(ruleName).Value;
            IEntity entity = EntityFacade.ConvertObjectToEntity(obj, rule.EntityName);
            if (EvaluateCondition(rule, entity))
            {
                return ExecuteRuleExecutions(rule, entity);
            }
            return entity;
        }
        public static IEntity ExecuteRule(string ruleName, IEntity entity)
        {
            Rule rule = GetRuleByName(ruleName).Value;
            if (EvaluateCondition(rule, entity))
            {
                return ExecuteRuleExecutions(rule, entity);
            }
            return entity;
        }

        public static IEntity ExecuteRuleGroup(string ruleGroupName, IEntity entity)
        {
            List<Rule> rules = GetRulesInGroup(ruleGroupName).Select(r => r.Value).ToList(); ;
            rules = rules.OrderByDescending(r => r.Priority).ToList();
            foreach (Rule rule in rules)
            {
                entity = ExecuteRule(rule.RuleName, entity);
            }
            return entity;
        }
        private static bool EvaluateCondition(Rule rule, IEntity entity)
        {
            bool result = false;
            string expression = rule.RuleCondition;
            var entityParameter = Expression.Parameter(entity.GetType(), rule.EntityName);
            var methodParameter = Expression.Parameter(BasicMethodsManager.BasicMethodsManagerType, "M");
            var expressionToCompile = System.Linq.Dynamic.DynamicExpression.ParseLambda(new[] { entityParameter, methodParameter }, typeof(bool), expression);
            result = (bool)expressionToCompile.Compile().DynamicInvoke(entity, BasicMethodsManager.BasicMethodsManagerInstance);
            return result;
        }

        // This method could be moved to extension as well, but then we would be writing all rule logic in extension,
        // which doesn't make any sense
        private static IEntity ExecuteRuleExecutions(Rule rule, IEntity entity)
        {
            List<RuleExecution> executions = rule.RuleExecution.OrderBy(execution => execution.Order).ToList();

            foreach (var execution in executions)
            {
                Type propertyType = entity.GetPropertyType(execution.PropertyName);
                string expression = execution.Execution;
                var entityParameter = Expression.Parameter(entity.GetType(), rule.EntityName);
                var methodParameter = Expression.Parameter(BasicMethodsManager.BasicMethodsManagerType, "M");
                var expressionToCompile = System.Linq.Dynamic.DynamicExpression.ParseLambda(new[] { entityParameter, methodParameter }, propertyType, expression);
                var result = expressionToCompile.Compile().DynamicInvoke(entity, BasicMethodsManager.BasicMethodsManagerInstance);
                entity.SetProperty(execution.PropertyName, result);
            }
            return entity;
        }

        #endregion

        #region private methods

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
