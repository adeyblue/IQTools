using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IQTracker
{
    public partial class IQTrackForm : Form
    {
        delegate uint CalcIQU(uint stageScore);
        TextBox[] scoreBoxes = null;
        Label[] iqBoxes = null;
        CalcIQU[] iqCalcFuncsU = null;
        int currentLevel;

        public IQTrackForm()
        {
            InitializeComponent();
            scoreBoxes = new TextBox[] { 
                stage1Score, 
                stage2Score, 
                stage3Score, 
                stage4Score, 
                stage5Score, 
                stage6Score, 
                stage7Score, 
                stage8Score, 
                finalStageScore 
            };
            iqBoxes = new Label[] { 
                stage1IQ, 
                stage2IQ, 
                stage3IQ, 
                stage4IQ, 
                stage5IQ, 
                stage6IQ, 
                stage7IQ, 
                stage8IQ, 
                finalStageIQ 
            };
            iqCalcFuncsU = new CalcIQU[]{
                CalcStage1IQU,
                CalcStage2IQU,
                CalcStage3IQU,
                CalcStage4IQU,
                CalcStage5IQU,
                CalcStage6IQU,
                CalcStage7IQU,
                CalcStage8IQU,
                CalcFinalStageIQU
            };
            currentLevel = 0;
            ResetUI();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateIQs();
            updateButton.Focus();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            ResetUI();
            resetButton.Focus();
        }

        private void iqTrackText_Click(object sender, EventArgs e)
        {
            currentLevel = (currentLevel + 1) % 5;
            iqTrackText.Text = "IQ Tracker - L" + currentLevel.ToString();
        }

        private void ResetUI()
        {
            currentIQText.Text = "0";
            foreach (Label l in iqBoxes)
            {
                l.Text = String.Empty;
            }
            foreach (TextBox t in scoreBoxes)
            {
                t.Text = String.Empty;
            }
        }

        private void UpdateIQs()
        {
            int i = 0;
            uint previousStageScore = 0;
            uint totalIQ = 0;
            for (; i < iqBoxes.Length; ++i)
            {
                string stageScoreText = scoreBoxes[i].Text;
                if (String.IsNullOrEmpty(stageScoreText))
                {
                    break;
                }
                uint totalScore;
                if (!UInt32.TryParse(stageScoreText, out totalScore) || (totalScore < previousStageScore))
                {
                    scoreBoxes[i].BackColor = Color.Gray;
                    break;
                }
                scoreBoxes[i].BackColor = Color.Black;
                uint stageScore = totalScore - previousStageScore;
                previousStageScore = totalScore;
                stageScore = MultiplyStageScore(stageScore);
                uint stageIq = iqCalcFuncsU[i](stageScore);
                iqBoxes[i].Text = stageIq.ToString();
                totalIQ += stageIq;
            }
            totalIQ = Math.Min(totalIQ, 999);
            currentIQText.Text = totalIQ.ToString();
        }

        uint MultiplyStageScore(uint score)
        {
            switch (currentLevel)
            {
                case 0:
                default:
                {
                    // 1x the score
                    return score;
                }
                break;
                case 1: // 1.25x the score
                {
                    return score + (score >> 2);
                }
                break;
                case 2: // 1.33x the score
                {
                    ulong multScore = score << 1;
                    multScore += score;
                    ulong mulRes = multScore * 0xcccccccdu;
                    uint dividedScore = (uint)(mulRes >> 32);
                    dividedScore >>= 3;
                    return score + dividedScore;
                }
                break;
                case 3: // 1.45x the score
                {
                    ulong multScore = score << 3;
                    multScore += score;
                    ulong mulRes = multScore * 0xcccccccdu;
                    uint dividedScore = (uint)(mulRes >> 32);
                    dividedScore >>= 4;
                    return score + dividedScore;
                }
                break;
                case 4: // 1.5x the score
                {
                    return score + (score >> 1);
                }
                break;
            }
        }

        // these are literal translations of the functions in the games assembly code
        // There are easier ways to represent these calculations, but these methods
        // ensure the same results
        uint CalcStage1IQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0x51EB851Fu;
            uint a1 = (uint)(mulRes >> 32);
            uint onePercent = a1 >> 5;
            uint fourPercent = onePercent << 2;
            uint fivePercent = fourPercent + onePercent;
            uint eightyPercent = fivePercent << 4;
            uint seventyFivePercent = eightyPercent - fivePercent;
            ulong sixHundredPercent = seventyFivePercent << 3;;
            mulRes = sixHundredPercent * 0xd1b71759u;
            a1 = (uint)(mulRes >> 32);
            return a1 >> 13;
        }

        uint CalcStage2IQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0x51EB851Fu;
            uint a1 = (uint)(mulRes >> 32);
            uint onePercent = a1 >> 5;
            uint sixteenPercent = onePercent << 4;
            uint seventeenPercent = sixteenPercent + onePercent;
            uint sixtyEightPercent = seventeenPercent << 2;
            uint sixtyNinePercent = sixtyEightPercent + onePercent;
            uint twoSeventySixPercent = sixtyNinePercent << 2;
            uint twoSeventyFivePc = twoSeventySixPercent - onePercent;
            ulong fiveFiftyPc = twoSeventyFivePc << 1;
            mulRes = fiveFiftyPc * 0xd1b71759u;
            a1 = (uint)(mulRes >> 32);
            return a1 >> 13;
        }

        uint CalcStage3IQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0x10624DD3u;
            uint a1 = (uint)(mulRes >> 32);
            return a1 >> 7;
        }

        uint CalcStage4IQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0x51EB851Fu;
            uint a1 = (uint)(mulRes >> 32);
            uint onePercent = a1 >> 5;
            uint eightPercent = onePercent << 3;
            uint sevenPercent = eightPercent - onePercent;
            uint twoTwentyFourPc = sevenPercent << 5;
            uint twoTwentyFivePc = twoTwentyFourPc + onePercent;
            ulong fourFiftyPc = twoTwentyFivePc << 1;
            mulRes = fourFiftyPc * 0xd1b71759u;
            a1 = (uint)(mulRes >> 32);
            return a1 >> 13;
        }

        uint CalcStage5IQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0xd1b71759u;
            uint a1 = (uint)(mulRes >> 32);
            return a1 >> 11;
        }

        uint CalcStage6IQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0x51EB851Fu;
            uint a1 = (uint)(mulRes >> 32);
            uint onePercent = a1 >> 5;
            uint twoPercent = onePercent << 1;
            uint threePercent = twoPercent + onePercent;
            uint twelvePc = threePercent << 2;
            uint elevenPc = twelvePc - onePercent;
            uint oneSevenSixPc = elevenPc << 4;
            uint oneSevenFivePc = oneSevenSixPc - onePercent;
            ulong threeFiftyPc = oneSevenFivePc << 1;
            mulRes = threeFiftyPc * 0xd1b71759u;
            a1 = (uint)(mulRes >> 32);
            return a1 >> 13;
        }

        uint CalcStage7IQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0x51EB851Fu;
            uint a1 = (uint)(mulRes >> 32);
            uint onePercent = a1 >> 5;
            uint fourPercent = onePercent << 2;
            uint fivePercent = fourPercent + onePercent;
            uint eightyPc = fivePercent << 4;
            uint seventyFivePc = eightyPc - fivePercent;
            ulong threeHundredPc = seventyFivePc << 2;
            mulRes = threeHundredPc * 0xd1b71759u;
            a1 = (uint)(mulRes >> 32);
            return a1 >> 13;
        }

        uint CalcStage8IQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0x10624DD3u;
            uint a1 = (uint)(mulRes >> 32);
            return a1 >> 8;
        }

        uint CalcFinalStageIQU(uint multipliedScore)
        {
            ulong multipliedScoreLong = multipliedScore;
            ulong mulRes = multipliedScoreLong * 0xd1b71759u;
            uint a1 = (uint)(mulRes >> 32);
            return a1 >> 12;
        }

        ushort[] g_iqFinalScoreMultipliers = new ushort[]{
            500,
            480,
            250,
            235,
            220,
            205,
            185,
            165,
            155,
            155
        };

        class GamePerformance
        {
            // score at end of stages / 100 (minus bonus)
            // for instance score first stage (without bonus) = 145700 - stageScoresInHundreds[0] = 1457 / 0x5b1
            // at end of stage 2 (without bonus) = 267500 = stageScoresInHundreds[0] = 2675 / 0xa73
            // 0x0
            public ushort[] stageScoresInHundreds;

            // the number of perfects per stage (cumulative). 
            // This counts perfects only, not excellents
            // 0x28 - 800F9CF4
            public ushort[] stagePerfects;

            // the number of excellents per stage (cumulative).
            // so one on the first stage = stageExcellents[0] = 1
            // and two on the second stage = stageExcellents[1] = 3
            // 0x3c
            public ushort[] stageExcellents;

            // cumulative over the game, upto 9 entrues
            // (one for each stage)
            // so for all of first stage = numberOfPuzzlesFaced[0] = 9
            // then for start of second stage = numberOfPuzzlesFaced[1] = 10
            // 0x50
            public ushort[] numberOfPuzzlesFaced;

            // Stage score after multiplication
            // 0x64
            public uint[] adjustedStageScores;

            // 0x8c
            public ushort[] stageIQScores;

            // 0xA0
            public ushort totalIQ;

            public GamePerformance()
            {
                stageScoresInHundreds = new ushort[9];
                stagePerfects = new ushort[9] { 2, 5, 8, 0xc, 0xf, 0xf, 0x12, 0x12, 0x13 };
                stageExcellents = new ushort[9] { 7, 9, 0xb, 0xe, 0x10, 0x12, 0x13, 0x17, 0x19};
                numberOfPuzzlesFaced = new ushort[9];
                adjustedStageScores = new uint[9];
                stageIQScores = new ushort[9];
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        struct MultiType8ByteArray
        {
            [FieldOffset(0)]
            public ulong whole8;

            [FieldOffset(0)]
            public uint lowInt;
            [FieldOffset(4)]
            public uint highInt;

            [FieldOffset(0)]
            public ushort short1;
            [FieldOffset(2)]
            public ushort short2;
            [FieldOffset(4)]
            public ushort short3;
            [FieldOffset(6)]
            public ushort short4;

            [FieldOffset(0)]
            public byte byte1;
            [FieldOffset(1)]
            public byte byte2;
            [FieldOffset(2)]
            public byte byte3;
            [FieldOffset(3)]
            public byte byte4;
            [FieldOffset(4)]
            public byte byte5;
            [FieldOffset(5)]
            public byte byte6;
            [FieldOffset(6)]
            public byte byte7;
            [FieldOffset(7)]
            public byte byte8;

            public ushort shortAt(int i)
            {
                switch (i)
                {
                    case 0: return short1;
                    case 1: return short2;
                    case 2: return short3;
                    case 3: return short4;
                }
                return 0;
            }

            public void shortAt(int i, ushort val)
            {
                switch (i)
                {
                    case 0: short1 = val; break;
                    case 1: short2 = val; break;
                    case 2: short3 = val; break;
                    case 3: short4 = val; break;
                }
            }

            public uint intAt(int i)
            {
                switch (i)
                {
                    case 0: return lowInt;
                    case 1: return highInt;
                }
                return 0;
            }

            public void intAt(int i, uint val)
            {
                switch (i)
                {
                    case 0: lowInt = val; break;
                    case 1: highInt = val; break;
                }
            }

            public byte byteAt(int i)
            {
                switch (i)
                {
                    case 0: return byte1;
                    case 1: return byte2;
                    case 2: return byte3;
                    case 3: return byte4;
                    case 4: return byte5;
                    case 5: return byte6;
                    case 6: return byte7;
                    case 7: return byte8;
                }
                return 0;
            }

            public void byteAt(int i, byte val)
            {
                switch (i)
                {
                    case 0: byte1 = val; break;
                    case 1: byte2 = val; break;
                    case 2: byte3 = val; break;
                    case 3: byte4 = val; break;
                    case 4: byte5 = val; break;
                    case 5: byte6 = val; break;
                    case 6: byte7 = val; break;
                    case 7: byte8 = val; break;
                }
            }

            public MultiType8ByteArray Clone()
            {
                MultiType8ByteArray newCopy = new MultiType8ByteArray();
                newCopy.whole8 = whole8;
                return newCopy;
            }
        }

        uint CalculateIQFinalIQ(
            int currentStage, // 0 based, sp 0 -8 for main game, tektonics is 9
            byte difficulty // 0 = normal, 3 = ultra hard
        )
        {
            ushort previousStageScore = 0;
            ushort[] arrayAt80103454 = new ushort[2] { 12, 10 };
            ushort[] baseDifficultyScore = new ushort[4] { 100, 110, 115, 120 };
            // this and the next array in memory are the same
            // and the value that picks this array never seems to change
            // so I think this is safe to hardcode
            byte[] arrayAt800f9450 = new byte[] {0, 1, 0, 0, 1, 0, 0, 0};
            // aka t2, base of this struct is 800f9ccc
            GamePerformance t2 = new GamePerformance(), t5 = t2;
            MultiType8ByteArray a2ArrayValue = new MultiType8ByteArray(); // sp + 0x10 + 0x14

            // aka a1Array & topStack
            MultiType8ByteArray runningMultipliedScore = new MultiType8ByteArray(); // sp + 0 + 0x4

            MultiType8ByteArray a1ArrayCopy = new MultiType8ByteArray(); // sp + 0x8 + 0xc
            MultiType8ByteArray fourShortArrayVal = new MultiType8ByteArray(); // sp + 0x18 + 0x1c
            MultiType8ByteArray values1814 = new MultiType8ByteArray(); // sp + 0x20 + 0x24
            // i = $t7, only loop counter, not used for indexing
            // $t6 = only used for indexing g_iqFinalScoreMultipliers
            // $t5 is the same as $t2 or the entire loop
            for (int stageIter = 0; stageIter < currentStage; ++stageIter)
            {
                ushort puzzlesFaced = t2.numberOfPuzzlesFaced[stageIter];
                if (puzzlesFaced != 0)
                {
                    // current stage score is Minus of stage bonus
                    ushort endStageScoreInHundreds = t2.stageScoresInHundreds[stageIter];
                    ushort scoredThisStage = (ushort)(endStageScoreInHundreds - previousStageScore);
                    ushort curStageMultiplier = g_iqFinalScoreMultipliers[stageIter];
                    uint curStageMultipliedScore = (uint)(scoredThisStage * curStageMultiplier);
                    // 0x80076B30
                    t5.adjustedStageScores[stageIter] = curStageMultipliedScore;
                    // 80076B3C
                    a2ArrayValue.whole8 = curStageMultipliedScore;
                    // a0 carries the previous stage score between loops, but we have separate var for that
                    uint a0 = 0;
                    // j = $a3 register
                    //for (int j = 0; j < 4; ++j)
                    //{
                    //    ushort scoreByteShort = a2ArrayValue.shortAt(j);
                    //    ushort a1Value = a1Array.shortAt(j);
                    //    a0 += a1Value;
                    //    a0 += scoreByteShort;
                    //    a1Array.shortAt(j, (ushort)(a0 & 0xffff));
                    //    a0 = (uint)((int)a0 >> 16);
                    //}
                    // this is the equivalent of the commented out for loop above
                    runningMultipliedScore.whole8 += a2ArrayValue.whole8;
                    // 80076B68
                    // this is the loading of v0 and v1 and then saving them a few lines later
                    a1ArrayCopy = runningMultipliedScore.Clone();
                    // these two are the sequence from 80076B88 to 80076B9C
                    ushort difficultyBonusPercentage = baseDifficultyScore[difficulty]; // baseDifficultyScore array = 8010344C
                    values1814.whole8 = 0;
                    // 80076BB0
                    fourShortArrayVal.whole8 = difficultyBonusPercentage;
                    // first nested loop
                    // loop from 80076BB4 to 80076C18
                    // i = $a3
                    /*for (int i = 0; i < 4; ++i)
                    {
                        // 80076BB4
                        a0 = 0;
                        int fourValIter = 0;
                        // 80076BC0
                        // j = $a1
                        // loop from 80076C74 to 80076CBC
                        for (int j = 0; j < 4; ++j)
                        {
                            ushort v1 = a1ArrayCopy.shortAt(i);
                            ushort v0 = fourShortArrayVal.shortAt(fourValIter);
                            uint valMul = (uint)(v1 * v0);
                            a0 += valMul;
                            // 80076C90 
                            int totalLoops = i + j;
                            if (totalLoops >= 4)
                            {
                                break;
                            }
                            ++fourValIter;
                            v1 = values1814.shortAt(totalLoops);
                            a0 += v1;
                            values1814.shortAt(totalLoops, (ushort)a0);
                            a0 >>= 16;
                        }
                    }*/
                    // this is what the above commented nested for loop does
                    // this is essentially
                    // values1814 = runningMultipliedScore * difficultyBonusPercent;
                    values1814.whole8 = a1ArrayCopy.whole8 * fourShortArrayVal.whole8;
                    // 80076C1C
                    a1ArrayCopy = values1814.Clone();
                    // #something' here is usually / always 1, which always retrns 10
                    byte something = arrayAt800f9450[1];
                    a2ArrayValue.whole8 = arrayAt80103454[something];
                    fourShortArrayVal.whole8 = 0;

                    // second nested loop
                    // loop from 80076C68 to 80076CD0
                    // i = $a3
                    /*for (int i = 0; i < 4; ++i)
                    {
                        // 80076BB4
                        a0 = 0;
                        int a2ArrayIter = 0;
                        // 80076BC0
                        // j = $a1
                        // loop from 80076C74 to 80076CBC
                        for (int j = 0; j < 4; ++j)
                        {
                            ushort v1 = a1ArrayCopy.shortAt(i);
                            ushort v0 = a2ArrayValue.shortAt(a2ArrayIter);
                            uint valMul = (uint)(v1 * v0);
                            a0 += valMul;
                            // 80076C90 
                            int totalLoops = i + j;
                            if (totalLoops >= 4)
                            {
                                break;
                            }
                            ++a2ArrayIter;
                            v1 = fourShortArrayVal.shortAt(totalLoops);
                            a0 += v1;
                            fourShortArrayVal.shortAt(totalLoops, (ushort)a0);
                            a0 >>= 16;
                        }
                    }*/
                    // this is the equivalent to the commented out nested loops above
                    fourShortArrayVal.whole8 = a1ArrayCopy.whole8 * a2ArrayValue.whole8;
                    // 80076CD0
                    a1ArrayCopy = fourShortArrayVal.Clone();
                    // the three loads at 80076CE0
                    ushort perfects = t2.stagePerfects[stageIter];
                    ushort excellents = t2.stageExcellents[stageIter];
                    ushort numPuzzlesFaced = t2.numberOfPuzzlesFaced[stageIter];
                    a0 = (uint)(perfects + excellents);
                    // this temp stuff is a0 * 55
                    uint temp = a0 << 3;
                    temp -= a0;
                    temp <<= 3;
                    temp -= a0;
                    // 80076D28
                    temp = temp / numPuzzlesFaced;
                    fourShortArrayVal.whole8 = 0;
                    a2ArrayValue.whole8 = temp + 100;
                    // third nested loop
                    // loop from 80076D50 to 80076DB4
                    // i = $a3
                    /*for (int i = 0; i < 4; ++i)
                    {
                        a0 = 0;
                        int a2ArrayIter = 0;
                        // j = $a1
                        // loop from 80076C74 to 80076CBC
                        for (int j = 0; j < 4; ++j)
                        {
                            ushort v1 = a1ArrayCopy.shortAt(i);
                            ushort v0 = a2ArrayValue.shortAt(a2ArrayIter);
                            uint valMul = (uint)(v1 * v0);
                            a0 += valMul;
                            // 80076C90 
                            int totalLoops = i + j;
                            if (totalLoops >= 4)
                            {
                                break;
                            }
                            ++a2ArrayIter;
                            v1 = fourShortArrayVal.shortAt(totalLoops);
                            a0 += v1;
                            fourShortArrayVal.shortAt(totalLoops, (ushort)a0);
                            a0 >>= 16;
                        }
                    }*/
                    // this is the equivalent to the commented out nested loops above
                    fourShortArrayVal.whole8 = a1ArrayCopy.whole8 * a2ArrayValue.whole8;
                    // 80076DB8
                    a1ArrayCopy = fourShortArrayVal.Clone();
                    a2ArrayValue.whole8 = 0;
                    a0 = 0;
                    // i = $a3 - 80076DBC
                    // loop from 80076DE0 to 80076E30
                    for (int i = 3; i >= 0; --i)
                    {
                        a0 += a1ArrayCopy.shortAt(i);
                        uint divRes = a0 / 10000; // 80076DEC & 80076E14
                        uint mod = a0 % 10000; // 80076E18
                        a0 = mod << 16;
                        a2ArrayValue.shortAt(i, (ushort)divRes);
                    }
                    // 80076E34
                    a0 = 0;
                    // not a typo
                    a1ArrayCopy = a2ArrayValue.Clone();
                    a2ArrayValue.whole8 = 0;
                    // loop from 80076E5C to 80076EAC
                    for (int i = 3; i >= 0; --i)
                    {
                        a0 += a1ArrayCopy.shortAt(i);
                        uint divRes = a0 / 10000; // 80076E68 & 80076E90
                        uint mod = a0 % 10000; // 80076E94
                        a0 = mod << 16;
                        a2ArrayValue.shortAt(i, (ushort)divRes);
                    }
                    a0 = 0;
                    // not a typo
                    a1ArrayCopy = a2ArrayValue.Clone();
                    a2ArrayValue.whole8 = 0;
                    // loop from 80076EDC to 80076F2C
                    for (int i = 3; i >= 0; --i)
                    {
                        a0 += a1ArrayCopy.shortAt(i);
                        uint divRes = a0 / 10; // 80076EE8 & 80076F10
                        uint mod = a0 % 10; // 80076F14
                        a0 = mod << 16;
                        a2ArrayValue.shortAt(i, (ushort)divRes);
                    }
                    a1ArrayCopy = a2ArrayValue.Clone();
                    // for next loop, this is the a0 assignation at 80076F40
                    previousStageScore = endStageScoreInHundreds;
                }
                else // numPuzzles == 0
                {
                    a1ArrayCopy.whole8 = 0;
                }
                t2.stageIQScores[stageIter] = a1ArrayCopy.shortAt(0);
            }
            ushort totalIQ = a1ArrayCopy.shortAt(0);
            t2.totalIQ = totalIQ;
            // tektonics
            if (currentStage == 9)
            {
                byte indexer = difficulty; // difficulty
                // indexer * 13
                uint actualIndex = (uint)(indexer << 1); // * 2
                actualIndex += indexer; // * 3
                actualIndex <<= 2; // * 12
                actualIndex += indexer; // * 13
                // uint iqBonus = 
            }
            return t2.totalIQ;
        }

        // In the game, this calculation is run after the end of the 9 normal stages
        // and if continued into Tektonics, is run again after Tektonics 
        // the Tektonics calculation only calculates for tektonics
        // data for all other stages is 0
        uint CalculateIQFinalIQSimplified(
            int currentStage, // 0 based, so 0-8 for main game, tektonics is 9
            byte difficulty, // 0 = normal, 3 = ultra hard
            GamePerformance gamePerformance // aka t2 and t5
        )
        {
            ushort previousStageScore = 0;
            ushort[] arrayAt80103454 = new ushort[2] { 12, 10 };
            ushort[] baseDifficultyScore = new ushort[4] { 100, 110, 115, 120 };
            // this and the next array in memory are the same
            // and the value that picks this array never seems to change
            // so I think this is safe to hardcode
            byte[] arrayAt800f9450 = new byte[] {0, 1, 0, 0, 1, 0, 0, 0};
            // aka a1ArrayCopy
            ulong temp1 = 0; // sp + 0x8 + 0xc

            // aka a2ArrayValue
            ulong temp2; // sp + 0x10 + 0x14

            // aka fourShortArrayVal
            ulong temp4; // sp + 0x18 + 0x1c
            ulong highestIq = 0;

            // aka a1Array & topStack
            ulong runningMultipliedScore = 0; // sp + 0 + 0x4
            // i = $t7, only loop counter, not used for indexing
            // $t6 = only used for indexing g_iqFinalScoreMultipliers
            // $t5 is the same as $t2 or the entire loop
            for (int stageIter = 0; stageIter < currentStage; ++stageIter)
            {
                ushort puzzlesFaced = gamePerformance.numberOfPuzzlesFaced[stageIter];
                if (puzzlesFaced != 0)
                {
                    // tektonics is calculated standalone
                    if (stageIter == 9)
                    {
                        previousStageScore = 0;
                        runningMultipliedScore = 0;
                    }
                    // current stage score is Minus of stage bonus
                    ushort endStageScoreInHundreds = gamePerformance.stageScoresInHundreds[stageIter];
                    ushort scoredThisStage = (ushort)(endStageScoreInHundreds - previousStageScore);
                    ushort curStageMultiplier = g_iqFinalScoreMultipliers[stageIter];
                    uint curStageMultipliedScore = (uint)(scoredThisStage * curStageMultiplier);
                    // 0x80076B30
                    gamePerformance.adjustedStageScores[stageIter] = curStageMultipliedScore;
                    runningMultipliedScore += curStageMultipliedScore;
                    // 80076B3C
                    temp2 = curStageMultipliedScore;
                    
                    // these two are the sequence from 80076B88 to 80076B9C
                    ushort difficultyBonusPercentage = baseDifficultyScore[difficulty]; // baseDifficultyScore array = 8010344C
                    ulong difficultyMultipliedScore = runningMultipliedScore * difficultyBonusPercentage;
                    // 'indexer' here is usually / always 1, which always returns 10
                    // may be different for TekTonics, needs checking
                    byte indexer = arrayAt800f9450[1];
                    temp2 = arrayAt80103454[indexer];

                    // difficultyMultipliedScore * found val
                    temp4 = difficultyMultipliedScore * temp2;
                    // 80076CD0
                    temp1 = temp4;
                    // the three loads at 80076CE0
                    ushort perfects = gamePerformance.stagePerfects[stageIter];
                    ushort excellents = gamePerformance.stageExcellents[stageIter];
                    ushort numPuzzlesFaced = gamePerformance.numberOfPuzzlesFaced[stageIter];
                    uint numBetterThanGreat = (uint)(perfects + excellents);
                    uint betterBonus = (numBetterThanGreat * 55) / numPuzzlesFaced;

                    temp2 = betterBonus + 100;
                    temp4 = temp1 * temp2;
                    ulong stageIq = temp4 / (10000ul * 10000ul * 10ul);
                    temp1 = stageIq;
                    // for next loop, this is the a0 assignation at 80076F40
                    previousStageScore = endStageScoreInHundreds;
                    if (stageIter < 9)
                    {
                        highestIq = stageIq;
                    }
                }
                else // numPuzzles == 0
                {
                    temp1 = 0;
                }
                gamePerformance.stageIQScores[stageIter] = (ushort)temp1;
            }
            ushort totalIQ = (ushort)(temp1 + highestIq);
            return totalIQ;
        }
    }
}

