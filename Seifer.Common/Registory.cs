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
using System.Text;
using Microsoft.Win32;

namespace Seifer.Common
{
	public class Registory
	{
		public static void SetRegistory(string regPathName, string key, string value)
		{
			StringBuilder sbRegkey = new StringBuilder();
			sbRegkey.Append(regPathName);
			sbRegkey.Append(", ");
			sbRegkey.Append(key);
			SetRegistory(sbRegkey.ToString(), value);
		}

		public static string GetRegistory(string regPathName, string key)
		{
			StringBuilder sbRegkey = new StringBuilder();
			sbRegkey.Append(regPathName);
			sbRegkey.Append(", ");
			sbRegkey.Append(key);
			
			return GetRegistory(sbRegkey.ToString());
		}

		private static void SetRegistory(string key, string value)
		{
			string strSubKey = null;
			string strName = null;
			DevideKey(key, ref strSubKey, ref strName);

			RegistryUtil.HKLMWrite(strSubKey, strName, value);
		}

		private static string GetRegistory(string key)
		{
			string strSubKey = null;
			string strName = null;
			DevideKey(key, ref strSubKey, ref strName);

			return (string)RegistryUtil.HKLMRead(strSubKey, strName);
		}

		private static void DevideKey(string key, ref string subKey, ref string name)
		{
			if (!key.ToUpper().StartsWith(@"HKLM\")) 
			{
				throw new ArgumentException("Local Machine could be read only" + key);
			}
			if (key.Split(',').Length != 2)
			{
                throw new ArgumentException("key Invalid" + key);
			}

			subKey = key.Split(',')[0].Substring(5).Trim();
			name = key.Split(',')[1].Trim();
		}

		private static void DeleteRegistory(string key)
		{
			string strSubKey = null;
			string strName = null;
			DevideKey(key, ref strSubKey, ref strName);

			RegistryUtil.HKLMDelete(strSubKey, strName);
		}

		public static void DeleteRegistory(string regPathName, string key)
		{
			StringBuilder sbRegkey = new StringBuilder();
			sbRegkey.Append(regPathName);
			sbRegkey.Append(", ");
			sbRegkey.Append(key);
			
			DeleteRegistory(sbRegkey.ToString());
		}
	}
}
