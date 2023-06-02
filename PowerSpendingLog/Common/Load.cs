using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Load
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double ForecastValue { get; set; }
        public double MeasuredValue { get; set; }
        public double AbsolutePercentageDeviation { get; set; }
        public double SquaredDeviation { get; set; }
        public int ForecastFileId { get; set; }
        public int MeasuredFileId { get; set; }

        public static int _nextId = 0;

        public Load()
        {
            Id = _nextId ;
        }

        public bool CalculateDeviations()
        {
            if (ForecastValue == 0 || MeasuredValue == 0)
            {
                return false;
            }

            // Pretpostavimo da je ime ključa u App.config "DeviationCalculationMethod"
            var method = ConfigurationManager.AppSettings["DeviationCalculationMethod"];

            switch (method)
            {
                case "AbsolutePercentage":
                    AbsolutePercentageDeviation = Math.Abs(MeasuredValue - ForecastValue) / MeasuredValue * 100;
                    return true;
                case "Squared":
                    SquaredDeviation = Math.Pow((MeasuredValue - ForecastValue) / MeasuredValue, 2);
                    return true;
                default:
                    throw new InvalidOperationException($"Unknown method for calculating deviation: {method}");
            }
        }
    }
}
