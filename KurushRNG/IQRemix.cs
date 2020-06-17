using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQRemix
{
    static class Program
    {
        // mazes are stored upside down
        static void VerticallyFlipMaze(byte[] maze, int width, int height)
        {
            byte[] tempRow = new byte[width];
            int halfHeight = height / 2;
            for (int i = 0; i < halfHeight; ++i)
            {
                Array.Copy(maze, i * width, tempRow, 0, width);
                Array.Copy(maze, (height - 1 - i) * width, maze, i * width, width);
                Array.Copy(tempRow, 0, maze, (height - 1 - i) * width, width);
            }
        }

        static KurushRNG.Program.PuzzleStats WritePuzzles(IQFinal.Program.WaveInformation wi, out KurushRNG.Program.TRNStats trnStats)
        {
            int width = wi.puzWidth, height = wi.puzHeight;
            int[] trns = wi.trns.ToArray();
            int numPuzzles = wi.numPuzzles;
            KurushRNG.Program.PuzzleStats puzStats = IQFinal.Program.ComputePuzzleStats(wi.decompPuzzles, width, height);
            trnStats = KurushRNG.Program.ComputeTRNStats(trns);
            return puzStats;
        }

        static public void DumpPuzzles()
        {
            IQFinal.Program.WaveInformation[] waves = new IQFinal.Program.WaveInformation[]
            {
                // Stage 1
                new IQFinal.Program.WaveInformation(0, 5, 5, 5, 0, 0x262), 
                new IQFinal.Program.WaveInformation(1, 5, 5, 6, 0x262, 0x262), 
                new IQFinal.Program.WaveInformation(2, 5, 5, 7, 0x4c4, 0x25D), 
                new IQFinal.Program.WaveInformation(3, 5, 5, 8, 0x721, 0x20A), 
                //new IQFinal.Program.WaveInformation(3, 5, 5, 8, 0x721, 0x20A), 
                //new IQFinal.Program.WaveInformation(3, 5, 5, 8, 0x721, 0x20A), 
                // forbidden maze
                // forbidden wall
                // Stage 2
                new IQFinal.Program.WaveInformation(4, 6, 6, 6, 0x92b, 0x264), 
                new IQFinal.Program.WaveInformation(5, 6, 6, 7, 0xb8f, 0x260), 
                new IQFinal.Program.WaveInformation(6, 6, 6, 8, 0xdef, 0x25e), 
                new IQFinal.Program.WaveInformation(7, 6, 6, 9, 0x104d, 0x1fd), 
                //new IQFinal.Program.WaveInformation(7, 6, 6, 9, 0x104d, 0x1fd),
                //new IQFinal.Program.WaveInformation(7, 6, 6, 9, 0x104d, 0x1fd),
                // forbidden maze
                // forbidden wall
                // Stage 3
                new IQFinal.Program.WaveInformation(8, 7, 7, 7, 0x124a, 0x25f),
                new IQFinal.Program.WaveInformation(9, 7, 7, 8, 0x14a9, 0x25c), 
                new IQFinal.Program.WaveInformation(10, 7, 7, 9, 0x1705, 0x25b), 
                new IQFinal.Program.WaveInformation(11, 7, 7, 10, 0x1960, 0xd4), 
                // forbidden maze
                // forbidden wall
                // Stage 4
                new IQFinal.Program.WaveInformation(12, 8, 8, 9, 0x1a34, 0xc4), 
                new IQFinal.Program.WaveInformation(13, 8, 8, 10, 0x1afe, 0xd1), 
                // forbidden maze
                // forbidden wall
                // Stage 5
                //new IQFinal.Program.WaveInformation(14, 9, 9, 10, 0x1bcf, 0x68), 
                new IQFinal.Program.WaveInformation(14, 9, 9, 10, 0x1bcf, 0x68)
                // forbidden maze
                // forbidden wall
            };
            IQFinal.Program.WaveInformation[] mazeWaves = new IQFinal.Program.WaveInformation[]
            {
                // Stage 1
                new IQFinal.Program.WaveInformation(3, 5, 5, 8, 0, 5), 
                // Stage 2
                new IQFinal.Program.WaveInformation(7, 6, 6, 9, 5, 5), 
                // Stage 3
                new IQFinal.Program.WaveInformation(8, 7, 7, 10, 10, 5),
                // Stage 4
                new IQFinal.Program.WaveInformation(13, 8, 8, 10, 15, 5), 
                // Stage 5
                new IQFinal.Program.WaveInformation(14, 9, 9, 10, 20, 5)
            };
            IQFinal.Program.WaveInformation[] wallWaves = new IQFinal.Program.WaveInformation[]
            {
                // Stage 1
                new IQFinal.Program.WaveInformation(3, 5, 5, 8, 0, 5), 
                // Stage 2
                new IQFinal.Program.WaveInformation(7, 6, 6, 9, 5, 5), 
                // Stage 3
                new IQFinal.Program.WaveInformation(8, 7, 7, 10, 10, 5),
                // Stage 4
                new IQFinal.Program.WaveInformation(13, 8, 8, 10, 15, 5), 
                // Stage 5
                new IQFinal.Program.WaveInformation(14, 9, 9, 10, 20, 5)
            };
            byte[] puzzles = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQRemix-remix.cmp");
            byte[] mazes = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQRemix-mazes.cmp");
            byte[] walls = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQRemix-walls.cmp");
            List<int> puzzlePos = IQFinal.Program.GetPuzzleOffsets(puzzles);
            List<int> mazesPos = IQFinal.Program.GetPuzzleOffsets(mazes);
            List<int> wallsPos = IQFinal.Program.GetPuzzleOffsets(walls);
            int totalCubes = 0;
            int advanCubes = 0;
            int forbidCubes = 0;
            float totalAverage = 0;
            foreach (IQFinal.Program.WaveInformation wi in mazeWaves)
            {
                IQFinal.Program.DecompWavePuzzles(wi, mazes, mazesPos);
                WriteMazes(wi);
            }
            foreach (IQFinal.Program.WaveInformation wi in wallWaves)
            {
                IQFinal.Program.DecompWavePuzzles(wi, walls, wallsPos);
                WriteWalls(wi);
            }
            foreach (IQFinal.Program.WaveInformation wi in waves)
            {
                IQFinal.Program.DecompWavePuzzles(wi, puzzles, puzzlePos);
                KurushRNG.Program.TRNStats trnStats;
                KurushRNG.Program.PuzzleStats puzStats = WritePuzzles(wi, out trnStats);
                totalCubes += puzStats.totalPuzzleCubes;
                advanCubes += puzStats.advanCubes;
                forbidCubes += puzStats.forbiddenCubes;
                totalAverage += trnStats.average;
            }
            totalAverage /= waves.Length;
            int normalCubes = totalCubes - advanCubes - forbidCubes;
            Console.WriteLine("I.Q. Remix+ stats:");
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

        //for IQ Final
        static ushort DetermineWhichPuzzle(uint arg, IQFinal.Program.WaveInformation wi)
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
