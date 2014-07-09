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
using System.Collections.Generic;
using System.Xml;
using System.Configuration;
using System.Xml.Serialization;

namespace Seifer.Common
{
	public class SectionValueItemCache
	{
		private const int		CheckInterval = 2;	// [sec]

		private const string	SectionTagName  = "Section";
		private const string	IDName          = "id";
		private const string	DescriptionName = "description";
		private const string	RootName        = "SectionValueConfiguration";
		private const string	NodeName        = "/" + RootName + "/" + SectionTagName;
		private const string	TagName         = "add";
		private const string	KeyName         = "key";
		private const string	ValueName       = "value";

		private static SectionValueItemCache _singleton = null;
		private static DateTime _lastCheck = DateTime.MinValue;

		private string		_dirName;
		private string		_extention;
		private SortedList	_configList;
		private SortedList	_fileDateList;

		private SectionValueItemCache()
		{
			_configList   = new SortedList();
			_fileDateList = new SortedList();
			_dirName = ".";
			_extention = ".xml";
			try
			{
				string strDirectory = SectionValueConfigurationHandler.GetConfig("SectionValueConfiguration");
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
			SortedList slSectionList = new SortedList();

			using(Mutex mutex = new Mutex(false, typeof(SectionValueItemCache).FullName))
			{
				try
				{
					mutex.WaitOne();
					SectionValueConfiguration items;
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(SectionValueConfiguration));

					using(StreamReader streamReader = new StreamReader(path))
					{
						items = (SectionValueConfiguration)xmlSerializer.Deserialize(streamReader);
					}

					if (items.Items == null)
					{
						return null;
					}

					foreach(SectionValueConfigurationSection section in items.Items)
					{
						SortedList slAddList = new SortedList();
						try
						{
							foreach(SectionValueConfigurationSectionAdd add in section.add)
							{
								if(!slAddList.Contains(add.key))
								{
									slAddList.Add(add.key, add.value);
								}
							}
							if(!slSectionList.Contains(section.id))
							{
								slSectionList.Add(section.id, slAddList);
							}
						}
						catch(Exception)
						{

						}
					}
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}

			return slSectionList;
		}

		private static void Remove(string fullName)
		{
			SectionValueItemCache cache = Singleton;

			cache._configList.Remove(fullName);
			cache._fileDateList.Remove(fullName);
		}

		private static SectionValueItemCache CreateConfiguration(string fullName)
		{
			SectionValueItemCache	cache = Singleton;
			string strPath = Path.Combine(Directory, fullName);
			FileInfo				fileInfo;

			lock(typeof(SectionValueItemCache))
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

			SortedList sectionList = Deserialize(strPath);
			lock(typeof(SectionValueItemCache))
			{
				cache._configList.Add(fullName, sectionList);
				cache._fileDateList.Add(fullName, fileInfo.LastWriteTime);
			}

			return cache;
		}

		private static SectionValueItemCache Singleton
		{
			get
			{
				lock(typeof(SectionValueItemCache))
				{
					if(_singleton == null)
					{
						_singleton = new SectionValueItemCache();
					}
				}
				return _singleton;
			}
		}

		public static string Directory
		{
			get
			{
				lock(typeof(SectionValueItemCache))
				{
					return Singleton._dirName;
				}
			}
		}

		public static string Extention
		{
			get
			{
				return Singleton._extention;
			}
		}

		private static SortedList GetSections(string fileName)
		{
			string strFullName = fileName + Extention;
			CreateConfiguration(strFullName);

			SortedList slItems;
			lock(typeof(SectionValueItemCache))
			{
				slItems = (SortedList)Singleton._configList[strFullName];
			}
			if(slItems == null)
			{
				return null;
			}

			return slItems;
		}

		public static ICollection GetSectionKeys(string fileName)
		{
			return GetSections(fileName).Keys;
		}

		public static ICollection GetSectionKeys(string fileName, string sectionName)
		{
			string strFullName = fileName + Extention;
			SectionValueItemCache me = CreateConfiguration(strFullName);
			SortedList slList = (SortedList)me._configList;

			SortedList slSections = (SortedList)slList[strFullName];
			if(slSections == null)
			{
				return null;
			}

			SortedList slAdd = (SortedList)slSections[sectionName];
			if(slAdd == null)
			{
				return null;
			}

			return slAdd.Keys;
		}

        public static SortedList GetItems(string fileName, string sectionName)
		{
			string strFullName = fileName + Extention;
			SectionValueItemCache me = CreateConfiguration(strFullName);
			SortedList slList = (SortedList)me._configList;

			SortedList slSections = (SortedList)slList[strFullName];
			if(slSections == null)
			{
				return null;
			}

			SortedList slAdd = (SortedList)slSections[sectionName];
			if(slAdd == null)
			{
				return null;
			}

            return slAdd;
		}

		public static string GetAppSetting(string fileName, string sectionName, string key)
		{
			string strFullName = fileName + Extention;
			SectionValueItemCache cache = CreateConfiguration(strFullName);
			SortedList slList = (SortedList)cache._configList;

			SortedList slSections = (SortedList)slList[strFullName];
			if(slSections == null)
			{
				return null;
			}

			SortedList slAdd = (SortedList)slSections[sectionName];
			if(slAdd == null)
			{
				return null;
			}

			return (string)slAdd[key];
		}

		public static void SetAppSetting(string fileName, string sectionName, string dsctiption, string key, string value)
		{
			string strFullName = fileName + Extention;
			string strPath = Path.Combine(Directory, strFullName);

            FileInfo fi = new FileInfo(strPath);
            if (!fi.Exists)
            {
                CreateAppSetting(fileName);
            }
			using(Mutex mutex = new Mutex(false, typeof(SectionValueItemCache).FullName))
			{
				try
				{
					mutex.WaitOne();
					XmlDocument doc = new XmlDataDocument();
					doc.Load(strPath);
					XmlNode root = doc.DocumentElement;

					if(GetAppSetting(fileName, sectionName, key) == null)
					{
						string strNode = String.Format("{0}[@{1}='{2}']", NodeName, IDName, sectionName);

						XmlNodeList sectionList = doc.SelectNodes(strNode);
						if(sectionList.Item(0) == null)
						{
							XmlNode section = doc.CreateElement(SectionTagName);
							XmlAttribute attrId   = doc.CreateAttribute(IDName);
							XmlAttribute attrDesc = doc.CreateAttribute(DescriptionName);
							attrId.Value          = sectionName;
							attrDesc.Value        = dsctiption;
							section.Attributes.SetNamedItem(attrId);
							section.Attributes.SetNamedItem(attrDesc);
							root.AppendChild(section);
							sectionList = doc.SelectNodes(strNode);
						}
						XmlNode nodeSection    = sectionList.Item(0);
						XmlNode nodeAdd        = doc.CreateElement(TagName);
						XmlAttribute attrKey   = doc.CreateAttribute(KeyName);
						XmlAttribute attrValue = doc.CreateAttribute(ValueName);
						attrKey.Value          = key;
						attrValue.Value        = value;
						nodeAdd.Attributes.SetNamedItem(attrKey);
						nodeAdd.Attributes.SetNamedItem(attrValue);
						nodeSection.AppendChild(nodeAdd);
					}
					else
					{
						string strNode = String.Format("{0}[@{1}='{2}']/{3}[@{4}='{5}']",
								NodeName, IDName, sectionName, TagName, KeyName, key);
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

			lock(typeof(SectionValueItemCache))
			{
				Remove(strFullName);
			}
		}

		public static void SetAppSetting(string fileName, string sectionName, string key, string value)
		{
			SetAppSetting(fileName, sectionName, String.Empty, key, value);
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

		public static void CreateAppSetting(string fileName, string sectionName, string key, string value)
		{
			CreateAppSetting(fileName);
			SetAppSetting(fileName, sectionName, key, value);
		}

		public static void DeleteSection(string fileName, string sectionName)
		{
			string strFullName = fileName + Extention;
			string strPath = Path.Combine(Directory, strFullName);

			using(Mutex mutex = new Mutex(false, typeof(SectionValueItemCache).FullName))
			{
				try
				{
					mutex.WaitOne();
					XmlDocument doc = new XmlDataDocument();
					doc.Load(strPath);

					XmlNode rootNode = doc.DocumentElement;

					string strNode = String.Format("{0}[@{1}='{2}']",
						NodeName, IDName, sectionName);
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

			lock(typeof(SectionValueItemCache))
			{
				Remove(strFullName);
			}
		}

		public static void DeleteAppSetting(string fileName, string sectionName, string key)
		{
			string strFullName = fileName + Extention;
			string strPath = Path.Combine(Directory, strFullName);

			using(Mutex mutex = new Mutex(false, typeof(SectionValueItemCache).FullName))
			{
				try
				{
					mutex.WaitOne();
					XmlDocument doc = new XmlDataDocument();
					doc.Load(strPath);
					XmlNode rootNode = doc.DocumentElement;

					string strKey = String.Format("{0}[@{1}='{2}']/{3}[@{4}='{5}']",
						NodeName, IDName, sectionName, TagName, KeyName, key);
					XmlNodeList xmlKeyNodeList = rootNode.SelectNodes(strKey);
					foreach(XmlNode xmlKeyNode in xmlKeyNodeList)
					{
						xmlKeyNode.ParentNode.RemoveChild(xmlKeyNode);
					}

					string strNode = String.Format("{0}[@{1}='{2}']",
						NodeName, IDName, sectionName);
					XmlNodeList xmlSectionNodeList = rootNode.SelectNodes(strNode);
					foreach(XmlNode xmlSectionNode in xmlSectionNodeList)
					{
						if(xmlSectionNode.ChildNodes.Count == 0)
						{
							rootNode.RemoveChild(xmlSectionNode);
						}
					}
					doc.Save(strPath);
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}

			lock(typeof(SectionValueItemCache))
			{
				Remove(strFullName);
			}
		}
	}
}
