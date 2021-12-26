using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using PuzzlePreview;

namespace KurushRNG
{
    class Program
    {
        const string USAGE =
            "IQTasRNG{0}" +
            "{0}" +
            "Usage: IQTasRNG <gameName> <outputDir>{0}" +
            "{0}" +
            "Where <gameName> is either IQ or IQFinal to pick which game output is generated for{0}" +
            "and <outputDir> is where to put the text and diagram output";

        private static IGameInformation g_gameInfo;
        private static string g_outputDir;

        class PuzzleSet
        {
            public List<PickedPuzzle>[] waves;
            public PuzzleSet(List<PickedPuzzle>[] inWaves)
            {
                waves = inWaves;
            }
        }

        class PickedPuzzle
        {
            public bool flipped;
            public bool canDupe;
            public bool isDupe;
            public byte[] puzzle;

            public PickedPuzzle(byte[] puzzleData, int inPuzzle, int inPoints, bool inFlipped, bool inCanDupe, bool inIsDupe)
            {
                PuzzleNumber = inPuzzle;
                Points = inPoints;
                flipped = inFlipped;
                canDupe = inCanDupe;
                puzzle = puzzleData;
                isDupe = inIsDupe;
            }

            public PickedPuzzle Clone()
            {
                byte[] newPuzzle = new byte[puzzle.Length];
                Buffer.BlockCopy(puzzle, 0, newPuzzle, 0, puzzle.Length);
                return new PickedPuzzle(newPuzzle, PuzzleNumber, Points, flipped, canDupe, isDupe);
            }

            public PickedPuzzle CloneAsDupe()
            {
                byte[] newPuzzle = new byte[puzzle.Length];
                Buffer.BlockCopy(puzzle, 0, newPuzzle, 0, puzzle.Length);
                return new PickedPuzzle(newPuzzle, PuzzleNumber, Points, flipped, canDupe, true);
            }

            public int Capturable
            {
                get
                {
                    int numCapturable = 0;
                    foreach(byte b in puzzle)
                    {
                        if(b < 2)
                        {
                            ++numCapturable;
                        }
                    }
                    return numCapturable;
                }
            }

            public int Points
            {
                get;
                private set;
            }

            public int PuzzleNumber
            {
                get;
                private set;
            }
        }

        class Wave : IEnumerable<PickedPuzzle>
        {
            public int numPuzzles;
            public PuzzleSizeDetails puzzleInfo;
            public List<PickedPuzzle> puzzles;

            public Wave(List<PickedPuzzle> inPuzzles, PuzzleSizeDetails inPuzzleInfo)
            {
                puzzles = inPuzzles;
                puzzleInfo = inPuzzleInfo;
                numPuzzles = puzzles.Count;
                Recalculate();
            }

            public void Recalculate()
            {
                int capt = 0;
                int points = 0;
                int turns = 0;
                foreach (PickedPuzzle p in puzzles)
                {
                    capt += p.Capturable;
                    points += p.Points;
                    turns += puzzleInfo.trns[p.PuzzleNumber];
                }
                Capturable = capt;
                Points = points;
                Turns = turns;
            }

            public Wave Clone()
            {
                List<PickedPuzzle> clonedPuzList = new List<PickedPuzzle>();
                puzzles.ForEach(
                    (x) =>
                    {
                        clonedPuzList.Add(x.Clone());
                    }
                );
                return new Wave(clonedPuzList, puzzleInfo);
            }

            public int width
            {
                get
                {
                    return puzzleInfo.width;
                }
            }

            public int height
            {
                get
                {
                    return puzzleInfo.height;
                }
            }

            public int Points
            {
                get;
                private set;
            }

            public int Capturable
            {
                get;
                private set;
            }

            public int Turns
            {
                get;
                private set;
            }

            public int GetBestScoreAndPosition(out int index)
            {
                int bestScore = 0, bestIndex = 0;
                for (int i = 0; i < puzzles.Count; ++i)
                {
                    int puzzPoints = puzzles[i].Points;
                    if (puzzPoints > bestScore)
                    {
                        bestScore = puzzPoints;
                        bestIndex = i;
                    }
                }
                index = bestIndex;
                return bestScore;
            }

