#define REPORT_EACH_FILE
#define END_REPORT
#define DIAGNOSE

#if DIAGNOSE
using System.Diagnostics;
#endif
using NReco.VideoConverter;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleMediaConverter
{
    internal class Program
    {
#if REPORT_EACH_FILE
        private static object lockObj = new object();
#endif
        private static string _targetFormat;
        private static string _startFormat;

        private static void Main(string[] args)
        {
            try
            { 
                string[] files = ParseArgs(args);
#if DIAGNOSE
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif
                Convert(SplitIntoChunks(files.Length), files);
#if DIAGNOSE
                sw.Stop();
                Console.WriteLine($"{sw.ElapsedMilliseconds} ms for {files.Length} files");
#endif

#if END_REPORT
                var cColor = Console.ForegroundColor;

                Console.Write($"Converted {files.Length} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{_startFormat}");
                Console.ForegroundColor = cColor;
                Console.Write(" files to ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{_targetFormat}\n");
                Console.ForegroundColor = cColor;
#endif
                Console.Read();
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine("insufficient arguments: {dir} {old file types} {new file types}");
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Something went wrong");
            }
        }

        /// <summary>
        /// Parses command line arguments
        /// </summary>
        /// <param name="args">cmd line arguments</param>
        /// <returns>array of file paths</returns>
        private static string[] ParseArgs(string[] args)
        {
            if (args.Length < 3)
                throw new ArgumentException(); // unrecoverable

            _startFormat = args[1];
            _targetFormat = args[2];
            return Directory.GetFiles(args[0], $"*.{_startFormat}", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// "Splits" file array to chunks by calculating each
        /// chunks start and end index.
        /// </summary>
        /// <param name="arrLen">file array length</param>
        /// <param name="chunkCount">how many chunks</param>
        /// <returns>Array of chunk start and end indexes</returns>
        private static ChunkRange[] GetChunkIndexes(int arrLen, int chunkCount)
        {
            var chunks = new ChunkRange[chunkCount];
            int chunkSize = (int)Math.Ceiling((double)arrLen / chunkCount);

            int chunkIndex = 0; // chunk array (result array) index
            int arrIndex = 0;
            while (arrIndex < arrLen)
            {
                chunks[chunkIndex] = new ChunkRange(arrIndex, arrIndex += chunkSize);
                arrIndex++; // bump index for next loop iteration
                chunkIndex++;
            }
            //last index most likely is out of range, lets fix that
            chunks[chunkCount - 1].End = arrLen - 1; 
            return chunks;
        }

        /// <summary>
        /// Converts files to desired format. If chunk count is higher
        /// than one, uses multiple threads
        /// </summary>
        private static void Convert(ChunkRange[] chunks, string[] files)
        {
            if (chunks.Length < 2) // single threaded solution
            {
                ConvertFiles(chunks[0], files);
                return;
            }
            else // multithreaded solution
            {
                Task[] fileConvertTasks = new Task[chunks.Length];
                int taskIndex = 0;
                foreach (var chunkRange in chunks)
                {
                    fileConvertTasks[taskIndex] = Task.Factory.StartNew(() => ConvertFiles(chunkRange, files));
                    taskIndex++;
                }
                Task.WaitAll(fileConvertTasks);
            }
        }

        /// <summary>
        /// Converts chunk of files to desired file format.
        /// </summary>
        private static void ConvertFiles(ChunkRange chunkRange, string[] files)
        {
            FFMpegConverter ffMpeg = new FFMpegConverter();

            for (int i = chunkRange.Start; i <= chunkRange.End; i++)
            {
                if (string.IsNullOrEmpty(files[i])) break;
                ffMpeg.ConvertMedia(files[i], files[i].Replace(_startFormat, _targetFormat), _targetFormat);

#if REPORT_EACH_FILE
                lock (lockObj) // if program run with multiple threads, console colors get fucked up.
                {
                    string filename = files[i].Substring(files[i].LastIndexOf('\\')+1);
                    string oldFileFormat = filename.Substring(filename.LastIndexOf('.') + 1);
                    var cColor = Console.ForegroundColor;

                    Console.Write("[ ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Success");
                    Console.ForegroundColor = cColor;
                    Console.Write(" ] ");
                    Console.Write($"{filename.Substring(0, filename.LastIndexOf('.')+1)}");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{oldFileFormat} ");
                    Console.ForegroundColor = cColor;
                    Console.Write("to ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{ _targetFormat}\n");
                    Console.ForegroundColor = cColor;
                }
#endif
            }
        }

        /// <summary>
        /// Creates array of ChunkIndexes objects. Calculates how many
        /// chunks is requred for file array length.
        /// </summary>
        private static ChunkRange[] SplitIntoChunks(int arrLen)
        {
            if (arrLen < 1) throw new Exception("Files not found");
            else if (arrLen >= 20 && arrLen < 50) return GetChunkIndexes(arrLen, 2);
            else if (arrLen >= 50 && arrLen < 100) return GetChunkIndexes(arrLen, 4);
            else if (arrLen >= 100 && arrLen < 250) return GetChunkIndexes(arrLen, 6);
            else return GetChunkIndexes(arrLen, 8);
        }

        /// <summary>
        /// Chunk start and end indexes.
        /// </summary>
        private struct ChunkRange
        {
            public int Start { get; set; }
            public int End { get; set; }

            public ChunkRange(int start, int end)
            {
                Start = start;
                End = end;
            }
        }
    }
}
