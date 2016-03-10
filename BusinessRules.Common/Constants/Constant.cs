using System;

namespace BusinessRules.Common
{
    public class Constant
    {
        #region fields
        private string _constantTypeStr;
        #endregion
        #region properties
        public string ConstantName { get; set; }
        public string ConstantValue { get; set; }
        public string ConstantTypeStr
        {
            get { return _constantTypeStr; }
            set
            {
                _constantTypeStr = value;
                ConvertConstantStrToType();
            }
        }
        public Type ConstantType { get; set; }
        #endregion

        #region methods
        public void ConvertConstantStrToType()
        {
            ConstantType = Type.GetType(ConstantTypeStr, true, true);
        }
        #endregion
    }
}
