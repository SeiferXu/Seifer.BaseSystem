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
using System.Reflection;
using System.Runtime.InteropServices;

namespace Seifer.Common
{
	internal class DPAPI
	{
		[DllImport("Crypt32.dll", SetLastError=true,
			 CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		private static extern bool CryptProtectData(
			ref DATA_BLOB pDataIn,
			String szDataDescr,
			ref DATA_BLOB pOptionalEntropy,
			IntPtr pvReserved,
			ref CRYPTPROTECT_PROMPTSTRUCT pPromptStruct,
			int dwFlags,
			ref DATA_BLOB pDataOut);

		[DllImport("Crypt32.dll", SetLastError=true,
			 CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		private static extern bool CryptUnprotectData(
			ref DATA_BLOB pDataIn,
			String szDataDescr,
			ref DATA_BLOB pOptionalEntropy,
			IntPtr pvReserved,
			ref CRYPTPROTECT_PROMPTSTRUCT pPromptStruct,
			int dwFlags,
			ref DATA_BLOB pDataOut);

		[DllImport("kernel32.dll",
			 CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		private unsafe static extern int FormatMessage(int dwFlags,
			ref IntPtr lpSource,
			int dwMessageId,
			int dwLanguageId,
			ref String lpBuffer, int nSize,
			IntPtr *Arguments);

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			internal struct DATA_BLOB
		{
			public int cbData;
			public IntPtr pbData;
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			internal struct CRYPTPROTECT_PROMPTSTRUCT
		{
			public int cbSize;
			public int dwPromptFlags;
			public IntPtr hwndApp;
			public String szPrompt;
		}

		static private IntPtr NullPtr = ((IntPtr)((int)(0)));

		private const int CRYPTPROTECT_UI_FORBIDDEN = 0x1;
		private const int CRYPTPROTECT_LOCAL_MACHINE = 0x4;

		internal enum Store 
		{

			USE_MACHINE_STORE = 0, 

			USE_USER_STORE = 1
		};

		internal DPAPI( )
		{
		} 

		internal static byte[] Encrypt(byte[] plainText, byte[] optionalEntropy, Store store)
		{
			bool retVal = false;
			DATA_BLOB plainTextBlob = new DATA_BLOB();
			DATA_BLOB cipherTextBlob = new DATA_BLOB();
			DATA_BLOB entropyBlob = new DATA_BLOB();
			CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();
			byte[] cipherText = null;

			InitPromptstruct(ref prompt);
			int dwFlags;
			try
			{
				try
				{
					int bytesSize = plainText.Length;
					plainTextBlob.pbData = Marshal.AllocHGlobal(bytesSize);
					if(IntPtr.Zero == plainTextBlob.pbData)
					{
						throw new DPAPIException("No Data", null);
					}
					plainTextBlob.cbData = bytesSize;
					Marshal.Copy(plainText, 0, plainTextBlob.pbData, bytesSize);
				}
				catch(Exception ex)
				{
					throw new DPAPIException("Error", ex);
				}
				if(Store.USE_MACHINE_STORE == store)
				{
					dwFlags = CRYPTPROTECT_LOCAL_MACHINE|CRYPTPROTECT_UI_FORBIDDEN;

					if(null == optionalEntropy)
					{
						optionalEntropy = new byte[0];
					}
					try
					{
						int bytesSize = optionalEntropy.Length;
						entropyBlob.pbData = Marshal.AllocHGlobal(optionalEntropy.Length);;
						if(IntPtr.Zero == entropyBlob.pbData)
						{
							throw new DPAPIException
                                ("No Data", null);
						}
						Marshal.Copy(optionalEntropy, 0, entropyBlob.pbData, bytesSize);
						entropyBlob.cbData = bytesSize;
					}
					catch(Exception ex)
					{
                        throw new DPAPIException("Error", ex);
					}
				}
				else
				{
					dwFlags = CRYPTPROTECT_UI_FORBIDDEN;
				}
				retVal = CryptProtectData(ref plainTextBlob, "", ref entropyBlob,
					IntPtr.Zero, ref prompt, dwFlags,
					ref cipherTextBlob);
				if(false == retVal)
				{
                    throw new DPAPIException("Error" +
						GetErrorMessage(Marshal.GetLastWin32Error()), null);
				}

				cipherText = new byte[cipherTextBlob.cbData];
				Marshal.Copy(cipherTextBlob.pbData, cipherText, 0, cipherTextBlob.cbData);
			}
			catch(Exception ex)
			{
                throw new DPAPIException("Error", ex);
			}
			finally
			{
				if( IntPtr.Zero != plainTextBlob.pbData )
				{
					Marshal.FreeHGlobal( plainTextBlob.pbData );
				}
				if( IntPtr.Zero != entropyBlob.pbData )
				{
					Marshal.FreeHGlobal( entropyBlob.pbData );
				}
				if( IntPtr.Zero != cipherTextBlob.pbData )
				{
					Marshal.FreeHGlobal( cipherTextBlob.pbData );
				}
			} //-- finally

			return cipherText;
		} //-- public static byte[] Encrypt(byte[] plainText, byte[] optionalEntropy, Store store)

		internal static byte[] Decrypt(byte[] cipherText, byte[] optionalEntropy, Store store)
		{
			bool retVal = false;
			DATA_BLOB plainTextBlob = new DATA_BLOB();
			DATA_BLOB cipherBlob = new DATA_BLOB();
			DATA_BLOB entropyBlob = new DATA_BLOB();
			byte[] plainText = null;

			CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();
			InitPromptstruct(ref prompt);
			try
			{
				try
				{
					int cipherTextSize = cipherText.Length;
					cipherBlob.pbData = Marshal.AllocHGlobal(cipherTextSize);
					if(IntPtr.Zero == cipherBlob.pbData)
					{
						throw new DPAPIException("No Data", null);
					}
					cipherBlob.cbData = cipherTextSize;
					Marshal.Copy(cipherText, 0, cipherBlob.pbData, cipherBlob.cbData);
				}
				catch(Exception ex)
				{
                    throw new DPAPIException("Error", ex);
				}

				int dwFlags;
				if(Store.USE_MACHINE_STORE == store)
				{
					dwFlags = CRYPTPROTECT_LOCAL_MACHINE|CRYPTPROTECT_UI_FORBIDDEN;

					if(null == optionalEntropy)
					{
						optionalEntropy = new byte[0];
					}
					try
					{
						int bytesSize = optionalEntropy.Length;
						entropyBlob.pbData = Marshal.AllocHGlobal(bytesSize);
						if(IntPtr.Zero == entropyBlob.pbData)
						{
							throw new DPAPIException
								("No Data", null);
						}
						entropyBlob.cbData = bytesSize;
						Marshal.Copy(optionalEntropy, 0, entropyBlob.pbData, bytesSize);
					}
					catch(Exception ex)
					{
                        throw new DPAPIException("Error", ex);
					}
				}
				else
				{
					dwFlags = CRYPTPROTECT_UI_FORBIDDEN;
				}
				retVal = CryptUnprotectData(ref cipherBlob, null, ref entropyBlob,
					IntPtr.Zero, ref prompt, dwFlags,
					ref plainTextBlob);
				if(false == retVal)
				{
                    throw new DPAPIException("Error" +
						GetErrorMessage(Marshal.GetLastWin32Error()), null);
				}

				plainText = new byte[plainTextBlob.cbData];
				Marshal.Copy(plainTextBlob.pbData, plainText, 0, plainTextBlob.cbData);
			}
			catch(Exception ex)
			{
                throw new DPAPIException("Error", ex);
			}
			finally
			{
				if(IntPtr.Zero != cipherBlob.pbData)
				{
					Marshal.FreeHGlobal(cipherBlob.pbData);
				}
				if(IntPtr.Zero != entropyBlob.pbData)
				{
					Marshal.FreeHGlobal(entropyBlob.pbData);
				}
				if(IntPtr.Zero != plainTextBlob.pbData)
				{
					Marshal.FreeHGlobal(plainTextBlob.pbData);
				}
			} //-- finally

			return plainText;
		} //-- ublic static byte[] Decrypt(byte[] cipherText, byte[] optionalEntropy, Store store)

		private static void InitPromptstruct(ref CRYPTPROTECT_PROMPTSTRUCT ps)
		{
			ps.cbSize = Marshal.SizeOf(typeof(CRYPTPROTECT_PROMPTSTRUCT));
			ps.dwPromptFlags = 0;
			ps.hwndApp = NullPtr;
			ps.szPrompt = null;
		} //-- private void InitPromptstruct(ref CRYPTPROTECT_PROMPTSTRUCT ps)

		private unsafe static String GetErrorMessage(int errorCode)
		{
			int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
			int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
			int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
			int messageSize = 255;
			String lpMsgBuf = "";
			int dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM |
				FORMAT_MESSAGE_IGNORE_INSERTS;
			IntPtr ptrlpSource = new IntPtr();
			IntPtr prtArguments = new IntPtr();
			int retVal = FormatMessage(dwFlags, ref ptrlpSource, errorCode, 0,
				ref lpMsgBuf, messageSize, &prtArguments);
			if(0 == retVal)
			{
                throw new DPAPIException("Error" +
					errorCode, null);
			}
			return lpMsgBuf;
		} //-- private unsafe static String GetErrorMessage(int errorCode)
	} //-- public class DPAPI
}
