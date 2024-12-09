using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day9 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            List<int> diskMap = input.Select(x => int.Parse(x.ToString())).ToList();

            List<int> disk = new List<int>();
            InitDisk(diskMap, disk);
            CompressDisk(disk);

            return ChecksumOfDisk(disk);
        }

        private string ChecksumOfDisk(List<int> disk)
        {
            long checksum = 0;
            for (int i = 0; i < disk.Count; i++)
            {
                if (disk[i] == -1)
                    continue;
                checksum += disk[i] * i;
            }
            return checksum.ToString();
        }

        private void CompressDisk(List<int> disk)
        {
            int i = 0;
            int j = disk.Count - 1;

            while (i < disk.Count && j > i)
            {
                while (j > i && disk[j] == -1)
                {
                    j--;
                }

                while (i < disk.Count && disk[i] != -1)
                {
                    i++;
                }

                if (i < disk.Count && j > i)
                {
                    disk[i] = disk[j];
                    disk[j] = -1;
                    i++;
                    j--;
                }
            }
        }

        private static void InitDisk(List<int> diskMap, List<int> disk)
        {
            bool isSpace = false;
            int currIdx = 0;
            foreach (var mapEl in diskMap)
            {
                for (int i = 0; i < mapEl; i++)
                {
                    if (isSpace)
                    {
                        disk.Add(-1);
                    }
                    else
                    {
                        disk.Add(currIdx);
                    }
                }

                if (!isSpace)
                    currIdx++;

                isSpace = !isSpace;
            }
        }

        public string Ch2(string input)
        {
            List<int> diskMap = input.Select(x => int.Parse(x.ToString())).ToList();

            List<int> disk = new List<int>();
            InitDisk(diskMap, disk);
            CompressDiskCh2(disk);

            return ChecksumOfDisk(disk);
        }

        private void CompressDiskCh2(List<int> disk)
        {
            int currNumbersToMove = disk.Max(x => x);
            int currNumbersToMove_Amount = disk.Count(x => x == currNumbersToMove);

            int i = 0;
            int j = disk.Count - 1;

            while (i < disk.Count && j > i && currNumbersToMove > -1)
            {
                while (j > i && disk[j] == -1)
                {
                    j--;
                }

                while (i < j && !EnoughFreeSpace(disk, i, j, currNumbersToMove_Amount))
                {
                    i++;
                }

                if (i < j)
                {
                    MoveBlock(disk, i, j, currNumbersToMove_Amount, currNumbersToMove);
                }

                currNumbersToMove--;
                currNumbersToMove_Amount = disk.Count(x => x == currNumbersToMove);
                i = 0;
                j = disk.FindLastIndex(x => x == currNumbersToMove);
            }
        }

        private void MoveBlock(List<int> disk, int i, int j, int currNumbersToMove_Amount, int currNumbersToMove)
        {
            for (int k = 0; k < currNumbersToMove_Amount; k++)
            {
                disk[i + k] = currNumbersToMove;
                disk[j - k] = -1;
            }
        }

        private bool EnoughFreeSpace(List<int> disk, int i, int j, int currNumbersToMove_Amount)
        {
            if(i + currNumbersToMove_Amount >= j)            
                return false;

            for (int k = i; k < i + currNumbersToMove_Amount; k++)
            {
                if (disk[k] != -1)
                    return false;
            }

            return true;
        }
    }
}
