using DataTransform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class InputDataToMemoryTest
    {
        Mock<IErrorHandler> _errorHandler = new Mock<IErrorHandler>();
        private IInputDataToMemory _inputDataToMemory;

        [TestMethod]
        public void WhenObjectCreatedThenInputTablesIsNotNull()
        {
            _inputDataToMemory = new InputDataToMemory(_errorHandler.Object);
            Assert.IsNotNull(_inputDataToMemory.InputTables);
        }

        [TestMethod]
        public void WhenAddingIncorrectLineThenReturnFalse()
        {
            _inputDataToMemory = new InputDataToMemory(_errorHandler.Object);
            string line = "Incorrect Line";
            bool expected;
            expected = _inputDataToMemory.AddLine(line);
            Assert.AreEqual(false, expected);
        }

        [TestMethod]
        public void WhenAddingCorrectLineThenReturnTrue()
        {
            _inputDataToMemory = new InputDataToMemory(_errorHandler.Object);
            string line = "Comp, 1992, 1992, 110.0";
            bool expected;
            expected = _inputDataToMemory.AddLine(line);
            Assert.AreEqual(true, expected);
        }

        [TestMethod]
        public void WhenReadingCorrectHeaderThenReturnTrue()
        {
            _inputDataToMemory = new InputDataToMemory(_errorHandler.Object);
            string line = "Product, Origin Year, Development Year, Incremental Value";
            bool expected;
            expected = _inputDataToMemory.HeaderValidated(line);
            Assert.AreEqual(true, expected);
        }

        [TestMethod]
        public void WhenReadingInCorrectHeaderThenReturnFalse()
        {
            _inputDataToMemory = new InputDataToMemory(_errorHandler.Object);
            string line = "Incorrect Product, Origin Year, Development Year, Incremental Value";
            bool expected;
            expected = _inputDataToMemory.HeaderValidated(line);
            Assert.AreEqual(false, expected);
        }
    }
}
