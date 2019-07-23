using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataTransform
{
    // Alias for the structure presenting input data:
    // key is a product name and a value is a list of Tuples of ("Origin Year", "Development Year2", "Incremental Value")
    using InputTableDic = Dictionary<string, List<Tuple<int, int, double>>>;

    public interface IMatrixLoader
    {
        bool LoadDataFromFile();
        InputTableDic InputTables { get; }
    }

    /// <summary>
    /// This class loads and verifies data from input file.
    /// </summary>
    public class MatrixLoader : IMatrixLoader
    {
        private readonly IErrorHandler _errorHandler;
        private IInputDataToMemory _inputDataToMemory;
        private readonly IFileServices _fileServices;
        public static string DefaultInputFileName = "Input.txt";

        public MatrixLoader(IErrorHandler errorHandler, IInputDataToMemory inputDataToMemory, IFileServices fileServices)
        {
            _errorHandler = errorHandler;
            _inputDataToMemory = inputDataToMemory;
            _fileServices = fileServices;
        }

        public InputTableDic InputTables { get; private set; }

        public bool LoadDataFromFile()
        {
            List<string> inputFileLines;

            try
            {
                string inputFilePath = _fileServices.GetOpenFileName(DefaultInputFileName);
                _errorHandler.SetLogFilePath(Path.GetDirectoryName(inputFilePath));
                inputFileLines = _fileServices.ReadLines(inputFilePath).ToList();
            }
            catch (Exception)
            {
                ShowError("MaxDevYear");
                return false;
            }

            if (!_inputDataToMemory.HeaderValidated(inputFileLines[0]))
            {
                ShowError("Invalid file header.");
                return false;
            }

            for (int i = 1; i < inputFileLines.Count; i++)
            {
                if (!_inputDataToMemory.AddLine(inputFileLines[i]))
                {
                    //ShowError("Invalid input file.");
                    //return false;
                }
            }
            InputTables = _inputDataToMemory.InputTables;
            return true;
        }

        void ShowError(string msg)
        {
            _fileServices.MessageBox(string.Format("{0} The program terminated", msg));
            _errorHandler.LogError(msg);
        }
    }
}
