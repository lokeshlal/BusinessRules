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
        public static Dictionary<string, int> methodNamesCache = new Dictionary<string, int>();
        public static string methodsFolder = ConfigurationManager.Configuration.BasicMethodsPath; //"libraries";

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

        public static Dictionary<string, int> GetAllMethodNames()
        {
            if (methodNamesCache.Count == 0)
            {
                Dictionary<string, int> methodNames = new Dictionary<string, int>();
                foreach (KeyValuePair<string, Assembly> kvp in methodLibraryCaches)
                {
                    foreach (Type t in kvp.Value.GetTypes())
                    {
                        MethodInfo[] methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
                        foreach (MethodInfo method in methods)
                        {
                            Attribute[] attributes = Attribute.GetCustomAttributes(method);
                            if (attributes.Select(a => a is HelperMethodAttribute).Count() > 0)
                            {

                                methodNames.Add(string.Format("[{0}][{1}][{2}]", kvp.Key, t.FullName, method.Name),
                                    ((HelperMethodAttribute)method.GetCustomAttribute(typeof(HelperMethodAttribute))).NoOfParameters);
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
                MethodInfo[] methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (MethodInfo method in methods)
                {
                    Attribute[] attributes = Attribute.GetCustomAttributes(method);
                    if (attributes.Select(a => a is HelperMethodAttribute).Count() > 0)
                    {
                        methodNamesCache.Add(string.Format("[{0}][{1}][{2}]", assemblyPath, t.FullName, method.Name),
                            method.GetParameters().Count());
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
