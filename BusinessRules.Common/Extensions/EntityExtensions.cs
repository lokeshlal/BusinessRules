using System;

namespace BusinessRules.Common
{
    public static class EntityExtensions
    {
        public static T GetProperty<T>(this IEntity obj, object propertyName)
        {
            var propertyNameStr = Convert.ToString(propertyName);
            return (T)obj.GetType().GetProperty(propertyNameStr).GetValue(obj);
        }

        public static object GetProperty(this IEntity obj, object propertyName)
        {
            var propertyNameStr = Convert.ToString(propertyName);
            if (propertyNameStr.Contains("."))
            {
                string[] propertyNames = propertyNameStr.Split(new char[] { '.' }, 2);
                IEntity propertyObj = (IEntity)obj.GetType().GetProperty(propertyNames[0]).GetValue(obj);//.PropertyType;
                return propertyObj.GetProperty(propertyNames[1]);
            }
            else {
                return obj.GetType().GetProperty(propertyNameStr).GetValue(obj);
            }
        }

        public static void SetProperty(this IEntity obj, string propertyName, object value)
        {
            if (propertyName.Contains("."))
            {
                string[] propertyNames = propertyName.Split(new char[] { '.' }, 2);
                IEntity propertyObj = (IEntity)obj.GetType().GetProperty(propertyNames[0]).GetValue(obj);//.PropertyType;
                // if null, then initialize the property as well
                if (propertyObj == null)
                {
                    propertyObj = (IEntity)Activator.CreateInstance(obj.GetType().GetProperty(propertyNames[0]).PropertyType);
                }
                // set property and assign back to object
                propertyObj.SetProperty(propertyNames[1], value);
                obj.SetProperty(propertyNames[0], propertyObj);
            }
            else
            {
                obj.GetType().GetProperty(propertyName).SetValue(obj, value);
            }
        }

        public static Type GetPropertyType(this IEntity obj, string propertyName)
        {
            if (propertyName.Contains("."))
            {
                string[] propertyNames = propertyName.Split(new char[] { '.' }, 2);
                IEntity propertyObj = (IEntity)obj.GetType().GetProperty(propertyNames[0]).GetValue(obj);//.PropertyType;
                // if null, then initialize the property as well
                if (propertyObj == null)
                {
                    propertyObj = (IEntity)Activator.CreateInstance(obj.GetType().GetProperty(propertyNames[0]).PropertyType);
                }
                // set property and assign back to object
                return propertyObj.GetPropertyType(propertyNames[1]);
            }
            else
            {
                return obj.GetType().GetProperty(propertyName).PropertyType;
            }
        }
    }
}
