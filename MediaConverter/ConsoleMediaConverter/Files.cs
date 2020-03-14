using System;
using System.IO;

namespace ConsoleMediaConverter
{
    internal class Files
    {
        public string[] Paths { get; private set; }
        public int Count => Paths != null ? Paths.Length : 0;

        /// <summary>
        /// Instantiates Files object.
        /// </summary>
        /// <param name="searchLocation">search directory</param>
        /// <param name="format">file format</param>
        /// <param name="search">search option</param>
        internal Files(string searchLocation, string format, SearchOption search)
        {
            Paths = Directory.GetFiles(searchLocation, $"*.{format}", search);
        }


        /// <summary>
        /// Splits paths to equal sized slices.
        /// </summary>
        /// <returns>Array of slice start and end indexes</returns>
        internal ArraySlice[] SplitIntoSlices()
        {
            if (Count > 20 && Count < 60) return GetSliceIndexes(Count, 2);
            else if (Count >= 60 && Count < 120) return GetSliceIndexes(Count, 4);
            else if (Count >= 120 && Count < 240) return GetSliceIndexes(Count, 6);
            else return GetSliceIndexes(Count, 8);
        }


        /// <summary>
        /// "Splits" file array to slices by calculating each
        /// slice start and end index.
        /// </summary>
        /// <param name="arrLen">file array length</param>
        /// <param name="sliceCount">how many slices</param>
        /// <returns>Array of slice start and end indexes</returns>
        private ArraySlice[] GetSliceIndexes(int arrLen, int sliceCount)
        {
            var chunks = new ArraySlice[sliceCount];
            int chunkSize = (int)Math.Ceiling((double)arrLen / sliceCount);

            int chunkIndex = 0; // chunk array (result array) index
            int arrIndex = 0;
            while (arrIndex < arrLen)
            {
                chunks[chunkIndex] = new ArraySlice(arrIndex, arrIndex += chunkSize);
                arrIndex++; // bump index for next loop iteration
                chunkIndex++;
            }
            //last index most likely is out of range, lets fix that
            chunks[sliceCount - 1].End = arrLen - 1;
            return chunks;
        }
    }
}
