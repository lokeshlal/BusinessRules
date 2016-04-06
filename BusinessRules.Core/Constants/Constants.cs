using BusinessRules.Common;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System;

namespace BusinessRules.Core
{
    public class Constants
    {
        #region private fields
        internal static Dictionary<string, Constant> constantCache = new Dictionary<string, Constant>();
        private static string constantPath = ConfigurationManager.Configuration.ConstantsPath;
        private static Constants instance = null;
        private static readonly object lockObj = new object();
        #endregion

        #region .ctor
        static Constants()
        {
            if (!File.Exists(constantPath))
            {
                XDocument constantsDoc = new XDocument(new XElement("root"));
                constantsDoc.Save(constantPath);
            }
            LoadConstants();
        }

        public static Constants Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(lockObj)
                    {
                        if(instance == null)
                        {
                            instance = new Constants();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        #region methods
        public void AddorUpdateConstant<T>(string constantName, T constantValue)
        {
            if(constantCache.ContainsKey(constantName))
            {
                UpdateConstant<T>(constantName, constantValue);
            }
            else
            {
                SaveConstant<T>(constantName, constantValue);
            }
        }

        public void SaveConstant<T>(string constantName, T constantValue)
        {
            AddConstants(new Constant()
            {
                ConstantName = constantName,
                ConstantValue = Convert.ToString(constantValue),
                ConstantTypeStr = typeof(T).FullName
            });
        }

        public void UpdateConstant<T>(string constantName, T constantValue)
        {
            UpdateConstants(new Constant()
            {
                ConstantName = constantName,
                ConstantValue = Convert.ToString(constantValue),
                ConstantTypeStr = typeof(T).FullName
            });
        }

        public void DeleteConstant(string constantName)
        {
            DeleteConstants(constantName);
        }

        public object GetConstant(object constantName)
        {
            var constantNameStr = Convert.ToString(constantName);
            return Convert.ChangeType(constantCache[constantNameStr].ConstantValue
                , constantCache[constantNameStr].ConstantType);
        }

        public Constant GetConstantDefinition(object constantName)
        {
            var constantNameStr = Convert.ToString(constantName);
            return constantCache[constantNameStr];
        }

        public T GetConstant<T>(object constantName)
        {
            var constantNameStr = Convert.ToString(constantName);
            return (T)Convert.ChangeType(constantCache[constantNameStr].ConstantValue
                , constantCache[constantNameStr].ConstantType);
        }
        #endregion


        #region private methods
        private static void LoadConstants()
        {
            if (constantCache.Count > 0) return;

            XDocument constantsDoc = XDocument.Load(constantPath, LoadOptions.None);
            foreach (XElement constantElement in constantsDoc.Element("root").Descendants("Constant"))
            {
                constantCache.Add(constantElement.Attribute("name").Value,
                    new Constant()
                    {
                        ConstantName = constantElement.Attribute("name").Value,
                        ConstantTypeStr = constantElement.Attribute("type").Value,
                        ConstantValue = constantElement.Attribute("value").Value
                    });
            }
        }

        private void UpdateConstants(Constant updatedConstant)
        {
            XDocument constantsDoc = XDocument.Load(constantPath, LoadOptions.None);
            XElement updatedElement = constantsDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == updatedConstant.ConstantName);
            updatedElement.SetAttributeValue("type", updatedConstant.ConstantTypeStr);
            updatedElement.SetAttributeValue("value", updatedConstant.ConstantValue);

            //update cache
            constantCache[updatedConstant.ConstantName] = updatedConstant;

            SaveConstantsFile(constantsDoc);

        }

        private void DeleteConstants(string constantName)
        {
            XDocument constantsDoc = XDocument.Load(constantPath, LoadOptions.None);

            constantsDoc.Element("root").Descendants().First(d => d.Attribute("name").Value == constantName).Remove();
            //update cache
            constantCache.Remove(constantName);

            SaveConstantsFile(constantsDoc);

        }
        
        private void AddConstants(Constant newConstant)
        {
            XDocument constantsDoc = XDocument.Load(constantPath, LoadOptions.None);
            XElement newElement = new XElement("Constant");
            newElement.SetAttributeValue("name", newConstant.ConstantName);
            newElement.SetAttributeValue("type", newConstant.ConstantTypeStr);
            newElement.SetAttributeValue("value", newConstant.ConstantValue);

            //update cache
            constantCache.Add(newConstant.ConstantName, newConstant);

            constantsDoc.Element("root").Add(newElement);

            SaveConstantsFile(constantsDoc);
        }
        private static void SaveConstantsFile(XDocument constantsDoc)
        {
            constantsDoc.Save(constantPath);
        }

        #endregion
    }
}
