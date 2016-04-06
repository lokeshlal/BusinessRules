using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System;
using BusinessRules.Common;

namespace BusinessRules.Core
{
    public class BasicMethodsManager
    {
        public static Dictionary<string, Assembly> methodLibraryCaches = new Dictionary<string, Assembly>();
        public static Dictionary<string, string> methodNamesCache = new Dictionary<string, string>();
        public static string methodsFolder = ConfigurationManager.Configuration.BasicMethodsPath; //"libraries";
        public static Type BasicMethodsManagerType
        {
            get
            {
                foreach (KeyValuePair<string, Assembly> kvp in methodLibraryCaches)
                {
                    foreach (Type t in kvp.Value.GetTypes())
                    {
                        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                        foreach (MethodInfo method in methods)
                        {
                            Attribute[] attributes = Attribute.GetCustomAttributes(method);
                            if (attributes.Select(a => a is HelperMethodAttribute).Count() > 0
                                && method.GetCustomAttribute(typeof(HelperMethodAttribute)) != null)
                            {
                                return t;
                            }
                        }
                    }
                }
                return null;
            }
        }

        public static object BasicMethodsManagerInstance
        {
            get
            {
                return Activator.CreateInstance(BasicMethodsManagerType);
            }
        }
        static BasicMethodsManager()
        {
            if (methodLibraryCaches.Count == 0)
                LoadAssemblies();
        }
        private static void LoadAssemblies()
        {
            DirectoryInfo libraryDirectory = new DirectoryInfo(methodsFolder);
            foreach (FileInfo dll in libraryDirectory.GetFiles("*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(dll.FullName);
                methodLibraryCaches.Add(dll.Name, assembly);
            }
        }

        public static Dictionary<string, string> GetAllMethodNames()
        {
            if (methodNamesCache.Count == 0)
            {
                Dictionary<string, string> methodNames = new Dictionary<string, string>();
                foreach (KeyValuePair<string, Assembly> kvp in methodLibraryCaches)
                {
                    foreach (Type t in kvp.Value.GetTypes())
                    {
                        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                        foreach (MethodInfo method in methods)
                        {
                            Attribute[] attributes = Attribute.GetCustomAttributes(method);
                            if (attributes.Select(a => a is HelperMethodAttribute).Count() > 0
                                && method.GetCustomAttribute(typeof(HelperMethodAttribute)) != null)
                            {

                                //methodNames.Add(string.Format("[{0}][{1}][{2}]", kvp.Key, t.FullName, method.Name),
                                //methodNames.Add(((HelperMethodAttribute)method.GetCustomAttribute(typeof(HelperMethodAttribute))).DisplayName,
                                methodNames.Add(method.Name,
                                    ((HelperMethodAttribute)method.GetCustomAttribute(typeof(HelperMethodAttribute))).ParametersString);
                                //((HelperMethodAttribute)method.GetCustomAttribute(typeof(HelperMethodAttribute))).NoOfParameters);
                            }
                        }
                    }
                }
                methodNamesCache = methodNames;
            }
            return methodNamesCache;
        }

        public static void AddAssembly(string assemblyPath)
        {
            FileInfo dll = new FileInfo(assemblyPath);
            Assembly assembly = Assembly.LoadFile(dll.FullName);
            methodLibraryCaches.Add(dll.Name, assembly);

            // Add new methods to method names cache
            foreach (Type t in assembly.GetTypes())
            {
                MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (MethodInfo method in methods)
                {
                    Attribute[] attributes = Attribute.GetCustomAttributes(method);
                    if (attributes.Select(a => a is HelperMethodAttribute).Count() > 0
                        && method.GetCustomAttribute(typeof(HelperMethodAttribute)) != null)
                    {
                        methodNamesCache.Add(method.Name,
                            ((HelperMethodAttribute)method.GetCustomAttribute(typeof(HelperMethodAttribute))).ParametersString);
                    }
                }
            }
        }

        // method name will be like "[Library][ClassName][MethodName]" -- [dll.Name][TypeName][MethodName]
        public static MethodInfo GetMethod(object methodName)
        {
            var methodNameStr = Convert.ToString(methodName);
            string[] librayClassMethodName = methodNameStr.Split(new string[] { "][" }, 3, System.StringSplitOptions.None);
            var type = methodLibraryCaches[librayClassMethodName[0].Replace("[", "")]
                .GetType(librayClassMethodName[1]);
            return type.GetMethod(librayClassMethodName[2].Replace("]", ""));
        }
    }
}
