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

	[XmlRootAttribute(Namespace="", IsNullable=false)]
	[Serializable]
	public class SectionValueConfiguration
	{

		[XmlElementAttribute("Section", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public SectionValueConfigurationSection[] Items;
	}

	
}
