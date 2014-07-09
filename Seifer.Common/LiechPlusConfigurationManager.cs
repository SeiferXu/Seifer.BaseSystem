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
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;

namespace Seifer.Common
{
    public class LiechPlusConfigurationManager
	{
		public static string GetCustomAppSetting(string tagName, string key)
		{
			NameValueCollection nameValues;
			nameValues = (NameValueCollection)
					System.Configuration.ConfigurationManager.GetSection(tagName);

			return nameValues[key];
		}

		public static string GetAppSetting(string key)
		{
            return System.Configuration.ConfigurationManager.AppSettings[key];
		}

        public static string GetConnection(string key)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

		public static string GetAppSetting(string fileName, string key)
		{
			return NameValueItemCache.GetAppSetting(fileName, key);
		}

		public static string GetAppSetting(string fileName, string sectionName, string key)
		{
			return SectionValueItemCache.GetAppSetting(fileName, sectionName, key);
		}

		public static void SetAppSetting(string fileName, string key, string value)
		{
			NameValueItemCache.SetAppSetting(fileName, key, value);
		}

		public static void SetAppSetting(string fileName, string sectionName, string key, string value)
		{
			SectionValueItemCache.SetAppSetting(fileName, sectionName, key, value);
		}

        //public static string GetUserSetting(string fileName, string key)
        //{
        //    return UserValueItemCache.GetAppSetting(fileName, key);
        //}

        //public static void SetUserSetting(string fileName, string key, string value)
        //{
        //    UserValueItemCache.SetAppSetting(fileName, key, value);
        //}

		public static ConfigurationException CreateException(string fileName, string key)
		{
			return new ConfigurationException(String.Format("{0}({1})is not found", fileName, key));
		}

		public static ConfigurationException CreateException(string fileName, string section, string key)
		{
            return new ConfigurationException(String.Format("{0}({1}:{2})is not found", fileName, section, key));
		}
		
		public static void DeleteAppSetting(string fileName, string key)
		{
			NameValueItemCache.DeleteAppSetting(fileName, key);
		}

		public static void DeleteAppSetting(string fileName, string sectionName, string key)
		{
			SectionValueItemCache.DeleteAppSetting(fileName, sectionName, key);
		}
		
		public static void DeleteSection(string fileName, string sectionName)
		{
			SectionValueItemCache.DeleteSection(fileName, sectionName);
		}
	}
}
