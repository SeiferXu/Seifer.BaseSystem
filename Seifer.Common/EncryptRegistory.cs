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
using System.Security.Cryptography;

namespace Seifer.Common
{
	public class EncryptRegistory
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

		public static byte[] Get3DESKey(string regPathName,string key)
		{
			string strEncryptKey = GetRegistory(regPathName,key);
			byte[] salt = Encoding.UTF8.GetBytes(strEncryptKey.Length.ToString());  
			PasswordDeriveBytes secretKey = new PasswordDeriveBytes(strEncryptKey, salt);  
			return secretKey.GetBytes(24);
		}

		public static byte[] Get3DESIV(string regPathName,string key)
		{
			string strEncryptIV = GetRegistory(regPathName,key);
			byte[] btSalt = Encoding.UTF8.GetBytes(strEncryptIV.Length.ToString());  
			PasswordDeriveBytes secretKey = new PasswordDeriveBytes(strEncryptIV, btSalt);  
			return secretKey.GetBytes(24);
		}

		private static void SetRegistory(string key, string value)
		{
			string strSubKey = null;
			string strName = null;
			DevideKey(key, ref strSubKey, ref strName);

			string strEncryptValue = Convert.ToBase64String(
				DPAPI.Encrypt(
				Encoding.Default.GetBytes(value), 
				null, 
				DPAPI.Store.USE_MACHINE_STORE));

			RegistryUtil.HKLMWrite(strSubKey, strName, strEncryptValue);
		}

		private static string GetRegistory(string key)
		{
			string strSubKey = null;
			string strName = null;
			DevideKey(key, ref strSubKey, ref strName);

			string strEncryptValue = (string)RegistryUtil.HKLMRead(strSubKey, strName);
			string strDecryptValue = null;

			if (strEncryptValue != null)
			{
				strDecryptValue = Encoding.Default.GetString(
					DPAPI.Decrypt(
					Convert.FromBase64String(strEncryptValue), null, DPAPI.Store.USE_MACHINE_STORE));
			}

			return strDecryptValue;
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
			string subKey = null;
			string name = null;
			DevideKey(key, ref subKey, ref name);

			RegistryUtil.HKLMDelete(subKey, name);
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
