/* ======================================================================

    G9 Class Library

    Copyright(C) [?].. All rights reserved.

    $Summary : 
    $System  : G9
    $Designer: 
    $Workfile:   $
    $Revision:   $

    $Header  :   $
========================================================================= */
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Collections.Generic;
using System.Collections;
using System.Data;

namespace Seifer.Common
{
    public class Reflection
    {
        private static readonly string ActiveFormContextName = "__ActiveForm__";

        public static void SetCallContext(string name, object obj)
        {
            if (CallContext.GetData(name) != null)
            {
                CallContext.FreeNamedDataSlot(name);
            }
            CallContext.SetData(name, obj);
        }

        public static object GetCallContext(string name)
        {
            return CallContext.GetData(name);
        }

        public static void FreeCallContext(string name)
        {
            CallContext.FreeNamedDataSlot(name);
        }

        public static void SetThreadActiveForm(Form form)
        {
            Reflection.SetCallContext(Reflection.ActiveFormContextName, form);
        }

        public static Form GetThreadActiveForm()
        {
            return (Form)GetCallContext(ActiveFormContextName);
        }

        public static void FreeThreadActiveForm()
        {
            FreeCallContext(ActiveFormContextName);
        }

        public static object CreateObjectInstance(string typeString, params object[] args)
        {
            object obj = Activator.CreateInstance(GetObjectType(typeString), args);
            if (obj is Form)
            {
                Form parent = GetThreadActiveForm();
                if (parent != null)
                {
                    ((Form)obj).Owner = parent;
                }
            }
            return obj;
        }

        public static object CreateObjectInstance(Type type)
        {
            return type.Assembly.CreateInstance(type.FullName, true);

        }

        public static Type GetObjectType(string typeString)
        {
            typeString = typeString.Trim();

            if ((typeString == null) || (typeString.Length == 0))
            {
                throw new ArgumentException("Type name is empty.", typeString);
            }

            string[] strTypeInfo = typeString.Split(",".ToCharArray(), 2);
            string strTypeName = strTypeInfo[0].Trim();
            string strAssemblyName;

            if (strTypeInfo.Length == 2)
            {
                strAssemblyName = strTypeInfo[1].Trim();
            }
            else
            {
                if (strTypeName.LastIndexOf(".") <= 0)
                {
                    throw new ArgumentException("Ivalid Config Format", typeString);
                }
                strAssemblyName = strTypeName.Substring(0, strTypeName.LastIndexOf("."));
            }

            Assembly asm = Assembly.Load(strAssemblyName);
            return asm.GetType(strTypeName, true, true);
        }

        public static Type GetObjectPropertyType(object obj, string propertyName)
        {
            BindingFlags flag = BindingFlags.GetField | BindingFlags.GetProperty
                | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            PropertyInfo property = obj.GetType().GetProperty(propertyName, flag);
            return property.PropertyType;
        }

        public static void SetObjectProperty(object obj, string propertyName, object propertyValue)
        {
            BindingFlags flag = BindingFlags.SetProperty | BindingFlags.NonPublic
                | BindingFlags.Public | BindingFlags.Instance;

            PropertyInfo property = obj.GetType().GetProperty(propertyName, flag);
            object val = ConvertValueType(property.PropertyType, propertyValue);

            obj.GetType().InvokeMember(propertyName, flag, null, obj,
                new object[] { val }, CultureInfo.InvariantCulture);
        }

        public static object GetObjectProperty(object obj, string propertyName)
        {
            BindingFlags flag = BindingFlags.GetField | BindingFlags.GetProperty
                | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            return obj.GetType().InvokeMember(propertyName,
                flag, null, obj, null, CultureInfo.InvariantCulture);
        }

        public static void SetProperty(Form form, string controlName, string propertyName, string propertyValue)
        {
            object objProperty;

            if (controlName == form.Name)
            {
                objProperty = form;
            }
            else
            {
                objProperty = GetObjectProperty(form, controlName);
            }

            Type type = GetObjectPropertyType(objProperty, propertyName);
            TypeConverter converter = TypeDescriptor.GetConverter(type);

            SetObjectProperty(objProperty, propertyName, converter.ConvertFromString(propertyValue));
        }

        public static object GetProperty(Form form, string controlName, string propertyName)
        {
            object objProperty;

