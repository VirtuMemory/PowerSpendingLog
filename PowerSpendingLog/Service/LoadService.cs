using Common;
using Database;
using System;
using System.IO;

namespace Service
{
    public class LoadService : ILoadService
    {
        public delegate void UpdateDatabaseHandler(Load load);
        public event UpdateDatabaseHandler UpdateDatabase;
        private ILoadRepository _loadRepository;
        private bool db = false;
        private int processedRows = 1;
        private int totalRows = 0;
        private static int ForecastFileId = -1;
        private static int MeasuredFileId = -1;


        private void ProcessRow(DateTime timeStemp)
        {
            
            if (processedRows == totalRows)
            {
                bool done = false;
                foreach(Load load in _loadRepository.GetAllLoads(timeStemp))
                {
                    done =  load.CalculateDeviations();
                    UpdateDatabase?.Invoke(load);

                }
                if (done)
                    Console.WriteLine($"DB updated with calculated deviations for {timeStemp.ToString("yyyy MM dd")} Loads");
            }
            processedRows++;
        }

        public Result ImportWorkLoad(WorkLoad workLoad)
        {
            Result result = new Result();
            if (!db)
            {
                _loadRepository = DatabaseFactory.CreateDatabase(workLoad.DbType);
                if (_loadRepository is XMLLoadRepository)
                {
                    UpdateDatabase += _loadRepository.UpdateLoad;
                }
                else if (_loadRepository is InMemoryLoadRepository)
                {
                    UpdateDatabase += _loadRepository.UpdateLoad;
                }
                db = true;
            }


            // Provera tipa datoteke
            var fileType = workLoad.FileName.StartsWith("forecast") ? LoadType.Forecast
                          : workLoad.FileName.StartsWith("measured") ? LoadType.Measured
                          : throw new FormatException("Nepravilan tip datoteke.");

            if (fileType == LoadType.Forecast)
            {
                ForecastFileId++;
            }
            else if (fileType == LoadType.Measured)
            {
                MeasuredFileId++;
            }

            // Čitanje svih linija iz CSV datoteke
            workLoad.MS.Position = 0; // Resetujemo poziciju na početak
            StreamReader reader = new StreamReader(workLoad.MS);
            string text = reader.ReadToEnd();


            // Parsiranje CSV stringa
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var linesCount = lines.Length;

            if (lines[0].Contains("TIME_STAMP"))
            {
                linesCount--;
            }

            if (lines[lines.Length - 1].Equals(""))
            {
                linesCount--;
            }

            // Provera da li je broj linija 23, 24 ili 25
            if (linesCount < 23 || linesCount > 25)
            {
                string message = $"Nepravilan broj linija u fajlu {workLoad.FileName}. Očekuje se 23, 24 ili 25 linija, ali je pročitano {lines.Length}.";
                result.ResultType = ResultTypes.Failed;
                result.ResultMessage = message;
                CreateAudit(message);
            }
            else
            {
                totalRows = linesCount;
                foreach (var line in lines)
                {
                    if (line.Equals("")) 
                    {
                        continue;
                    }
                    // Deljenje svake linije na sat i potrošnju
                    var parts = line.Split(',');


                    if (parts.Length != 2)
                    {
                        string message = $"Linija '{line}' u fajlu {workLoad.FileName} nije pravilno formatirana.";
                        result.ResultType = ResultTypes.Failed;
                        result.ResultMessage = message;
                        CreateAudit(message);
                        break;
                    }

                    if (parts[0].Equals("TIME_STAMP"))
                        continue;

                    var time = parts[0];
                    var consumption = double.Parse(parts[1]);

                    // Kreiranje ili ažuriranje objekta Load
                    CreateOrUpdateLoad(DateTime.Parse(time), consumption, fileType);
                }
            }

            // Kreiranje objekta ImportedFile
            CreateImportedFile(workLoad.FileName);
            processedRows = 1;
            Console.WriteLine(workLoad.FileName);
            return result;
        }

        private void CreateAudit(string message)
        {
            var audit = new Audit { Message = message, Timestamp = DateTime.Now };
            _loadRepository.AddAudit(audit);
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
            ProcessRow(timestamp);
            _loadRepository.UpdateLoad(load);
        }

        private void CreateImportedFile(string fileName)
        {
            var importedFile = new ImportedFile { FileName = fileName };
            _loadRepository.AddImportedFile(importedFile);
        }
    }
}
