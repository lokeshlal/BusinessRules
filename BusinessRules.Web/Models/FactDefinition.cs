using System.Collections.Generic;

namespace BusinessRules.Web
{
    public class FactDefinition
    {
        public string factName { get; set; }

        public List<FactFieldDefinition> fields { get; set; }

    }

    public class FactFieldDefinition
    {
        public string fieldName { get; set; }
        public string fieldType { get; set; }

    }

    public class EntityName
    {
        public string name { get; set; }
    }
}