            if (controlName == form.Name)
            {
                objProperty = form;
            }
            else
            {
                objProperty = GetObjectProperty(form, controlName);
            }

            return GetObjectProperty(objProperty, propertyName);
        }

        public static string GetFieldDesc(Type objType, string fieldName, bool inherit)
        {
            FieldInfo[] fields = objType.GetFields();
            new ArrayList();
            foreach (FieldInfo info in fields)
            {
                if (string.Compare(info.Name, fieldName, true) == 0)
                {
                    object[] customAttributes = info.GetCustomAttributes(typeof(DescriptionAttribute), inherit);
                    if ((customAttributes != null) && (customAttributes.Length != 0))
                    {
                        DescriptionAttribute attribute = customAttributes[0] as DescriptionAttribute;
                        if (attribute != null)
                        {
                            return attribute.Description;
                        }
                    }
                }
            }
            return null;
        }



        public static object InvokeMethodByName(object rObj, string methodName, params object[] paramVals)
        {
            object obj2;
            if (rObj == null)
            {
                throw new ArgumentNullException(string.Format("反射调用方法 {0}({1}) 的方法载体为NULL", methodName, getParamsValuesMsg(paramVals)), new ArgumentNullException("rObj"));
            }
            MethodInfo method = rObj.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new ArgumentNullException(string.Format("对象{0}中不包含该方法{1}", rObj.GetType().FullName, methodName), new ArgumentNullException("method"));
            }
            try
            {
                obj2 = method.Invoke(rObj, paramVals);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return obj2;
        }

        private static string getParamsValuesMsg(object[] paramsVals)
        {
            if ((paramsVals == null) || (paramsVals.Length == 0))
            {
                return string.Empty;
            }
            string str = string.Empty;
            foreach (object obj2 in paramsVals)
            {
                str = str + ((obj2 != null) ? obj2.ToString() : "") + ";";
            }
            return str;
        }

        public static bool CheckObjectExistsProperty(object rObj, string proName)
        {
            return (rObj.GetType().GetProperty(proName) != null);
        }

        public static bool CheckTypeExistsProperty(Type dataType, string proName)
        {
            Type type = dataType;
            return (type.GetProperty(proName) != null);
        }

        public static bool CheckPropertyValueIsNull(object rObj, string proName)
        {
            object obj2 = GetObjectProperty(rObj, proName);
            if (obj2 != null)
            {
                return (obj2.ToString().Length == 0);
            }
            return true;
        }

        public static object ConvertValueType(Type convertType, object value)
        {
            Type o = null;
            if ((value != null) && (value != DBNull.Value))
            {
                o = value.GetType();
            }
            if ((o != null) && convertType.Equals(o))
            {
                return value;
            }
            if (convertType.IsGenericType)
            {
                if (convertType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if ((value == null) || (value == DBNull.Value))
                    {
                        return null;
                    }
                    if (o.Equals(typeof(string)) && (((string)value) == string.Empty))
                    {
                        return null;
                    }
                }
                convertType = GetPropertyType(convertType);
            }
            if (((o != null) && convertType.IsEnum) && o.Equals(typeof(string)))
            {
                return Enum.Parse(convertType, value.ToString());
            }
            if (((o != null) && convertType.IsPrimitive) && (o.Equals(typeof(string)) && string.IsNullOrEmpty((string)value)))
            {
                value = 0;
            }
            if ((value == null) || (value == DBNull.Value))
            {
                if (convertType.IsValueType)
                {
                    return getValueTypeDefaultValue(convertType);
                }
                return null;
            }
            if (convertType.Equals(typeof(bool)))
            {
                string str = value.ToString();
                return (((str == "1") || str.ToUpper().Equals("TRUE")));
            }
            try
            {
                return Convert.ChangeType(value, GetPropertyType(convertType));
            }
            catch (Exception exception)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(GetPropertyType(convertType));
                if ((converter == null) || !converter.CanConvertFrom(value.GetType()))
                {
                    throw exception;
                }
                return converter.ConvertFrom(value);
            }
        }

