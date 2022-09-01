using System;
using System.IO;
using IntelliAbbXamarinTemplate.Services;

namespace IntelliAbbXamarinTemplate.Droid.Services
{
    public class FileService : IFileService
    {
        public string GetCacheFilePath()
        {
            var fileName = "appdatabasename.db3";
            var dirPath = Android.App.Application.Context.CacheDir?.AbsolutePath;

            if (string.IsNullOrEmpty(dirPath))
                throw new FileNotFoundException("Directory path to the file is not valid");

            var path = Path.Combine(dirPath, fileName);

            object pass = new object();
            lock (pass)
            {
                if (!File.Exists(path))
                    File.Create(path);
            }
            return path;
        }

        public string GetCssFilePath()
        {
            return "file:///android_asset/";
        }
    }
}