            public PickedPuzzle this[int i]
            {
                get
                {
                    return puzzles[i];
                }
            }

            public IEnumerator<PickedPuzzle> GetEnumerator()
            {
                return puzzles.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return puzzles.GetEnumerator();
            }
        }

        class WaveSet : IEnumerable<Wave>
        {
            public List<Wave> waves;
            public WaveSet()
            {
                waves = new List<Wave>();
            }

            public void Add(Wave w)
            {
                waves.Add(w);
            }

            public int Points
            {
                get
                {
                    int points = 0;
                    foreach (Wave w in waves)
                    {
                        points += w.Points;
                    }
                    return points;
                }
            }

            public int Capturable
            {
                get
                {
                    int num = 0;
                    foreach (Wave w in waves)
                    {
                        num += w.Capturable;
                    }
                    return num;
                }
            }

            public int Turns
            {
                get
                {
                    int num = 0;
                    foreach (Wave w in waves)
                    {
                        num += w.Turns;
                    }
                    return num;
                }
            }

            public Wave this[int i]
            {
                get
                {
                    return waves[i];
                }
            }

            public WaveSet Clone()
            {
                WaveSet ws = new WaveSet();
                foreach (Wave w in waves)
                {
                    ws.Add(w.Clone());
                }
                return ws;
            }

            public IEnumerator<Wave> GetEnumerator()
            {
                return waves.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return waves.GetEnumerator();
            }
        }

        class WaveSetInfo
        {
            public WaveSetInfo(int frame, WaveSet waveSet)
            {
                Frame = frame;
                Set = waveSet;
            }

            public int Frame
            {
                get;
                private set;
            }

            public WaveSet Set
            {
                get;
                private set;
            }

            public int Points
            {
                get
                {
                    return Set.Points;
                }
            }

            public int Capturable
            {
                get
                {
                    return Set.Capturable;
                }
            }

            public int Turns
            {
                get
                {
                    return Set.Turns;
                }
            }
        }

        static byte[] CopyPuzzle(byte[] puzzleData, int width, int height, bool flipped)
        {
            byte[] puzzle = new byte[width * height];
            Buffer.BlockCopy(puzzleData, 0, puzzle, 0, puzzleData.Length);
            if (flipped)
            {
                for (int y = 0; y < height; ++y)
                {
                    Array.Reverse(puzzle, y * width, width);
                }
            }
            return puzzle;
        }

        static List<WaveSetInfo> Find10BestInRNGRange(int rngStartSeed, int rngEndSeed, List<StageWaveInformation> wavesToGenerate)
        {
            int numGood = 0;
            List<WaveSetInfo> best10Sets = new List<WaveSetInfo>();
            // i = number of frames rendered 
            for (; rngStartSeed <= rngEndSeed; ++rngStartSeed)
            {
                g_gameInfo.FrameSeed = rngStartSeed;
                WaveSet noDupeSet = new WaveSet();
                foreach (StageWaveInformation swi in wavesToGenerate)
                {
                    Wave w = GetPuzzleSet(swi);
                    if (w == null) break;
                    //if (wave1Pts == 0)
                    //{
                    //    continue;
                    //}
                    noDupeSet.Add(w);
                }
                if (noDupeSet.waves.Count != wavesToGenerate.Count) continue;
                ++numGood;

                WaveSet bestSet = g_gameInfo.CheckSquashedScores ? CheckSquashedScores(noDupeSet) : noDupeSet;
                int bestPoints = bestSet.Points;
                WaveSetInfo wsi = new WaveSetInfo(rngStartSeed, bestSet);
                int numCaptures = bestSet.Capturable;
                int turns = bestSet.Turns;
#if CAPTURES
                int insertIndex = best10Sets.FindIndex((x) => { return numCaptures > x.Capturable; });
#elif POINTS
                int insertIndex = best10Sets.FindIndex((x) => { return bestPoints > x.Points; });
#elif TURNS
                int insertIndex = best10Sets.FindIndex((x) => { return turns < x.Turns; });
#endif
                if (insertIndex == -1)
                {
                    if (best10Sets.Count < 10)
                    {
                        best10Sets.Add(wsi);
                    }
                }
                else
                {
                    best10Sets.Insert(insertIndex, wsi);
                    if (best10Sets.Count > 10)
                    {
                        best10Sets.RemoveAt(10);
                    }
                }
            }
            return best10Sets;
        }

