using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataTransform
{
    public interface IPrintMatrix
    {
        void PrintTriangleMatrices(int earliestOriginYear, int matrixSize, Dictionary<string, double[,]> resultMatrices);
    }

    /// <summary>
    /// This class provides a service for printing origin year, number of development years 
    /// and accumulated matrix into result file.
    /// </summary>
    public class PrintMatrix : IPrintMatrix
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IFileServices _fileServices;

        public PrintMatrix(IErrorHandler errorHandler, IFileServices fileServices)
        {
            _errorHandler = errorHandler;
            _fileServices = fileServices;
        }

        public void PrintTriangleMatrices(int earliestOriginYear, int matrixSize, Dictionary<string, double[,]> resultMatrices)
        {
            string inputFilePath = _fileServices.GetSaveFileName("Output.txt");

            try
            {
                using (StreamWriter file = new StreamWriter(inputFilePath))
                {
                    file.WriteLine("{0}, {1}", earliestOriginYear, matrixSize);
                    StringBuilder builder = new StringBuilder();

                    foreach (var matrix in resultMatrices)
                    {
                        builder.AppendFormat("{0}", matrix.Key);
                        for (int row = 0, size = matrixSize; row < matrixSize; row++, size--)
                        {
                            for (int col = 0; col < size; col++)
                            {
                                builder.AppendFormat(", {0}", matrix.Value[row, col]);
                            }
                        }
                        file.WriteLine(builder);
                        builder.Clear();
                    }
                }
            }
            catch (Exception)
            {
                _errorHandler.LogError("Cannot open output file");
            }
        }
    }
}
