using BusinessRules.Common;
using BusinessRules.Core;
using System;
using System.Collections.Generic;

namespace BusinessRules.BasicMethodsLibrary
{
    public partial class BasicMethods : IOperation
    {
        [HelperMethod(2)]
        public static int Add(List<Operand> operands, IEntity fact)
        {
            int value = 0;
            int finalvalue = 0;
            foreach (Operand operand in operands)
            {
                value = GetOperandValue(fact, value, operand);
                finalvalue = finalvalue + value;
            }
            return finalvalue;
        }

        [HelperMethod(2)]
        public static int Multiply(List<Operand> operands, IEntity fact)
        {
            int value = 0;
            int finalvalue = 1;
            foreach (Operand operand in operands)
            {
                value = GetOperandValue(fact, value, operand);
                finalvalue = finalvalue * value;
            }
            return finalvalue;
        }

        // only two operands are allowed
        [HelperMethod(2)]
        public static int Subtract(List<Operand> operands, IEntity fact)
        {
            int value = 0;
            int finalValue = 0;
            int count = 0;
            foreach (Operand operand in operands)
            {
                value = GetOperandValue(fact, value, operand);
                if (count == 0)
                {
                    finalValue = value;
                }
                else
                {
                    finalValue = finalValue - value;
                }
            }
            return finalValue;
        }

        // only two operands are allowed
        [HelperMethod(2)]
        public static int Divide(List<Operand> operands, IEntity fact)
        {
            int value = 0;
            int finalValue = 0;
            int count = 0;
            foreach (Operand operand in operands)
            {
                value = GetOperandValue(fact, value, operand);
                if (count == 0)
                {
                    finalValue = value;
                }
                else
                {
                    finalValue = finalValue / value;
                }
            }
            return finalValue;
        }

        // only two operands are allowed
        [HelperMethod(2)]
        public static int Remainder(List<Operand> operands, IEntity fact)
        {
            int value = 0;
            int finalValue = 0;
            int count = 0;
            foreach (Operand operand in operands)
            {
                value = GetOperandValue(fact, value, operand);
                if (count == 0)
                {
                    finalValue = value;
                }
                else
                {
                    finalValue = finalValue % value;
                }
            }
            return finalValue;
        }

        #region private methods
        private static int GetOperandValue(IEntity fact, int value, Operand operand)
        {
            switch (operand.OperandType)
            {
                case OperandValueType.Constant:
                    value = GetConstantValue<int>(operand);
                    break;
                case OperandValueType.CustomMethod:
                    value = GetCustomMethodValue<int>(fact, operand);
                    break;
                case OperandValueType.Property:
                    value = GetEntityValue<int>(fact, operand);
                    break;
                case OperandValueType.Value:
                    value = Convert.ToInt32(operand.OperandValue);
                    break;
            }

            return value;
        }

        private static T GetEntityValue<T>(IEntity fact, Operand operand)
        {
            return (T)fact.GetProperty<T>(operand.OperandValue);
        }

        private static T GetCustomMethodValue<T>(IEntity fact, Operand operand)
        {
            return (T)typeof(BasicMethods).GetMethod(Convert.ToString(operand.OperandValue)).Invoke(null, new object[] { operand.MethodParameters, fact });
        }

        private static T GetConstantValue<T>(Operand operand)
        {
            return (T)Constants.Instance.GetConstant<T>(operand.OperandValue);
        }
        #endregion
    }
}
