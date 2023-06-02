using Common;
using Database;
using System;
using System.IO;

namespace Service
{
    public class FileProcessingHelper
    {
        public LoadType ParseLoadType(string fileName)
        {
            return fileName.StartsWith("forecast") ? LoadType.Forecast
                          : fileName.StartsWith("measured") ? LoadType.Measured
                          : throw new FormatException("Unknown file type.");
        }

        public string ReadWorkLoadText(WorkLoad workLoad)
        {
            workLoad.MS.Position = 0; 
            StreamReader reader = new StreamReader(workLoad.MS);
            return reader.ReadToEnd();
        }

        public string[] ParseCSVText(string text)
        {
            return text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public bool IsCorrectLinesCount(string[] lines)
        {
            return CalculateLinesCount(lines) >= 23 && CalculateLinesCount(lines) <= 25;
        }

        public int CalculateLinesCount(string[] lines)
        {
            var linesCount = lines.Length;

            if (lines[0].Contains("TIME_STAMP"))
            {
                linesCount--;
            }

            if (lines[lines.Length - 1].Equals(""))
            {
                linesCount--;
            }

            return linesCount;
        }

        public Result CreateResultAndAudit(ResultTypes resultType, string message, ILoadRepository loadRepository)
        {
            Result result = new Result { ResultType = resultType, ResultMessage = message };
            CreateAudit(message, loadRepository);
            return result;
        }

        public void CreateAudit(string message, ILoadRepository loadRepository)
        {
            var audit = new Audit { Message = message, Timestamp = DateTime.Now };
            loadRepository.AddAudit(audit);
        }

        public void CreateImportedFile(string fileName, ILoadRepository loadRepository)
        {
            var importedFile = new ImportedFile { FileName = fileName };
            loadRepository.AddImportedFile(importedFile);
        }

    }
}
