using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DataTransform
{
    public interface IFileServices
    {
        string GetSaveFileName(string fileName);
        string GetOpenFileName(string fileName);
        IEnumerable<string> ReadLines(string inputFilePath);
        void MessageBox(string msg);
    }

    /// <summary>
    /// This class provides simplified UI for the console application.
    /// </summary>
    class FileServices : IFileServices
    {
        public IEnumerable<string> ReadLines(string inputFilePath)
        {
            return File.ReadLines(inputFilePath);
        }

        public string GetSaveFileName(string fileName)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = fileName;

            return fileDialog.ShowDialog() == DialogResult.Cancel
                ? null
                : fileDialog.FileName;
        }

        public string GetOpenFileName(string fileName)
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Multiselect = false };
            fileDialog.FileName = fileName;
            fileDialog.InitialDirectory = @"../../" + AppDomain.CurrentDomain.BaseDirectory;

            return fileDialog.ShowDialog() == DialogResult.Cancel
                ? null
                : fileDialog.FileName;
        }

        public void MessageBox(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg, "Info", MessageBoxButtons.OK);
        }
    }
}
