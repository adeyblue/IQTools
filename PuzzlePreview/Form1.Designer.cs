namespace PuzzlePreview
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.puzzleSizes = new System.Windows.Forms.ComboBox();
            this.sizeText = new System.Windows.Forms.Label();
            this.puzzleNumText = new System.Windows.Forms.Label();
            this.puzzleImageBox = new System.Windows.Forms.PictureBox();
            this.nextPuzzleBut = new System.Windows.Forms.Button();
            this.puzzleTRN = new System.Windows.Forms.Label();
            this.trnTextBox = new System.Windows.Forms.TextBox();
            this.seedText = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.flippedCheck = new System.Windows.Forms.CheckBox();
            this.bombCheck = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.stepCountUpDown = new System.Windows.Forms.NumericUpDown();
            this.initialSeedTextBox = new System.Windows.Forms.TextBox();
            this.puzzleNumBox = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.puzzleImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stepCountUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.puzzleNumBox)).BeginInit();
            this.SuspendLayout();
            // 
            // puzzleSizes
            // 
            this.puzzleSizes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.puzzleSizes.FormattingEnabled = true;
            this.puzzleSizes.Items.AddRange(new object[] {
            "4x2",
            "4x3",
            "4x4",
            "4x5",
            "4x6",
            "5x4",
            "5x5",
            "5x6",
            "5x7",
            "5x8",
            "6x6",
            "6x7",
            "6x8",
            "6x9",
            "7x7",
            "7x8",
            "7x9"});
            this.puzzleSizes.Location = new System.Drawing.Point(41, 6);
            this.puzzleSizes.Name = "puzzleSizes";
            this.puzzleSizes.Size = new System.Drawing.Size(121, 21);
            this.puzzleSizes.TabIndex = 0;
            this.puzzleSizes.SelectedIndexChanged += new System.EventHandler(this.puzzleSizes_SelectedIndexChanged);
            // 
            // sizeText
            // 
            this.sizeText.AutoSize = true;
            this.sizeText.Location = new System.Drawing.Point(5, 9);
            this.sizeText.Name = "sizeText";
            this.sizeText.Size = new System.Drawing.Size(30, 13);
            this.sizeText.TabIndex = 1;
            this.sizeText.Text = "Size:";
            // 
            // puzzleNumText
            // 
            this.puzzleNumText.AutoSize = true;
            this.puzzleNumText.Location = new System.Drawing.Point(168, 9);
            this.puzzleNumText.Name = "puzzleNumText";
            this.puzzleNumText.Size = new System.Drawing.Size(47, 13);
            this.puzzleNumText.TabIndex = 2;
            this.puzzleNumText.Text = "Number:";
            // 
            // puzzleImageBox
            // 
            this.puzzleImageBox.Location = new System.Drawing.Point(0, 115);
            this.puzzleImageBox.Name = "puzzleImageBox";
            this.puzzleImageBox.Size = new System.Drawing.Size(268, 280);
            this.puzzleImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.puzzleImageBox.TabIndex = 4;
            this.puzzleImageBox.TabStop = false;
            this.puzzleImageBox.Resize += new System.EventHandler(this.puzzleImageBox_Resize);
            this.puzzleImageBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.puzzleImageBox_Click);
            // 
            // nextPuzzleBut
            // 
            this.nextPuzzleBut.Enabled = false;
            this.nextPuzzleBut.Location = new System.Drawing.Point(8, 55);
            this.nextPuzzleBut.Name = "nextPuzzleBut";
            this.nextPuzzleBut.Size = new System.Drawing.Size(73, 26);
            this.nextPuzzleBut.TabIndex = 6;
            this.nextPuzzleBut.Text = "Next &Puzzle";
            this.nextPuzzleBut.UseVisualStyleBackColor = true;
            this.nextPuzzleBut.Click += new System.EventHandler(this.nextPuzzleBut_Click);
            // 
            // puzzleTRN
            // 
            this.puzzleTRN.AutoSize = true;
            this.puzzleTRN.Location = new System.Drawing.Point(118, 33);
            this.puzzleTRN.Name = "puzzleTRN";
            this.puzzleTRN.Size = new System.Drawing.Size(33, 13);
            this.puzzleTRN.TabIndex = 7;
            this.puzzleTRN.Text = "TRN:";
            // 
            // trnTextBox
            // 
            this.trnTextBox.Location = new System.Drawing.Point(157, 31);
            this.trnTextBox.MaxLength = 2;
            this.trnTextBox.Name = "trnTextBox";
            this.trnTextBox.ReadOnly = true;
            this.trnTextBox.Size = new System.Drawing.Size(24, 20);
            this.trnTextBox.TabIndex = 8;
            // 
            // seedText
            // 
            this.seedText.AutoSize = true;
            this.seedText.Location = new System.Drawing.Point(5, 32);
            this.seedText.Name = "seedText";
            this.seedText.Size = new System.Drawing.Size(35, 13);
            this.seedText.TabIndex = 9;
            this.seedText.Text = "Seed:";
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(174, 85);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(89, 24);
            this.resetButton.TabIndex = 11;
            this.resetButton.Text = "Reset Puzzle";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // flippedCheck
            // 
            this.flippedCheck.AutoSize = true;
            this.flippedCheck.Location = new System.Drawing.Point(197, 32);
            this.flippedCheck.Name = "flippedCheck";
            this.flippedCheck.Size = new System.Drawing.Size(66, 17);
            this.flippedCheck.TabIndex = 14;
            this.flippedCheck.Text = "Flipped?";
            this.flippedCheck.UseVisualStyleBackColor = true;
            this.flippedCheck.Click += new System.EventHandler(this.flippedCheck_Click);
            // 
            // bombCheck
            // 
            this.bombCheck.AutoSize = true;
            this.bombCheck.Location = new System.Drawing.Point(115, 90);
            this.bombCheck.Name = "bombCheck";
            this.bombCheck.Size = new System.Drawing.Size(53, 17);
            this.bombCheck.TabIndex = 15;
            this.bombCheck.Text = "Bomb";
            this.bombCheck.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Step";
            // 
            // stepCountUpDown
            // 
            this.stepCountUpDown.Location = new System.Drawing.Point(40, 87);
            this.stepCountUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.stepCountUpDown.Name = "stepCountUpDown";
            this.stepCountUpDown.Size = new System.Drawing.Size(69, 20);
            this.stepCountUpDown.TabIndex = 18;
            this.stepCountUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // initialSeedTextBox
            // 
            this.initialSeedTextBox.Location = new System.Drawing.Point(41, 31);
            this.initialSeedTextBox.MaxLength = 7;
            this.initialSeedTextBox.Name = "initialSeedTextBox";
            this.initialSeedTextBox.Size = new System.Drawing.Size(68, 20);
            this.initialSeedTextBox.TabIndex = 20;
            this.initialSeedTextBox.TextChanged += new System.EventHandler(this.initialSeedTextBox_TextChanged);
            // 
            // puzzleNumBox
            // 
            this.puzzleNumBox.Location = new System.Drawing.Point(221, 7);
            this.puzzleNumBox.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.puzzleNumBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.puzzleNumBox.Name = "puzzleNumBox";
            this.puzzleNumBox.Size = new System.Drawing.Size(41, 20);
            this.puzzleNumBox.TabIndex = 21;
            this.puzzleNumBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.puzzleNumBox.ValueChanged += new System.EventHandler(this.puzzleNumBox_ValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 395);
            this.Controls.Add(this.puzzleNumBox);
            this.Controls.Add(this.initialSeedTextBox);
            this.Controls.Add(this.stepCountUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bombCheck);
            this.Controls.Add(this.flippedCheck);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.seedText);
            this.Controls.Add(this.trnTextBox);
            this.Controls.Add(this.puzzleTRN);
            this.Controls.Add(this.nextPuzzleBut);
            this.Controls.Add(this.puzzleImageBox);
            this.Controls.Add(this.puzzleNumText);
            this.Controls.Add(this.sizeText);
            this.Controls.Add(this.puzzleSizes);
            this.Name = "MainForm";
            this.Text = "IQ Puzzle Preview";
            ((System.ComponentModel.ISupportInitialize)(this.puzzleImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stepCountUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.puzzleNumBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox puzzleSizes;
        private System.Windows.Forms.Label sizeText;
        private System.Windows.Forms.Label puzzleNumText;
        private System.Windows.Forms.PictureBox puzzleImageBox;
        private System.Windows.Forms.Button nextPuzzleBut;
        private System.Windows.Forms.Label puzzleTRN;
        private System.Windows.Forms.TextBox trnTextBox;
        private System.Windows.Forms.Label seedText;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.CheckBox flippedCheck;
        private System.Windows.Forms.CheckBox bombCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown stepCountUpDown;
        private System.Windows.Forms.TextBox initialSeedTextBox;
        private System.Windows.Forms.NumericUpDown puzzleNumBox;
    }
}

