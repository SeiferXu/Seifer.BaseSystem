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
using System.IO;
using System.Threading;
using System.Collections;
using System.Xml;

namespace Seifer.Common
{
	public class NameValueItemCache
	{ 
		private const int		CheckInterval = 2;	// [sec]

		private const string	TagName   = "add";
		private const string	RootName  = "appSettings";
		private const string	NodeName  = "/" + RootName + "/" + TagName;
		private const string	KeyName   = "key";
		private const string	ValueName = "value";

		private static NameValueItemCache _singleton = null;
		private static DateTime _lastCheck = DateTime.MinValue;

		private string		_dirName;
		private string		_extention;
		private SortedList	_configList;
		private SortedList	_fileDateList;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private NameValueItemCache()
		{
			_configList   = new SortedList();
			_fileDateList = new SortedList();
			_dirName = ".";
			_extention = ".xml";
			try
			{
				string strDirectory = NameValueConfigurationHandler.GetConfig("NameValueConfiguration");
				if(strDirectory != null)
				{
					_dirName = strDirectory;
				}
			}
			catch
			{
			}
		}

		private static SortedList Deserialize(string path)
		{
			string		strKey, strValue;
			SortedList	slResult = new SortedList();

			using(Mutex mutex = new Mutex(false, typeof(NameValueItemCache).FullName))
			{
				try
				{
					mutex.WaitOne();
					XmlDocument xmlDocument = new XmlDataDocument();
					xmlDocument.Load(path);

					XmlNode root = xmlDocument.DocumentElement;
					XmlNodeList select = xmlDocument.SelectNodes(NodeName);
					for(int i = 0; i < select.Count; i++)
					{
						strKey   = select.Item(i).SelectSingleNode("@" + KeyName).Value;
						strValue = select.Item(i).SelectSingleNode("@" + ValueName).Value;
						slResult.Add(strKey, strValue);
					}
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}

			return slResult;
		}

		private static void Remove(string fullName)
		{
			NameValueItemCache me = Singleton;

			me._configList.Remove(fullName);
			me._fileDateList.Remove(fullName);
		}

		private static NameValueItemCache CreateConfiguration(string fullName)
		{
			NameValueItemCache	cache = Singleton;
			string strPath = Path.Combine(Directory, fullName);
			FileInfo fileInfo;

			lock(typeof(NameValueItemCache))
			{
				if(cache._configList.Contains(fullName))
				{
					DateTime dtmNow  = DateTime.Now;
					if(_lastCheck.AddSeconds(CheckInterval) > dtmNow)
					{
						return cache;
					}
					_lastCheck = dtmNow;

					fileInfo = new FileInfo(strPath);
					if(fileInfo.LastWriteTime == (DateTime)cache._fileDateList[fullName])
					{
						return cache;
					}
					Remove(fullName);
				}
				else
				{
					fileInfo = new FileInfo(strPath);
				}
			}

			SortedList slItemsList = Deserialize(strPath);
			lock(typeof(NameValueItemCache))
			{
				cache._configList.Add(fullName, slItemsList);
				cache._fileDateList.Add(fullName, fileInfo.LastWriteTime);
			}

			return cache;
		}

		private static NameValueItemCache Singleton
		{
			get
			{
				lock(typeof(NameValueItemCache))
				{
					if(_singleton == null)
					{
						_singleton = new NameValueItemCache();
					}
				}
				return _singleton;
			}
		}

		private static string Directory
		{
			get
			{
				return Singleton._dirName;
			}
		}

		public static string Extention
		{
			get
			{
				return Singleton._extention;
			}
		}

		public static ICollection GetItemKeys(string fileName)
		{
			SortedList slItems = GetList(fileName);
			if (slItems == null)
			{
				return null;
			}

			return slItems.Keys;
		}

		public static ICollection GetItems(string fileName)
		{
			SortedList slItems = GetList(fileName);
			if (slItems == null)
			{
				return null;
			}

			return slItems.Values;
		}

		private static SortedList GetList(string fileName)
		{
			string strFullName = fileName + Extention;
			NameValueItemCache me = CreateConfiguration(strFullName);
			SortedList slIist = (SortedList)me._configList;

			return (SortedList)slIist[strFullName];
		}

		public static string GetAppSetting(string fileName, string key)
		{
			string strFullName = fileName + Extention;
			NameValueItemCache me = CreateConfiguration(strFullName);
			SortedList slList = (SortedList)me._configList;

			SortedList slItems = (SortedList)slList[strFullName];
			if(slItems == null)
			{
				return null;
			}

			return (string)slItems[key];
		}

		public static void SetAppSetting(string fileName, string key, string value)
		{
			string strFullName = fileName + Extention;
			string strPath = Path.Combine(Directory, strFullName);

            FileInfo fi = new FileInfo(strPath);
            if (!fi.Exists)
            {
                CreateAppSetting(fileName);
            }

			using(Mutex mutex = new Mutex(false, typeof(NameValueItemCache).FullName))
			{
				try
				{
					mutex.WaitOne();
					XmlDocument doc = new XmlDocument();
					doc.Load(strPath);
					XmlNode root = doc.DocumentElement;

					if(GetAppSetting(fileName, key) == null)
					{
						XmlNode insert = doc.CreateElement(TagName);
						XmlAttribute attrKey   = doc.CreateAttribute(KeyName);
						XmlAttribute attrValue = doc.CreateAttribute(ValueName);
						attrKey.Value   = key;
						attrValue.Value = value;
						insert.Attributes.SetNamedItem(attrKey);
						insert.Attributes.SetNamedItem(attrValue);
						root.AppendChild(insert);
					}
					else
					{
						string strNode = String.Format("{0}[@{1}='{2}']", NodeName, KeyName, key);
						XmlNodeList update = doc.SelectNodes(strNode);
						for(int i = 0; i < update.Count; i++)
						{
							update.Item(i).SelectSingleNode("@" + ValueName).Value = value;
						}
					}
					doc.Save(strPath);
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}

			lock(typeof(NameValueItemCache))
			{
				Remove(strFullName);
			}
		}

		public static void CreateAppSetting(string fileName)
		{
			string strPath = Path.Combine(Directory, fileName + Extention);

			if(File.Exists(strPath) == true)
			{
				return;
			}

			using(StreamWriter sw = new StreamWriter(strPath, false, Encoding.UTF8))
			{
				sw.Write(String.Format(
					"<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}<{1} />",
					Environment.NewLine, RootName));
			}
		}

		public static void CreateAppSetting(string fileName, string key, string value)
		{
			CreateAppSetting(fileName);
			SetAppSetting(fileName, key, value);
		}
		
		public static void DeleteAppSetting(string fileName, string key)
		{
			string strFullName = fileName + Extention;
			string strPath = Path.Combine(Directory, strFullName);

			using(Mutex mutex = new Mutex(false, typeof(NameValueItemCache).FullName))
			{
				try
				{
					mutex.WaitOne();
					XmlDocument doc = new XmlDataDocument();
					doc.Load(strPath);
					XmlNode rootNode = doc.DocumentElement;

					string strNode = String.Format("{0}[@{1}='{2}']",
						NodeName, KeyName, key);
					XmlNodeList xmlNodeList = rootNode.SelectNodes(strNode);
					foreach(XmlNode xmlNode in xmlNodeList)
					{
						rootNode.RemoveChild(xmlNode);
					}
					doc.Save(strPath);
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			
			lock(typeof(NameValueItemCache))
			{
				Remove(strFullName);
			}
		}
	}
}
