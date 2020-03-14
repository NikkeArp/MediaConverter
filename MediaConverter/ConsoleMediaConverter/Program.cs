using NReco.VideoConverter;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleMediaConverter
{
    internal class Program
    {
        private static readonly object _lockObj = new object();
        private static Arguments _args;
        private static Output _output;
        private static Files _files;

        private static void Main(string[] args)
        {
            try
            {
                _args = new Arguments(args);
                _output = new Output(_args.StartFormat, _args.TargetFormat);
                
                if (_args.HelpRequested)
                {
                    Output.DisplayHelp();
                    return;
                }

                // Get targeted files from specified location
                _files = new Files(_args.FileLocation, _args.StartFormat, 
                    _args.RecursiveFileSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                bool useSingleThread = _files.Count <= 20 || !_args.MultiThreadRequested;
                if (useSingleThread) ConvertFiles(_files.Paths);
                else ConvertFilesMultiThread(_files.SplitIntoSlices(), _files.Paths);

                _output.EndReport(_files.Count);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                var argEx = ex as CmdArgumentException;
                if (argEx != null)
                    Console.Error.WriteLine($"   {argEx.Argument}");

                if (ex is UnknownArgumentException)
                {
                    Output.DisplayInfo();
                }
            }
        }


        /// <summary>
        /// Converts files to desired format. If chunk count is higher
        /// than one, uses multiple threads
        /// </summary>
        private static void ConvertFilesMultiThread(ArraySlice[] chunks, string[] files)
        {
            Task[] fileConvertTasks = new Task[chunks.Length];
            int taskIndex = 0;
            foreach (var chunkRange in chunks)
            {
                fileConvertTasks[taskIndex] = Task.Factory.StartNew(() => ProcessSlice(chunkRange, files));
                taskIndex++;
            }
            Task.WaitAll(fileConvertTasks);   
        }


        /// <summary>
        /// Converts slice of files to desired file format.
        /// If logging is on, prints every file to console.
        /// </summary>
        private static void ProcessSlice(ArraySlice slice, string[] files)
        {
            FFMpegConverter ffMpeg = new FFMpegConverter();
            for (int i = slice.Start; i <= slice.End; i++)
            {
                if (string.IsNullOrEmpty(files[i]))
                {
                    break;
                }
                else
                {
                    string newFile = files[i].Substring(0, files[i].Length - _args.StartFormat.Length) + _args.TargetFormat;
                    ffMpeg.ConvertMedia(files[i], newFile, _args.TargetFormat);
                    if (_args.LogEachFile)
                    {
                        lock (_lockObj) // needed for multithreading, colors get otherwise mixed up
                        {
                            string filename = files[i].Substring(files[i].LastIndexOf('\\') + 1);
                            _output.LogFileConversion(filename);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Converts files to target format. If logging
        /// is on, prints every file to console.
        /// </summary>
        /// <param name="filePaths"></param>
        private static void ConvertFiles(string[] filePaths)
        {
            FFMpegConverter converter = new FFMpegConverter();
            foreach (string file in filePaths)
            {
                string newFile = file.Substring(0, file.Length - _args.StartFormat.Length) + _args.TargetFormat;
                converter.ConvertMedia(file, newFile, _args.TargetFormat);
                if (_args.LogEachFile)
                {
                    string filename = file.Substring(file.LastIndexOf('\\') + 1);
                    _output.LogFileConversion(filename);
                }
            }
        }
    }
}
