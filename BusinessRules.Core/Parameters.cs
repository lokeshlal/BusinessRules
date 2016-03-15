using BusinessRules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessRules.Core
{
    public class Parameters
    {
        public static Dictionary<string, string> AvialableDataTypes()
        {
            Dictionary<string, string> availableDataTypes = new Dictionary<string, string>();

            // Adding primitive types
            availableDataTypes.Add("Integer", PrimitiveTypes.GetNameByType(typeof(int)));
            availableDataTypes.Add("String", PrimitiveTypes.GetNameByType(typeof(string)));

            // Add user defined types
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.IsDynamic))
            {
                foreach (Type t in asm.GetTypes().Where(t1 => t1.Module.ScopeName.Equals("MainModule")))
                {
                    availableDataTypes.Add(t.Name, t.Name);
                }
            }

            return availableDataTypes;
        }

        public static List<string> AvialableFacts()
        {
            return EntityFacade.typeCache.Keys.ToList();
        }

        public static List<string> AvialableRules()
        {
            return RulesManager.rulesCache.Keys.ToList();
        }


        public static Dictionary<string, int> AvialableMethods()
        {
            return BasicMethodsManager.GetAllMethodNames();
        }

        public static Dictionary<string, string> AvialableRelationalConditions()
        {
            Dictionary<string, string> availableRelationalConditions = new Dictionary<string, string>();
            foreach (RelationalOperator val in Enum.GetValues(typeof(RelationalOperator)))
            {
                availableRelationalConditions.Add(
                    val.GetAttributeOfType<OperatorAttribute>().DisplayName,
                    val.ToString()
                    );
            }
            return availableRelationalConditions;
        }

        public static Dictionary<string, string> AvialableArithmeticConditions()
        {
            Dictionary<string, string> available = new Dictionary<string, string>();
            foreach (ArithmeticOperator val in Enum.GetValues(typeof(ArithmeticOperator)))
            {
                available.Add(
                    val.GetAttributeOfType<OperatorAttribute>().DisplayName,
                    val.ToString()
                    );
            }
            return available;
        }

        public static Dictionary<string, string> AvialableLogicalConditions()
        {
            Dictionary<string, string> available = new Dictionary<string, string>();
            foreach (LogicalOperator val in Enum.GetValues(typeof(LogicalOperator)))
            {
                available.Add(
                    val.GetAttributeOfType<OperatorAttribute>().DisplayName,
                    val.ToString()
                    );
            }
            return available;
        }

        public static Dictionary<string, string> AvialableOperandValueType()
        {
            Dictionary<string, string> available = new Dictionary<string, string>();
            foreach (OperandValueType val in Enum.GetValues(typeof(OperandValueType)))
            {
                available.Add(
                    val.GetAttributeOfType<OperatorAttribute>().DisplayName,
                    val.ToString()
                    );
            }
            return available;
        }
    }
}