        static void FindBestOverallCaptures(List<StageWaveInformation>[] stages)
        {
            int rngStart = 940;
            int rngEnd = 0x800;
            List<WaveSetInfo> selectedWaves = new List<WaveSetInfo>(stages.Length);
            foreach (List<StageWaveInformation> stage in stages)
            {
                List<WaveSetInfo> best10 = Find10BestInRNGRange(rngStart, rngEnd, stage);
                best10.Sort(
                    (x, y) => {
                        if (x.Capturable != y.Capturable)
                        {
                            return x.Capturable.CompareTo(y.Capturable);
                        }
                        else return x.Frame.CompareTo(y.Frame);
                    }
                );
                WaveSetInfo best1 = best10[0];
                selectedWaves.Add(best1);
                rngStart = best1.Frame + 3600;
                rngEnd = rngStart + (2 * 3600);
            }
            int capturable = 0;
            foreach (WaveSetInfo wsi in selectedWaves)
            {
                capturable += wsi.Capturable;
                DrawPuzzles("T:\\IQ\\best", wsi);
            }
            Console.WriteLine("Best of the best of each stage totals {0}/3259 capturable cubes ({1:F1}%)", capturable, (capturable / 3259.0f) * 100);
        }

        static IGameInformation LoadGameInfo(string game)
        {
            switch(game.ToLowerInvariant())
            {
                case "iq":
                {
                    return new IQ();
                }
                break;
                case "iqfinal":
                {
                    IQFinal final = new IQFinal();
                    // we don't have to deal with tektonics
                    final.Waves.RemoveRange(final.Waves.Count - 20, 20);
                    return final;
                }
                break;
                default:
                {
                    return null;
                }
                break;
            }
        }

        static void Main(string[] args)
        {
            if ((args.Length < 2) || ((g_gameInfo = LoadGameInfo(args[0])) == null))
            {
                Console.WriteLine(USAGE, Environment.NewLine);
                return;
            }
            g_outputDir = args[1];

            //FindBestOverallCaptures(stages);

            int startPoint = 508;
            int endPoint = 2508;
            //int endPoint = 0x3c41;
            List<WaveSetInfo> best10Sets = Find10BestInRNGRange(startPoint, endPoint, g_gameInfo.Waves);
            
            //Console.WriteLine("Found {0} candidates out of a total of {1}", numGood, (endPoint - startPoint) + 1);
            Console.WriteLine("Best 10 shortest turn sequences found between frames {0}-{1} are", startPoint, endPoint);
            foreach (WaveSetInfo result in best10Sets)
            {
                Console.WriteLine("Frame {0} - {1} points, {2} captures, {3} turns", result.Frame, result.Points, result.Capturable, result.Turns);
                PrintPuzzles(result.Set);
                DrawPuzzles(g_outputDir, result);
            }
        }

