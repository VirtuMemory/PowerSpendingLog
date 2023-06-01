using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class InMemoryLoadRepository : ILoadRepository
    {
        public Dictionary<int, Audit> IMAudit { get; set; } = new Dictionary<int, Audit>();
        public Dictionary<int, Load> IMLoad { get; set; } = new Dictionary<int, Load>();
        public Dictionary<int, ImportedFile> IMImportedFile { get; set; } = new Dictionary<int, ImportedFile>();

        public void AddAudit(Audit audit)
        {
            if (audit == null) throw new ArgumentNullException(nameof(audit));
            if (IMAudit.ContainsKey(audit.Id)) throw new ArgumentException($"Audit with ID {audit.Id} already exists.");
            IMAudit.Add(audit.Id, audit);
        }

        public void AddImportedFile(ImportedFile importedFile)
        {
            if (importedFile == null) throw new ArgumentNullException(nameof(importedFile));
            if (IMImportedFile.ContainsKey(importedFile.Id)) throw new ArgumentException($"Imported file with ID {importedFile.Id} already exists.");
            IMImportedFile.Add(importedFile.Id, importedFile);
        }

        public Load GetLoad(DateTime timestamp)
        {
            return IMLoad.Values.FirstOrDefault(load => load.Timestamp == timestamp);
        }

        public void UpdateLoad(Load load)
        {
            if (load == null) throw new ArgumentNullException(nameof(load));
            //if (!IMLoad.ContainsKey(load.Id)) throw new ArgumentException($"No load with ID {load.Id} exists to update.");
            IMLoad[load.Id] = load;
        }
    }
}
