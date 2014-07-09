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
using Microsoft.Win32;

namespace Seifer.Common
{
    public class RegistryUtil
	{
        public static void HKLMWrite(string subKey, string name, object value)
		{
			RegistryKey keyHKLM = Registry.LocalMachine;
			RegistryKey targetKey = keyHKLM.OpenSubKey("SOFTWARE").CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

			targetKey.SetValue(name, value);
			targetKey.Close();
		}

        public static object HKLMRead(string subKey, string name)
		{
			RegistryKey keyHKLM = Registry.LocalMachine;
            RegistryKey targetKey = keyHKLM.OpenSubKey("SOFTWARE").OpenSubKey(subKey);
			if (targetKey == null) 
			{
				return null;
			}
			return targetKey.GetValue(name);
		}

        public static void HKLMDelete(string subKey, string name)
		{
			RegistryKey keyHKLM = Registry.LocalMachine;
            RegistryKey targetKey = keyHKLM.OpenSubKey("SOFTWARE").OpenSubKey(subKey, true);
			targetKey.DeleteValue(name);
		}
	}
}
