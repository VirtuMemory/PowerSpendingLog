using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public interface ILoadRepository
    {
        void AddAudit(Audit audit);
        Load GetLoad(DateTime timestamp);
        void UpdateLoad(Load load);
        void AddImportedFile(ImportedFile importedFile);
        List<Load> GetAllLoads(DateTime timeStemp);
    }
}
