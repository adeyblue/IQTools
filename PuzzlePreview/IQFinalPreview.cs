using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuzzlePreview
{
    class IQFinal : IGameInformation
    {
        private int frameSeed;

        public IQFinal()
        {
            byte[] puzzleData = File.ReadAllBytes(@"..\..\..\KurushRNG\IQFINAL-QUESTION.CMP");
            LoadPuzzles(puzzleData);
            frameSeed = 0;
        }

        public List<PuzzleSizeDetails> PuzzleInfo
        {
            get;
            private set;
        }

        public List<StageWaveInformation> Waves
        {
            get;
            set;
        }

        public bool CheckSquashedScores
        {
            get
            {
                // you can't get perfect / excellent and re-do the puzzle
                // so there's no point checking
                return false;
            }
        }

        private int Rand()
        {
            int s = Seed;
            long mul = s * 0x41C64E6D;
            int loRes = (int)(mul & 0xFFFFFFFF);
            loRes += 0x3039;
            int shRes = loRes >> 16;
            Seed = loRes;
            return shRes & 0x7FFF;
        }

        public int Seed
        {
            get;
            set;
        }

        public int FrameSeed
        {
            get
            {
                return frameSeed;
            }
            set
            {
                Seed = 0;
                for (int i = 0; i < value; ++i)
                {
                    Rand();
                }
                uint seedSoFar = (uint)Seed; // seed = 0x2e75cf10
                uint initSeed = seedSoFar;
                seedSoFar <<= 2;
                seedSoFar += initSeed;
                seedSoFar += 1;
                Seed = (int)seedSoFar;
                frameSeed = value;
            }
        }

        static private ulong UBigMul(uint a, uint b)
        {
            ulong ulA = (ulong)a;
            ulong ulB = (ulong)b;
            return ulA * ulB;
        }

        public int GetNextPuzzle(int numSizePuzzles, out bool isFlipped)
        {
            uint seed = (uint)Seed;
            uint seedSoFar = seed; // seed = 0x2e75cf10
            seedSoFar <<= 2;
            seedSoFar += seed;
            seedSoFar += 1;
            seed = seedSoFar;
            Seed = (int)seed;

            // all this determines if the puzzle should be flipped - this code at 80043500 to the jump
            ulong flippedRes = UBigMul((uint)seedSoFar, 0xCCCCCCCD);
            uint highRes = (uint)(flippedRes >> 32);
            uint eighth = highRes >> 3;
            uint runningTotal = eighth << 2;
            runningTotal += eighth;
            runningTotal <<= 1;
            uint subRes = seedSoFar - runningTotal;
            isFlipped = subRes < 3;

            uint puzArg = seedSoFar & 0xFFFF;
            return DetermineWhichPuzzle(puzArg, numSizePuzzles);
        }

        static private ushort DetermineWhichPuzzle(uint arg, int wavePuzzles)
        {
            ushort numPuzzles = (ushort)wavePuzzles; // this is lw $v0, dword_1F800050; lw $v0, 0xDBC($v0); lh $v1, 6($v0)
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

        private void LoadPuzzles(byte[] puzzles)
        {
            int puzzlePos = 0;
            List<StageWaveInformation> waves = new List<StageWaveInformation>();
            List<PuzzleSizeDetails> puzzleSizes = new List<PuzzleSizeDetails>(sizeWaves.Length);
            foreach (WaveInformation wi in sizeWaves)
            {
                DecompWavePuzzles(wi, puzzles, ref puzzlePos);
                PuzzleSizeDetails psd = new PuzzleSizeDetails();
                psd.width = wi.puzWidth;
                psd.height = wi.puzHeight;
                psd.trns = wi.trns.ToArray();
                psd.puzzles = wi.decompPuzzles;
                puzzleSizes.Add(psd);
                StageWaveInformation wave = new StageWaveInformation(wi.puzzlesPerWave, psd);
                for (ushort i = 0; i < wi.waveRepeats; ++i)
                {
                    waves.Add(wave);
                }
            }
            Waves = waves;
            PuzzleInfo = puzzleSizes;
        }

        private class WaveInformation
        {
            public ushort waveIndex;
            public byte puzzlesPerWave;
            public byte puzWidth;
            public byte puzHeight;
            public ushort startingPuzzlePosOffset;
            public ushort numPuzzles;
            public ushort waveRepeats;
            public List<int> trns;
            public List<byte[]> decompPuzzles;

            public WaveInformation(ushort waveIdx, byte puzzlesPerWaveIn, byte width, byte height, ushort off, ushort num, ushort repeats)
            {
                waveIndex = waveIdx;
                puzzlesPerWave = puzzlesPerWaveIn;
                puzWidth = width;
                puzHeight = height;
                startingPuzzlePosOffset = off;
                numPuzzles = num;
                waveRepeats = repeats;
                trns = null;
                decompPuzzles = null;
            }
        }

        private WaveInformation[] sizeWaves = new WaveInformation[]
        {
            // 1st stage
            new WaveInformation(0, 3, 4, 2, 0, 0x190, 1),
            new WaveInformation(1, 3, 4, 3, 0x190, 0x190, 1),
            new WaveInformation(2, 3, 4, 4, 0x320, 0x190, 1),
            // 2nd stage
            new WaveInformation(3, 3, 4, 5, 0x4b0, 0x258, 2),
            new WaveInformation(4, 3, 4, 6, 0x708, 0x258, 1),
            // 3rd stage
            new WaveInformation(5, 3, 5, 5, 0x960, 0x258, 1),
            new WaveInformation(6, 3, 5, 6, 0xbb8, 0x258, 2),
            // 4th stage
            new WaveInformation(7, 3, 5, 7, 0xe10, 0x258, 2),
            new WaveInformation(8, 3, 5, 8, 0x1068, 0x1f4, 1),
            // 5th stage
            new WaveInformation(9, 3, 6, 6, 0x125c, 0x258, 1),
            new WaveInformation(10, 3, 6, 7, 0x14b4, 0x258, 2),
            // 6th stage
            new WaveInformation(11, 2, 6, 8, 0x170c, 0x258, 2),
            new WaveInformation(12, 2, 6, 9, 0x1964, 0x1f4, 1),
            // 7th stage
            new WaveInformation(13, 3, 7, 7, 0x1b58, 0x258, 1),
            new WaveInformation(14, 3, 7, 8, 0x1db0, 0x258, 2),
            // 8th stage
            new WaveInformation(15, 2, 7, 9, 0x2008, 0x258, 2),
            new WaveInformation(16, 2, 7, 10, 0x2260, 0xc8, 1),
            // final stage
            new WaveInformation(17, 2, 8, 9, 0x2328, 0xc8, 2),
            new WaveInformation(18, 2, 8, 10, 0x23f0, 0xc8, 1),
            // tectonics
            new WaveInformation(19, 1, 9, 10, 0x24b8, 0x14, 20)
        };

        private void DecompWavePuzzles(WaveInformation wi, byte[] puzzles, ref int puzzlePos)
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
                int thisPuzzPos = puzzlePos;
                int numBytes = puzzles[thisPuzzPos];
                puzzlePos += numBytes;
                Buffer.BlockCopy(puzzles, thisPuzzPos + 1, buffer, 0, numBytes - 1);
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

        private byte DecompressPuzzle(byte[] data, int cubesInPuzzle)
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
    }
}
