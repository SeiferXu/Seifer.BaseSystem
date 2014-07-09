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
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

namespace Seifer.Common
{
    public class NameValueConfigurationHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlNode node = section.SelectSingleNode("@directory");

            if (node == null || node.Equals(string.Empty))
            {
                throw new System.NullReferenceException("Not node found");
            }

            return node.Value;
        }

        public static string GetConfig(string section)
        {
            return Convert.ToString(System.Configuration.ConfigurationManager.GetSection(section));
        }
    }
}
