using System.Collections.Generic;

namespace QFSW.QC.Comparators
{
    public class AlphanumComparator : IComparer<string>
    {
        private const int MaxStackSize = 512;

        public unsafe int Compare(string x, string y)
        {
            if (x == null) return 0;
            if (y == null) return 0;

            var len1 = x.Length;
            var len2 = y.Length;

            if (len1 + len2 + 2 <= MaxStackSize)
            {
                var buffer1 = stackalloc char[len1 + 1];
                var buffer2 = stackalloc char[len2 + 1];

                return Compare(x, buffer1, len1, y, buffer2, len2);
            }
            else
            {
                var buffer1 = new char[len1 + 1];
                var buffer2 = new char[len2 + 1];

                fixed (char* ptr1 = buffer1)
                fixed (char* ptr2 = buffer2)
                {
                    return Compare(x, ptr1, len1, y, ptr2, len2);
                }
            }
        }

        public unsafe int Compare(string x, char* buffer1, int len1, string y, char* buffer2, int len2)
        {
            var marker1 = 0;
            var marker2 = 0;

            while (marker1 < len1 && marker2 < len2)
            {
                var ch1 = x[marker1];
                var ch2 = y[marker2];

                var loc1 = 0;
                var loc2 = 0;

                do
                {
                    buffer1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                        ch1 = x[marker1];
                    else
                        break;
                } while (char.IsDigit(ch1) == char.IsDigit(buffer1[0]));

                do
                {
                    buffer2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                        ch2 = y[marker2];
                    else
                        break;
                } while (char.IsDigit(ch2) == char.IsDigit(buffer2[0]));

                //null terminate buffers
                buffer1[loc1] = buffer2[loc2] = (char) 0;

                int result;
                if (char.IsDigit(buffer1[0]) && char.IsDigit(buffer2[0]))
                {
                    var chunk1 = ParseInt(buffer1);
                    var chunk2 = ParseInt(buffer2);
                    result = chunk1 - chunk2;
                }
                else
                {
                    result = CompareStrings(buffer1, buffer2);
                }

                if (result != 0) return result;
            }

            return len1 - len2;
        }

        private unsafe int ParseInt(char* buffer)
        {
            var acc = 0;

            while (*buffer != 0)
            {
                acc *= 10;
                acc += *buffer++ - '0';
            }

            return acc;
        }

        private unsafe int CompareStrings(char* buffer1, char* buffer2)
        {
            var index = 0;
            while (buffer1[index] != 0 && buffer2[index] != 0)
            {
                var c1 = buffer1[index];
                var c2 = buffer2[index++];

                if (c1 > c2)
                    return 1;
                if (c1 < c2) return -1;
            }

            if (buffer1[index] != 0)
                return 1;
            if (buffer2[index] != 0)
                return -1;
            return 0;
        }
    }
}