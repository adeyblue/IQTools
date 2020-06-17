using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQMania
{
    static class Program
    {
        static KurushRNG.Program.PuzzleStats ComputePuzzleStats(List<byte[]> puzzles, int width, int height)
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

        static byte DecompressPuzzle(byte[] data, int cubesInPuzzle)
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

        class WaveInformation
        {
            public ushort unk;
            public byte puzWidth;
            public byte puzHeight;
            public ushort startingPuzzlePosOffset;
            public ushort numPuzzles;
            public List<int> trns;
            public List<byte[]> decompPuzzles;

            public WaveInformation(ushort inUnk, byte width, byte height, byte unused, ushort off, ushort num)
            {
                unk = inUnk;
                puzWidth = width;
                puzHeight = height;
                startingPuzzlePosOffset = off;
                numPuzzles = num;
                trns = null;
                decompPuzzles = null;
            }
        }

        static void DecompWavePuzzles(WaveInformation wi, byte[] puzzles, List<int> puzzPos)
        {
            int width = wi.puzWidth;
            int height = wi.puzHeight;
            int totalCubes = height * width;
            byte[] buffer = new byte[32];
            byte[] cleanBuffer = new byte[32];
            int lastPuzzle = wi.startingPuzzlePosOffset + wi.numPuzzles;
            List<int> trnList = wi.trns = new List<int>(wi.numPuzzles);
            List<byte[]> decompList = wi.decompPuzzles = new List<byte[]>(wi.numPuzzles);
            for (int i = wi.startingPuzzlePosOffset; i < lastPuzzle; ++i)
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
            }
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

        static public void DumpPuzzles()
        {
            WaveInformation[] waves = new WaveInformation[]
            {
                // unused
                new WaveInformation(1, 4, 2, 0x2, 0, 0x190),
                // Stage 1
                new WaveInformation(1, 4, 3, 0x3, 0x190, 0x190), 
                new WaveInformation(2, 4, 4, 0x4, 0x320, 0x190), 
                new WaveInformation(3, 4, 5, 0x5, 0x4b0, 0x258), 
                new WaveInformation(4, 4, 6, 0x6, 0x708, 0x258), 
                // Stage 2
                new WaveInformation(5, 5, 5, 0x5, 0x960, 0x262), 
                new WaveInformation(6, 5, 6, 0x6, 0xbc2, 0x262), 
                // Stage 3
                new WaveInformation(7, 5, 7, 0x7, 0xe24, 0x260), 
                new WaveInformation(8, 5, 8, 0x8, 0x1084, 0x20d), 
                // Stage 4
                new WaveInformation(9, 6, 6, 0x6, 0x1291, 0x268), 
                new WaveInformation(10, 6, 7, 0x7, 0x14f9, 0x265), 
                // Stage 5
                new WaveInformation(11, 6, 8, 0x8, 0x175e, 0x263), 
                new WaveInformation(12, 6, 9, 0x9, 0x19c1, 0x201), 
                // Stage 6
                new WaveInformation(13, 7, 7, 0x7, 0x1bc2, 0x263), 
                new WaveInformation(14, 7, 8, 0x8, 0x1e25, 0x260), 
                // Stage 7
                new WaveInformation(15, 7, 9, 0x9, 0x2085, 0x25f), 
                new WaveInformation(16, 7, 10, 0xa, 0x22e4, 0xd4), 
                // Stage 8
                new WaveInformation(17, 8, 9, 0x9, 0x23b8, 0xca), 
                new WaveInformation(18, 8, 10, 0xa, 0x2482, 0xd1), 
                // Stage 9
                new WaveInformation(19, 9, 10, 0xa, 0x2553, 0x7c)
            };
            byte[] puzzles = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQMania-remix.cmp");
            List<int> puzzlePos = IQFinal.Program.GetPuzzleOffsets(puzzles);
            int totalCubes = 0;
            int advanCubes = 0;
            int forbidCubes = 0;
            float totalAverage = 0;
            foreach (WaveInformation wi in waves)
            {
                DecompWavePuzzles(wi, puzzles, puzzlePos);
                KurushRNG.Program.TRNStats trnStats;
                KurushRNG.Program.PuzzleStats puzStats = WritePuzzles(wi, out trnStats);
                totalCubes += puzStats.totalPuzzleCubes;
                advanCubes += puzStats.advanCubes;
                forbidCubes += puzStats.forbiddenCubes;
                totalAverage += trnStats.average;
            }
            totalAverage /= waves.Length;
            int normalCubes = totalCubes - advanCubes - forbidCubes;
            Console.WriteLine("I.Q. Mania stats:");
            Console.WriteLine("Average TRN of the game: {0:f2}", totalAverage);
            Console.WriteLine("Total advan cubes: {0} ({1:f2}%)", advanCubes, (((float)advanCubes / totalCubes) * 100.0f));
            Console.WriteLine("Total forbid cubes: {0} ({1:f2}%)", forbidCubes, (((float)forbidCubes / totalCubes) * 100.0f));
            Console.WriteLine("Total normal cubes: {0} ({1:f2}%)", normalCubes, (((float)normalCubes / totalCubes) * 100.0f));
            Console.WriteLine();
        }

        /*
         * byte[] maniaBytes = File.ReadAllBytes(@"C:\Users\Adrian\Downloads\IQ_Mania_Retail_JPN_PSP-Caravan\IQManiaS1.dump");
            int[] widths = new int[] {4, 5, 5, 6, 6, 7, 7, 8, 9 };
            int[] offsets = new int[] {
                0x1df5c4, 0x1DF674, 0x1DF744, 0x1Df814, 0x1Df8E4, 0x1Df9B4, 0x1DFA84, 0x1DFB14, 0x1DfBA4, 0x1DFBE4
            };
            using (StreamWriter sw = new StreamWriter("IQManiaWaves.txt"))
            {
                for (int i = 0; i < widths.Length; ++i)
                {
                    sw.WriteLine("// Stage {0}", i + 1);
                    int offset = offsets[i], nextOffset = offsets[i + 1];
                    while (offset < nextOffset)
                    {
                        ushort index = BitConverter.ToUInt16(maniaBytes, offset);
                        ushort unk1 = BitConverter.ToUInt16(maniaBytes, offset + 2);
                        ushort unk2 = BitConverter.ToUInt16(maniaBytes, offset + 4);
                        ushort unk3 = BitConverter.ToUInt16(maniaBytes, offset + 6);
                        ushort height = BitConverter.ToUInt16(maniaBytes, offset + 8);
                        ushort unk5 = BitConverter.ToUInt16(maniaBytes, offset + 10);
                        ushort runningPuzzleOffset = BitConverter.ToUInt16(maniaBytes, offset + 12);
                        ushort numPuzzles = BitConverter.ToUInt16(maniaBytes, offset + 14);
                        sw.WriteLine(
                            "new WaveInformation({0}, {4}, {5}, 0x{6:x}, 0x{7:x}, 0x{8:x}), ",
                            index, 
                            0, 0,
                            0,
                            widths[i],
                            height,
                            unk5,
                            runningPuzzleOffset,
                            numPuzzles
                        );
                        offset += 0x10;
                    }
                }
            }
         */

        static public ushort FindNum(int offset)
        {
            /* lw      $v0, dword_1F800050  # v0 = 800bc664
TEXT:80043890                                          # a0 = a number
TEXT:80043898                 nop
TEXT:8004389C                 lw      $v0, 0xDBC($v0)  # v0 = 800bb514
TEXT:800438A0                 nop
TEXT:800438A4                 lh      $v1, 6($v0)      # v1 = 0x190

             */
            return (ushort)((offset == 6) ? 0x190u : 0u);
        }

        static ushort DetermineWhichPuzzle(uint arg, WaveInformation wi)
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
            ulong mulRes = IQFinal.Program.UBigMul(arg, multiplicand); // multu
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
                mulRes = IQFinal.Program.UBigMul(arg, multiplicand); // multu   $a0, $a2
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
                mulRes = IQFinal.Program.UBigMul(arg, multiplicand); // multu   $a0, $a2
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
