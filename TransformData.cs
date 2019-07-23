using System;
using System.Collections.Generic;
using System.Linq;

namespace DataTransform
{
    // Alias for the structure presenting input data:
    // key is a product name and a value is a list of Tuples of ("Origin Year", "Development Year2", "Incremental Value")
    using InputTableDic = Dictionary<string, List<Tuple<int, int, double>>>;

    public class TransformData
    {
        /// <summary>
        /// InputTables are stored in Dictionary where Key is a product name 
        /// and Value is a list of Tuples of ("Origin Year", "Development Year2", "Incremental Value")
        /// </summary>
        private InputTableDic _inputTables = new InputTableDic();

        /// <summary>
        /// _resMatrices presents a list of matrices presenting input data; a key is a table name 
        /// and a value is a 2D matrix containing a data from an input file converted into table.
        /// </summary>
        private Dictionary<string, double[,]> _resultMatrices = new Dictionary<string, double[,]>();

        /// <summary>
        /// _size is a number of development years 
        /// </summary>
        private int _size;

        private int _earliestOriginYear;
        private readonly IMatrixLoader _matrixLoader;
        private readonly IPrintMatrix _printMatrix;

        public TransformData(IMatrixLoader matrixLoader, IPrintMatrix printMatrix)
        {
            _matrixLoader = matrixLoader;
            _printMatrix = printMatrix;
        }

        /// <summary>
        /// These are top level actions we take.
        /// </summary>
        public void IncrementalToCumulative()
        {
            if (!LoadDataFromFile())
                return;
            InitIncrementalMatrices();
            IncrementalDataToAccumulative();
            PrintAccumulatedTriangleMatrices();
        }

        private void InitEarliestYearAndSize()
        {
            int latestOriginYear = _inputTables.Max(e => e.Value.Max(t => t.Item1));
            _earliestOriginYear = _inputTables.Min(e => e.Value.Min(t => t.Item1));
            _size = latestOriginYear - _earliestOriginYear + 1;
        }

        private bool LoadDataFromFile()
        {
            if (!_matrixLoader.LoadDataFromFile())
            {
                return false;
            }

            _inputTables = _matrixLoader.InputTables;
            InitEarliestYearAndSize();
            return true;
        }

        /// <summary>
        /// This function populates the basic working structure - 2D matrix - with input data.
        /// For indexing is used normalized Origin and Development year's value by subtracting from them the _earliestOriginYear.
        /// </summary>
        private void InitIncrementalMatrices()
        {
            foreach (var inputTable in _inputTables)
            {
                double[,] incementalMatrix = new double[_size, _size];

                for (int i = 0; i < inputTable.Value.Count; i++)
                {
                    int row = inputTable.Value[i].Item1 - _earliestOriginYear;
                    int column = inputTable.Value[i].Item2 - inputTable.Value[i].Item1;

                    incementalMatrix[row, column] = inputTable.Value[i].Item3;
                }
                _resultMatrices.Add(inputTable.Key, incementalMatrix);
            }
        }

        private void IncrementalDataToAccumulative()
        {
            foreach (var matrix in _resultMatrices)
            {
                for (int row = 0, size = _size; row < _size; row++, size--)
                {
                    for (int col = 1; col < size; col++)
                    {
                        matrix.Value[row, col] += matrix.Value[row, col - 1];
                    }
                }
            }
        }

        private void PrintAccumulatedTriangleMatrices()
        {
            _printMatrix.PrintTriangleMatrices(_earliestOriginYear, _size, _resultMatrices);
        }
    }
}
