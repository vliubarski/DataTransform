using System;
using System.Collections.Generic;
using System.Globalization;

namespace DataTransform
{
    // Alias for the structure presenting input data:
    // key is a product name and a value is a list of Tuples of ("Origin Year", "Development Year2", "Incremental Value")
    using InputTableDic = Dictionary<string, List<Tuple<int, int, double>>>;

    public interface IInputDataToMemory
    {
        InputTableDic InputTables { get; }
        bool AddLine(string line);
        bool HeaderValidated(string line);
    }

    /// <summary>
    /// This class stores input file in memory.
    /// After validating its header, it stores each line in dictionary where a key is a product name 
    /// and a value is a list of Tuples of ("Origin Year", "Development Year2", "Incremental Value").
    /// The alias for this structure is InputTableDic.
    /// </summary>
    public class InputDataToMemory : IInputDataToMemory
    {
        //assuming the file format is unique we have _valHeader for format validation
        static private string[] _valHeader = { "Product", "Origin Year", "Development Year", "Incremental Value" };
        readonly char[] _delimiters = { ',' };

        private readonly IErrorHandler _errorHandler;
        private const int MinOrigYear = 1900;
        private const int MaxDevYear = 2050;
        readonly List<string> _errorProducts = new List<string>();

        public InputTableDic InputTables { get; }

        public InputDataToMemory(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            InputTables = new InputTableDic();
        }

        public bool AddLine(string line)
        {
            string product = line.Split(_delimiters)[0];
            var validLine = ValidateLine(line);
            if (validLine == null)
            {
                if (InputTables.ContainsKey(product))
                {
                    InputTables.Remove(product);
                }
                return false;
            }

            if (InputTables.ContainsKey(product))
            {
                InputTables[product].Add(validLine);
            }
            else
            {
                InputTables.Add(product, new List<Tuple<int, int, double>> { validLine});
            }
            return true;
        }

        private Tuple<int, int, double> ValidateLine(string line)
        {
            string[] splittedFileLine = line.Split(_delimiters);
            int origYear, devYear;
            double incremValue;
            string product = splittedFileLine[0];

            if (string.IsNullOrEmpty(line) || _errorProducts.Contains(product) || splittedFileLine.Length != 4)
            {
                LogError("Wrong line :", line);
                return null;
            }

            if (!int.TryParse(splittedFileLine[1], out origYear))
            {
                LogError(origYear.ToString(), product);
                return null;
            }

            if (!int.TryParse(splittedFileLine[2], out devYear))
            {
                LogError(devYear.ToString(), product);
                return null;
            }

            if (!double.TryParse(splittedFileLine[3], out incremValue))
            {
                LogError(incremValue.ToString(CultureInfo.DefaultThreadCurrentCulture), product);
                return null;
            }
            if (!ValidOrigAndDevYears(origYear, devYear))
            {
                LogError(origYear + " or " + devYear, product);
                return null;
            }
            return Tuple.Create(origYear, devYear, incremValue);
        }

        private void LogError(string val, string product)
        {
            _errorHandler.LogError($"Invalid value {val} for {product}");
            _errorProducts.Add(product);
        }

        private bool ValidOrigAndDevYears(int origYear, int devYear)
        {
            return origYear >= MinOrigYear && devYear <= MaxDevYear && origYear <= devYear;
        }

        public bool HeaderValidated(string line)
        {
            string[] header = line.Split(_delimiters);

            for (int i = 0; i < header.Length; i++)
            {
                if (!header[i].Trim().Equals(_valHeader[i].Trim()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
