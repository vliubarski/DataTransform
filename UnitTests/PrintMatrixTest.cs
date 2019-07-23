using System.Collections.Generic;
using DataTransform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class PrintMatrixTest
    {
        Mock<IErrorHandler> _errorHandler = new Mock<IErrorHandler>();
        Mock<IFileServices> _fileServices = new Mock<IFileServices>();

        private PrintMatrix _printMatrix;

        [TestMethod]
        public void WhenPrintTriangleMatricesCalledWithEmptyNameThenItThrows()
        {
            Dictionary<string, double[,]> resultMatrices = new Dictionary<string, double[,]>();
            _errorHandler = new Mock<IErrorHandler>();

            _fileServices.Setup(m => m.GetSaveFileName(It.IsAny<string>())).Returns("");
            _printMatrix = new PrintMatrix(_errorHandler.Object, _fileServices.Object);

            _printMatrix.PrintTriangleMatrices(1990, 4, resultMatrices);

            _errorHandler.Verify(m => m.LogError(It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void WhenPrintTriangleMatricesCalledWithValidNameThenItThrows()
        {
            Dictionary<string, double[,]> resultMatrices = new Dictionary<string, double[,]>();
            _errorHandler = new Mock<IErrorHandler>();

            _fileServices.Setup(m => m.GetSaveFileName(It.IsAny<string>())).Returns("a.a");
            _printMatrix = new PrintMatrix(_errorHandler.Object, _fileServices.Object);

            _printMatrix.PrintTriangleMatrices(1990, 4, resultMatrices);

            _errorHandler.Verify(m => m.LogError(It.IsAny<string>()), Times.Never);
        }
    }
}