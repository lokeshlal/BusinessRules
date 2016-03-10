using System.Collections.Generic;

namespace BusinessRules.Common
{
    public class EntityDefinition
    {
        public string EntityName { get; set; }
        public List<EntityFieldDefinition> EntityFields { get; set; }
    }
}
