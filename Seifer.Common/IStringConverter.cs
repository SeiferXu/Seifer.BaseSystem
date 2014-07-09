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

namespace Seifer.Common
{
	public interface IStringConverter
	{
		string ToString(byte[] source);
		byte[] ToByteArray(string source);
	}
}
