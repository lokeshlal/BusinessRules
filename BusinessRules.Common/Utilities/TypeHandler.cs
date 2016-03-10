using System;
using System.Linq;

namespace BusinessRules.Common
{
    public class TypeHandler
    {
        public static Type GetType(string typeStr)
        {
            try
            {
                return Type.GetType(typeStr, true, true);
            }
            catch
            {
                try
                {
                    return AppDomain.CurrentDomain.GetAssemblies()
                        .First(a => a.IsDynamic && a.FullName == typeStr + ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
                        .GetTypes().First(t => t.FullName == typeStr);
                }
                catch
                {
                    // most probably self reference property
                    return null;
                }
            }
        }
    }
}
