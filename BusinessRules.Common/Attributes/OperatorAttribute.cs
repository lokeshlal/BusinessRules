using System;

namespace BusinessRules.Common
{
    public class OperatorAttribute : Attribute
    {
        private string _displayName;
        private bool _isHidden;
        public OperatorAttribute(string displayName)
        {
            _displayName = displayName;
        }

        public OperatorAttribute(string displayName, bool isHidden)
        {
            _displayName = displayName;
            _isHidden = isHidden;
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public bool IsHidden
        {
            get { return _isHidden; }
            set { _isHidden = value; }
        }
    }
}
