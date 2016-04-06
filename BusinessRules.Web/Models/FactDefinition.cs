using System.Collections.Generic;

namespace BusinessRules.Web
{
    public class ExecuteRuleDefinition
    {
        public object entity { get; set; }
        public string ruleName { get; set; }
    }
    public class RuleDefinition
    {
        public string ruleName { get; set; }
        public string entityName { get; set; }
        public string ruleGroup { get; set; }
        public int priority { get; set; }
        public string ruleCondition { get; set; }
        public List<RuleExecutionDefinition> ruleExecution { get; set; }
    }

    public class RuleExecutionDefinition
    {
        public string propertyName { get; set; }
        public string execution { get; set; }
        public int order { get; set; }
    }
    public class FactDefinition
    {
        public string factName { get; set; }

        public List<FactFieldDefinition> fields { get; set; }

    }

    public class FactFieldDefinition
    {
        public string fieldName { get; set; }
        public string fieldType { get; set; }

    }

    public class Name
    {
        public string name { get; set; }
    }

    public class ConstantDefinition
    {
        public string constantName { get; set; }
        public string constantValue { get; set; }
        public string constantTypeStr { get; set; }
    }
}