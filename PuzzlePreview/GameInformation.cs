using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzlePreview
{
    class PuzzleSizeDetails
    {
        public int[] trns;
        public List<byte[]> puzzles;
        public int width;
        public int height;
    }

    class StageWaveInformation
    {
        public PuzzleSizeDetails WaveInfo {get; private set;}
        public int NumPuzzles {get; private set;}

        public StageWaveInformation(int numPuzzles, PuzzleSizeDetails puzzles)
        {
            WaveInfo = puzzles;
            NumPuzzles = numPuzzles;
        }
    }

    interface IGameInformation
    {
        List<PuzzleSizeDetails> PuzzleInfo
        {
            get;
        }

        List<StageWaveInformation> Waves
        {
            get;
        }

        bool CheckSquashedScores { get; }
        int Seed {get; set;}
        int FrameSeed { get; set; }
        int GetNextPuzzle(int numSizePuzzles, out bool isFlipped);
    }
}
