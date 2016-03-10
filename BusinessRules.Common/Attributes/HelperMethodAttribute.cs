using System;

namespace BusinessRules.Common
{
    public class HelperMethodAttribute : Attribute
    {
        private int _noOfParameters;

        public HelperMethodAttribute()
        {
            _noOfParameters = 1;
        }

        public HelperMethodAttribute(int noOfParameters)
        {
            _noOfParameters = noOfParameters;
        }

        public int NoOfParameters
        {
            get { return _noOfParameters; }
            set { _noOfParameters = value; }
        }
    }
}
