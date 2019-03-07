using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Com.Viauco.JsonOrm
{
    class EncryptStream
    {
        private static byte[] key = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };

        public static String read(String filePath)
        {
            String data = String.Empty;

            FileStream input = new FileStream(filePath,FileMode.Open, FileAccess.Read);
            using (SymmetricAlgorithm algo = SymmetricAlgorithm.Create()) // Creates the default implementation, which is RijndaelManaged. 
            {
                algo.Padding = PaddingMode.None;
                using (CryptoStream stream = new CryptoStream(input, algo.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    
                    StreamReader reader = new StreamReader(stream);
                    data = reader.ReadToEnd();

                    reader.Close();
                    stream.Close();
                }
            }
            return data;
        }

        public static void write(string outputPath, string data)
        {
            
            FileStream output = new FileStream(outputPath,FileMode.OpenOrCreate, FileAccess.Write);

            using (SymmetricAlgorithm algo = SymmetricAlgorithm.Create()) //Creates the default implementation, which is RijndaelManaged. 
            {
                algo.Padding = PaddingMode.None;
                using (CryptoStream stream = new CryptoStream(output, algo.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    byte[] bytes = ASCIIEncoding.ASCII.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                    output.Close();
                    stream.Close();
                }
            }
        }
    }
}
