using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PuzzlePreview
{
    public partial class MainForm : Form
    {
        // 70 bytes between puzzles
        // 200 puzzles per size
        // 14,000 bytes between puzzle sizes
        private Type[] gameInfoTypes = new Type[] { typeof(IQ), typeof(IQFinal) };
        private List<byte[]> theseSizePuzzles;
        private byte[] currentPuzzleData;
        private int[] currentTrns;
        private Bitmap puzzleImage;
        private int puzWidth;
        private int puzHeight;
        private int curPuzzleNumber;
        private bool isFlippedPuzzle;
        private Font textFont;
        private IGameInformation loadedGameInfo;

        private const int cubeWidth = 40;
        private const int cubeHeight = 40;

        public MainForm()
        {
            InitializeComponent();
            foreach (Type t in gameInfoTypes)
            {
                gameNameBox.Items.Add(t.Name);
            }
            puzWidth = puzHeight = 0;
            curPuzzleNumber = 200;
            loadedGameInfo = null;
            puzzleImage = null;
            currentPuzzleData = null;
            currentTrns = null;
            theseSizePuzzles = null;
            textFont = new Font(FontFamily.GenericSansSerif, 16.0f);
        }

        private void nextPuzzleBut_Click(object sender, EventArgs e)
        {
            curPuzzleNumber = loadedGameInfo.GetNextPuzzle(theseSizePuzzles.Count, out isFlippedPuzzle);
            flippedCheck.Checked = isFlippedPuzzle;
            puzzleNumBox.Text = curPuzzleNumber.ToString();
            initialSeedTextBox.Text = loadedGameInfo.Seed.ToString();
            RedrawPuzzleAndUpdateTRN();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            stepCountUpDown.Value = 1;
            RedrawPuzzleAndUpdateTRN();
        }

        private void initialSeedTextBox_TextChanged(object sender, EventArgs e)
        {
            string seedText = initialSeedTextBox.Text;
            int seed = 0;
            Int32.TryParse(seedText, out seed);
            loadedGameInfo.Seed = seed;
            nextPuzzleBut.Enabled = (!String.IsNullOrEmpty(seedText) && (seed != 0));
        }

        private void puzzleNumBox_ValueChanged(object sender, EventArgs e)
        {
            int puzzle = (int)puzzleNumBox.Value;
            int maxPuzNum = theseSizePuzzles.Count;
            if((puzzle < 1) || (puzzle > maxPuzNum))
            {
                MessageBox.Show("Invalid puzzle number, must be between 1 and " + maxPuzNum.ToString(), "Puzzle Preview");
                return;
            }
            curPuzzleNumber = puzzle - 1;
            flippedCheck.Checked = isFlippedPuzzle = false;
            stepCountUpDown.Value = 1;
            RedrawPuzzleAndUpdateTRN();
        }

        private void RedrawPuzzleAndUpdateTRN()
        {
            if(IsEverythingSet())
            {
                int trn = currentTrns[curPuzzleNumber];
                trnTextBox.Text = trn.ToString();
                stepCountUpDown.Maximum = trn;
                currentPuzzleData = CopyPuzzle(isFlippedPuzzle);
                puzzleImageBox.Image = null;
                if (puzzleImage != null)
                {
                    puzzleImage.Dispose();
                }
                puzzleImage = DrawPuzzleDiagram();
                puzzleImageBox.Image = puzzleImage;
            }
        }

        private void puzzleSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (puzzleSizes.SelectedItem != null)
            {
                PuzzleSizeDetails puzzleGroup = loadedGameInfo.PuzzleInfo[puzzleSizes.SelectedIndex];
                puzWidth = puzzleGroup.width;
                puzHeight = puzzleGroup.height;
                currentTrns = puzzleGroup.trns;
                theseSizePuzzles = puzzleGroup.puzzles;
                stepCountUpDown.Value = 1;
                RedrawPuzzleAndUpdateTRN();
            }
        }

        private bool IsEverythingSet()
        {
            return (puzWidth > 0) && (puzHeight > 0) && (curPuzzleNumber < 200) && (loadedGameInfo != null);
        }

        private byte[] CopyPuzzle(bool flipped)
        {
            byte[] puzzle = new byte[puzWidth * puzHeight];
            Array.Copy(theseSizePuzzles[curPuzzleNumber], puzzle, puzzle.Length);
            if (flipped)
            {
                for (int y = 0; y < puzHeight; ++y)
                {
                    Array.Reverse(puzzle, y * puzWidth, puzWidth);
                }
            }
            return puzzle;
        }

        private void puzzleImageBox_Click(object sender, MouseEventArgs e)
        {
            if (puzzleImage != null)
            {
                DrawCapture(e.Location, e.Button);
                puzzleImageBox.Refresh();
            }
        }

        private void flippedCheck_Click(object sender, EventArgs e)
        {
            isFlippedPuzzle = flippedCheck.Checked;
            stepCountUpDown.Value = 1;
            RedrawPuzzleAndUpdateTRN();
        }

        private Bitmap DrawPuzzleDiagram()
        {
            Brush[] qubeBrushes = {Brushes.BurlyWood, Brushes.Green, Brushes.Black};
            Bitmap bm = new Bitmap(puzWidth * cubeWidth, puzHeight * cubeHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            Pen blackPen = Pens.Black;
            using(Graphics g = Graphics.FromImage(bm))
            {
                g.Clear(Color.Gray);
                int yLoc = 0;
                for (int y = 0; y < puzHeight; ++y)
                {
                    int xLoc = 0;
                    for (int x = 0; x < puzWidth; ++x)
                    {
                        byte qube = currentPuzzleData[(y * puzWidth) + x];
                        Brush b = qubeBrushes[qube];
                        Rectangle r = new Rectangle(xLoc, yLoc, cubeWidth, cubeHeight);
                        g.FillRectangle(b, r);
                        g.DrawRectangle(blackPen, r);
                        xLoc += cubeWidth;
                    }
                    yLoc += cubeHeight;
                }
            }
            return bm;
        }

        private void DrawCapture(Point mouseClick, MouseButtons mb)
        {
            DrawSingleCapture(mouseClick);
            if (bombCheck.Checked || ((mb & MouseButtons.Right) != 0))
            {
                int ptX = mouseClick.X, ptY = mouseClick.Y;
                for (int i = -1; i < 2; ++i)
                {
                    int newX = (ptX + (i * cubeWidth));
                    for (int j = -1; j < 2; ++j)
                    {
                        // already drawn the middle one
                        if ((i == 0) && (j == 0))
                        {
                            continue;
                        }
                        int newY = (ptY + (j * cubeHeight));
                        DrawSingleCapture(new Point(newX, newY));
                    }
                }
            }
        }

        private void DrawSingleCapture(Point mouseClick)
        {
            int x = mouseClick.X;
            int y = mouseClick.Y;
            // ignore those that are outside of the bounds of the puzzle
            if((x < 0) || (y < 0) || (x > (puzWidth * cubeWidth)) || (y > (puzHeight * cubeHeight)))
            {
                return;
            }
            string moveNumber = stepCountUpDown.Value.ToString();
            using (Graphics g = Graphics.FromImage(puzzleImage))
            {
                int boxXLoc = (x / cubeWidth) * cubeWidth;
                int boxYLoc = (y / cubeHeight) * cubeHeight;
                Rectangle r = new Rectangle(boxXLoc, boxYLoc, cubeWidth, cubeHeight);
                g.FillRectangle(Brushes.Gray, r);
                int xCellPos = (moveNumber.Length > 1) ? 5 : 10;
                g.DrawString(moveNumber, textFont, Brushes.Black, new PointF(boxXLoc + xCellPos, boxYLoc + 10));
            }
        }

        private void puzzleImageBox_Resize(object sender, EventArgs e)
        {
            Size clientSize = this.ClientSize;
            int boxBottom = puzzleImageBox.Bottom;
            int curWinHeight = clientSize.Height;
            if (boxBottom > curWinHeight)
            {
                this.Height += (boxBottom - curWinHeight);
            }
            int boxWidth = puzzleImageBox.Right;
            int curWinWidth = clientSize.Width;
            if (boxWidth > curWinWidth)
            {
                this.Width += (boxWidth - curWinWidth);
            }
        }

        private void gameNameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int selectedIndex = cb.SelectedIndex;
            if (selectedIndex != -1)
            {
                Type gameToCreate = gameInfoTypes[selectedIndex];
                LoadGame(gameToCreate);
            }
        }

        private void LoadGame(Type game)
        {
            IGameInformation newGameInfo = (IGameInformation)Activator.CreateInstance(game);
            loadedGameInfo = newGameInfo;
            curPuzzleNumber = 200;
            ComboBox.ObjectCollection puzzleSizeItems = puzzleSizes.Items;
            puzzleSizeItems.Clear();
            foreach (PuzzleSizeDetails psd in newGameInfo.PuzzleInfo)
            {
                string size = String.Format("{0}x{1}", psd.width, psd.height);
                puzzleSizeItems.Add(size);
            }
        }
    }
}
