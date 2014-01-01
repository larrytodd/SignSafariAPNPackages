using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
 

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
           UtilityFunctions.CreateManifest(path);
           UtilityFunctions.CreateSignature(path,Path.Combine(certPath,certName),certPassword);
        }
    }
}
