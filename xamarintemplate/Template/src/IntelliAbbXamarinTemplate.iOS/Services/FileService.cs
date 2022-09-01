using IntelliAbbXamarinTemplate.Services;
using Foundation;
using System.IO;

namespace IntelliAbbXamarinTemplate.iOS.Services
{
    public class FileService : IFileService
    {
        public string GetCssFilePath()
        {
            return NSBundle.MainBundle.BundlePath;
        }

        public string GetCacheFilePath()
        {
            var fileName = "appdatabasename.db3";
            var dirPath = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User)[0];

            if (string.IsNullOrEmpty(dirPath))
                throw new FileNotFoundException("Directory path to the file is not valid");


            var path = Path.Combine(dirPath, fileName);

            object pass = new object();
            lock (pass)
            {
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
            }
            return path;
        }
    }
}
