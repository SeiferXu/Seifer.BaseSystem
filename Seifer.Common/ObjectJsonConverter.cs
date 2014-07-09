using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Seifer.Common
{
    /// <summary>
    /// Json的转换
    /// </summary>
    public class ObjectJsonConverter
    {
        /// <summary>
        /// 将数据类型转换成Json字符串
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="obj">数据对象</param>
        /// <param name="jsonConverts">转换格式,在Newtonsoft.Json.Converters.DataTableConverter命名空间下</param>
        /// <returns></returns>
        public static string JsonConvertSerialize<T>(T obj, params JsonConverter[] jsonConverts)
        {
            string jsonStr = JsonConvert.SerializeObject(obj, jsonConverts);
            return jsonStr;
        }

        /// <summary>
        /// 将Json字符串转换成数据类型对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static T JsonConvertDeserialize<T>(string jsonStr)
        {
            T entity = default(T);
            if (String.IsNullOrEmpty(jsonStr))
                return entity;
            entity = (T)JsonConvert.DeserializeObject(jsonStr, typeof(T));
            return entity;
        }
    }
}
