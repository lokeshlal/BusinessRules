using BusinessRules.Common;
using System;
using System.Collections.Concurrent;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BusinessRules.Core
{
    public static class EntityFacade
    {
        internal static ConcurrentDictionary<string, Type> typeCache = new ConcurrentDictionary<string, Type>();
        private static readonly object lockObj = new object();
        private static string entityPath = ConfigurationManager.Configuration.EntitiesPath;

        #region .ctor
        static EntityFacade()
        {
            if (!File.Exists(entityPath))
            {
                XDocument entitiesDoc = new XDocument(new XElement("root"));
                entitiesDoc.Save(entityPath);
            }
            LoadEntities();
        }
        #endregion

        #region public methods
        public static IEntity GetType(EntityDefinition entity, bool persist = true)
        {
            if (!typeCache.ContainsKey(entity.EntityName))
            {
                Type newType = EntityTypeBuilder.CreateNewType(entity);
                lock (lockObj) //even though ConcurrentDictionary is used, but still using lock 
                {
                    typeCache.TryAdd(entity.EntityName, newType);
                }
                if (persist)
                    AddEntity(entity);
            }
            return GetTypeObject(entity.EntityName);

        }

        public static IEntity GetType(string entityName)
        {
            return GetTypeObject(entityName);
        }

        public static IEntity ConvertObjectToEntity(object obj, string entityName)
        {
            IEntity entity = (IEntity)((Newtonsoft.Json.Linq.JObject)obj).ToObject(typeCache[entityName]);
            return entity;
        }

        public static bool IsEntityExists(string entityName)
        {
            if (typeCache.ContainsKey(entityName))
            {
                return true;
            }
            return false;
        }

        public static void AddorUpdateEntity(EntityDefinition entity)
        {
            if (IsEntityExists(entity.EntityName))
            {
                GetType(entity, false);
                UpdateEntity(entity);
            }
            else
            {
                GetType(entity);
            }
        }

        public static void RemoveEntity(string entityName)
        {
            try
            {
                XDocument xDoc = XDocument.Load(entityPath, LoadOptions.None);

                xDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == entityName).Remove();
                //update cache
                Type deletedEntity;
                typeCache.TryRemove(entityName, out deletedEntity);

                SaveEntities(xDoc);
            }
            catch { /* Eat exception */ }
        }

        public static EntityDefinition GetEntityDefinition(string entityName)
        {
            XDocument xDoc = XDocument.Load(entityPath, LoadOptions.None);
            XElement element = xDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == entityName);

            EntityDefinition entityDefinition = new EntityDefinition()
            {
                EntityName = element.Attribute("name").Value,
                EntityFields = Serializer.Deserialize<List<EntityFieldDefinition>>(element.Attribute("fields").Value)
            };
            return entityDefinition;
        }

        public static void DeleteEntity(string entityName)
        {
            XDocument xDoc = XDocument.Load(entityPath, LoadOptions.None);
            XElement deletedElement = xDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == entityName);
            deletedElement.Remove();
            SaveEntities(xDoc);
        }
        #endregion

        #region private methods

        private static IEntity GetTypeObject(string typeName)
        {
            var myObject = Activator.CreateInstance(typeCache[typeName]);
            return (IEntity)myObject;
        }

        private static void AddEntity(EntityDefinition entity)
        {
            XDocument xDoc = XDocument.Load(entityPath, LoadOptions.None);
            XElement newElement = new XElement("Entity");
            newElement.SetAttributeValue("name", entity.EntityName);
            newElement.SetAttributeValue("fields", Serializer.Serialize(entity.EntityFields));
            xDoc.Element("root").Add(newElement);
            SaveEntities(xDoc);
        }

        private static void UpdateEntity(EntityDefinition entity)
        {
            XDocument xDoc = XDocument.Load(entityPath, LoadOptions.None);
            XElement updatedElement = xDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == entity.EntityName);
            updatedElement.SetAttributeValue("name", entity.EntityName);
            updatedElement.SetAttributeValue("fields", Serializer.Serialize(entity.EntityFields));
            SaveEntities(xDoc);
        }




        private static void LoadEntities()
        {
            XDocument xDoc = XDocument.Load(entityPath, LoadOptions.None);

            foreach (XElement entity in xDoc.Element("root").Descendants("Entity"))
            {
                EntityDefinition entityDefinition = new EntityDefinition()
                {
                    EntityName = entity.Attribute("name").Value,
                    EntityFields = Serializer.Deserialize<List<EntityFieldDefinition>>(entity.Attribute("fields").Value)
                };
                GetType(entityDefinition, false);
            }
        }
        private static void SaveEntities(XDocument doc)
        {
            doc.Save(entityPath);
        }
        #endregion
    }
}
