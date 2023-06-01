using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Database
{
    public class XMLLoadRepository : ILoadRepository
    {
        private readonly string _auditFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Audits.xml");
        private readonly string _loadFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Loads.xml");
        private readonly string _importedFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "ImportedFiles.xml");

        public void AddAudit(Audit audit)
        {
            var audits = DeserializeFromFile<List<Audit>>(_auditFilePath) ?? new List<Audit>();
            audits.Add(audit);
            SerializeToFile(audits, _auditFilePath);
        }

        public void AddImportedFile(ImportedFile importedFile)
        {
            var importedFiles = DeserializeFromFile<List<ImportedFile>>(_importedFilePath) ?? new List<ImportedFile>();
            importedFiles.Add(importedFile);
            SerializeToFile(importedFiles, _importedFilePath);
        }

        public Load GetLoad(DateTime timestamp)
        {
            var loads = DeserializeFromFile<List<Load>>(_loadFilePath) ?? new List<Load>();
            return loads.FirstOrDefault(l => l.Timestamp == timestamp);
        }

        public void UpdateLoad(Load load)
        {
            var loads = DeserializeFromFile<List<Load>>(_loadFilePath);
            if (loads == null) throw new ArgumentException($"No load with ID {load.Id} exists to update.");
            var existingLoadIndex = loads.FindIndex(l => l.Id == load.Id);
            if (existingLoadIndex == -1) loads.Add(load);
            else loads[existingLoadIndex] = load;
            SerializeToFile(loads, _loadFilePath);
        }

        private T DeserializeFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default;

            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        private void SerializeToFile<T>(T data, string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, data);
            }
        }
    }
}
