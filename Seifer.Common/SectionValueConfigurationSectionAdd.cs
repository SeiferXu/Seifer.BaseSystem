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
using System.Xml.Serialization;

namespace Seifer.Common
{
	[Serializable]
	public class SectionValueConfigurationSectionAdd
	{
		[XmlAttributeAttribute()]
		public string key;

		[XmlAttributeAttribute()]
		public string value;
	}
}
