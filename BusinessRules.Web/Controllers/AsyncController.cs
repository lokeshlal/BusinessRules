using Newtonsoft.Json.Linq;
using System.Web.Http;
using BusinessRules.Common;
using BusinessRules.Core;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace BusinessRules.Web.Controllers
{
    public class AsyncController : ApiController
    {
        #region fact
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
            string entityNameStr = entityName.ToObject<Name>().name;
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
            string entityNameStr = entityName.ToObject<Name>().name;
            EntityFacade.DeleteEntity(entityNameStr);
            return "true";
        }
        #endregion

        #region constants
        [HttpPost]
        public string AddUpdateConstant(JObject constantDefinition)
        {
            var constant = constantDefinition.ToObject<ConstantDefinition>();
            Type constantType = PrimitiveTypes.GetTypeByName(constant.constantTypeStr);

            switch (constant.constantTypeStr.ToLower())
            {
                case "system.string":
                    Constants.Instance.AddorUpdateConstant(constant.constantName, constant.constantValue);
                    break;
                case "system.int32":
                    Constants.Instance.AddorUpdateConstant(constant.constantName, Convert.ToInt32(constant.constantValue));
                    break;
            }

            return "true";
        }

        [HttpPost]
        public string GetConstantDefinition(JObject constantName)
        {
            string constantNameStr = constantName.ToObject<Name>().name;
            Constant constant = Constants.Instance.GetConstantDefinition(constantNameStr);

            ConstantDefinition constantDefinition = new ConstantDefinition()
            {
                constantName = constant.ConstantName,
                constantValue = constant.ConstantValue,
                constantTypeStr = constant.ConstantTypeStr
            };

            return JsonConvert.SerializeObject(constantDefinition);
        }

        [HttpPost]
        public string DeleteConstant(JObject constantName)
        {
            string constantNameStr = constantName.ToObject<Name>().name;
            Constants.Instance.DeleteConstant(constantNameStr);
            return "true";
        }

        #endregion

        #region rules
        [HttpPost]
        public string AddUpdateRule(JObject ruleDefinition)
        {
            var o = ruleDefinition.ToObject<RuleDefinition>();

            Rule rule = new Rule()
            {
                EntityName = o.entityName,
                Priority = o.priority,
                RuleName = o.ruleName,
                RuleCondition = o.ruleCondition,
                RuleGroup = o.ruleGroup
            };
            rule.RuleExecution = new List<RuleExecution>();
            foreach (var ruleExecution in o.ruleExecution)
            {
                rule.RuleExecution.Add(new RuleExecution()
                {
                    PropertyName = ruleExecution.propertyName,
                    Execution = ruleExecution.execution,
                    Order = ruleExecution.order
                });
            }

            RulesManager.AddorUpdateRule(rule);

            return "true";
        }

        [HttpPost]
        public string GetRuleDefinition(JObject ruleName)
        {
            string ruleNameStr = ruleName.ToObject<Name>().name;
            Rule rule = RulesManager.GetRuleByName(ruleNameStr).Value;

            RuleDefinition ruleDefinition = new RuleDefinition()
            {
                entityName = rule.EntityName,
                priority = rule.Priority,
                ruleName = rule.RuleName,
                ruleCondition = rule.RuleCondition,
                ruleGroup = rule.RuleGroup
            };
            ruleDefinition.ruleExecution = new List<RuleExecutionDefinition>();
            foreach (var ruleExecution in rule.RuleExecution)
            {
                ruleDefinition.ruleExecution.Add(new RuleExecutionDefinition()
                {
                    propertyName = ruleExecution.PropertyName,
                    execution = ruleExecution.Execution,
                    order = ruleExecution.Order
                });
            }

            return JsonConvert.SerializeObject(ruleDefinition);
        }

        [HttpPost]
        public string DeleteRule(JObject ruleName)
        {
            string ruleNameStr = ruleName.ToObject<Name>().name;
            RulesManager.DeleteRule(ruleNameStr);
            return "true";
        }
        #endregion

        #region rule execution
        [HttpPost]
        public string ExecuteRule(ExecuteRuleDefinition executeRuleDefinition)
        {
            object entity = executeRuleDefinition.entity;
            string ruleName = executeRuleDefinition.ruleName;
            return JsonConvert.SerializeObject(RulesManager.ExecuteRule(ruleName, entity));
        }
        #endregion
    }
}
