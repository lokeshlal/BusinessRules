namespace BusinessRules.Common
{
    public enum LogicalOperator
    {
        [Operator("None", true)]
        None,
        [Operator("And")]
        And,
        [Operator("Or")]
        Or
        // Instead of Not use != true/false
        //,
        //[Operator("Not")]
        //Not
    }
}
