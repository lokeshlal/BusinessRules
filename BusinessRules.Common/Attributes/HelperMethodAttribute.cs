using System;

namespace BusinessRules.Common
{
    public class HelperMethodAttribute : Attribute
    {
        private int _noOfParameters;
        private string _parametersString;
        private string _displayName;

        public HelperMethodAttribute()
        {
            _noOfParameters = 1;
        }

        public HelperMethodAttribute(int noOfParameters)
        {
            _noOfParameters = noOfParameters;
        }

        public HelperMethodAttribute(string displayName, int noOfParameters)
        {
            _noOfParameters = noOfParameters;
            _displayName = displayName;
        }

        public HelperMethodAttribute(string displayName, int noOfParameters, string parameterString)
        {
            _noOfParameters = noOfParameters;
            _displayName = displayName;
            _parametersString = parameterString;
        }

        public int NoOfParameters
        {
            get { return _noOfParameters; }
            set { _noOfParameters = value; }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }

            set
            {
                _displayName = value;
            }
        }

        public string ParametersString
        {
            get
            {
                return _parametersString;
            }

            set
            {
                _parametersString = value;
            }
        }
    }
}
