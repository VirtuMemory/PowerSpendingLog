using Common;
using Database;
using System;

namespace Service
{
    public class LoadService : ILoadService
    {
        public delegate void UpdateDatabaseHandler(Load load);
        public event UpdateDatabaseHandler UpdateDatabase;
        private ILoadRepository _loadRepository;
        private readonly FileProcessingHelper _fileProcessingHelper;
        private bool db = false;
        private int processedRows = 1;
        private int totalRows = 0;
        private static int ForecastFileId = -1;
        private static int MeasuredFileId = -1;

        public LoadService()
        {
            _fileProcessingHelper = new FileProcessingHelper();
        }

        private void UpdateLoadInDatabase(Load load)
        {
            _loadRepository.UpdateLoad(load);
        }

        private void ProcessRow(DateTime timeStemp)
        {
            if (processedRows++ == totalRows)
            {
                bool done = false;
                foreach (Load load in _loadRepository.GetAllLoads(timeStemp))
                {
                    done = load.CalculateDeviations();
                    UpdateDatabase?.Invoke(load);
                }
                if (done)
                    Console.WriteLine($"DB updated with calculated deviations for {timeStemp:yyyy MM dd} Loads");
            }
        }

        public void SetLoadRepository(DBType db)
        {
            _loadRepository = DatabaseFactory.CreateDatabase(db);
            UpdateDatabase += UpdateLoadInDatabase;
        }

        public Result ImportWorkLoad(WorkLoad workLoad)
        {
            Result result;
            if (!db)
            {
                SetLoadRepository(workLoad.DbType);
                db = true;
            }

            // Provera tipa datoteke
            var fileType = _fileProcessingHelper.ParseLoadType(workLoad.FileName);
            SetFileId(fileType);

            string text = _fileProcessingHelper.ReadWorkLoadText(workLoad);

            var lines = _fileProcessingHelper.ParseCSVText(text);

            if (!_fileProcessingHelper.IsCorrectLinesCount(lines))
            {
                string message = $"Incorrect number of lines in the file {workLoad.FileName}. Expected 23, 24, or 25 lines, but {_fileProcessingHelper.CalculateLinesCount(lines)} were read.";
                result = _fileProcessingHelper.CreateResultAndAudit(ResultTypes.Failed, message, _loadRepository);
            }
            else
            {
                totalRows = _fileProcessingHelper.CalculateLinesCount(lines);
                result = ProcessWorkLoadLines(lines, workLoad, fileType);
            }
            _fileProcessingHelper.CreateImportedFile(workLoad.FileName, _loadRepository);
            processedRows = 1;
            Console.WriteLine("Data processing completed. File: " + workLoad.FileName);
            return result;
        }

        private void SetFileId(LoadType fileType)
        {
            if (fileType == LoadType.Forecast)
            {
                ForecastFileId++;
            }
            else if (fileType == LoadType.Measured)
            {
                MeasuredFileId++;
            }
        }

        private Result ProcessWorkLoadLines(string[] lines, WorkLoad workLoad, LoadType fileType)
        {
            Result result = new Result();

            foreach (string line in lines)
            {
                if (line.Equals("") || line.Contains("TIME_STAMP")) continue;

                var parts = line.Split(',');

                if (parts.Length != 2)
                {
                    string message = $"Line '{line}' in the file {workLoad.FileName} is not properly formatted.";
                    result =_fileProcessingHelper.CreateResultAndAudit(ResultTypes.Failed, message, _loadRepository);
                    break;
                }

                var time = parts[0];
                var consumption = double.Parse(parts[1]);

                // Kreiranje ili ažuriranje objekta Load
                CreateOrUpdateLoad(DateTime.Parse(time), consumption, fileType);
                ProcessRow(DateTime.Parse(time));
            }
            return result;
        }

        private void CreateOrUpdateLoad(DateTime timestamp, double consumption, LoadType loadType)
        {
            var load = _loadRepository.GetLoad(timestamp) ?? new Load { Timestamp = timestamp };

            if (loadType == LoadType.Forecast)
            {
                load.ForecastValue = consumption;
                load.ForecastFileId = ForecastFileId;
            }
            else if (loadType == LoadType.Measured)
            {
                load.MeasuredValue = consumption;
                load.MeasuredFileId = MeasuredFileId;
            }

            
            _loadRepository.UpdateLoad(load);
        }
    }
}
