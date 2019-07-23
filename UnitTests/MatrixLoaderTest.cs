using System;
using System.Collections.Generic;
using DataTransform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTests
{
    [TestClass]
    public class MatrixLoaderTest
    {
        Mock<IErrorHandler> _errorHandler = new Mock<IErrorHandler>();
        Mock<IInputDataToMemory> _inputDataToMemory = new Mock<IInputDataToMemory>();
        Mock<IFileServices> _fileServices = new Mock<IFileServices>();

        private MatrixLoader _matrixLoader;

        [SetUp]
        public void Start()
        {
            _fileServices.Setup(m => m.ReadLines(It.IsAny<string>())).Returns(new[] { "", "" });
            _fileServices.Setup(m => m.GetOpenFileName(It.IsAny<string>())).Throws<Exception>();
            _matrixLoader = new MatrixLoader(_errorHandler.Object, _inputDataToMemory.Object,
                _fileServices.Object);
            _errorHandler = new Mock<IErrorHandler>();
            _inputDataToMemory = new Mock<IInputDataToMemory>();
        }

        [Test]
        public void WhenFileServicesTrowsThenLoadDataFromFileReturnsFalse()
        {
            _fileServices.Setup(m => m.GetOpenFileName(It.IsAny<string>())).Throws<Exception>();
            _matrixLoader = new MatrixLoader(_errorHandler.Object, _inputDataToMemory.Object,
                _fileServices.Object);

            bool res = _matrixLoader.LoadDataFromFile();
            Assert.IsFalse(res);
        }

        [Test]
        public void WhenErrorHandlerTrowsThenLoadDataFromFileReturnsFalse()
        {
            _fileServices.Setup(m => m.ReadLines(It.IsAny<string>())).Returns(new[] { "", "" });
            _fileServices.Setup(m => m.GetOpenFileName(It.IsAny<string>())).Returns("");
            _matrixLoader = new MatrixLoader(_errorHandler.Object, _inputDataToMemory.Object,
                _fileServices.Object);

            bool res = _matrixLoader.LoadDataFromFile();
            Assert.IsFalse(res);
        }

        [Test]
        public void WhenIncorrectFileHeaderThenLoadDataFromFileReturnsFalse()
        {
            _fileServices.Setup(m => m.ReadLines(It.IsAny<string>())).Returns(new[] { "Incorrect header", "" });
            _fileServices.Setup(m => m.GetOpenFileName(It.IsAny<string>())).Returns(AppDomain.CurrentDomain.BaseDirectory);
            _errorHandler.Setup(m => m.SetLogFilePath(""));

            _matrixLoader = new MatrixLoader(_errorHandler.Object, _inputDataToMemory.Object,
                _fileServices.Object);

            bool res = _matrixLoader.LoadDataFromFile();
            Assert.IsFalse(res);
        }

        [Test]
        public void WhenIncorrectFileDataThenLoadDataFromFileReturnsFalse()
        {
            _fileServices.Setup(m => m.ReadLines(It.IsAny<string>())).Returns(new[] { "Correct header", "Incorrect data" });
            _fileServices.Setup(m => m.GetOpenFileName(It.IsAny<string>())).Returns(AppDomain.CurrentDomain.BaseDirectory);
            _errorHandler.Setup(m => m.SetLogFilePath(""));

            _inputDataToMemory.Setup(m => m.HeaderValidated(It.IsAny<string>())).Returns(true);

            _matrixLoader = new MatrixLoader(_errorHandler.Object, _inputDataToMemory.Object,
                _fileServices.Object);

            bool res = _matrixLoader.LoadDataFromFile();
            Assert.IsTrue(res);
        }

        [Test]
        public void WhenCorrectFileDataThenLoadDataFromFileReturnsTrue()
        {
            _fileServices.Setup(m => m.ReadLines(It.IsAny<string>())).Returns(new[] { "Correct header", "Comp, 1992, 1992, 110.0" });
            _fileServices.Setup(m => m.GetOpenFileName(It.IsAny<string>())).Returns(AppDomain.CurrentDomain.BaseDirectory);
            _errorHandler.Setup(m => m.SetLogFilePath(""));

            _inputDataToMemory.Setup(m => m.HeaderValidated(It.IsAny<string>())).Returns(true);
            _inputDataToMemory.Setup(m => m.AddLine(It.IsAny<string>())).Returns(true);

            _matrixLoader = new MatrixLoader(_errorHandler.Object, _inputDataToMemory.Object,
                _fileServices.Object);

            bool res = _matrixLoader.LoadDataFromFile();
            Assert.IsTrue(res);
        }

        [Test]
        public void WhenLoadDataCalledThenInputTablesIsInitialized()
        {
            _fileServices.Setup(m => m.ReadLines(It.IsAny<string>())).Returns(new[] { "Correct header", "Comp, 1992, 1992, 110.0" });
            _fileServices.Setup(m => m.GetOpenFileName(It.IsAny<string>())).Returns(AppDomain.CurrentDomain.BaseDirectory);
            _errorHandler.Setup(m => m.SetLogFilePath(""));

            _inputDataToMemory.Setup(m => m.HeaderValidated(It.IsAny<string>())).Returns(true);
            _inputDataToMemory.Setup(m => m.AddLine(It.IsAny<string>())).Returns(true);
            _inputDataToMemory.Setup(m => m.InputTables).Returns(new Dictionary<string, List<Tuple<int, int, double>>>());

            _matrixLoader = new MatrixLoader(_errorHandler.Object, _inputDataToMemory.Object,
                _fileServices.Object);

            _matrixLoader.LoadDataFromFile();
            Assert.IsNotNull(_matrixLoader.InputTables);
        }
    }
}
