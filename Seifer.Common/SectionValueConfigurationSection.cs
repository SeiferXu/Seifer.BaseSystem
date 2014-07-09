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
	public class SectionValueConfigurationSection
	{
		[XmlElementAttribute("add", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public SectionValueConfigurationSectionAdd[] add;

		[XmlAttributeAttribute()]
		public string id;

		[XmlAttributeAttribute()]
		public string description;
	}

}
