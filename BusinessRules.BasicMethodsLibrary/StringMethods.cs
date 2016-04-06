using BusinessRules.Common;

namespace M
{
    public partial class BasicMethods
    {
        // single parameter
        [HelperMethod("StringLengh", 1, "string")]
        public int StringLength(string value)
        {
            return value.Length;
        }



        // 2 parameter and second string will be concatenated to first one
        [HelperMethod("StringConcatenate", 2, "string, string")]
        public string StringConcatenate(string first, string second)
        {
            return string.Format("{0}{1}", first, second);
        }
    }
}
