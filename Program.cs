using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace IQTracker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new IQTrackForm());
        }

//        static int seed;
//        static int Rand()
//        {
//            int s = seed;
//            long mul = s * 0x41C64E6D;
//            int loRes = (int)(mul & 0xFFFFFFFF);
//            loRes += 0x3039;
//            int shRes = loRes >> 16;
//            seed = loRes;
//            return shRes & 0x7FFF;
//        }

//        /*
//        TEXT:80023A14                 lbu     $a0, g_forcePuzzlePattern
//        TEXT:80023A14                 nop
//        TEXT:80023A18                 andi    $v1, $a0, 0xFF
//        TEXT:80023A1C                 bne     $v1, $v0, loc_80023A64
//        TEXT:80023A20                 move    $a1, $zero
//        TEXT:80023A24                 jal     rand
//        TEXT:80023A28                 nop
//        TEXT:80023A2C                 li      $v1, 0x51EB851F  # v0 = rand result
//        TEXT:80023A34                 mult    $v0, $v1
//        TEXT:80023A38                 sra     $v1, $v0, 31
//        TEXT:80023A3C                 mfhi    $t9
//        TEXT:80023A40                 sra     $a0, $t9, 6      # This shift & the mult = divide by 200
//        TEXT:80023A44                 subu    $a0, $v1
//        TEXT:80023A48                 sll     $v1, $a0, 1
//        TEXT:80023A4C                 addu    $v1, $a0
//        TEXT:80023A50                 sll     $v1, 3
//        TEXT:80023A54                 addu    $v1, $a0 # all this multiplies by 200
//        TEXT:80023A58                 sll     $v1, 3
//        TEXT:80023A5C                 subu    $a0, $v0, $v1 # this subtracts the original rand
//        */

//        static int g_forcePuzzlePattern = 200;
//        static int g_currentPuzzle;

//        // 70 bytes between puzzles
//        // 14,000 bytes between puzzle sizes
//        static private int GetNextPuzzlePattern()
//        {
//            int nextVal = g_forcePuzzlePattern;
//            if (nextVal == 200)
//            {
//                int r = Rand();
//                int rDiv20 = r / 200;
//                int rShift31 = r >> 31;
//                nextVal = rDiv20 - rShift31;
//                nextVal *= 200;
//                //int nextDouble = nextVal << 1;
//                //nextDouble += nextVal;
//                //nextDouble <<= 3;
//                //nextDouble += nextVal;
//                //nextDouble <<= 3;
//                nextVal = r - nextVal;
//            }
//            int currentPuzzle = nextVal & 0xff;
//            g_currentPuzzle = currentPuzzle;
//            bool isFlipped = ((Rand() % 100) < 30);
//            return currentPuzzle;
//        }

//        static int g_groupNewFile = 0;
//        static int g_puzzleWidth = 4;
//        static int g_puzzleLength = 2;

//        static int NextBit()
//        {
//            int t6 = g_groupNewFile;
//            int t2 = 0;
//            int t8 = 0;
//            int t7 = 0x8008B678;
//            int width = g_puzzleWidth;
//            int currentPuzzle = g_currentPuzzle;
//loop:
//            if (t8 >= width)
//            {
//                ;
//            }
//            int a3 = 0;
//            int v0 = t2 << 16;
//            int t1 = v0 >> 16;
//            int t4 = t1 * 2604;
//            int puzzleTimes70 = currentPuzzle * 70;
//            int a2 = a3 << 16;
//loopP2:
//            a3 += 1;
//            int a1 = a3;
//            a2 >>= 16;
//            v0 = a2 * 3;
//            int a0 = a2 * 96;
//            a0 -= v0; // a2 * 93
//            int shiftedWidth = (width << 16) >> 16;
//            a0 *= 4; // a2 * 372
//            a0 += t4;
//            int puzLenTimes4 = g_puzzleLength * 4;
//            int offset = t7 + puzLenTimes4 + shiftedWidth;
//        }

//        static void Main()
//        {
//            GetNextPuzzlePattern();
//            Dictionary<int, List<int>> resToSeeds = new Dictionary<int, List<int>>();
//            for (int i = 0; i < UInt16.MaxValue; ++i)
//            {
//                seed = i;
//                int ret = Rand();
//                List<int> seeds;
//                if (resToSeeds.TryGetValue(ret, out seeds))
//                {
//                    seeds.Add(i);
//                }
//                else
//                {
//                    seeds = new List<int>();
//                    seeds.Add(i);
//                    resToSeeds.Add(ret, seeds);
//                }
//            }
//            foreach (KeyValuePair<int, List<int>> resPair in resToSeeds)
//            {
//                StringBuilder sb = new StringBuilder();
//                foreach(int i in resPair.Value)
//                {
//                    sb.AppendFormat("{0}, ", i);
//                }
//                Console.WriteLine("Res {0} = Seeds {1}", resPair.Key, sb.ToString());
//            }
//        }
    }
}
