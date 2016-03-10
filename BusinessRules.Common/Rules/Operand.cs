using System.Collections.Generic;

namespace BusinessRules.Common
{
    public class Operand
    {
        /// <summary>
        /// Property name, constant name, value, or method name
        /// </summary>
        public object OperandValue { get; set; }
        /// <summary>
        /// List of custom method parameters.
        /// Every parameter of custom method has to be of type <see cref="Operand"/> only
        /// </summary>
        public List<Operand> MethodParameters { get; set; }
        /// <summary>
        /// Property, constant, value or custom method
        /// </summary>
        public OperandValueType OperandType { get; set; }
        /// <summary>
        /// Property type or method return type.
        /// <para>
        /// Applicable only in case of when <see cref="OperandValue"/> is property and method
        /// </para>
        /// </summary>
        public string Type { get; set; }
    }
}