using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Merger
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string inputDirectoryPath;
            string inputFileNamePattern;
            string[] inputFilePaths;
            string outputFilePath = "";
            bool usedSelect = false;
            bool result = false;

            Console.WriteLine("Please enter the input directory path (type 'select' to open file dialog):");
            inputDirectoryPath = Console.ReadLine();
            if (inputDirectoryPath == "")
                Main(args);

            if (inputDirectoryPath.ToLower() == "select")
            {
                inputFilePaths = FileSelectDialog();
                if (inputFilePaths == null)
                 Main(args);

                foreach (var filePath in inputFilePaths)
                {
                    if (!File.Exists(filePath))
                        Main(args);
                }

                outputFilePath = GetOutputFilePath(Path.GetDirectoryName(inputFilePaths[0]));

                result = CombineFiles(outputFilePath, null, null, inputFilePaths);
                usedSelect = true;
            }

            if (!usedSelect)
            {
                if (!Directory.Exists(inputDirectoryPath))
                    Main(args);

                Console.WriteLine("Please enter the filename pattern (press enter for *.txt):");
                inputFileNamePattern = Console.ReadLine();
                inputFileNamePattern = inputFileNamePattern == "" ? "*.txt" : inputFileNamePattern;

                outputFilePath = GetOutputFilePath(inputDirectoryPath);

                result = CombineFiles(outputFilePath, inputDirectoryPath, inputFileNamePattern);
            }

            if (result)
            {
                Console.WriteLine("Done!");
                Console.WriteLine($"Output Path: {outputFilePath}");
                Console.WriteLine("Open file? (Y/N)");
                if (Console.ReadLine().ToLower() == "y")
                    Process.Start("notepad.exe", outputFilePath);
            }
            else
            {
                Main(args);
            }

            Console.ReadLine();
            return;
        }

        private static string[] FileSelectDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "All files |*.*|Text Files |*.txt";
            fileDialog.Multiselect = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileNames;
            }
            else
            {
                return null;
            }
        }

        private static string GetOutputFilePath(string inputDirectoryPath)
        {
            string outputFilePath;
            Console.WriteLine("Please enter the output directory path (same as input if empty, type 'select' to open folder dialog):");
            outputFilePath = Console.ReadLine();
            if (!Directory.Exists(outputFilePath) && outputFilePath != "" && outputFilePath != "select")
                GetOutputFilePath(inputDirectoryPath);

            switch (outputFilePath)
            {
                case "select":
                    var folderDialog = new FolderBrowserDialog();
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        return folderDialog.SelectedPath + "\\output.txt";
                    }
                    else
                    {
                        GetOutputFilePath(inputDirectoryPath);
                    }
                    break;
                case "":
                    return inputDirectoryPath + "\\output.txt";
                default:
                    return outputFilePath + "\\output.txt";
            }

            return outputFilePath + "\\output.txt";
        }

        private static bool CombineFiles(string outputFilePath, string inputDirectoryPath = null, string inputFileNamePattern = null, string[] inputFilePaths = null)
        {
            try
            {
                if (inputFilePaths == null)
                    inputFilePaths = Directory.GetFiles(inputDirectoryPath, inputFileNamePattern);

                string fileName;
                if (inputFilePaths.Length == 0)
                {
                    Console.WriteLine("No files found. \n");
                    return false;
                }
                Console.WriteLine($"Number of files: {inputFilePaths.Length}.");
                foreach (var filePath in inputFilePaths)
                {
                    fileName = Path.GetFileName(filePath);
                    Console.WriteLine($"Handling File: {fileName}");
                    File.AppendAllText(outputFilePath, $"\n####################Start of File: {fileName}####################\n{File.ReadAllText(filePath)}");
                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("No access to output Folder, please restart the program as admin or select a different directory.");
                Console.ReadLine();
                return false;
            }
        }
    }
}
