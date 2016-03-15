using Newtonsoft.Json.Linq;
using System.Web.Http;
using BusinessRules.Common;
using BusinessRules.Core;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BusinessRules.Web.Controllers
{
    public class AsyncController : ApiController
    {
        [HttpPost]
        public string AddUpdateFact(JObject factDefinition)
        {
            var o = factDefinition.ToObject<FactDefinition>();
            //var o = JsonConvert.DeserializeObject<FactDefinition>(factDefinition);

            EntityDefinition entity = new EntityDefinition();
            entity.EntityName = o.factName;
            entity.EntityFields = new List<EntityFieldDefinition>();
            foreach (var field in o.fields)
            {
                entity.EntityFields.Add(new EntityFieldDefinition()
                {
                    FieldName = field.fieldName,
                    FieldTypeStr = field.fieldType
                });
            }

            EntityFacade.AddorUpdateEntity(entity);

            return "true";
        }

        [HttpPost]
        public string GetEntityDefinition(JObject entityName)
        {
            string entityNameStr = entityName.ToObject<EntityName>().name;
            EntityDefinition entity = EntityFacade.GetEntityDefinition(entityNameStr);

            FactDefinition factDefinition = new FactDefinition();
            factDefinition.factName = entityNameStr;
            factDefinition.fields = new List<FactFieldDefinition>();

            foreach (var field in entity.EntityFields)
            {
                factDefinition.fields.Add(new FactFieldDefinition()
                {
                    fieldName = field.FieldName,
                    fieldType = field.FieldTypeStr
                });
            }

            return JsonConvert.SerializeObject(factDefinition);
        }

        [HttpPost]
        public string DeleteEntity(JObject entityName)
        {
            string entityNameStr = entityName.ToObject<EntityName>().name;
            EntityFacade.DeleteEntity(entityNameStr);
            return "true";
        }
    }
}
