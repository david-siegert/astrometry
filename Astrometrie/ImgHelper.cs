using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Astrometrie
{
    class ImgHelper
    {
        public static byte[] ImgToByteArray(string path)
        {
            //omezení na 4GB ?
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return bytes;
        }

        public static string GetImgPath()
        {
            string folder = Properties.Settings.Default.folderPath;

            string[] filePaths = Directory.GetFiles(
                @folder, "*", 
                SearchOption.TopDirectoryOnly);

            return filePaths.Last();
        }

    }
}
