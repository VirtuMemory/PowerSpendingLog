using Common;
using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dataBaseType = ConfigurationManager.AppSettings["DatabaseType"];

            if (!Enum.TryParse(dataBaseType, out DBType type))
                type = DBType.INMEMORY;
            Console.WriteLine($"{type} is being used.");

            ChannelFactory<ILoadService> factory = new ChannelFactory<ILoadService>("LoadService");
            ILoadService proxy = factory.CreateChannel();
            while (true)
            {
                Console.WriteLine("Enter the directory path from which you want to load .csv files");
                var path = Console.ReadLine();
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("You have entered a non-existent path, please try again");
                    continue;
                }

                WorkLoadSender sender = new WorkLoadSender(type, path, proxy);
                sender.SendFiles();

                Console.WriteLine("Enter EXIT to exit the program or any key to continue with the input");
                if (Console.ReadLine().ToUpper().Equals("EXIT"))
                    break;
            }

        }
    }
}
