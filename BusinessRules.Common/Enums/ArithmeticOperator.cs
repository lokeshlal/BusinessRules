namespace BusinessRules.Common
{
    public enum ArithmeticOperator
    {
        [Operator("+")]
        Plus,
        [Operator("-")]
        Minus,
        [Operator("%")]
        Remainder,
        [Operator("*")]
        Multiply,
        [Operator("/")]
        Divide
    }
}
