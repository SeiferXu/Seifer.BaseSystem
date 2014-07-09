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
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Xml.Serialization;
//using System.Web.Services.Protocols;

namespace Seifer.Common
{
	public class Serialization
	{
		public static byte[] Serialize(object source)
		{
			using(MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, source);
				return ms.ToArray();
			}
		}

		public static string Serialize(IStringConverter convert, object source)
		{
			return convert.ToString(Serialize(source));
		}

        public static string Serialize<T>(T data)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (MemoryStream s = new MemoryStream())
            {
                xmlSerializer.Serialize(s, data);
                UTF8Encoding utf8 = new UTF8Encoding(false, true);
                return utf8.GetString(s.ToArray());
            }
        }

		public static object Deserialize(byte[] source)
		{
			using(MemoryStream ms = new MemoryStream(source))
			{
				BinaryFormatter bf = new BinaryFormatter();
				return (object)bf.Deserialize(ms);
			}
		}

		public static object Deserialize(IStringConverter convert, string source)
		{
			return Deserialize(convert.ToByteArray(source));
		}

		public static string Encrypt(IStringConverter convert, object source, byte[] key)
		{
			return Encrypt(convert, source, key, key);
		}

        public static string EncryptDES(IStringConverter convert, string source, string key)
        {
            string strResult = String.Empty;
            
                MemoryStream msDest = new MemoryStream();
                try
                {
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                    //BinaryFormatter binaryFormatter = new BinaryFormatter();
                    //binaryFormatter.Serialize(memoryStream, source);
                    //byte[] buf = memoryStream.ToArray();

                    byte[] keys = new PasswordDeriveBytes(key, null).GetBytes(8);
                    MemoryStream ms = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(ms, des.CreateEncryptor(keys, keys), CryptoStreamMode.Write);
                    try
                    {
                        byte[] buf = Encoding.UTF8.GetBytes(source);
                        cryptoStream.Write(buf, 0, buf.Length);
                        cryptoStream.FlushFinalBlock();

                        strResult = convert.ToString(ms.ToArray());
                    }
                    finally
                    {
                        cryptoStream.Close();
                    }
                }
                finally
                {
                    msDest.Close();
                }

            return strResult;

            //byte[] inputByteArray = Encoding.Default.GetBytes(source);  
        }

		public static string Encrypt(IStringConverter convert, object source, byte[] key, byte[] iv)
		{
			string strResult = String.Empty;

			MemoryStream memoryStream = new MemoryStream();
			try
			{
				MemoryStream msDest = new MemoryStream();
				try{
					TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
					CryptoStream cryptoStream = new CryptoStream(msDest,
							tdes.CreateEncryptor(key, iv), CryptoStreamMode.Write);

					try
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						binaryFormatter.Serialize(memoryStream, source);
						byte[] buf = memoryStream.ToArray();
						cryptoStream.Write(buf, 0, buf.Length);
						cryptoStream.FlushFinalBlock();
						strResult = convert.ToString(msDest.ToArray());
					}
					finally
					{
						cryptoStream.Close();
					}
				}
				finally
				{
					msDest.Close();
				}
			}
			finally
			{
				memoryStream.Close();
			}

			return strResult;
		}

		public static object Decrypt(IStringConverter convert, string source, byte[] key)
		{
			return Decrypt(convert, source, key, key);
		}

		public static object Decrypt(IStringConverter convert, string source, byte[] key, byte[] iv)
		{
			object objResult = null;
			byte[] btSource = convert.ToByteArray(source);

			MemoryStream memoryStream = new MemoryStream(btSource);
			try
			{
				TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
				CryptoStream cryptoStream = new CryptoStream(memoryStream,
						tdes.CreateDecryptor(key, iv), CryptoStreamMode.Read);
				try
				{
					BinaryFormatter binaryFormatter  = new BinaryFormatter();
					objResult = (object)binaryFormatter.Deserialize(cryptoStream);
				}
				finally
				{
					cryptoStream.Close();
				}
			}
			finally
			{
				memoryStream.Close();
			}

			return objResult;
		}

		public static string EncryptString(IStringConverter convert, string source, byte[] key)
		{
			return EncryptString(convert, source, key, key);
		}

		public static string EncryptString(IStringConverter convert, string source, byte[] key, byte[] iv)
		{
			string strResult = String.Empty;

			MemoryStream memoryStream = new MemoryStream();
			try
			{
				MemoryStream msDest = new MemoryStream();
				try{
					TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
					CryptoStream cryptoStream = new CryptoStream(msDest,
							tdes.CreateEncryptor(key, iv), CryptoStreamMode.Write);
					try
					{
						byte[] btBuffer =  Encoding.UTF8.GetBytes(source);
						cryptoStream.Write(btBuffer, 0, btBuffer.Length);
						cryptoStream.FlushFinalBlock();
						strResult = convert.ToString(msDest.ToArray());
					}
					finally
					{
						cryptoStream.Close();
					}
				}
				finally
				{
					msDest.Close();
				}
			}
			finally
			{
				memoryStream.Close();
			}

			return strResult;
		}

		public static string DecryptString(IStringConverter convert, string source, byte[] key)
		{
			return DecryptString(convert, source, key, key);
		}

		public static string DecryptString(IStringConverter convert, string source, byte[] key, byte[] iv)
		{
			string strResult = String.Empty;
			byte[] btSource = convert.ToByteArray(source);

			MemoryStream memoryStream = new MemoryStream(btSource);
			try
			{
				TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
				CryptoStream cryptoStream = new CryptoStream(memoryStream,
						tdes.CreateDecryptor(key, iv), CryptoStreamMode.Read);
				try
				{
					byte[] btBuffer = new byte[source.Length];	
					int iLength = cryptoStream.Read(btBuffer, 0, btBuffer.Length);
					strResult = Encoding.UTF8.GetString(btBuffer,0,iLength);
				}
				finally
				{
					cryptoStream.Close();
				}
			}
			finally
			{
				memoryStream.Close();
			}

			return strResult;
		}

		public static void CreateRSAKeys(out string publicKey, out string privateKey)
		{
			CspParameters CSPParam = new CspParameters();
			CSPParam.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CSPParam);

			publicKey  = rsa.ToXmlString(false);
			privateKey = rsa.ToXmlString(true);
		}

		public static string Encrypt(IStringConverter convert, object source, string publicKey)
		{
			string strResult = String.Empty;

			CspParameters CSPParam = new System.Security.Cryptography.CspParameters();
			CSPParam.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CSPParam);

			rsa.FromXmlString(publicKey);

			MemoryStream memoryStream = new MemoryStream();
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, source);
				byte[] buf = memoryStream.ToArray();
				byte[] btEncrypted = rsa.Encrypt(buf, false);
				strResult = convert.ToString(btEncrypted);
			}
			finally
			{
				memoryStream.Close();
			}

			return strResult;
		}

		public static object Decrypt(IStringConverter convert, string source, string privateKey)
		{
			object objResult = null;
			byte[] btSource = convert.ToByteArray(source);

			CspParameters CSPParam = new System.Security.Cryptography.CspParameters();
			CSPParam.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider rsa =  new RSACryptoServiceProvider(CSPParam);

			rsa.FromXmlString(privateKey);

			byte[] btDecrypted = rsa.Decrypt(btSource, false);
			MemoryStream memoryStream = new MemoryStream(btDecrypted);
			try
			{
				BinaryFormatter binaryFormatter  = new BinaryFormatter();
				objResult = (object)binaryFormatter.Deserialize(memoryStream);
			}
			finally
			{
				memoryStream.Close();
			}

			return objResult;
		}

        public static string DecryptDES(IStringConverter convert, string source, string key)
        {
            string objResult = null;
            byte[] btSource = convert.ToByteArray(source);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] keys = new PasswordDeriveBytes(key, null).GetBytes(8);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(keys, keys), CryptoStreamMode.Write))
                {
                    cs.Write(btSource, 0, btSource.Length);
                    cs.FlushFinalBlock();
                    objResult = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            return objResult;
        }

		public static string EncryptString(IStringConverter convert, string source, string publicKey)
		{
			byte[] buf = Encoding.UTF8.GetBytes(source);

			CspParameters CSPParam = new System.Security.Cryptography.CspParameters();
			CSPParam.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CSPParam);

			rsa.FromXmlString(publicKey);
			byte[] btEncrypted = rsa.Encrypt(buf, false);

			return convert.ToString(btEncrypted);
		}

		public static string DecryptString(IStringConverter convert, string source, string privateKey)
		{
			byte[] btSource = convert.ToByteArray(source);

			CspParameters CSPParam = new System.Security.Cryptography.CspParameters();
			CSPParam.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider rsa =  new RSACryptoServiceProvider(CSPParam);

			rsa.FromXmlString(privateKey);
			byte[] btDecrypted = rsa.Decrypt(btSource, false);

			return Encoding.UTF8.GetString(btDecrypted);
		}

		public static string GetMD5HashString(IStringConverter convert, string source)
		{
			string strResult = String.Empty;
			byte[] btBuffer =  Encoding.UTF8.GetBytes(source);

			MemoryStream memoryStream = new MemoryStream(btBuffer);
			try
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] btResult = md5.ComputeHash(memoryStream);
				strResult = convert.ToString(btResult);
			}
			finally
			{
				memoryStream.Close();
			}

			return strResult;
		}

		public static string CreateNewGuid()
		{
			return Guid.NewGuid().ToString().ToUpper();
		}

        public static Guid CreateGuid()
        {
            return Guid.NewGuid();
        }

		public static string CreateNewGuid(IStringConverter convert)
		{
			Guid guid = Guid.NewGuid();
			return convert.ToString(guid.ToByteArray());;
		}
	}
}
