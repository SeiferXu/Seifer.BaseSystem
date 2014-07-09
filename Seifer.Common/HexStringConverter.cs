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
	public class HexStringConverter : IStringConverter
	{
		public HexStringConverter()
		{
		}

		public string ToString(byte[] source)
		{
			StringBuilder sbHexString = new StringBuilder();

			foreach(byte b in source)
			{
				string code = Convert.ToString(b, 16);
				if(code.Length == 1)
				{
					sbHexString.Append("0");
				}
				sbHexString.Append(code);
			}

			return sbHexString.ToString();
		}

		public byte[] ToByteArray(string source)
		{
			byte[] btResult = new byte[source.Length / 2];
			int iBytes = 0;

			for(int count = 0; count < source.Length; count += 2)
			{
				btResult[iBytes++] = Convert.ToByte(source.Substring(count, 2), 16);
			}

			return btResult;
		}
	}
}
