using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seifer.Common
{
    public class PowerCalculateCommon
    {
        /// <summary>
        /// 根据次方数,计算
        /// </summary>
        /// <param name="power">次方数例如:2^0,就是0这个参数</param>
        /// <returns></returns>
        public static int PowerCalculate(List<int> power)
        {
            int ret = 0;
            if (power != null && power.Count() > 0)
            {
                ret = (int)Math.Pow(2, power[0]);

                for (int i = 1; i < power.Count(); i++)
                {
                    ret = ret | (int)Math.Pow(2, power[i]);
                }
            }
            return ret;
        }

        /// <summary>
        /// 次方数的计算
        /// </summary>
        /// <param name="oldData">原始数据</param>
        /// <param name="isAdd">加/减</param>
        /// <param name="power">次方数例如:2^0,就是0这个参数</param>
        /// <returns></returns>
        public static int PowerCalculate(int oldData, bool isAdd, List<int> power)
        {
            int ret = 0;
            if (isAdd)
                ret = (oldData | PowerCalculate(power));
            else
                ret = (oldData & ~PowerCalculate(power));
            return ret;
        }

        /// <summary>
        /// 根据次方数,判断存在
        /// </summary>
        /// <param name="oldData">原始数据</param>
        /// <param name="power">次方数例如:2^0,就是0这个参数</param>
        /// <returns></returns>
        public static bool PowerCalculate(int oldData, int power)
        {
            int temp = oldData & (int)Math.Pow(2, power);
            if (temp == (int)Math.Pow(2, power))
                return true;
            return false;
        }

        /// <summary>
        /// 根据次方的结果,计算
        /// </summary>
        /// <param name="powerValue">2的次方结果的集合</param>
        /// <returns></returns>
        public static int PowerCalculateV(List<int> powerValue)
        {
            int ret = 0;
            if (powerValue != null && powerValue.Count() > 0)
            {
                ret = (int)powerValue[0];

                for (int i = 1; i < powerValue.Count(); i++)
                {
                    ret = ret | (int)powerValue[i];
                }
            }
            return ret;
        }

        public static int PowerCalculateV<T>(List<T> powerValue) where T : struct
        {
            int ret = 0;
            if (powerValue != null && powerValue.Count() > 0)
            {
                ret = Convert.ToInt32(powerValue[0]);

                for (int i = 1; i < powerValue.Count(); i++)
                {
                    ret = ret | Convert.ToInt32(powerValue[i]);
                }
            }
            return ret;
        }
        /// <summary>
        /// 根据次方的结果,判断存在
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static bool PowerCalculateV(int oldData, int power)
        {
            int temp = oldData & power;
            if (temp == (int)power)
                return true;
            return false;
        }

        public static bool PowerCalculateV(decimal oldData, int power)
        {
            int temp = Convert.ToInt32(oldData) & power;
            if (temp == (int)power)
                return true;
            return false;
        }
        /// <summary>
        /// 根据次方的结果,判断存在
        /// </summary>
        /// <param name="oldData">原始数据</param>
        /// <param name="isAdd">加/减</param>
        /// <param name="power">加/减的数字</param>
        /// <returns></returns>
        public static int PowerCalculateV(int oldData, bool isAdd, int power)
        {
            int ret = 0;
            if (isAdd)
                ret = (oldData | power);
            else
                ret = (oldData & ~power);
            return ret;
        }
    }
}
