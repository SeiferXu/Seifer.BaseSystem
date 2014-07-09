using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Reflection;
namespace Seifer.Common
{
    public static class ExtendMethods
    {
        public static List<T> CopyTo<T>(this List<T> formList, List<T> toList)
        {
            if (default(List<T>) == toList)
            {
                toList = new List<T>();
            }
            foreach (var item in formList)
                toList.Add(item);

            return toList;
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        /// 将DataTable转换成List泛型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertToList<T>(this DataTable dt) where T : new()
        {
            try
            {
                // 定义集合
                List<T> ts = new List<T>();

                // 获得此模型的类型
                Type type = typeof(T);

                string tempName = "";

                foreach (DataRow dr in dt.Rows)
                {
                    T t = new T();

                    // 获得此模型的公共属性
                    PropertyInfo[] propertys = t.GetType().GetProperties();

                    foreach (PropertyInfo pi in propertys)
                    {
                        if (pi.PropertyType.FullName.Contains("Int32") || pi.PropertyType.FullName.Contains("Int16")
                            || pi.PropertyType.FullName.Contains("Decimal") || pi.PropertyType.FullName.Contains("Boolean")
                            || pi.PropertyType.FullName.Contains("DateTime") || pi.PropertyType.FullName.Contains("Nullable")
                            || pi.PropertyType.FullName.Contains("String"))//各种预知类型判定
                        {
                            tempName = pi.Name;

                            // 检查DataTable是否包含此列
                            if (dt.Columns.Contains(tempName))
                            {
                                // 判断此属性是否有Setter
                                if (!pi.CanWrite) continue;

                                object value = dr[tempName];
                                if (value != DBNull.Value)
                                    pi.SetValue(t, value, null);
                            }
                        }
                    }

                    ts.Add(t);
                }

                return ts;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 在2的次方中计算拥有指定属性的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="propertyName"></param>
        /// <param name="compareValue"></param>
        /// <returns></returns>
        public static List<T> ExitPowerList<T>(this List<T> list, string propertyName, decimal compareValue, bool catched) where T : class
        {
            if (list == null || list.Count == 0)
                return list;
            var tempList = new List<T>();
            try
            {
                //list.ForEach(t =>
                //{
                //    object obj = typeof(T).GetProperty(propertyName).GetValue(t, null);
                //    bool isExit = PowerCalculateCommon.PowerCalculateV(Convert.ToDecimal(obj), (int)compareValue);
                //    if (isExit)
                //        tempList.Add(t);
                //});
                tempList.AddRange(list.Where(item => PowerCalculateCommon.PowerCalculateV(
                    Reflection.GetObjectProperty(item, propertyName) == null ? -99 : Convert.ToDecimal(Reflection.GetObjectProperty(item, propertyName)), (int)compareValue) == catched));
            }
            catch
            {
            }

            return tempList;
        }

        /// <summary>
        /// 获取枚举类型的Description属性的描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(this Enum enumObj)
        {
            FieldInfo fieldInfo = typeof(T).GetField(enumObj.ToString());
            if (fieldInfo != null)
            {
                var descriptionAttribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true)[0]
                                                            as DescriptionAttribute;
                if (descriptionAttribute != null)
                    return descriptionAttribute.Description;
            }
            return "";
        }

    }
}
