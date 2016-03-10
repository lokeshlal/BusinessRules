using BusinessRules.Common;
using BusinessRules.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessRules.Test
{
    [TestClass]
    public class CoreUnitTest
    {
        #region Entity
        [TestMethod]
        public void GetEntityObjectTest()
        {
            EntityFacade.RemoveEntity("Person");

            List<EntityFieldDefinition> definition = new List<EntityFieldDefinition>();
            definition.Add(new EntityFieldDefinition() { FieldName = "FirstName", FieldTypeStr = "System.String" });
            definition.Add(new EntityFieldDefinition() { FieldName = "Age", FieldTypeStr = "System.Int32" });
            definition.Add(new EntityFieldDefinition() { FieldName = "IncrementedAge", FieldTypeStr = "System.Int32" });

            IEntity obj = EntityFacade.GetType(new EntityDefinition() { EntityName = "Person", EntityFields = definition });
            obj.SetProperty("FirstName", "Hello");

            Assert.AreEqual(obj.GetProperty<string>("FirstName"), "Hello", "Get Entity Test Failed");

            IEntity objByName = EntityFacade.GetType("Person");
            Assert.AreEqual(objByName.GetType().GetProperties().First(p => p.Name == "Age").PropertyType, typeof(int));
        }

        [TestMethod]
        public void GetEntityObjectFromCacheTest()
        {
            EntityFacade.RemoveEntity("Person");

            List<EntityFieldDefinition> definition = new List<EntityFieldDefinition>();
            definition.Add(new EntityFieldDefinition() { FieldName = "FirstName", FieldTypeStr = "System.String" });
            definition.Add(new EntityFieldDefinition() { FieldName = "Age", FieldTypeStr = "System.Int32" });
            definition.Add(new EntityFieldDefinition() { FieldName = "IncrementedAge", FieldTypeStr = "System.Int32" });

            IEntity obj = EntityFacade.GetType(new EntityDefinition() { EntityName = "Person", EntityFields = definition });
            obj.SetProperty("FirstName", "Hello");

            definition.Add(new EntityFieldDefinition() { FieldName = "IncrementedAge1", FieldTypeStr = "System.Int32" });

            IEntity obj1 = EntityFacade.GetType(new EntityDefinition() { EntityName = "Person", EntityFields = definition });
            try
            {
                obj.SetProperty("IncrementedAge1", 23);
                Assert.Fail("IncrementedAge1 should not be present in Person definition");
            }
            catch
            {
            }
        }
        #endregion
    }
}
