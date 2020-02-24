using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMediaConverter
{
    class Program
    {
        private static string targetFormat;
        private static string startFormat;

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Insuficient arguments: {dir} {old file types} {new file types}");
                return;
            }
            try
            {
                startFormat = args[1];
                targetFormat = args[2];
                string[] files = Directory.GetFiles(args[0], $"*.{startFormat}", SearchOption.TopDirectoryOnly);
                ExecuteTasks(SplitIntoChunks(files.Length), files);
                Console.WriteLine($"Converted {files.Length} {startFormat} files to {targetFormat}");
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong");
            }
        }

        private static ArrayChunk[] GetChunkIndexes(int arrLen, int chunkCount)
        {
            var chunks = new ArrayChunk[chunkCount];
            int chunkSize = (int)Math.Ceiling((double)arrLen / chunkCount);

            int chunkIndex = 0, arrIndex = 0;
            while (arrIndex < arrLen)
            {
                chunks[chunkIndex] = new ArrayChunk(arrIndex, arrIndex += chunkSize);
                arrIndex++;
                chunkIndex++;
            }
            chunks[chunkCount - 1].End = arrLen - 1;
            return chunks;
        }

        private static void ExecuteTasks(ArrayChunk[] chunks, string[] files)
        {
            Task[] fileConvertTasks = new Task[chunks.Length];
            int taskIndex = 0;
            foreach (var chunk in chunks)
            {
                fileConvertTasks[taskIndex] = Task.Factory.StartNew(() => ConvertFiles(chunk, files));
                taskIndex++;
            }
            Task.WaitAll(fileConvertTasks);
        }

        private static void ConvertFiles(ArrayChunk chunk, string[] files)
        {
            FFMpegConverter ffMpeg = new FFMpegConverter();

            for (int i = chunk.Start; i <= chunk.End; i++)
            {
                if (string.IsNullOrEmpty(files[i])) break;
                ffMpeg.ConvertMedia(files[i], files[i].Replace(startFormat, targetFormat), targetFormat);
                Console.WriteLine($"{files[i]} converted to {targetFormat}");
            }
        }

        private static ArrayChunk[] SplitIntoChunks(int arrLen)
        {
            if (arrLen < 1) throw new Exception("Files not found");
            else if (arrLen >= 20 && arrLen < 50) return GetChunkIndexes(arrLen, 2);
            else if (arrLen >= 50 && arrLen < 100) return GetChunkIndexes(arrLen, 4);
            else if (arrLen >= 100 && arrLen < 250) return GetChunkIndexes(arrLen, 6);
            else return GetChunkIndexes(arrLen, 8);
        }

        private struct ArrayChunk
        {
            public int Start { get; set; }
            public int End { get; set; }

            public ArrayChunk(int start, int end)
            {
                Start = start;
                End = end;
            }
        }
    }
}
