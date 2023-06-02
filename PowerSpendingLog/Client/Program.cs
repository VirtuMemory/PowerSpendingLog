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
                Console.WriteLine("Unesite putanju direktorijuma iz kojeg zelite da uciatte .csv fajlove");
                var path = Console.ReadLine();
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("Uneli ste ne postojecu putanju, pokusajte ponovo");
                    continue;
                }

                WorkLoadSender sender = new WorkLoadSender(type, path, proxy);
                sender.SendFiles();

                Console.WriteLine("Unesite EXIT za izlazak iz programa ili bilo koje dugme da nastavite sa unosom");
                if (Console.ReadLine().ToUpper().Equals("EXIT"))
                    break;
            }

        }
    }
}
