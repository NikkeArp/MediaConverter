using System;

namespace ConsoleMediaConverter
{
    internal class Output
    {
        private readonly string _startFormat;
        private readonly string _targetFormat;

        /// <summary>
        /// Instantiates Output object
        /// </summary>
        /// <param name="startFormat">start file format</param>
        /// <param name="targetFormat">target file format</param>
        internal Output(string startFormat, string targetFormat)
        {
            _startFormat = startFormat;
            _targetFormat = targetFormat;
        }

        /// <summary>
        /// Displays info about the program
        /// </summary>
        internal static void DisplayInfo()
        {
            Console.WriteLine("Usage: medcvert [DIRECTORY] [START_FORMAT] [TARGET_FORMAT] ...[OPTIONS]...");
            Console.WriteLine("Try 'medcvert -help' for more information");
        }

        /// <summary>
        /// Logs file conversion
        /// </summary>
        /// <param name="filename">filename</param>
        internal void LogFileConversion(string filename)
        {
            var cColor = Console.ForegroundColor;
            Console.Write($"{filename.Substring(0, filename.LastIndexOf('.') + 1)}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{_startFormat} ");
            Console.ForegroundColor = cColor;
            Console.Write("to ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{_targetFormat}\n");
            Console.ForegroundColor = cColor;
        }

        /// <summary>
        /// Displays help
        /// </summary>
        internal static void DisplayHelp()
        {
            Console.WriteLine("Convert media files to different file formats.");
            Console.WriteLine("Usage: medcvert [DIRECTORY] [START_FORMAT] [TARGET_FORMAT] ...[OPTIONS]...");
            Console.WriteLine("Example: medcvert C:\\Users\\nikke\\videos webm mp4 -r -log");
            Console.WriteLine("Optional settings:");
            Console.WriteLine("    -r");
            Console.WriteLine("    -R       recursive file search");
            Console.WriteLine();
            Console.WriteLine("    -l");
            Console.WriteLine("    -log     logs every file conversion to console");
            Console.WriteLine();
            Console.WriteLine("    -t");
            Console.WriteLine("    -T       use multiple threads");
            Console.WriteLine();
            Console.WriteLine("    -help    display help");
        }

        /// <summary>
        /// End report after conversions are complete
        /// </summary>
        /// <param name="fileCount">amount of files converted</param>
        internal void EndReport(int fileCount)
        {
            var cColor = Console.ForegroundColor;

            Console.Write($"Converted {fileCount} ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{_startFormat}");
            Console.ForegroundColor = cColor;
            Console.Write(" files to ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{_targetFormat}\n");
            Console.ForegroundColor = cColor;
        }
    }
}
