using DddsUtils.Logging.NetStandard;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace emri_service.Auth
{
	// A collection of classes from DDDSUtils.Security used for MRR token generation (see CreateMRRToken() below)
	public class MRRTokenManager : IMRRTokenManager
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;

        private readonly int _tokenValidInMinutes;
        private readonly byte[] _tokenKey;
        private readonly string _secretPhrase;


        public MRRTokenManager(IConfiguration configuration, ILogFactory logFactory)
	    {
            Configuration = configuration;
            _logger = logFactory.GetLogger("MRRTokenManager");

            _tokenValidInMinutes = int.TryParse(Configuration["MRR:Token:tokenValidMinutes"], out int number) ? number : 720;
            _tokenKey = Encoding.ASCII.GetBytes((Configuration["MRR:Token:tokenKey"] ?? string.Empty).PadRight(32, '-'));
            _secretPhrase = Configuration["MRR:Token:secretPhrase"] ?? string.Empty;
        }

        public class TokenDetails
        {
            public int UserId { get; set; }
            public string IpAddress { get; set; }
            public DateTime Timestamp { get; set; }
            public string SecretPhrase { get; set; }
            public string ApplicationName { get; set; }
            public string ClientId { get; set; }
        }

        public string GetMRRToken()
        {
            var tokenDetails = new TokenDetails
            {
                ApplicationName = "MRR",
                UserId = 1831,
                IpAddress = "127.0.0.1",
                Timestamp = DateTime.Now
            };

            try
            {
                return GenerateToken(tokenDetails);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return string.Empty;
        }

        public TokenDetails GetTokenDetails(string token)
        {
            try
            {
                string decryptedToken = Calligraphy.Decompose(token, _tokenKey);
                string[] tokenParts = decryptedToken.Split('|');
                var details = new TokenDetails
                {
                    SecretPhrase = tokenParts.ElementAtOrDefault(0) ?? string.Empty,
                    UserId = int.Parse(tokenParts.ElementAtOrDefault(1)),
                    IpAddress = tokenParts.ElementAtOrDefault(2) ?? string.Empty,
                    Timestamp = new DateTime(long.Parse(tokenParts.ElementAtOrDefault(3))),
                    ApplicationName = tokenParts.ElementAtOrDefault(4) ?? string.Empty,
                    ClientId = tokenParts.ElementAtOrDefault(5) ?? string.Empty
                };

                return details;
            }
            catch (Exception ex)
            {
                throw new SecurityException("Failed to decrypt or deserialize token [{token}]", ex);
            }
        }

        public bool IsValidToken(string base64token, out TokenDetails details)
        {
            try
            {
                details = GetTokenDetails(base64token);
                if (details == null)
                {
                    throw new SecurityException("Could not decrypt the token.");

                }
                else
                {
                    bool validSecret = details.SecretPhrase.ToLower().Equals(_secretPhrase);
                    double span = DateTime.Now.Subtract(details.Timestamp).TotalMinutes;

                    if (!validSecret)
                    {
                        throw new SecurityException("Secret does not match while validating token");
                    }

                    if (span > _tokenValidInMinutes)
                    {
                        throw new SecurityException("Token expired in minute count: {span}; tokenValidMinutes: {TokenValidInMinutes}");

                    }

                    return validSecret && (span < _tokenValidInMinutes);
                }
            }
            catch (Exception e)
            {
                throw new SecurityException("Failed to validated token", e);

            }
        }

        private string GenerateToken(TokenDetails details)
        {
            var token = Calligraphy.Compose(string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                _secretPhrase, details.UserId, details.IpAddress, details.Timestamp.Ticks.ToString(),
                details.ApplicationName, details.ClientId), _tokenKey);

            if (token == null)
            {
                throw new SecurityException("Failed to generate token");

            }
            return token;
        }

        private class Calligraphy
        {
            private static readonly string DELIMITER = "@@@@DDDS@@@@";

            /// <param name="plainText"></param>
            /// <param name="key"></param>
            /// <returns>Base64 encoded representation of encrypted plaintext</returns>
            internal static string Compose(string plainText, byte[] key)
            {
                byte[] encrypted;
                // Create an AesManaged object
                // with the specified key and IV.

                //add 16 bytes of garbage that is stripped out on decryption
                plainText = string.Format("{0}{1}{2}", Guid.NewGuid().ToString("n").Substring(0, 16), DELIMITER, plainText);

                try
                {
                    using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                    {
                        aesAlg.Key = key;
                        aesAlg.GenerateIV();

                        // Create a decryptor to perform the stream transform.
                        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                        // Create the streams used for encryption.
                        using (MemoryStream msEncrypt = new MemoryStream())
                        {
                            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }

                    string encryptedText = Convert.ToBase64String(encrypted);

                    // Return the encrypted bytes from the memory stream.
                    return encryptedText;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            /// <param name="cipherText">Base64 Encoded representation of encrypted string</param>
            /// <param name="key"></param>
            /// <returns>Decrypted string in plaintext</returns>
            internal static string Decompose(string cipherText, byte[] key)
            {
                // Declare the string used to hold
                // the decrypted text.
                string plaintext = null;

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                try
                {
                    // Create an AesManaged object
                    // with the specified key and IV.
                    using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                    {
                        aesAlg.Key = key;
                        aesAlg.GenerateIV();

                        // Create a decrytor to perform the stream transform.
                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                        // Create the streams used for decryption.
                        using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                var pb = new byte[cipherBytes.Length];
                                var l = csDecrypt.Read(pb, 0, pb.Length);
                                plaintext = Encoding.ASCII.GetString(pb.Take(l).ToArray());
                            }
                        }
                    }

                    return plaintext.Substring(plaintext.IndexOf(DELIMITER) + DELIMITER.Length);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}
