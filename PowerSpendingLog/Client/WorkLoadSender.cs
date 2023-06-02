using Common;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Client
{
    public class WorkLoadSender
    {
        private readonly ConcurrentBag<string> importedFiles = new ConcurrentBag<string>();

        private readonly DBType dbType;
        private readonly string path;
        private readonly ILoadService proxy;

        public WorkLoadSender(DBType dbType, string path, ILoadService proxy)
        {
            this.dbType = dbType;
            this.path = path;
            this.proxy = proxy;
        }


        public void SendFiles()
        {
            string[] files = GetAllFiles(path);
            foreach (string filePath in files)
            {
                SendFile(filePath);
            }
        }

        public static string[] GetAllFiles(string path)
        {
            // IMPLEMENTIRATI
            return Directory.GetFiles(path, "*.csv", SearchOption.TopDirectoryOnly);
        }


        private MemoryStream GetMemoryStream(string filePath)
        {
            // IMPLEMENTIRATI
            MemoryStream ms = new MemoryStream();
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(ms);
                fileStream.Close();
            }
            return ms;
        }

        public void SendFile(string filePath)
        {
            if (importedFiles.Contains(filePath))
            {
                Console.WriteLine($"File {Path.GetFileName(filePath)} has already been uploaded.");
                return;
            }
            var fileName = Path.GetFileName(filePath);
            WorkLoad workLoad = new WorkLoad(GetMemoryStream(filePath), fileName, dbType);

            try
            {
                var res = proxy.ImportWorkLoad(workLoad);

                workLoad.Dispose();

                if (res.ResultType == ResultTypes.Failed)
                {
                    Console.WriteLine($"Upload file {fileName} unsuccesful. Error message: {res.ResultMessage}");
                }
                else
                {
                    Console.WriteLine($"Upload file {fileName} imported succesfully.");
                    importedFiles.Add(filePath);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }


        }
    }
}
