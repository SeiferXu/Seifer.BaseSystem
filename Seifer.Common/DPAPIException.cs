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

namespace Seifer.Common
{
	/// <summary>
	/// DPAPILib 
	/// </summary>
	[Serializable]
	internal class DPAPIException: ApplicationException
	{
		internal DPAPIException( string message, Exception e ) : base(message, e)
		{
		} 

		public DPAPIException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) :	base(info, context) 
		{ 
		} 
	} 
}
