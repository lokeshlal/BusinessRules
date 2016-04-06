using BusinessRules.Common;

namespace M
{
    public partial class BasicMethods : IOperation
    {
        [HelperMethod("AddInt", 2, "int, int")]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [HelperMethod("MulInt", 2, "int, int")]
        public int Multiply(int a, int b)
        {
            return a * b;
        }
    }
}
