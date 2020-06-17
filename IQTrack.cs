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

        //ushort[] g_iqFinalScoreMultipliers = new ushort[]{
        //    500,
        //    480,
        //    250,
        //    235,
        //    220,
        //    205,
        //    185,
        //    165,
        //    155,
        //    155
        //};

        //uint CalculateIQFinalIQ()
        //{
        //    int[] numberOfPuzzlesFaced = new int[]{};
        //    // these scores are pre stage bonus
        //    uint[] stageScores = new uint[9]{};
        //    int lastStage = 9;
        //    int previousStageScore = 0;
        //    int[] multipliedStageScores = new int[9]{};
        //    ulong accumulatedMultipliedScore = 0;
        //    byte[] arrayAt800f925c = new byte[9];
        //    ushort[] arrayAt8010344C = new ushort[4] { 100, 110, 115, 120};
        //    uint var1c = 0, var18 = 0, var14 = 0, fourValArrayVal = 0;
        //    for (int i = 0; i < lastStage; ++i)
        //    {
        //        // current stage score is Minus of stage bonus
        //        ulong multiStageScore = stageScores[i] * g_iqFinalScoreMultipliers[i];
        //        multipliedStageScores[i] = (int)multiStageScore;
        //        // this is the array indexed by a2
        //        byte[] multiStageScoreBytes = BitConverter.GetBytes(multiStageScore);
        //        byte[] accumulatedMultiStageScoreBytes = BitConverter.GetBytes(accumulatedMultipliedScore);
        //        // 80076B3C
        //        uint a0 = 0;
        //        for (int j = 0; j < 4; ++j)
        //        {
        //            // accumulatedMultipliedScore here is the array indexed by a1
        //            a0 += BitConverter.ToUInt16(accumulatedMultiStageScoreBytes, j * 2);
        //            // multiStageScore
        //            a0 += BitConverter.ToUInt16(multiStageScoreBytes, j * 2);
        //            byte[] a0Bytes = BitConverter.GetBytes(a0);
        //            Array.Copy(a0Bytes, 0, accumulatedMultiStageScoreBytes, j * 2, 2);
        //            a0 >>= 16;
        //        }
        //        // update the real score (this isn't done in the disassembly)
        //        accumulatedMultipliedScore = BitConverter.ToUInt64(accumulatedMultiStageScoreBytes, 0);
        //        // loaded at 80076B70, stored at 80076B80
        //        uint currentIQ = (uint)accumulatedMultipliedScore;
        //        uint var2c = (uint)(accumulatedMultipliedScore >> 32);
        //        // these two are the sequence from 80076B88 to 80076B9C
        //        uint arrayIndexer = arrayAt800f925c[1];
        //        var1c = 0;
        //        var18 = 0;
        //        var14 = 0;
        //        // 80076BB0
        //        fourValArrayVal = arrayAt8010344C[arrayIndexer];

        //        previousStageScore = currentStageScore;
        //    }
        //}
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