        public static Type GetPropertyType(Type propertyType)
        {
            Type nullableType = propertyType;
            if (nullableType.IsGenericType && (nullableType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return Nullable.GetUnderlyingType(nullableType);
            }
            return nullableType;
        }

        private static object getValueTypeDefaultValue(Type dataType)
        {
            if (dataType.Equals(typeof(DateTime)))
            {
                return new DateTime();
            }
            if (dataType.Equals(typeof(bool)))
            {
                return false;
            }
            if (dataType.Equals(typeof(decimal)))
            {
                return 0M;
            }
            if (dataType.Equals(typeof(double)))
            {
                return 0.0;
            }
            if (dataType.Equals(typeof(int)))
            {
                return 0;
            }
            if (dataType.Equals(typeof(long)))
            {
                return (long)0;
            }
            if (dataType.Equals(typeof(uint)))
            {
                return 0;
            }
            if (dataType.Equals(typeof(short)))
            {
                return (short)0;
            }
            return null;
        }

        public static DataTable ToDataTable(object dataSource, string dataMember)
        {
            string name = dataSource.GetType().Name;
            DataTable table = null;
            string str2 = name;
            if (str2 == null)
            {
                return table;
            }
            if (!(str2 == "DataSet"))
            {
                if (str2 != "DataTable")
                {
                    if (str2 != "DataView")
                    {
                        return table;
                    }
                    return (dataSource as DataView).Table;
                }
            }
            else
            {
                DataSet set = dataSource as DataSet;
                if (!string.IsNullOrEmpty(dataMember))
                {
                    return set.Tables[dataMember];
                }
                return set.Tables[0];
            }
            return (dataSource as DataTable);
        }

        public static DataView ToDataView(object dataSource, string dataMember)
        {
            if (dataSource == null)
            {
                return null;
            }
            string name = dataSource.GetType().Name;
            DataView view = null;
            string str2 = name;
            if (str2 == null)
            {
                return view;
            }
            if (!(str2 == "DataSet"))
            {
                if (str2 != "DataTable")
                {
                    if (str2 != "DataView")
                    {
                        return view;
                    }
                    return (dataSource as DataView);
                }
            }
            else
            {
                DataSet set = dataSource as DataSet;
                if (!string.IsNullOrEmpty(dataMember))
                {
                    return set.Tables[dataMember].DefaultView;
                }
                return set.Tables[0].DefaultView;
            }
            return (dataSource as DataTable).DefaultView;
        }

        public static object ToGridViewSource(object dataSource)
        {
            if (dataSource == null)
            {
                return null;
            }
            string name = dataSource.GetType().Name;
            if (name == null)
            {
                return dataSource;
            }
            if (!(name == "DataSet"))
            {
                if (name == "DataTable")
                {
                    return (dataSource as DataTable).DefaultView;
                }
                if (name == "DataView")
                {
                }
                return dataSource;
            }
            DataSet set = dataSource as DataSet;
            return set.Tables[0].DefaultView;
        }

        public static T FillModelObject<T>(T entityModel, DataRow drData)
        {
            PropertyInfo[] properties = entityModel.GetType().GetProperties();
            DataTable table = drData.Table;
            foreach (PropertyInfo info in properties)
            {
                if ((info.CanWrite && table.Columns.Contains(info.Name)) && (drData[info.Name] != DBNull.Value))
                {
                    object obj2 = drData[info.Name];
                    info.SetValue(entityModel, ConvertValueType(info.PropertyType, obj2), null);
                }
            }
            return entityModel;
        }

        public static T FillModelObject<T>(object entity)
        {
            Type objType = typeof(T);
            T local = (T)CreateObjectInstance(objType);
            foreach (PropertyInfo info in objType.GetProperties())
            {
                if (info.CanWrite && CheckObjectExistsProperty(entity, info.Name))
                {
                    object obj2 = GetObjectProperty(entity, info.Name);
                    info.SetValue(local, ConvertValueType(info.PropertyType, obj2), null);
                }
            }
            return local;
        }

        public static T FillModelObjectNoCreate<T>(T orgEntity, T destEntity, params string[] excludePros)
        {
            if ((orgEntity == null) || (destEntity == null))
            {
                return default(T);
            }
            Type type = typeof(T);
            foreach (PropertyInfo info in type.GetProperties())
            {
                if (((info.CanWrite && info.CanRead) && !info.IsSpecialName) && ((excludePros == null) || (Array.IndexOf<string>(excludePros, info.Name) < 0)))
                {
                    info.SetValue(destEntity, info.GetValue(orgEntity, null), null);
                }
            }
            return destEntity;
        }
    }
}
