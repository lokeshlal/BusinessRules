namespace BusinessRules.Common
{
    public enum OperandValueType
    {
        [Operator("Value")]
        Value,
        [Operator("Property")]
        Property,
        [Operator("Constant")]
        Constant,
        [Operator("Custom method")]
        CustomMethod
    }
}
