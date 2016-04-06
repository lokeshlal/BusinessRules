namespace BusinessRules.Common
{
    public enum RelationalOperator
    {
        [Operator("==")]
        Equals,
        [Operator("!=")]
        NotEquals,
        [Operator(">")]
        GreaterThan,
        [Operator("<")]
        LessThan,
        [Operator(">=")]
        GreaterThanOrEqualsTo,
        [Operator("<=")]
        LessThanOrEqualsTo
    }
}
