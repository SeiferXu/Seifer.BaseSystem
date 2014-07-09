using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Seifer.Common
{
    public class CopyData
    {
        public static List<T2> CopyDataTo<T1, T2>(List<T1> t1List)
        {
            List<T2> resultT2List = new List<T2>();

            if (t1List != null)
            {
                foreach (T1 t1 in t1List)
                {
                    //T2 t2 = default(T2);

                    T2 t2 = System.Activator.CreateInstance<T2>();

                    foreach (PropertyInfo item in typeof(T1).GetProperties())
                    {
                        object ret = item.GetValue(t1, null);
                        if (item.Name != "EntityState" && item.Name != "Selected")
                        {
                            if (typeof(T2).GetProperty(item.Name) != null)
                            {
                                typeof(T2).GetProperty(item.Name).SetValue(t2, ret, null);
                            }
                        }

                    }
                    resultT2List.Add(t2);
                }
            }
            return resultT2List;
        }

        /// <summary>
        /// 将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static DataTable ToDataTable(IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
    }
}
