using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BusinessRules.Common
{
    public class EntityFieldDefinition
    {
        #region fields
        private string _fieldTypeStr;
        #endregion
        #region properties
        public string FieldName { get; set; }
        public string FieldTypeStr
        {
            get { return _fieldTypeStr; }
            set
            {
                _fieldTypeStr = value;
                ConvertFieldStrToType();
            }
        }

        // Type contains System.Runtime which is not serializable, hence ignoring FieldType
        [XmlIgnore]
        public Type FieldType { get; set; }
        #endregion

        #region methods
        public void ConvertFieldStrToType()
        {
            FieldType = TypeHandler.GetType(FieldTypeStr);
        }
        #endregion
    }
}
