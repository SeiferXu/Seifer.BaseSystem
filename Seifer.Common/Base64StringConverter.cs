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

namespace Seifer.Common
{
	public class Base64StringConverter : IStringConverter
	{
		public Base64StringConverter()
		{
		}

		public string ToString(byte[] source)
		{
			return Convert.ToBase64String(source);
		}

		public byte[] ToByteArray(string source)
		{
			return Convert.FromBase64String(source);
		}
	}
}
