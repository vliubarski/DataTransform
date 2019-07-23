using System;
using System.Collections.Generic;
using DataTransform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestClass]
    public class TransformDataTest
    {
        private Mock<IMatrixLoader> _matrixLoader = new Mock<IMatrixLoader>();
        private Mock<IPrintMatrix> _printMatrix = new Mock<IPrintMatrix>();

        private TransformData _transformData;

        [SetUp]
        public void Start()
        {
            _matrixLoader = new Mock<IMatrixLoader>();
            _printMatrix = new Mock<IPrintMatrix>();
        }


        [TestMethod]
        public void WhenLoadDataFromFileReturnsFalse_ThenReturns()
        {
            _matrixLoader.Setup(m => m.LoadDataFromFile()).Returns(false);
            _transformData = new TransformData(_matrixLoader.Object, _printMatrix.Object);
            _transformData.IncrementalToCumulative();

            _matrixLoader.Verify(m => m.InputTables, Times.Never);
        }

        [TestMethod]
        public void WhenLoadDataFromFileReturnsFalse_ThenContinues()
        {
            Dictionary<string, List<Tuple<int, int, double>>> table
                = new Dictionary<string, List<Tuple<int, int, double>>>
                {
                    {
                        "1", new List<Tuple<int, int, double>>
                        {
                            Tuple.Create(1, 1, 3.0),
                            Tuple.Create(1, 2, 4.0),
                            Tuple.Create(2, 2, 5.0)
                        }
                    }
                };

            _matrixLoader.Setup(m => m.LoadDataFromFile()).Returns(true);
            _matrixLoader.Setup(m => m.InputTables).Returns(table);

            _transformData = new TransformData(_matrixLoader.Object, _printMatrix.Object);
            _transformData.IncrementalToCumulative();

            _matrixLoader.Verify(m => m.InputTables, Times.Once);
        }

        [TestMethod]
        public void WhenLoadDataFromFileContinuesThenPrintTriangleMatricesCalled()
        {
            Dictionary<string, List<Tuple<int, int, double>>> table
                = new Dictionary<string, List<Tuple<int, int, double>>>
                {
                    {
                        "1", new List<Tuple<int, int, double>>
                        {
                            Tuple.Create(1, 1, 3.0),
                            Tuple.Create(1, 2, 4.0),
                            Tuple.Create(2, 2, 5.0)
                        }
                    }
                };

            _matrixLoader.Setup(m => m.LoadDataFromFile()).Returns(true);
            _matrixLoader.Setup(m => m.InputTables).Returns(table);

            _transformData = new TransformData(_matrixLoader.Object, _printMatrix.Object);
            _transformData.IncrementalToCumulative();

            _printMatrix.Verify(m => m.PrintTriangleMatrices(1, 2, It.IsAny<Dictionary<string, double[,]>>()), Times.Once);
        }
    }
}