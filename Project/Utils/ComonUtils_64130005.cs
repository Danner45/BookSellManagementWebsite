using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Project_64130005.Utils
{
    public class ComonUtils_64130005
    {
        public static string HashPassword(string password)
        {
            try
            {
                // Create an instance of MD5
                using (MD5 md5 = MD5.Create())
                {
                    // Compute hash of the input string
                    byte[] messageDigest = md5.ComputeHash(Encoding.UTF8.GetBytes(password));

                    // Convert the byte array to a hexadecimal string
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in messageDigest)
                    {
                        sb.Append(b.ToString("x2")); // Convert each byte to a two-character hexadecimal string
                    }

                    return sb.ToString();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error generating hash: " + e.Message);
            }
        }
    }
}