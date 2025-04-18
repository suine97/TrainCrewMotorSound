namespace TrainCrewMotorSound
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.Button_OpenFile = new System.Windows.Forms.Button();
            this.TrackBar_MotorVolume = new System.Windows.Forms.TrackBar();
            this.TrackBar_RunVolume = new System.Windows.Forms.TrackBar();
            this.ComboBox_RunSound = new System.Windows.Forms.ComboBox();
            this.GroupBox_MotorSound = new System.Windows.Forms.GroupBox();
            this.ComboBox_RegenerationLimit = new System.Windows.Forms.ComboBox();
            this.Label_RegenerationLimit_Unit = new System.Windows.Forms.Label();
            this.Label_RegenerationLimit = new System.Windows.Forms.Label();
            this.Label_MotorVolume = new System.Windows.Forms.Label();
            this.GroupBox_RunSound = new System.Windows.Forms.GroupBox();
            this.Label_RunSelect = new System.Windows.Forms.Label();
            this.Label_RunVolume = new System.Windows.Forms.Label();
            this.CheckBox_TopMost = new System.Windows.Forms.CheckBox();
            this.Label_Parameters = new System.Windows.Forms.Label();
            this.GroupBox_Infomation = new System.Windows.Forms.GroupBox();
            this.CheckBox_NotchUnLinked = new System.Windows.Forms.CheckBox();
            this.CheckBox_EB = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_MotorVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_RunVolume)).BeginInit();
            this.GroupBox_MotorSound.SuspendLayout();
            this.GroupBox_RunSound.SuspendLayout();
            this.GroupBox_Infomation.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_OpenFile
            // 
            this.Button_OpenFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Button_OpenFile.Location = new System.Drawing.Point(38, 93);
            this.Button_OpenFile.Name = "Button_OpenFile";
            this.Button_OpenFile.Size = new System.Drawing.Size(132, 38);
            this.Button_OpenFile.TabIndex = 0;
            this.Button_OpenFile.Text = "BVE車両ファイル読込";
            this.Button_OpenFile.UseVisualStyleBackColor = false;
            this.Button_OpenFile.Click += new System.EventHandler(this.Button_OpenFile_Click);
            // 
            // TrackBar_MotorVolume
            // 
            this.TrackBar_MotorVolume.AutoSize = false;
            this.TrackBar_MotorVolume.BackColor = System.Drawing.Color.White;
            this.TrackBar_MotorVolume.Location = new System.Drawing.Point(6, 89);
            this.TrackBar_MotorVolume.Maximum = 100;
            this.TrackBar_MotorVolume.Name = "TrackBar_MotorVolume";
            this.TrackBar_MotorVolume.Size = new System.Drawing.Size(200, 30);
            this.TrackBar_MotorVolume.TabIndex = 1;
            this.TrackBar_MotorVolume.TickFrequency = 10;
            this.TrackBar_MotorVolume.Value = 100;
            this.TrackBar_MotorVolume.Scroll += new System.EventHandler(this.TrackBar_MotorVolume_Scroll);
            // 
            // TrackBar_RunVolume
            // 
            this.TrackBar_RunVolume.AutoSize = false;
            this.TrackBar_RunVolume.BackColor = System.Drawing.Color.White;
            this.TrackBar_RunVolume.Location = new System.Drawing.Point(6, 89);
            this.TrackBar_RunVolume.Maximum = 100;
            this.TrackBar_RunVolume.Name = "TrackBar_RunVolume";
            this.TrackBar_RunVolume.Size = new System.Drawing.Size(200, 30);
            this.TrackBar_RunVolume.TabIndex = 2;
            this.TrackBar_RunVolume.TickFrequency = 10;
            this.TrackBar_RunVolume.Value = 100;
            this.TrackBar_RunVolume.Scroll += new System.EventHandler(this.TrackBar_RunVolume_Scroll);
            // 
            // ComboBox_RunSound
            // 
            this.ComboBox_RunSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_RunSound.FormattingEnabled = true;
            this.ComboBox_RunSound.Location = new System.Drawing.Point(81, 28);
            this.ComboBox_RunSound.Name = "ComboBox_RunSound";
            this.ComboBox_RunSound.Size = new System.Drawing.Size(125, 20);
            this.ComboBox_RunSound.TabIndex = 3;
            this.ComboBox_RunSound.SelectedIndexChanged += new System.EventHandler(this.ComboBox_RunSound_SelectedIndexChanged);
            // 
            // GroupBox_MotorSound
            // 
            this.GroupBox_MotorSound.Controls.Add(this.ComboBox_RegenerationLimit);
            this.GroupBox_MotorSound.Controls.Add(this.Label_RegenerationLimit_Unit);
            this.GroupBox_MotorSound.Controls.Add(this.Label_RegenerationLimit);
            this.GroupBox_MotorSound.Controls.Add(this.Label_MotorVolume);
            this.GroupBox_MotorSound.Controls.Add(this.TrackBar_MotorVolume);
            this.GroupBox_MotorSound.ForeColor = System.Drawing.Color.Blue;
            this.GroupBox_MotorSound.Location = new System.Drawing.Point(209, 12);
            this.GroupBox_MotorSound.Name = "GroupBox_MotorSound";
            this.GroupBox_MotorSound.Size = new System.Drawing.Size(213, 125);
            this.GroupBox_MotorSound.TabIndex = 4;
            this.GroupBox_MotorSound.TabStop = false;
            this.GroupBox_MotorSound.Text = "モータ音";
            // 
            // ComboBox_RegenerationLimit
            // 
            this.ComboBox_RegenerationLimit.BackColor = System.Drawing.SystemColors.Window;
            this.ComboBox_RegenerationLimit.DropDownHeight = 120;
            this.ComboBox_RegenerationLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_RegenerationLimit.DropDownWidth = 30;
            this.ComboBox_RegenerationLimit.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ComboBox_RegenerationLimit.FormattingEnabled = true;
            this.ComboBox_RegenerationLimit.IntegralHeight = false;
            this.ComboBox_RegenerationLimit.ItemHeight = 15;
            this.ComboBox_RegenerationLimit.Location = new System.Drawing.Point(95, 21);
            this.ComboBox_RegenerationLimit.Name = "ComboBox_RegenerationLimit";
            this.ComboBox_RegenerationLimit.Size = new System.Drawing.Size(72, 23);
            this.ComboBox_RegenerationLimit.TabIndex = 12;
            this.ComboBox_RegenerationLimit.SelectedIndexChanged += new System.EventHandler(this.ComboBox_RegenerationLimit_SelectedIndexChanged);
            // 
            // Label_RegenerationLimit_Unit
            // 
            this.Label_RegenerationLimit_Unit.BackColor = System.Drawing.SystemColors.Control;
            this.Label_RegenerationLimit_Unit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_RegenerationLimit_Unit.Location = new System.Drawing.Point(173, 24);
            this.Label_RegenerationLimit_Unit.Name = "Label_RegenerationLimit_Unit";
            this.Label_RegenerationLimit_Unit.Size = new System.Drawing.Size(34, 20);
            this.Label_RegenerationLimit_Unit.TabIndex = 11;
            this.Label_RegenerationLimit_Unit.Text = "km/h";
            this.Label_RegenerationLimit_Unit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_RegenerationLimit
            // 
            this.Label_RegenerationLimit.BackColor = System.Drawing.SystemColors.Control;
            this.Label_RegenerationLimit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_RegenerationLimit.Location = new System.Drawing.Point(6, 24);
            this.Label_RegenerationLimit.Name = "Label_RegenerationLimit";
            this.Label_RegenerationLimit.Size = new System.Drawing.Size(83, 20);
            this.Label_RegenerationLimit.TabIndex = 9;
            this.Label_RegenerationLimit.Text = "回生失効速度";
            this.Label_RegenerationLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_MotorVolume
            // 
            this.Label_MotorVolume.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_MotorVolume.Location = new System.Drawing.Point(6, 66);
            this.Label_MotorVolume.Name = "Label_MotorVolume";
            this.Label_MotorVolume.Size = new System.Drawing.Size(200, 20);
            this.Label_MotorVolume.TabIndex = 5;
            this.Label_MotorVolume.Text = "全体音量：100%";
            this.Label_MotorVolume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GroupBox_RunSound
            // 
            this.GroupBox_RunSound.Controls.Add(this.Label_RunSelect);
            this.GroupBox_RunSound.Controls.Add(this.Label_RunVolume);
            this.GroupBox_RunSound.Controls.Add(this.TrackBar_RunVolume);
            this.GroupBox_RunSound.Controls.Add(this.ComboBox_RunSound);
            this.GroupBox_RunSound.ForeColor = System.Drawing.Color.Blue;
            this.GroupBox_RunSound.Location = new System.Drawing.Point(209, 150);
            this.GroupBox_RunSound.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.GroupBox_RunSound.Name = "GroupBox_RunSound";
            this.GroupBox_RunSound.Size = new System.Drawing.Size(213, 125);
            this.GroupBox_RunSound.TabIndex = 5;
            this.GroupBox_RunSound.TabStop = false;
            this.GroupBox_RunSound.Text = "走行音";
            // 
            // Label_RunSelect
            // 
            this.Label_RunSelect.BackColor = System.Drawing.SystemColors.Control;
            this.Label_RunSelect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_RunSelect.Location = new System.Drawing.Point(6, 27);
            this.Label_RunSelect.Name = "Label_RunSelect";
            this.Label_RunSelect.Size = new System.Drawing.Size(69, 20);
            this.Label_RunSelect.TabIndex = 7;
            this.Label_RunSelect.Text = "走行音選択";
            this.Label_RunSelect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_RunVolume
            // 
            this.Label_RunVolume.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_RunVolume.Location = new System.Drawing.Point(6, 66);
            this.Label_RunVolume.Name = "Label_RunVolume";
            this.Label_RunVolume.Size = new System.Drawing.Size(200, 20);
            this.Label_RunVolume.TabIndex = 6;
            this.Label_RunVolume.Text = "全体音量：100%";
            this.Label_RunVolume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CheckBox_TopMost
            // 
            this.CheckBox_TopMost.AutoSize = true;
            this.CheckBox_TopMost.Checked = true;
            this.CheckBox_TopMost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_TopMost.Location = new System.Drawing.Point(12, 12);
            this.CheckBox_TopMost.Name = "CheckBox_TopMost";
            this.CheckBox_TopMost.Size = new System.Drawing.Size(84, 16);
            this.CheckBox_TopMost.TabIndex = 6;
            this.CheckBox_TopMost.Text = "最前面表示";
            this.CheckBox_TopMost.UseVisualStyleBackColor = true;
            this.CheckBox_TopMost.CheckedChanged += new System.EventHandler(this.CheckBox_TopMost_CheckedChanged);
            // 
            // Label_Parameters
            // 
            this.Label_Parameters.Font = new System.Drawing.Font("游ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Label_Parameters.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_Parameters.Location = new System.Drawing.Point(6, 28);
            this.Label_Parameters.Name = "Label_Parameters";
            this.Label_Parameters.Size = new System.Drawing.Size(179, 85);
            this.Label_Parameters.TabIndex = 7;
            this.Label_Parameters.Text = "車両ファイル読込：";
            // 
            // GroupBox_Infomation
            // 
            this.GroupBox_Infomation.Controls.Add(this.Label_Parameters);
            this.GroupBox_Infomation.ForeColor = System.Drawing.Color.Blue;
            this.GroupBox_Infomation.Location = new System.Drawing.Point(12, 150);
            this.GroupBox_Infomation.Name = "GroupBox_Infomation";
            this.GroupBox_Infomation.Size = new System.Drawing.Size(191, 125);
            this.GroupBox_Infomation.TabIndex = 7;
            this.GroupBox_Infomation.TabStop = false;
            this.GroupBox_Infomation.Text = "情報";
            // 
            // CheckBox_NotchUnLinked
            // 
            this.CheckBox_NotchUnLinked.AutoSize = true;
            this.CheckBox_NotchUnLinked.Checked = true;
            this.CheckBox_NotchUnLinked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_NotchUnLinked.Location = new System.Drawing.Point(12, 34);
            this.CheckBox_NotchUnLinked.Name = "CheckBox_NotchUnLinked";
            this.CheckBox_NotchUnLinked.Size = new System.Drawing.Size(72, 16);
            this.CheckBox_NotchUnLinked.TabIndex = 8;
            this.CheckBox_NotchUnLinked.Text = "ノッチ連動";
            this.CheckBox_NotchUnLinked.UseVisualStyleBackColor = true;
            this.CheckBox_NotchUnLinked.CheckedChanged += new System.EventHandler(this.CheckBox_NotchUnLinked_CheckedChanged);
            // 
            // CheckBox_EB
            // 
            this.CheckBox_EB.AutoSize = true;
            this.CheckBox_EB.Checked = true;
            this.CheckBox_EB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_EB.Location = new System.Drawing.Point(12, 56);
            this.CheckBox_EB.Name = "CheckBox_EB";
            this.CheckBox_EB.Size = new System.Drawing.Size(139, 16);
            this.CheckBox_EB.TabIndex = 9;
            this.CheckBox_EB.Text = "非常ブレーキ時回生オフ";
            this.CheckBox_EB.UseVisualStyleBackColor = true;
            this.CheckBox_EB.CheckedChanged += new System.EventHandler(this.CheckBox_EB_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 281);
            this.Controls.Add(this.CheckBox_EB);
            this.Controls.Add(this.CheckBox_NotchUnLinked);
            this.Controls.Add(this.GroupBox_Infomation);
            this.Controls.Add(this.CheckBox_TopMost);
            this.Controls.Add(this.GroupBox_RunSound);
            this.Controls.Add(this.GroupBox_MotorSound);
            this.Controls.Add(this.Button_OpenFile);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "TrainCrewMotorSound V1.0.12 (Build 2025.04.18)";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_MotorVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_RunVolume)).EndInit();
            this.GroupBox_MotorSound.ResumeLayout(false);
            this.GroupBox_RunSound.ResumeLayout(false);
            this.GroupBox_Infomation.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_OpenFile;
        private System.Windows.Forms.TrackBar TrackBar_MotorVolume;
        private System.Windows.Forms.TrackBar TrackBar_RunVolume;
        private System.Windows.Forms.ComboBox ComboBox_RunSound;
        private System.Windows.Forms.GroupBox GroupBox_MotorSound;
        private System.Windows.Forms.Label Label_MotorVolume;
        private System.Windows.Forms.GroupBox GroupBox_RunSound;
        private System.Windows.Forms.Label Label_RunVolume;
        private System.Windows.Forms.Label Label_RunSelect;
        private System.Windows.Forms.CheckBox CheckBox_TopMost;
        private System.Windows.Forms.Label Label_Parameters;
        private System.Windows.Forms.GroupBox GroupBox_Infomation;
        private System.Windows.Forms.CheckBox CheckBox_NotchUnLinked;
        private System.Windows.Forms.Label Label_RegenerationLimit;
        private System.Windows.Forms.Label Label_RegenerationLimit_Unit;
        private System.Windows.Forms.ComboBox ComboBox_RegenerationLimit;
        private System.Windows.Forms.CheckBox CheckBox_EB;
    }
}