/*
This is the CalculateIQ function from the PAL version. The US version is identical.
The original JP version uses the same formulas but uses differernt instructions 
- (likely due to using an older version of the compiler)
 

TEXT:80033968 CalculateIQ:                             # CODE XREF: sub_8001F520+118p
TEXT:80033968                                          # sub_80031608+84p ...
TEXT:80033968                 lbu     $a0, g_currentGameSpeedLevel  # 8006D575
TEXT:80033970                 nop
TEXT:80033974                 sltiu   $v0, $a0, 5
TEXT:80033978                 beqz    $v0, loc_80033A24  # jumptable 80033990 default case
TEXT:8003397C                 sll     $v0, $a0, 2
TEXT:80033980                 lw      $v0, levelJumpTable($v0)
TEXT:8003398C                 nop
TEXT:80033990                 jr      $v0              # switch 5 cases
TEXT:80033994                 nop
TEXT:80033998  # ---------------------------------------------------------------------------
TEXT:80033998
TEXT:80033998 level0:                                  # CODE XREF: CalculateIQ+28j
TEXT:80033998                                          # DATA XREF: TEXT:levelJumpTableo
TEXT:80033998                 lw      $v1, g_levelScore  # jumptable 80033990 case 0
TEXT:800339A0                 j       loc_80033A24     # jumptable 80033990 default case
TEXT:800339A4                 nop
TEXT:800339A8  # ---------------------------------------------------------------------------
TEXT:800339A8
TEXT:800339A8 level1:                                  # CODE XREF: CalculateIQ+28j
TEXT:800339A8                                          # DATA XREF: TEXT:levelJumpTableo
TEXT:800339A8                 lw      $v1, g_levelScore  # jumptable 80033990 case 1
TEXT:800339B0                 j       loc_80033A20
TEXT:800339B4                 srl     $v0, $v1, 2      # 1.25 * the score
TEXT:800339B8  # ---------------------------------------------------------------------------
TEXT:800339B8
TEXT:800339B8 level2:                                  # CODE XREF: CalculateIQ+28j
TEXT:800339B8                                          # DATA XREF: TEXT:levelJumpTableo
TEXT:800339B8                 lw      $a0, g_levelScore  # jumptable 80033990 case 2
TEXT:800339C0                 li      $v1, 0xCCCCCCCD  # mul by 0xcccccccd & shr 3 is short for divide by 10
TEXT:800339C8                 sll     $v0, $a0, 1
TEXT:800339CC                 addu    $v0, $a0
TEXT:800339D0                 multu   $v0, $v1         # this multiplies the score by 3 and divides it by 10
TEXT:800339D4                 mfhi    $a1
TEXT:800339D8                 srl     $v0, $a1, 3
TEXT:800339DC                 j       loc_80033A24     # jumptable 80033990 default case
TEXT:800339E0                 addu    $v1, $a0, $v0
TEXT:800339E4  # ---------------------------------------------------------------------------
TEXT:800339E4
TEXT:800339E4 level3:                                  # CODE XREF: CalculateIQ+28j
TEXT:800339E4                                          # DATA XREF: TEXT:levelJumpTableo
TEXT:800339E4                 lw      $a0, g_levelScore  # jumptable 80033990 case 3
TEXT:800339EC                 li      $v1, 0xCCCCCCCD
TEXT:800339F4                 sll     $v0, $a0, 3
TEXT:800339F8                 addu    $v0, $a0
TEXT:800339FC                 multu   $v0, $v1
TEXT:80033A00                 mfhi    $a1
TEXT:80033A04                 srl     $v0, $a1, 4
TEXT:80033A08                 j       loc_80033A24     # jumptable 80033990 default case
TEXT:80033A0C                 addu    $v1, $a0, $v0
TEXT:80033A10  # ---------------------------------------------------------------------------
TEXT:80033A10
TEXT:80033A10 level4:                                  # CODE XREF: CalculateIQ+28j
TEXT:80033A10                                          # DATA XREF: TEXT:levelJumpTableo
TEXT:80033A10                 lw      $v1, g_levelScore  # jumptable 80033990 case 4
TEXT:80033A18                 nop
TEXT:80033A1C                 srl     $v0, $v1, 1      # Find half, below adds it on to the original
TEXT:80033A1C                                          # so 1.5x the score
TEXT:80033A20
TEXT:80033A20 loc_80033A20:                            # CODE XREF: CalculateIQ+48j
TEXT:80033A20                 addu    $v1, $v0
TEXT:80033A24
TEXT:80033A24 loc_80033A24:                            # CODE XREF: CalculateIQ+10j
TEXT:80033A24                                          # CalculateIQ+38j ...
TEXT:80033A24                 lbu     $a0, g_completedStages  # jumptable 80033990 default case
TEXT:80033A2C                 nop
TEXT:80033A30                 sltiu   $v0, $a0, 9
TEXT:80033A34                 beqz    $v0, loc_80033BC8  # jumptable 80033A4C default case
TEXT:80033A38                 sll     $v0, $a0, 2
TEXT:80033A3C                 lw      $v0, stageJumpTable($v0)  # v1 = modified score
TEXT:80033A48                 nop
TEXT:80033A4C                 jr      $v0              # switch 9 cases
TEXT:80033A50                 nop
TEXT:80033A54  # ---------------------------------------------------------------------------
TEXT:80033A54
TEXT:80033A54 stage1:                                  # CODE XREF: CalculateIQ+E4j
TEXT:80033A54                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033A54                 li      $v0, 0x51EB851F  # jumptable 80033A4C case 0
TEXT:80033A5C                 multu   $v1, $v0
TEXT:80033A60                 mfhi    $a1
TEXT:80033A64                 srl     $v1, $a1, 5      # mul & srl = divide score by 100
TEXT:80033A68                 sll     $v0, $v1, 2      # multiply divided score by 4
TEXT:80033A6C                 addu    $v0, $v1         # add it back to the divided sore
TEXT:80033A70                 sll     $v1, $v0, 4      # multiply that by 16
TEXT:80033A74                 subu    $v1, $v0         # subtract one lot (multiply by 15)
TEXT:80033A78                 j       loc_80033B84
TEXT:80033A7C                 sll     $v1, 3           # multiply the result by 8
TEXT:80033A80  # ---------------------------------------------------------------------------
TEXT:80033A80
TEXT:80033A80 stage2:                                  # CODE XREF: CalculateIQ+E4j
TEXT:80033A80                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033A80                 li      $v0, 0x51EB851F  # jumptable 80033A4C case 1
TEXT:80033A88                 multu   $v1, $v0
TEXT:80033A8C                 mfhi    $a1
TEXT:80033A90                 srl     $v1, $a1, 5
TEXT:80033A94                 sll     $v0, $v1, 4
TEXT:80033A98                 addu    $v0, $v1
TEXT:80033A9C                 sll     $v0, 2
TEXT:80033AA0                 addu    $v0, $v1
TEXT:80033AA4                 sll     $v0, 2
TEXT:80033AA8                 subu    $v0, $v1
TEXT:80033AAC                 li      $v1, 0xD1B71759
TEXT:80033AB4                 j       loc_80033B8C
TEXT:80033AB8                 sll     $v0, 1
TEXT:80033ABC  # ---------------------------------------------------------------------------
TEXT:80033ABC
TEXT:80033ABC stage3:                                  # CODE XREF: CalculateIQ+E4j
TEXT:80033ABC                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033ABC                 li      $v0, 0x10624DD3  # jumptable 80033A4C case 2
TEXT:80033AC4                 multu   $v1, $v0         # from 1244800 result of this block = 622
TEXT:80033AC8                 mfhi    $a1
TEXT:80033ACC                 j       loc_80033BC8     # jumptable 80033A4C default case
TEXT:80033AD0                 srl     $v1, $a1, 7
TEXT:80033AD4  # ---------------------------------------------------------------------------
TEXT:80033AD4
TEXT:80033AD4 stage4:                                  # CODE XREF: CalculateIQ+E4j
TEXT:80033AD4                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033AD4                 li      $v0, 0x51EB851F  # jumptable 80033A4C case 3
TEXT:80033ADC                 multu   $v1, $v0
TEXT:80033AE0                 mfhi    $a1
TEXT:80033AE4                 srl     $v1, $a1, 5
TEXT:80033AE8                 sll     $v0, $v1, 3
TEXT:80033AEC                 subu    $v0, $v1
TEXT:80033AF0                 sll     $v0, 5
TEXT:80033AF4                 addu    $v0, $v1
TEXT:80033AF8                 li      $v1, 0xD1B71759
TEXT:80033B00                 j       loc_80033B8C
TEXT:80033B04                 sll     $v0, 1
TEXT:80033B08  # ---------------------------------------------------------------------------
TEXT:80033B08
TEXT:80033B08 stage5:                                  # CODE XREF: CalculateIQ+E4j
TEXT:80033B08                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033B08                 li      $v0, 0xD1B71759  # jumptable 80033A4C case 4
TEXT:80033B10                 multu   $v1, $v0         # from 1244800 result of this block = 497
TEXT:80033B14                 mfhi    $a1
TEXT:80033B18                 j       loc_80033BC8     # jumptable 80033A4C default case
TEXT:80033B1C                 srl     $v1, $a1, 0xB
TEXT:80033B20  # ---------------------------------------------------------------------------
TEXT:80033B20
TEXT:80033B20 stage6:                                  # CODE XREF: CalculateIQ+E4j
TEXT:80033B20                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033B20                 li      $v0, 0x51EB851F  # jumptable 80033A4C case 5
TEXT:80033B28                 multu   $v1, $v0
TEXT:80033B2C                 mfhi    $a1
TEXT:80033B30                 srl     $v1, $a1, 5
TEXT:80033B34                 sll     $v0, $v1, 1
TEXT:80033B38                 addu    $v0, $v1
TEXT:80033B3C                 sll     $v0, 2
TEXT:80033B40                 subu    $v0, $v1
TEXT:80033B44                 sll     $v0, 4
TEXT:80033B48                 subu    $v0, $v1
TEXT:80033B4C                 li      $v1, 0xD1B71759
TEXT:80033B54                 j       loc_80033B8C
TEXT:80033B58                 sll     $v0, 1
TEXT:80033B5C  # ---------------------------------------------------------------------------
TEXT:80033B5C
TEXT:80033B5C stage7:                                  # CODE XREF: CalculateIQ+E4j
TEXT:80033B5C                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033B5C                 li      $v0, 0x51EB851F  # jumptable 80033A4C case 6
TEXT:80033B64                 multu   $v1, $v0
TEXT:80033B68                 mfhi    $a1
TEXT:80033B6C                 srl     $v1, $a1, 5
TEXT:80033B70                 sll     $v0, $v1, 2
TEXT:80033B74                 addu    $v0, $v1
TEXT:80033B78                 sll     $v1, $v0, 4
TEXT:80033B7C                 subu    $v1, $v0
TEXT:80033B80                 sll     $v1, 2
TEXT:80033B84
TEXT:80033B84 loc_80033B84:                            # CODE XREF: CalculateIQ+110j
TEXT:80033B84                 li      $v0, 0xD1B71759  # this and the following divide the modified scores by 10000
TEXT:80033B8C
TEXT:80033B8C loc_80033B8C:                            # CODE XREF: CalculateIQ+14Cj
TEXT:80033B8C                                          # CalculateIQ+198j ...
TEXT:80033B8C                 multu   $v1, $v0
TEXT:80033B90                 mfhi    $a1
TEXT:80033B94                 j       loc_80033BC8     # jumptable 80033A4C default case
TEXT:80033B98                 srl     $v1, $a1, 0xD
TEXT:80033B9C  # ---------------------------------------------------------------------------
TEXT:80033B9C
TEXT:80033B9C stage8:                                  # CODE XREF: CalculateIQ+E4j
TEXT:80033B9C                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033B9C                 li      $v0, 0x10624DD3  # jumptable 80033A4C case 7
TEXT:80033BA4                 multu   $v1, $v0
TEXT:80033BA8                 mfhi    $a1
TEXT:80033BAC                 j       loc_80033BC8     # jumptable 80033A4C default case
TEXT:80033BB0                 srl     $v1, $a1, 8      # produces 311 for 1244800
TEXT:80033BB4  # ---------------------------------------------------------------------------
TEXT:80033BB4
TEXT:80033BB4 finalStage:                              # CODE XREF: CalculateIQ+E4j
TEXT:80033BB4                                          # DATA XREF: TEXT:stageJumpTableo
TEXT:80033BB4                 li      $v0, 0xD1B71759  # jumptable 80033A4C case 8
TEXT:80033BBC                 multu   $v1, $v0
TEXT:80033BC0                 mfhi    $a1
TEXT:80033BC4                 srl     $v1, $a1, 12     # modified score divided by 5000
TEXT:80033BC8
TEXT:80033BC8 loc_80033BC8:                            # CODE XREF: CalculateIQ+CCj
TEXT:80033BC8                                          # CalculateIQ+164j ...
TEXT:80033BC8                 lw      $v0, g_currentIQ  # jumptable 80033A4C default case
TEXT:80033BD0                 sw      $zero, g_levelScore
TEXT:80033BD8                 addu    $v0, $v1
TEXT:80033BDC                 sw      $v0, g_currentIQ
TEXT:80033BE4                 slti    $v0, 0x3E8
TEXT:80033BE8                 bnez    $v0, locret_80033BF8
TEXT:80033BEC                 li      $v0, 0x3E7
TEXT:80033BF0                 sw      $v0, g_currentIQ
TEXT:80033BF8
TEXT:80033BF8 locret_80033BF8:                         # CODE XREF: CalculateIQ+280j
TEXT:80033BF8                 jr      $ra
TEXT:80033BFC                 nop
TEXT:80033BFC  # End of function CalculateIQ

*/