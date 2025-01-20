using System.Security.Cryptography;
using System.Text;

namespace TCPClient.Services
{
	public class AESHelper : IEncryptionService
	{
		private readonly string _key;
		private readonly string _iv;

		public AESHelper(string key, string iv)
		{
			_key = key;
			_iv = iv;
		}

		public string Encrypt(string plainText)
		{
			try
			{
				using (Aes aesAlg = Aes.Create())
				{
					aesAlg.Key = Encoding.UTF8.GetBytes(_key);
					aesAlg.IV = Encoding.UTF8.GetBytes(_iv);

					ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

					using (MemoryStream msEncrypt = new MemoryStream())
					{
						using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						{
							using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
							{
								swEncrypt.Write(plainText);
							}
						}

						return Convert.ToBase64String(msEncrypt.ToArray());
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in encryption: {ex.Message}");
				return null;
			}
		}

		public string Decrypt(string cipherText)
		{
			try
			{
				using (Aes aesAlg = Aes.Create())
				{
					aesAlg.Key = Encoding.UTF8.GetBytes(_key);
					aesAlg.IV = Encoding.UTF8.GetBytes(_iv);

					ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

					using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
					{
						using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{
							using (StreamReader srDecrypt = new StreamReader(csDecrypt))
							{
								return srDecrypt.ReadToEnd();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in decryption: {ex.Message}");
				return null;
			}
		}
	}
}
