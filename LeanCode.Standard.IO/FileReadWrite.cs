using LeanCode.Standard.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LeanCode.Standard.IO
{
    public class FileReadWrite
    {
        private object _synclock = new object();
        public void Append(string fileName, string text)
        {
            lock (_synclock)
            {
                if (!File.Exists(fileName))
                {
                    using(var stream = File.Create(fileName)){

                    }
                }
                using (StreamWriter w = File.AppendText(fileName))
                {
                    w.WriteLine(text);
                }
            }
        }

        public string GetOrCreateDatedFullPath(string baseFilePath, string groupFolder, string fileName)
        {
            var currentDateDirectoryName = $"{baseFilePath}{"\\"}{DateTime.Today.ToString("yyyyMMdd")}{"\\"}";
            if (!string.IsNullOrEmpty(groupFolder))
            {
                currentDateDirectoryName = $"{currentDateDirectoryName}{groupFolder}{"\\"}";
            }
            if (!Directory.Exists(currentDateDirectoryName))
            {
                Directory.CreateDirectory(currentDateDirectoryName);
            }
            return  $"{currentDateDirectoryName}{fileName}";
        }

        public List<T> GetData<T>(string filePath)
        {
            var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fullPath = $"{basePath}\\{filePath}";
            if (!File.Exists(fullPath))
            {
                return new List<T>();
            }
            var contents = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<List<T>>(contents);
        }

        public List<string> GetContentsByLine(string filePath)
        {
            var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fullPath = $"{basePath}\\{filePath}";
            if (!File.Exists(fullPath))
            {
                return new List<string>();
            }
            var contents = File.ReadAllLines(fullPath);
            return contents.ToList();
        }
    }
}
