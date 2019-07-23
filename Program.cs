using System;

namespace DataTransform
{
    /// <summary>
    /// This class works as a factory creating all needed objects for TransformData object and running it.
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main()
        {
            IErrorHandler errorHandler = new ErrorHandler();
            IInputDataToMemory inputDataToMemory = new InputDataToMemory(errorHandler);
            IFileServices fileServices = new FileServices();

            IPrintMatrix printMatrix = new PrintMatrix(errorHandler, fileServices);
            IMatrixLoader loadMatrices = new MatrixLoader(errorHandler, inputDataToMemory, fileServices);

            TransformData transformer = new TransformData(loadMatrices, printMatrix);

            transformer.IncrementalToCumulative();
        }
    }
}
