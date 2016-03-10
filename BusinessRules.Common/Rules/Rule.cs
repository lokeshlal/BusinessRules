using System.Collections.Generic;

namespace BusinessRules.Common
{
    public class Rule
    {
        public string RuleName { get; set; }
        public string EntityName { get; set; }
        public string RuleGroup { get; set; }
        public int Priority { get; set; }
        public RuleCondition RuleCondition { get; set; }
        public List<RuleExecution> RuleExecution { get; set; }
    }
}
