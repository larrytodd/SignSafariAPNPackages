using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
 

namespace SignSafariAPNPackages
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = string.Empty;
            string certPath = string.Empty;
            string certName = string.Empty;
            string certPassword = string.Empty;
            foreach(string arg in args)
            {
                if(arg.StartsWith("path="))
                {
                    path = arg.Remove(0, 5);
                }
                if(arg.StartsWith("certpath="))
                {
                    certPath = arg.Remove(0, 9);
                }
                if(arg.StartsWith("certname"))
                {
                    certName = arg.Remove(0, 9);
                }
                if(arg.StartsWith("password"))
                {
                    certPassword = arg.Remove(0, 9);
                }
            }
            CreateManifest(path);
            CreateSignature(path,Path.Combine(certPath,certName),certPassword);
        }
        private static void CreateManifest(string path)
        {
            //Add the file name and hash value to dictionary and then create a json object.
            Dictionary<string, string> file = new Dictionary<string, string>();
            StringBuilder sb;
            foreach (string fileName in GetRawFiles())
            {
                try
                {
                    using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Open))
                    {
                        using (BufferedStream bs = new BufferedStream(fs))
                        {
                            using (SHA1Managed sha1 = new SHA1Managed())
                            {
                                byte[] hash = sha1.ComputeHash(bs);
                                sb = new StringBuilder(2 * hash.Length);
                                foreach (byte b in hash)
                                {
                                    sb.AppendFormat("{0:X2}", b);
                                }
                            }
                        }
                    }
                }
                catch(IOException ex)
                {
                    throw new Exception("Problem creating manifest" + " Error: " + ex.Message + " Stack: " + ex.StackTrace);
                }
                file.Add(fileName, sb.ToString());
            }
            string jsonString = JsonConvert.SerializeObject(file, Formatting.Indented);
            if (File.Exists(Path.Combine(path,"manifest.json")))
            {
                File.Delete(Path.Combine(path,"manifest.json"));
            }
            try 
            {
                //Build file using JSON object
                using (FileStream fs = File.Create(Path.Combine(path,"manifest.json")))
                {
                    Byte[] content = new UTF8Encoding(true).GetBytes(jsonString);
                    fs.Write(content, 0, content.Length);
                }
            }
            catch(IOException ex)
            {
                throw new Exception("Problem creating manifest" + " Error: " + ex.Message + " Stack: " + ex.StackTrace);
            }
        }
        private static void CreateSignature(string path, string certFullPath, string certPassword)
        {
            if (File.Exists(Path.Combine(path,"manifest.json")))
            {
                string manifest = File.ReadAllText(Path.Combine(path, "manifest.json"));
                Byte[] content = new UTF8Encoding(true).GetBytes(manifest);
                try
                {
                    ContentInfo contentInfo = new ContentInfo(content);
                    SignedCms signedCMS = new SignedCms(contentInfo, true);
                    //Create detached pck#7 signature using cert obtained from Apple Developer Network
                    System.Security.Cryptography.X509Certificates.X509Certificate2 cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certFullPath, certPassword);
                    CmsSigner signer = new CmsSigner(cert);
                    signer.IncludeOption = X509IncludeOption.EndCertOnly;
                    signedCMS.ComputeSignature(signer);
                    using (FileStream fs = File.Create(Path.Combine(path, "signature")))
                    {
                        Byte[] contentToWrite = signedCMS.Encode();
                        fs.Write(contentToWrite, 0, contentToWrite.Length);
                    }
                }
                catch(CryptographicException ex)
                {
                    throw new Exception("Problem creating signature" + " Error: " + ex.Message + " Stack: " + ex.StackTrace);
                }
                catch(IOException ex)
                {
                    throw new Exception("Problem creating manifest" + " Error: " + ex.Message + " Stack: " + ex.StackTrace);
                }
            }
            else
            {
                throw new Exception("Cannot create signature of manifest.json. File does not exist");
            }
        }
        private static string[] GetRawFiles()
        {
            //Required package structure for images and dictionary
            return new string[]{
                "icon.iconset/icon_16x16.png",
                "icon.iconset/icon_16x16@2x.png",
                "icon.iconset/icon_32x32.png",
                "icon.iconset/icon_32x32@2x.png",
                "icon.iconset/icon_128x128.png",
                "icon.iconset/icon_128x128@2x.png",
                "website.json" 
            };
        }

        private static byte[] GetHash(string input)
        {
            HashAlgorithm algorithm = SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
        private static string GetHash(Stream s, HashAlgorithm hasher)
        {
            var hash = hasher.ComputeHash(s);
            var hashStr = Convert.ToBase64String(hash);
            return hashStr.TrimEnd('=');
        }
    }
}
