using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace IQFinal
{
    static class Program
    {
        static public KurushRNG.Program.PuzzleStats ComputePuzzleStats(List<byte[]> puzzles, int width, int height)
        {
            List<int> advMaxPuzzles = new List<int>();
            List<int> forbMaxPuzzles = new List<int>();
            List<int> normMaxPuzzles = new List<int>();
            List<int> advMinPuzzles = new List<int>();
            List<int> forbMinPuzzles = new List<int>();
            List<int> normMinPuzzles = new List<int>();
            List<KurushRNG.Program.PuzzleInfo> puzzInfs = new List<KurushRNG.Program.PuzzleInfo>();
            int minNorm = Int16.MaxValue, maxNorm = Int16.MinValue;
            int minAdv = Int16.MaxValue, maxAdv = Int16.MinValue;
            int minForb = Int16.MaxValue, maxForb = Int16.MinValue;
            KurushRNG.Program.PuzzleStats stats = new KurushRNG.Program.PuzzleStats();
            int numPuzzles = puzzles.Count;
            stats.totalPuzzleCubes = (width * height) * numPuzzles;
            for (int i = 0; i < numPuzzles; ++i)
            {
                KurushRNG.Program.PuzzleInfo puzzInf = new KurushRNG.Program.PuzzleInfo();
                byte[] puzzle = puzzles[i];
                for (int y = 0; y < height; ++y)
                {
                    int rowOffset = width * y;
                    for (int x = 0; x < width; ++x)
                    {
                        switch (puzzle[rowOffset + x])
                        {
                            case 0:
                            {
                                ++puzzInf.norm;
                            }
                            break;
                            case 1:
                            {
                                ++puzzInf.adv;
                            }
                            break;
                            case 2:
                            {
                                ++puzzInf.forb;
                            }
                            break;
                        }
                    }
                }
                puzzInfs.Add(puzzInf);
                stats.forbiddenCubes += puzzInf.forb;
                stats.normalCubes += puzzInf.norm;
                stats.advanCubes += puzzInf.adv;
                if (puzzInf.norm < minNorm)
                {
                    minNorm = puzzInf.norm;
                }
                if (puzzInf.norm > maxNorm)
                {
                    maxNorm = puzzInf.norm;
                }
                if (puzzInf.forb < minForb)
                {
                    minForb = puzzInf.forb;
                }
                if (puzzInf.forb > maxForb)
                {
                    maxForb = puzzInf.forb;
                }
                if (puzzInf.adv < minAdv)
                {
                    minAdv = puzzInf.adv;
                }
                if (puzzInf.adv > maxAdv)
                {
                    maxAdv = puzzInf.adv;
                }
            }
            for (int i = 0; i < numPuzzles; ++i)
            {
                if (puzzInfs[i].adv == maxAdv)
                {
                    advMaxPuzzles.Add(i);
                }
                if (puzzInfs[i].adv == minAdv)
                {
                    advMinPuzzles.Add(i);
                }
                if (puzzInfs[i].forb == maxForb)
                {
                    forbMaxPuzzles.Add(i);
                }
                if (puzzInfs[i].forb == minForb)
                {
                    forbMinPuzzles.Add(i);
                }
                if (puzzInfs[i].norm == maxNorm)
                {
                    normMaxPuzzles.Add(i);
                }
                if (puzzInfs[i].norm == minNorm)
                {
                    normMinPuzzles.Add(i);
                }
            }
            int totalIndividual = stats.forbiddenCubes + stats.advanCubes + stats.normalCubes;
            if (totalIndividual != stats.totalPuzzleCubes)
            {
                throw new ArithmeticException(String.Format("Total cubes {0} != sum of individual cubes {1}", stats.totalPuzzleCubes, totalIndividual));
            }
            stats.maxAdvanCubes = maxAdv;
            stats.maxAdvanCubesPuzzles = advMaxPuzzles.ToArray();
            stats.minAdvanCubes = minAdv;
            stats.minAdvanCubesPuzzles = advMinPuzzles.ToArray();
            stats.maxNormCubes = maxNorm;
            stats.maxNormCubesPuzzles = normMaxPuzzles.ToArray();
            stats.minNormCubes = minNorm;
            stats.minNormCubesPuzzles = normMinPuzzles.ToArray();
            stats.maxForbidCubes = maxForb;
            stats.maxForbidCubesPuzzles = forbMaxPuzzles.ToArray();
            stats.minForbidCubes = minForb;
            stats.minForbidCubesPuzzles = forbMinPuzzles.ToArray();
            return stats;
        }

        static public byte DecompressPuzzle(byte[] data, int cubesInPuzzle)
        {
            int arrayPos = 31;
            byte previousByte = 0;
            long remainder = 0;
            long quotient = 0;
            cubesInPuzzle <<= 16;
            cubesInPuzzle >>= 16;
            do
            {
                byte thisByte = data[arrayPos];
                int bothBytes = (int)((previousByte << 8) + thisByte);
                bothBytes <<= 16;
                bothBytes >>= 16;
                quotient = Math.DivRem(bothBytes, cubesInPuzzle, out remainder);
                data[arrayPos] = (byte)quotient;
                previousByte = (byte)remainder;
                --arrayPos;
            }
            while (arrayPos >= 0);
            remainder <<= 16;
            remainder >>= 16;
            return (byte)remainder;
        }

        static public List<int> GetPuzzleOffsets(byte[] data)
        {
            List<int> puzzPos = new List<int>();
            int iter = 0;
            int fileLength = data.Length;
            byte firstOfPuzzle = 0;
            do
            {
                puzzPos.Add(iter);
                firstOfPuzzle = data[iter];
                iter += firstOfPuzzle;
            }
            while ((iter < fileLength) && (firstOfPuzzle != 0));
            return puzzPos;
        }

        public class DuplicatePuzzles
        {
            public ulong oppositeHash; // to find flipped identical puzzles
            public List<uint> identicalPuzzles;
            public DuplicatePuzzles()
            {
                identicalPuzzles = new List<uint>();
            }
        }

        public class WaveInformation
        {
            public ushort waveIndex;
            public byte puzzlesPerWave;
            public byte puzWidth;
            public byte puzHeight;
            public ushort startingPuzzlePosOffset;
            public ushort numPuzzles;
            public List<int> trns;
            public List<byte[]> decompPuzzles;
            // hash to duplicate puzzles map
            public Dictionary<ulong, DuplicatePuzzles> dupPuzzles;

            public WaveInformation(ushort waveIdx, byte puzzlesPerWaveIn, byte width, byte height, ushort off, ushort num)
            {
                waveIndex = waveIdx;
                puzzlesPerWave = puzzlesPerWaveIn;
                puzWidth = width;
                puzHeight = height;
                startingPuzzlePosOffset = off;
                numPuzzles = num;
                trns = null;
                decompPuzzles = null;
                dupPuzzles = new Dictionary<ulong, DuplicatePuzzles>();
            }

            public WaveInformation(WaveInformation inf)
            {
                waveIndex = inf.waveIndex;
                puzzlesPerWave = inf.puzzlesPerWave;
                puzWidth = inf.puzWidth;
                puzHeight = inf.puzHeight;
                startingPuzzlePosOffset = inf.startingPuzzlePosOffset;
                numPuzzles = inf.numPuzzles;
                trns = null;
                decompPuzzles = null;
                dupPuzzles = new Dictionary<ulong, DuplicatePuzzles>();
            }
        }

        // https://stackoverflow.com/a/13325262
        static public ulong SimpleHash(byte[] str)
        {
            ulong hash = 5381;
            foreach (byte b in str)
            {
                hash = ((hash << 5) + hash) + b;
            }
            return hash;
        }

        static public ulong ReverseSimpleHash(byte[] puzzle, byte height, byte width)
        {
            byte[] revPuzzle = new byte[puzzle.Length];
            Array.Copy(puzzle, revPuzzle, puzzle.Length);
            for (int i = 0; i < height; ++i)
            {
                int startPoint = i * width;
                Array.Reverse(revPuzzle, startPoint, width);
            }
            return SimpleHash(revPuzzle);
        }

        static public void DecompWavePuzzles(WaveInformation wi, byte[] puzzles, List<int> puzzPos)
        {
            int width = wi.puzWidth;
            int height = wi.puzHeight;
            int totalCubes = height * width;
            byte[] buffer = new byte[32];
            byte[] cleanBuffer = new byte[32];
            int lastPuzzle = wi.startingPuzzlePosOffset + wi.numPuzzles;
            List<int> trnList = wi.trns = new List<int>(wi.numPuzzles);
            List<byte[]> decompList = wi.decompPuzzles = new List<byte[]>(wi.numPuzzles);
            uint puzzleNumber = 0;
            for (int i = wi.startingPuzzlePosOffset; i < lastPuzzle; ++i, ++puzzleNumber)
            {
                byte[] decompPuzzle = new byte[totalCubes];
                Buffer.BlockCopy(cleanBuffer, 0, buffer, 0, cleanBuffer.Length);
                int puzzOff = puzzPos[i];
                int numBytes = puzzles[puzzOff] - 1;
                Buffer.BlockCopy(puzzles, puzzOff + 1, buffer, 0, numBytes);
                int trn = DecompressPuzzle(buffer, totalCubes) + 1;
                int revPuzIter = totalCubes - 1;
                while (revPuzIter >= 0)
                {
                    byte rem = DecompressPuzzle(buffer, 3);
                    decompPuzzle[revPuzIter] = rem;
                    --revPuzIter;
                }
                decompList.Add(decompPuzzle);
                trnList.Add(trn);
                ulong puzzleHash = SimpleHash(decompPuzzle);
                DuplicatePuzzles dupPuzzleList = null;
                if (wi.dupPuzzles.TryGetValue(puzzleHash, out dupPuzzleList))
                {
                    dupPuzzleList.identicalPuzzles.Add(puzzleNumber);
                }
                else
                {
                    ulong revHash = ReverseSimpleHash(decompPuzzle, wi.puzHeight, wi.puzWidth);
                    dupPuzzleList = new DuplicatePuzzles();
                    // don't bother with mirrors
                    dupPuzzleList.oppositeHash = (revHash != puzzleHash) ? revHash : 0;
                    dupPuzzleList.identicalPuzzles.Add(puzzleNumber);
                    wi.dupPuzzles.Add(puzzleHash, dupPuzzleList);
                }
            }
        }

        static uint CalculateUniquePuzzles(WaveInformation wi)
        {
            List<ulong> keys = new List<ulong>(wi.dupPuzzles.Keys);
            uint count = 0;
            while (keys.Count > 0)
            {
                ulong firstKey = keys[0];
                DuplicatePuzzles dup = wi.dupPuzzles[firstKey];
                keys.Remove(dup.oppositeHash);
                keys.RemoveAt(0);
                ++count;
            }
            return count;
        }

        static KurushRNG.Program.PuzzleStats WritePuzzles(WaveInformation wi, out KurushRNG.Program.TRNStats trnStats)
        {
            int width = wi.puzWidth, height = wi.puzHeight;
            int[] trns = wi.trns.ToArray();
            int numPuzzles = wi.numPuzzles;
            KurushRNG.Program.PuzzleStats puzStats = ComputePuzzleStats(wi.decompPuzzles, width, height);
            trnStats = KurushRNG.Program.ComputeTRNStats(trns);
            return puzStats;
        }

        static WaveInformation[] waves = new WaveInformation[]
        {
            // 1st stage
            new WaveInformation(0, 3, 4, 2, 0, 0x190),
            new WaveInformation(1, 3, 4, 3, 0x190, 0x190),
            new WaveInformation(2, 3, 4, 4, 0x320, 0x190),
            // 2nd stage
            new WaveInformation(3, 3, 4, 5, 0x4b0, 0x258),
            new WaveInformation(3, 3, 4, 5, 0x4b0, 0x258),
            new WaveInformation(4, 3, 4, 6, 0x708, 0x258),
            // 3rd stage
            new WaveInformation(5, 3, 5, 5, 0x960, 0x258),
            new WaveInformation(6, 3, 5, 6, 0xbb8, 0x258),
            new WaveInformation(6, 3, 5, 6, 0xbb8, 0x258),
            // 4th stage
            new WaveInformation(7, 3, 5, 7, 0xe10, 0x258),
            new WaveInformation(7, 3, 5, 7, 0xe10, 0x258),
            new WaveInformation(8, 3, 5, 8, 0x1068, 0x1f4),
            // 5th stage
            new WaveInformation(9, 3, 6, 6, 0x125c, 0x258),
            new WaveInformation(10, 3, 6, 7, 0x14b4, 0x258),
            new WaveInformation(10, 3, 6, 7, 0x14b4, 0x258),
            // 6th stage
            new WaveInformation(11, 2, 6, 8, 0x170c, 0x258),
            new WaveInformation(11, 2, 6, 8, 0x170c, 0x258),
            new WaveInformation(12, 2, 6, 9, 0x1964, 0x1f4),
            // 7th stage
            new WaveInformation(13, 3, 7, 7, 0x1b58, 0x258),
            new WaveInformation(14, 3, 7, 8, 0x1db0, 0x258),
            new WaveInformation(14, 3, 7, 8, 0x1db0, 0x258),
            // 8th stage
            new WaveInformation(15, 2, 7, 9, 0x2008, 0x258),
            new WaveInformation(15, 2, 7, 9, 0x2008, 0x258),
            new WaveInformation(16, 2, 7, 10, 0x2260, 0xc8),
            // final stage
            new WaveInformation(17, 2, 8, 9, 0x2328, 0xc8),
            new WaveInformation(17, 2, 8, 9, 0x2328, 0xc8),
            new WaveInformation(18, 2, 8, 10, 0x23f0, 0xc8),
            // tectonics
            new WaveInformation(19, 1, 9, 10, 0x24b8, 0x14)
        };

        static WaveInformation[] beforeExpansionWaves = new WaveInformation[]
        {
            // 1st stage
            new WaveInformation(0, 3, 4, 2, 0, 0xc8),
            new WaveInformation(1, 3, 4, 3, 0xc8, 0xc8),
            new WaveInformation(2, 3, 4, 4, 0x190, 0xc8),
            // 2nd stage
            new WaveInformation(3, 3, 4, 5, 0x258, 0xc8),
            new WaveInformation(3, 3, 4, 5, 0x258, 0xc8),
            new WaveInformation(4, 3, 4, 6, 0x320, 0xc8),
            // 3rd stage
            new WaveInformation(5, 3, 5, 5, 0x3e8, 0xc8),
            new WaveInformation(6, 3, 5, 6, 0x4b0, 0xc8),
            new WaveInformation(6, 3, 5, 6, 0x4b0, 0xc8),
            // 4th stage
            new WaveInformation(7, 3, 5, 7, 0x578, 0xc8),
            new WaveInformation(7, 3, 5, 7, 0x578, 0xc8),
            new WaveInformation(8, 3, 5, 8, 0x640, 0xc8), // think this is the one that doesnt exist, needs renumbering
            // 5th stage
            new WaveInformation(9, 3, 6, 6, 0x708, 0xc8),
            new WaveInformation(10, 3, 6, 7, 0x7d0, 0xc8),
            new WaveInformation(10, 3, 6, 7, 0x7d0, 0xc8),
            // 6th stage
            new WaveInformation(11, 2, 6, 8, 0x898, 0xc8),
            new WaveInformation(11, 2, 6, 8, 0x898, 0xc8),
            new WaveInformation(12, 2, 6, 9, 0x960, 0xc8),
            // 7th stage
            new WaveInformation(13, 3, 7, 7, 0xa28, 0xc8),
            new WaveInformation(14, 3, 7, 8, 0xaf0, 0xc8),
            new WaveInformation(14, 3, 7, 8, 0xaf0, 0xc8),
            // 8th stage
            new WaveInformation(15, 2, 7, 9, 0xbb8, 0xc8),
            new WaveInformation(15, 2, 7, 9, 0xbb8, 0xc8),
            new WaveInformation(16, 2, 7, 10, 0xc80, 0xc8),
            // final stage
            new WaveInformation(17, 2, 8, 9, 0xd48, 0xc8),
            new WaveInformation(17, 2, 8, 9, 0xd48, 0xc8),
            //new WaveInformation(18, 2, 8, 10, 0xe10, 0xc8),
            // tectonics
            new WaveInformation(19, 1, 9, 10, 0xe10, 0x14)
        };

        class TRNChange
        {
            public List<int> puzNum;
            public TRNChange()
            {
                puzNum = new List<int>();
            }
        }

        static private void DumpUnusedPuzzles()
        {
            byte[] puzzles = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQFinal-final.cmp");
            byte[] puzzlesDemo = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQFinalDemo-final.cmp");
            List<int> puzzlePos = GetPuzzleOffsets(puzzles);
            List<int> puzzlePosDemo = GetPuzzleOffsets(puzzlesDemo);
            int lastWaveId = -1;
            int puzChecked = 0, totalTRNChanges = 0;
            foreach (WaveInformation wi in beforeExpansionWaves)
            {
                if (wi.waveIndex == lastWaveId)
                {
                    continue;
                }
                Dictionary<int, TRNChange> trnChanges = new Dictionary<int, TRNChange>();
                lastWaveId = wi.waveIndex;
                DecompWavePuzzles(wi, puzzles, puzzlePos);
                WaveInformation wiDemo = new WaveInformation(wi);
                DecompWavePuzzles(wiDemo, puzzlesDemo, puzzlePosDemo);
                int loopEnd = wi.numPuzzles;
                for (int i = 0; i < loopEnd; ++i)
                {
                    ++puzChecked;
                    int trnDiff = wiDemo.trns[i] - wi.trns[i];
                    TRNChange changedPuzzles;
                    if (trnDiff != 0)
                    {
                        ++totalTRNChanges;
                        if (!trnChanges.TryGetValue(trnDiff, out changedPuzzles))
                        {
                            changedPuzzles = new TRNChange();
                            trnChanges.Add(trnDiff, changedPuzzles);
                        }
                        changedPuzzles.puzNum.Add(i);
                    }
                    if ((loopEnd == 600) && (i == 199)) i = 399;
                    else if ((loopEnd == 500) && (i == 99)) i = 299;
                }
                if (trnChanges.Count > 0)
                {
                    Console.WriteLine("TRN Diffs for {0}x{1} (negative = puzzle got easier)", wi.puzWidth, wi.puzHeight);
                    foreach (KeyValuePair<int, TRNChange> trnDiff in trnChanges)
                    {
                        List<int> changedPuzzles = trnDiff.Value.puzNum;
                        StringBuilder sb = new StringBuilder();
                        foreach (int puzNum in changedPuzzles)
                        {
                            sb.AppendFormat("{0}, ", puzNum);
                        }
                        sb.Length -= 2;
                        Console.WriteLine("Demo has {0} turns - {1} puzzles ({2})", trnDiff.Key, changedPuzzles.Count, sb.ToString());
                    }
                }
                //KurushRNG.Program.TRNStats trnStats;
                //KurushRNG.Program.PuzzleStats puzStats = WritePuzzles(wi, out trnStats);
                //totalCubes += puzStats.totalPuzzleCubes;
                //advanCubes += puzStats.advanCubes;
                //forbidCubes += puzStats.forbiddenCubes;
                //totalAverage += trnStats.average;
            }
            Console.WriteLine("{0}/{1} puzzles had their TRN changed", totalTRNChanges, puzChecked);
        }

        static public void DumpPuzzles()
        {
            DumpUnusedPuzzles();
            return;
            byte[] puzzlesDemo = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQFinalDemo-question.cmp");
            byte[] puzzles = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQFinal-question.cmp");
            List<int> puzzlePosDemo = GetPuzzleOffsets(puzzlesDemo);
            List<int> puzzlePos = GetPuzzleOffsets(puzzles);
            int totalCubes = 0;
            int advanCubes = 0;
            int forbidCubes = 0;
            int lastWaveId = -1;
            float totalAverage = 0;
            int totalTRNChanges = 0, puzChecked = 0;
            foreach (WaveInformation wi in waves)
            {
                if (wi.waveIndex == lastWaveId)
                {
                    continue;
                }
                Dictionary<int, TRNChange> trnChanges = new Dictionary<int, TRNChange>();
                lastWaveId = wi.waveIndex;
                DecompWavePuzzles(wi, puzzles, puzzlePos);
                WaveInformation wiDemo = new WaveInformation(wi);
                DecompWavePuzzles(wiDemo, puzzlesDemo, puzzlePosDemo);
                int loopEnd = wi.numPuzzles;
                for (int i = 0; i < loopEnd; ++i)
                {
                    ++puzChecked;
                    int trnDiff = wiDemo.trns[i] - wi.trns[i];
                    TRNChange changedPuzzles;
                    if (trnDiff != 0)
                    {
                        ++totalTRNChanges;
                        if (!trnChanges.TryGetValue(trnDiff, out changedPuzzles))
                        {
                            changedPuzzles = new TRNChange();
                            trnChanges.Add(trnDiff, changedPuzzles);
                        }
                        changedPuzzles.puzNum.Add(i);
                    }
                    if ((loopEnd == 600) && (i == 199)) i = 399;
                    else if ((loopEnd == 500) && (i == 99)) i = 299;
                }
                if (trnChanges.Count > 0)
                {
                    Console.WriteLine("TRN Diffs for {0}x{1} (negative = puzzle got easier)", wi.puzWidth, wi.puzHeight);
                    foreach (KeyValuePair<int, TRNChange> trnDiff in trnChanges)
                    {
                        List<int> changedPuzzles = trnDiff.Value.puzNum;
                        StringBuilder sb = new StringBuilder();
                        foreach(int puzNum in changedPuzzles)
                        {
                            sb.AppendFormat("{0}, ", puzNum);
                        }
                        sb.Length -= 2;
                        Console.WriteLine("Demo has {0} turns - {1} puzzles ({2})", trnDiff.Key, changedPuzzles.Count, sb.ToString());
                    }
                }
                //KurushRNG.Program.TRNStats trnStats;
                //KurushRNG.Program.PuzzleStats puzStats = WritePuzzles(wi, out trnStats);
                //totalCubes += puzStats.totalPuzzleCubes;
                //advanCubes += puzStats.advanCubes;
                //forbidCubes += puzStats.forbiddenCubes;
                //totalAverage += trnStats.average;
            }
            Console.WriteLine("{0}/{1} puzzles had their TRN changed", totalTRNChanges, puzChecked);
            totalAverage /= waves.Length;
            int normalCubes = totalCubes - advanCubes - forbidCubes;
            Console.WriteLine("I.Q. Final stats:");
            Console.WriteLine("Average TRN of the game: {0:f2}", totalAverage);
            Console.WriteLine("Total advan cubes: {0} ({1:f2}%)", advanCubes, (((float)advanCubes / totalCubes) * 100.0f));
            Console.WriteLine("Total forbid cubes: {0} ({1:f2}%)", forbidCubes, (((float)forbidCubes / totalCubes) * 100.0f));
            Console.WriteLine("Total normal cubes: {0} ({1:f2}%)", normalCubes, (((float)normalCubes / totalCubes) * 100.0f));
            Console.WriteLine();
        }

        static public ulong UBigMul(uint a, uint b)
        {
            ulong ulA = (ulong)a;
            ulong ulB = (ulong)b;
            return ulA * ulB;
        }

        private class PuzzleAppearance
        {
            public uint puzzle;
            public uint times;
            public uint timesFlipped;
        }

        static public void SeqMain()
        {
            Dictionary<string, PuzzleAppearance[]> puzzleAppearances = new Dictionary<string, PuzzleAppearance[]>();
            StringBuilder sb = new StringBuilder();
            using (StreamWriter sw = new StreamWriter("t:\\FinalPuzzleSequences.txt"))
            {
                int startPoint = 30000;
                KurushRNG.Program.seed = 0;
                for (int j = 0; j < startPoint - 1; ++j)
                {
                    KurushRNG.Program.Rand();
                }
                int lastSeed = KurushRNG.Program.seed;
                sw.WriteLine("Each | represents the next wave");
                sw.WriteLine("IF USING NUMBERS FROM THE HTML, SUBTRACT 1. THIS IS 0 BASED, THAT IS 1-BASED");
                uint totalPuzzles = 0;
                uint totalflipped = 0;
                for (int i = startPoint; i < 50000 + startPoint; ++i)
                {
                    // so we don't have to do all the loops to set the correct seed each time around
                    KurushRNG.Program.seed = lastSeed;
                    KurushRNG.Program.Rand();
                    lastSeed = KurushRNG.Program.seed;

                    uint seed = (uint)KurushRNG.Program.Rand();
                    uint flippedSeed = seed;
                    sw.WriteLine("Starting after {0} calls to rand", i);
                    sw.WriteLine("Puzzles:");
                    foreach (WaveInformation wi in waves)
                    {
                        // tektonics is fixed, so doesn't take part in this
                        if (wi.puzWidth == 9 && wi.puzHeight == 10)
                        {
                            continue;
                        }
                        string size = String.Format("{0}x{1}", wi.puzWidth, wi.puzHeight);
                        sw.Write("{0}: ", size);
                        PuzzleAppearance[] puzzles = null;
                        if (!puzzleAppearances.TryGetValue(size, out puzzles))
                        {
                            //last puzzle contains total information for this puzzle size
                            puzzles = new PuzzleAppearance[wi.numPuzzles + 1];
                            for (uint y = 0; y < wi.numPuzzles + 1; ++y)
                            {
                                puzzles[y] = new PuzzleAppearance();
                                puzzles[y].puzzle = y;
                                puzzles[y].times = 0;
                                puzzles[y].timesFlipped = 0;
                            }
                            puzzleAppearances.Add(size, puzzles);
                        }
                        PuzzleAppearance totalPuzzleInfo = puzzles[puzzles.Length - 1];
                        sb.Length = 0;
                        for (int k = 0; k < wi.puzzlesPerWave; ++k)
                        {
                            uint seedSoFar = seed; // seed = 0x2e75cf10
                            seedSoFar <<= 2;
                            seedSoFar += seed;
                            seedSoFar += 1;
                            seed = seedSoFar;
                            // it does it separately in the game code, so that's how I'm doing it too (800434C0)
                            uint flippedSeedSoFar = flippedSeed;
                            flippedSeedSoFar <<= 2;
                            flippedSeedSoFar += flippedSeed;
                            flippedSeedSoFar += 1;
                            flippedSeed = flippedSeedSoFar;

                            // all this determines if the puzzle should be flipped - this code at 80043500 to the jump
                            ulong flippedRes = UBigMul(flippedSeedSoFar, 0xCCCCCCCD);
                            uint highRes = (uint)(flippedRes >> 32);
                            uint eighth = highRes >> 3;
                            uint runningTotal = eighth << 2;
                            runningTotal += eighth;
                            runningTotal <<= 1;
                            uint subRes = flippedSeedSoFar - runningTotal;
                            uint canFlipNum = Convert.ToUInt32(subRes < 3); // stored at 0xDF5($a1) (a1 = 800bc664)

                            uint puzArg = seedSoFar & 0xFFFF;
                            ushort puzzle = DetermineWhichPuzzle(puzArg, wi);
                            bool didFlip = false;

                            // second part of flip decision - this part is at 80064428
                            uint flipInt = canFlipNum << 16;
                            if ((flipInt != 0) && (wi.puzHeight > 0))
                            {
                                ++puzzles[puzzle].timesFlipped;
                                ++totalflipped;
                                ++totalPuzzleInfo.timesFlipped;
                                didFlip = true;
                            }

                            sb.AppendFormat("{0}{1} ", puzzle, didFlip ? "(f)" : String.Empty);
                            Debug.Assert(puzzle < puzzles.Length);
                            ++puzzles[puzzle].times;
                            ++totalPuzzleInfo.times;
                            ++totalPuzzles;
                        }
                        sb.Length -= 1;
                        sb.Append(" | ");
                        sw.Write(sb.ToString());
                    }
                    sw.WriteLine(Environment.NewLine);
                }
                using (StreamWriter sw2 = new StreamWriter("T:\\finalpuzzleappearances.txt"))
                {
                    foreach (KeyValuePair<string, PuzzleAppearance[]> kvp in puzzleAppearances)
                    {
                        PrintPuzzleAppearances(
                            kvp.Value,
                            kvp.Key,
                            sw2,
                            totalPuzzles,
                            totalflipped
                         );
                    }
                }
            }
        }

        static private void PrintPuzzleAppearances(PuzzleAppearance[] appear, string puzzle, StreamWriter sw, ulong total, ulong flipped)
        {
            PuzzleAppearance totalInfo = appear[appear.Length - 1];
            Array.Resize(ref appear, appear.Length - 1);
            sw.WriteLine("Stats for {0} ({1} puzzles)", puzzle, appear.Length);
            Array.Sort(appear, new Comparison<PuzzleAppearance>((x, y) => { return y.times.CompareTo(x.times); }));
            sw.WriteLine(
                "Puzzle {0} = {1} ({2}%) - Flipped {3} ({4}%)",
                appear[0].puzzle,
                appear[0].times,
                (appear[0].times / (float)totalInfo.times) * 100,
                appear[0].timesFlipped,
                (appear[0].timesFlipped / (float)appear[0].times) * 100
            );
            PuzzleAppearance last = appear[appear.Length - 1];
            sw.WriteLine(
                "Puzzle {0} = {1} ({2}%) - Flipped {3} ({4}%)",
                last.puzzle,
                last.times,
                (last.times / (float)totalInfo.times) * 100,
                last.timesFlipped,
                (last.timesFlipped / (float)last.times) * 100
            );
            //for (int i = 0; i < appear.Length; ++i)
            //{
            //    sw.WriteLine(
            //        "Puzzle {0} = {1} ({2}%) - Flipped {3} ({4}%)",
            //        appear[i].puzzle,
            //        appear[i].times,
            //        (appear[i].times / (float)totalInfo.times) * 100,
            //        appear[i].timesFlipped,
            //        (appear[i].timesFlipped / (float)appear[i].times) * 100
            //    );
            //}
            sw.WriteLine("Total puzzles seen = {0}", totalInfo.times);
            sw.WriteLine("Total flipped puzzles = {0}", totalInfo.timesFlipped);

            Array.Sort(appear, new Comparison<PuzzleAppearance>((x, y) => { return y.timesFlipped.CompareTo(x.timesFlipped); }));
            sw.WriteLine("Most flipped puzzle: {0} ({1}%)", appear[0].puzzle, (appear[0].timesFlipped / (float)appear[0].times) * 100);
            last = appear[appear.Length - 1];
            sw.WriteLine("Least flipped puzzle: {0} ({1}%)", last.puzzle, (last.timesFlipped / (float)last.times) * 100);
            sw.WriteLine();
        }

        /*
IQ Final RNG
rand RNG is never seeded (no calls to srand, seed is only directly written to 0 it out on startup), rand is called once per frame during the game selection screens and while on a screen with a spinning globe

At the start of the game before the first stage starts, one more actual RNG number is taken. This number is multiplied by 5 and has 1 added is used to seed the puzzle RNG. For every puzzle that needs generating, the updated number is again multiplied by 5 and has 1 added to it. The new number replaces the old seed, and its lower 16 bits are then mathed to a number between 0 and the number of puzzles.
         * */

        static public ushort DetermineWhichPuzzle(uint arg, WaveInformation wi)
        {
            ushort numPuzzles = wi.numPuzzles; // this is lw $v0, dword_1F800050; lw $v0, 0xDBC($v0); lh $v1, 6($v0)
            int puzzlesToDivBy100 = 500; // li      $v0, 500
            /*
             * bne     $v1, $v0, loc_800438B8
             * li      $a3, 200
             * li      $a3, 100*/
            uint divisor = (numPuzzles != puzzlesToDivBy100) ? 200u : 100u; // the above sequence
            uint multiplicand = 0x51eb851f; // li      $a2, 0x51EB851F
            arg &= 0xffff;
            // all this divide by 50, then multiplies by 50
            ulong mulRes = UBigMul(arg, multiplicand); // multu
            uint highRes = (uint)(mulRes >> 32); // mfhi
            uint twoPercent = highRes >> 4; // srl     $v1, $t0, 4
            uint runningVal = twoPercent << 1; // sll     $v0, $v1, 1
            runningVal += twoPercent; // addu    $v0, $v1
            runningVal <<= 3; //sll     $v0, 3
            runningVal += twoPercent; // addu    $v0, $v1
            runningVal <<= 1; // sll     $v0, 1
            arg -= runningVal; // subu    $a0, $v0
            arg &= 0xffff; // andi a0, 0xffff
            uint a1 = twoPercent;
            if (arg < 23)
            {
                // if above = 
                // sltiu   $v0, $a0, 23     # v0 = (a0 < 23)
                // beqz    $v0, loc_80043930  # jump if not (a0 >= 23)
                // move    $a1, $v1         # a1 = 2%
                //
                // div     $v0, $a3
                // mfhi    $v1
                // move    $a1, $v1
                uint a1Anded = a1 & 0xffff;
                a1 = a1Anded % divisor;
            }
            else if (arg < 38)
            {
                arg = a1 & 0xffff;
                // else if above =
                // sltiu   $v0, $a0, 38
                // beqz    $v0, loc_80043968
                mulRes = UBigMul(arg, multiplicand); // multu   $a0, $a2
                highRes = (uint)(mulRes >> 32); // mfhi
                uint halfPercent = highRes >> 6; // srl     $v1, $t0, 6
                runningVal = halfPercent << 1; // sll     $v0, $v1, 1
                runningVal += halfPercent; // addu    $v0, $v1
                runningVal <<= 3; //sll     $v0, 3
                runningVal += halfPercent; // addu    $v0, $v1
                runningVal <<= 3; // sll     $v0, 3
                arg -= runningVal; // subu    $a0, $v0
                a1 = divisor + arg; // addu    $a1, $a3, $a0
            }
            else // arg >= 38
            {
                arg = a1 & 0xffff;
                mulRes = UBigMul(arg, multiplicand); // multu   $a0, $a2
                highRes = (uint)(mulRes >> 32); // mfhi
                uint halfPercent = highRes >> 6; // srl     $v1, $t0, 6
                runningVal = halfPercent << 1; // sll     $v0, $v1, 1
                runningVal += halfPercent; // addu    $v0, $v1
                runningVal <<= 3; //sll     $v0, 3
                runningVal += halfPercent; // addu    $v0, $v1
                runningVal <<= 3; // sll     $v0, 3
                arg -= runningVal; // subu    $a0, $v0
                uint temp = divisor + 200;
                a1 = arg + temp;
            }
            // this is 80043994
            uint temp2 = a1 & 0xffff; // andi    $v0, $a1, 0xFFFF
            while (temp2 >= numPuzzles)
            {
                a1 -= 200;
                temp2 = a1 & 0xffff;
            }
            //a1 += 200;
            return (ushort)(a1 & 0xffffu);
        }
    }
}
