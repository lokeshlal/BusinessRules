using System.Collections.Generic;

namespace BusinessRules.Common
{
    public class RuleCondition
    {
        public Operand OperandLHS { get; set; }
        public Operand OperandRHS { get; set; }
        public RelationalOperator RelationalOperator { get; set; }
        public LogicalOperator LogicalOperator { get; set; }
        public List<RuleCondition> Conditions { get; set; }
    }
}