        private static void DrawPuzzles(string outDir, WaveSetInfo puzzleSetInfo)
        {
            int maxTileWidth = 6;
            int numRows = 0;
            foreach (Wave w in puzzleSetInfo.Set)
            {
                numRows += (w.height + 2);
                maxTileWidth = Math.Max(maxTileWidth, (w.width + 1) * 3);
            }
            numRows += 2; // for header & footer
            const int rectWidth = 40;
            const int rectHeight = 40;
            using (Bitmap bm = new Bitmap((maxTileWidth * rectWidth) + 20, (numRows * rectHeight) + 10, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                Brush textColour = Brushes.Black;
                Pen p = Pens.Black;
                using(Font f = new Font(FontFamily.GenericSansSerif, 16))
                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.Clear(Color.DarkGray);
                    string header = String.Format("Frame {0} - Points {1} - Captures {2} - Turns {3} - flipped already flipped", puzzleSetInfo.Frame, puzzleSetInfo.Points, puzzleSetInfo.Capturable, puzzleSetInfo.Turns);
                    g.DrawString(header, f, textColour, new PointF(0, 0));
                    int wave = 1;
                    int yLoc = 40;
                    int[] qubes = new int[3]{0, 0, 0};
                    Brush[] qubeBrushes = new Brush[3] {Brushes.BurlyWood, Brushes.Green, Brushes.Black};
                    foreach(Wave w in puzzleSetInfo.Set)
                    {
                        string waveHeaderText = String.Format("Wave {0}: {1}x{2}", wave, w.width, w.height);
                        g.DrawString(waveHeaderText, f, textColour, new PointF(0, yLoc));
                        yLoc += rectHeight;
                        int xLoc = 0;
                        string puzzleHeaderFormat = "Puzzle {0} - TRN {1}{2}{3}";
                        int puzzle = 0;
                        PickedPuzzle pp = w[puzzle];
                        g.DrawString(
                            String.Format(
                                puzzleHeaderFormat, 
                                pp.PuzzleNumber + 1, 
                                w.puzzleInfo.trns[pp.PuzzleNumber], 
                                pp.flipped ? " (f)" : String.Empty,
                                pp.isDupe ? " (s)" : String.Empty
                            ),
                            f,
                            textColour,
                            new PointF(0, yLoc)
                        );
                        yLoc += rectHeight;
                        for (int y = 0; y < w.height; ++y)
                        {
                            puzzle = 0;
                            pp = w[puzzle];
                            for (int x = 0, puzzX = 0; x < (w.width * w.numPuzzles); ++x, ++puzzX)
                            {
                                if ((x != 0) && ((x % w.width) == 0))
                                {
                                    ++puzzle;
                                    pp = w[puzzle];
                                    puzzX = 0;
                                    xLoc += rectWidth;
                                    if (y == 0)
                                    {
                                        g.DrawString(
                                            String.Format(
                                                puzzleHeaderFormat, 
                                                pp.PuzzleNumber + 1, 
                                                w.puzzleInfo.trns[pp.PuzzleNumber], 
                                                pp.flipped ? " (f)" : String.Empty,
                                                pp.isDupe ? " (s)" : String.Empty
                                            ),
                                            f,
                                            textColour,
                                            new PointF(xLoc, yLoc - rectHeight)
                                        );
                                    }
                                }
                                byte qube = pp.puzzle[(y * w.width) + puzzX];
                                Brush b = qubeBrushes[qube];
                                ++qubes[qube];
                                Rectangle r = new Rectangle(xLoc, yLoc, rectWidth, rectHeight);
                                g.FillRectangle(b, r);
                                g.DrawRectangle(p, r);
                                xLoc += rectWidth;
                            }
                            yLoc += rectHeight;
                            xLoc = 0;
                        }
                        ++wave;
                    }
                    int totalCubes = qubes[0] + qubes[1] + qubes[2];
                    float totalCubesF = (float)totalCubes;
                    g.DrawString(
                        String.Format(
                            "Normal {0}/{3}({4:F1}%), Adv {1}/{3}({5:F1}%) Forb {2}/{3}({6:F1}%)",
                            qubes[0],
                            qubes[1],
                            qubes[2],
                            totalCubes,
                            (qubes[0] / totalCubesF) * 100,
                            (qubes[1] / totalCubesF) * 100,
                            (qubes[2] / totalCubesF) * 100
                        ),
                        f,
                        textColour,
                        new PointF(0, yLoc)
                    );
                }
#if CAPTURES
                string fileNameFormat = "{2}captures-{1}points-frame{0}.png";
#elif POINTS
                string fileNameFormat = "{1}points-{2}captures-frame{0}.png";
#elif TURNS
                string fileNameFormat = "{3}turns-{2}captures-frame{0}.png";
#endif
                string fileName = String.Format(fileNameFormat, puzzleSetInfo.Frame, puzzleSetInfo.Points, puzzleSetInfo.Capturable, puzzleSetInfo.Turns);
                bm.Save(Path.Combine(outDir, fileName), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void PrintPuzzles(WaveSet puzzleSet)
        {
            const string flipped = "(f) ";
            foreach (Wave w in puzzleSet)
            {
                Console.Write("{0}x{1}: ", w.width, w.height);
                foreach (PickedPuzzle p in w)
                {
                    Console.Write("{0} {1}{2}", p.PuzzleNumber + 1, p.flipped ? flipped : String.Empty, p.isDupe ? "(s)" : String.Empty);
                }
                Console.WriteLine();
            }
        }

        private static Wave GetPuzzleSet(StageWaveInformation waveInfo)
        {
            int points = 0;
            PuzzleSizeDetails puzzleDets = waveInfo.WaveInfo;
            int width = puzzleDets.width;
            int height = puzzleDets.height;
            int area = width * height;
            int numPuzzles = waveInfo.NumPuzzles;
            List<PickedPuzzle> wavePuzzles = new List<PickedPuzzle>(numPuzzles);
            bool flipped;
            int[] trns = puzzleDets.trns;
            for (int i = 0; i < numPuzzles; ++i)
            {
                int nextPuzzle = g_gameInfo.GetNextPuzzle(puzzleDets.puzzles.Count, out flipped);
                Debug.Assert((nextPuzzle >= 0) && (nextPuzzle < puzzleDets.puzzles.Count));
                byte[] puzzleData = puzzleDets.puzzles[nextPuzzle];
                int puzTrn = trns[nextPuzzle];
                if (puzTrn == 1) return null;
                byte[] puzzle = CopyPuzzle(puzzleData, width, height, flipped);
                bool hasForbidden;
                int thisPuzzlePoints = CalcPuzzlePoints(puzzle, out hasForbidden);
                PickedPuzzle puzz = new PickedPuzzle(puzzle, nextPuzzle, thisPuzzlePoints, flipped, !hasForbidden, false);
                wavePuzzles.Add(puzz);
                points += thisPuzzlePoints;
            }
            return new Wave(wavePuzzles, puzzleDets);
        }

        private static PickedPuzzle GetPuzzlePoints(byte[] puzzleData, int puzTrn, int width, int height, PickedPuzzle pp)
        {
            byte[] puzzle = CopyPuzzle(puzzleData, width, height, pp.flipped);
            bool hasForbidden;
            int thisPuzzlePoints = CalcPuzzlePoints(puzzle, out hasForbidden);
            return new PickedPuzzle(puzzle, pp.PuzzleNumber, (puzTrn == 1) ? 0 : thisPuzzlePoints, pp.flipped, !hasForbidden, pp.isDupe);
        }

        private static WaveSet CheckSquashedScores(WaveSet waves)
        {
            return DoWaveDupeRecursion(waves, 0, 0);
        }

        private static WaveSet DoWaveDupeRecursion(WaveSet baseSet, int wave, int puzzle)
        {
            int numWaves = baseSet.waves.Count;
            if (
                (wave >= numWaves) ||
                ((puzzle >= baseSet[wave].numPuzzles)) && (wave == (numWaves - 1)))
            {
                return baseSet;
            }
            Wave w = baseSet[wave];
            if (puzzle >= w.numPuzzles)
            {
                ++wave;
                w = baseSet[wave]; 
                puzzle = 0;
            }
            WaveSet afterDupingThisPuzzle;
            // we can't dupe the last puzzle of each wave
            // or if it has a forbidden cube
            if ((puzzle == (w.numPuzzles - 1)) || (!w[puzzle].canDupe))
            {
                afterDupingThisPuzzle = baseSet;
            }
            else
            {
                afterDupingThisPuzzle = CalculateKnockOnEffect(baseSet, wave, puzzle);
                // if this change would've introduced a puzzle with a TRN of 1
                // discard this dupe
                if (afterDupingThisPuzzle == null) afterDupingThisPuzzle = baseSet;
            }
            WaveSet noDupeSet = DoWaveDupeRecursion(baseSet, wave, puzzle + 1);
            WaveSet dupeSet = noDupeSet;
            if (afterDupingThisPuzzle != baseSet)
            {
                dupeSet = DoWaveDupeRecursion(afterDupingThisPuzzle, wave, puzzle + 2);
            }
#if POINTS
            int dupePoints = dupeSet.Points;
            int noDupePoints = noDupeSet.Points;
            WaveSet ret = dupePoints > noDupePoints ? dupeSet : noDupeSet;
#elif TURNS
            int dupeTurns = dupeSet.Turns;
            int noDupeTurns = noDupeSet.Turns;
            WaveSet ret = dupeTurns < noDupeTurns ? dupeSet : noDupeSet;
#elif CAPTURES
            int dupeCaptures = dupeSet.Capturable;
            int noDupeCaptures = noDupeSet.Capturable;
            WaveSet ret = dupeCaptures > noDupeCaptures ? dupeSet : noDupeSet;
#endif
            return ret;
        }

        private static WaveSet CalculateKnockOnEffect(WaveSet originalSet, int startWaveIdx, int startPoint)
        {
            WaveSet newSet = originalSet.Clone();
            Debug.Assert(startPoint < (newSet[startWaveIdx].numPuzzles - 1));
            // for these waves, ditch the last puzzle score and calculate the score of
            // the last puzzle of the previous wave as if that puzzle number had happened in this wave
            for (int i = 3; i > startWaveIdx; --i)
            {
                Wave w = newSet[i];
                List<PickedPuzzle> wavePuzzles = w.puzzles;
                wavePuzzles.RemoveAt(wavePuzzles.Count - 1);
                Wave prevWave = newSet[i - 1];
                List<PickedPuzzle> prevWavePuzzles = prevWave.puzzles;
                PickedPuzzle lastPrevWave = prevWavePuzzles[prevWavePuzzles.Count - 1];
                byte[] newPuzzleDetails = w.puzzleInfo.puzzles[lastPrevWave.PuzzleNumber];
                int trn = w.puzzleInfo.trns[lastPrevWave.PuzzleNumber];
                PickedPuzzle prevLastInThisWave = GetPuzzlePoints(newPuzzleDetails, trn, w.width, w.height, lastPrevWave);
                // puzzles with a TRN of 1 are no good, so we cancel
                if (prevLastInThisWave.Points == 0) return null;
                prevLastInThisWave.flipped = lastPrevWave.flipped;
                wavePuzzles.Insert(0, prevLastInThisWave);
                w.Recalculate();
            }
            // on this wave, we just need to insert the highscore at the same position (ie duplicate it)
            // then remove the last puzzle
            Wave startWave = newSet[startWaveIdx];
            List<PickedPuzzle> startWavePuzzles = startWave.puzzles;
            startWavePuzzles.Insert(startPoint, startWavePuzzles[startPoint].CloneAsDupe());
            startWavePuzzles.RemoveAt(startWavePuzzles.Count - 1);
            startWave.Recalculate();
//#if DEBUG
//            Debug.WriteLine("Orig puzzle set:");
//            for (int i = 0; i < originalSet.waves.Count; ++i)
//            {
//                Wave oldWave = originalSet[i];
//                for (int j = 0; j < oldWave.numPuzzles; ++j)
//                {
//                    PickedPuzzle pp = oldWave.puzzles[j];
//                    Debug.Write(String.Format("{0}={1} ", pp.puzzleNumber, pp.points));
//                }
//            }
//            Debug.WriteLine(String.Format("{0}New puzzle set:", Environment.NewLine));
//            for (int i = 0; i < newSet.waves.Count; ++i)
//            {
//                Wave newWave = newSet[i];
//                for (int j = 0; j < newWave.numPuzzles; ++j)
//                {
//                    PickedPuzzle pp = newWave.puzzles[j];
//                    Debug.Write(String.Format("{0}={1} ", pp.puzzleNumber, pp.points));
//                }
//            }
//            Debug.WriteLine("");
//#endif
            return newSet;
        }

        private static int CalcPuzzlePoints(byte[] puzzle, out bool hasForbidden)
        {
            int points = 10000;
            int extraPoints = 0;
            hasForbidden = false;
            foreach (byte b in puzzle)
            {
                if (b < 2)
                {
                    points += ((b + 1) * 100);
                    // add on extra points for advantage cubes
                    if (b == 1)
                    {
                        extraPoints += 400;
                    }
                }
                else
                {
                    // subtract extra points for forbidden cubes
                    extraPoints -= 1000;
                    hasForbidden = true;
                }
            }
            return points + extraPoints;
        }
    }
}
