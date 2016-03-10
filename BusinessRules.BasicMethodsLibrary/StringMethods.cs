using BusinessRules.Common;
using BusinessRules.Core;
using System;
using System.Collections.Generic;

namespace BusinessRules.BasicMethodsLibrary
{
    public partial class BasicMethods
    {
        // single parameter
        [HelperMethod(1)]
        public static int StringLength(List<Operand> operands, IEntity fact)
        {
            string value = string.Empty;
            value = GetStringOperandValue(operands[0], fact);
            return value.Length;
        }



        // 2 parameter and second string will be concatenated to first one
        [HelperMethod(2)]
        public static string StringConcatenate(List<Operand> operands, IEntity fact)
        {
            string firstString;
            string secondString;
            firstString = GetStringOperandValue(operands[0], fact);
            secondString = GetStringOperandValue(operands[0], fact);

            return string.Format("{0}{1}", firstString, secondString);
        }


        #region private
        private static string GetStringOperandValue(Operand operand, IEntity fact)
        {
            switch (operand.OperandType)
            {
                case OperandValueType.Constant:
                    return GetConstantValue<string>(operand);
                case OperandValueType.Value:
                    return Convert.ToString(operand.OperandValue);
                case OperandValueType.Property:
                    return GetEntityValue<string>(fact, operand);
                case OperandValueType.CustomMethod:
                    return GetCustomMethodValue<string>(fact, operand);
            }
            return string.Empty;
        }
        #endregion


    }
}
