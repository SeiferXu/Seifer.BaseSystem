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
using System.Collections;
using System.IO;

namespace Seifer.Common
{
	public class CompareToFileInfo : IComparer
	{
		public const int LastWriteTime = 1;

		private const int Name          = 2;

		public const int Asc  = 1;	

		private const int Desc = -1;	

		private int Type    = LastWriteTime;

		private int OrderBy = Asc;

		public CompareToFileInfo()
		{
		}

		public CompareToFileInfo(int type, int orderby) 
		{
			Type    = type;
			OrderBy = orderby;
		}

		public int Compare(object x, object y)
		{
			FileInfo fx = (FileInfo)x;
			FileInfo fy = (FileInfo)y;

			switch(Type)
			{
				case LastWriteTime:
					return (fx.LastWriteTime.CompareTo(fy.LastWriteTime) * OrderBy);
				case Name:
					return (fx.Name.CompareTo(fy.Name) * OrderBy);
			}
			return 0;
		}
	}
}
