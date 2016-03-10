namespace BusinessRules.Common
{
    public class RuleExecution
    {
        public Operand OperandLHS { get; set; }
        public Operand OperandRHS { get; set; }
        public int Order { get; set; }
    }
}