﻿using BusinessRules.Common;
using System;
using System.Collections.Concurrent;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace BusinessRules.Core
{
    public static class EntityFacade
    {
        private static ConcurrentDictionary<string, Type> typeCache = new ConcurrentDictionary<string, Type>();
        private static readonly object lockObj = new object();
        private static string entityPath = "entities.xml";

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

        public static void RemoveEntity(string entityName)
        {
            try {
                XDocument xDoc = XDocument.Load(entityPath, LoadOptions.None);

                xDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == entityName).Remove();
                //update cache
                Type deletedEntity;
                typeCache.TryRemove(entityName, out deletedEntity);

                SaveEntities(xDoc);
            }
            catch { /* Eat exception */ }
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
