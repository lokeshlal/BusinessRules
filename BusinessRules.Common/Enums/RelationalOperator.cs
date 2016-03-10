namespace BusinessRules.Common
{
    public enum RelationalOperator
    {
        [Operator("Equals to")]
        Equals,
        [Operator("Not equals to")]
        NotEquals,
        [Operator("Greater than")]
        GreaterThan,
        [Operator("Less than")]
        LessThan,
        [Operator("Greater than or equals to")]
        GreaterThanOrEqualsTo,
        [Operator("Less than or equals to")]
        LessThanOrEqualsTo
    }
}
