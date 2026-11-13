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
            this.GroupBox_AutoRun = new System.Windows.Forms.GroupBox();
            this.Label_AutoRunAcceleration = new System.Windows.Forms.Label();
            this.NumericUpDown_AutoRunAcceleration = new System.Windows.Forms.NumericUpDown();
            this.Label_AutoRunAcceleration_Unit = new System.Windows.Forms.Label();
            this.Label_AutoRunDeceleration = new System.Windows.Forms.Label();
            this.NumericUpDown_AutoRunDeceleration = new System.Windows.Forms.NumericUpDown();
            this.Label_AutoRunDeceleration_Unit = new System.Windows.Forms.Label();
            this.Label_AutoRunMaxSpeed = new System.Windows.Forms.Label();
            this.NumericUpDown_AutoRunMaxSpeed = new System.Windows.Forms.NumericUpDown();
            this.Label_AutoRunMaxSpeed_Unit = new System.Windows.Forms.Label();
            this.Label_AutoRunLowerSpeed = new System.Windows.Forms.Label();
            this.NumericUpDown_AutoRunLowerSpeed = new System.Windows.Forms.NumericUpDown();
            this.Label_AutoRunLowerSpeed_Unit = new System.Windows.Forms.Label();
            this.RadioButton_AutoRunDuration = new System.Windows.Forms.RadioButton();
            this.NumericUpDown_AutoRunMinutes = new System.Windows.Forms.NumericUpDown();
            this.Label_AutoRunMinutes_Unit = new System.Windows.Forms.Label();
            this.NumericUpDown_AutoRunSeconds = new System.Windows.Forms.NumericUpDown();
            this.Label_AutoRunSeconds_Unit = new System.Windows.Forms.Label();
            this.RadioButton_AutoRunEndTime = new System.Windows.Forms.RadioButton();
            this.DateTimePicker_AutoRunEndTime = new System.Windows.Forms.DateTimePicker();
            this.RadioButton_AutoRunNoLimit = new System.Windows.Forms.RadioButton();
            this.Label_AutoRunRemaining = new System.Windows.Forms.Label();
            this.Button_AutoRun = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_MotorVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_RunVolume)).BeginInit();
            this.GroupBox_MotorSound.SuspendLayout();
            this.GroupBox_RunSound.SuspendLayout();
            this.GroupBox_Infomation.SuspendLayout();
            this.GroupBox_AutoRun.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunAcceleration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunDeceleration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunMaxSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunLowerSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunSeconds)).BeginInit();
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
            // GroupBox_AutoRun
            // 
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunAcceleration);
            this.GroupBox_AutoRun.Controls.Add(this.NumericUpDown_AutoRunAcceleration);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunAcceleration_Unit);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunDeceleration);
            this.GroupBox_AutoRun.Controls.Add(this.NumericUpDown_AutoRunDeceleration);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunDeceleration_Unit);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunMaxSpeed);
            this.GroupBox_AutoRun.Controls.Add(this.NumericUpDown_AutoRunMaxSpeed);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunMaxSpeed_Unit);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunLowerSpeed);
            this.GroupBox_AutoRun.Controls.Add(this.NumericUpDown_AutoRunLowerSpeed);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunLowerSpeed_Unit);
            this.GroupBox_AutoRun.Controls.Add(this.RadioButton_AutoRunDuration);
            this.GroupBox_AutoRun.Controls.Add(this.NumericUpDown_AutoRunMinutes);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunMinutes_Unit);
            this.GroupBox_AutoRun.Controls.Add(this.NumericUpDown_AutoRunSeconds);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunSeconds_Unit);
            this.GroupBox_AutoRun.Controls.Add(this.RadioButton_AutoRunEndTime);
            this.GroupBox_AutoRun.Controls.Add(this.DateTimePicker_AutoRunEndTime);
            this.GroupBox_AutoRun.Controls.Add(this.RadioButton_AutoRunNoLimit);
            this.GroupBox_AutoRun.Controls.Add(this.Label_AutoRunRemaining);
            this.GroupBox_AutoRun.Controls.Add(this.Button_AutoRun);
            this.GroupBox_AutoRun.ForeColor = System.Drawing.Color.Blue;
            this.GroupBox_AutoRun.Location = new System.Drawing.Point(12, 281);
            this.GroupBox_AutoRun.Name = "GroupBox_AutoRun";
            this.GroupBox_AutoRun.Size = new System.Drawing.Size(410, 150);
            this.GroupBox_AutoRun.TabIndex = 10;
            this.GroupBox_AutoRun.TabStop = false;
            this.GroupBox_AutoRun.Text = "自動走行モード";
            // 
            // Label_AutoRunAcceleration
            // 
            this.Label_AutoRunAcceleration.AutoSize = true;
            this.Label_AutoRunAcceleration.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunAcceleration.Location = new System.Drawing.Point(8, 25);
            this.Label_AutoRunAcceleration.Name = "Label_AutoRunAcceleration";
            this.Label_AutoRunAcceleration.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunAcceleration.TabIndex = 11;
            this.Label_AutoRunAcceleration.Text = "加速度";
            // 
            // NumericUpDown_AutoRunAcceleration
            // 
            this.NumericUpDown_AutoRunAcceleration.DecimalPlaces = 1;
            this.NumericUpDown_AutoRunAcceleration.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NumericUpDown_AutoRunAcceleration.Location = new System.Drawing.Point(68, 22);
            this.NumericUpDown_AutoRunAcceleration.Name = "NumericUpDown_AutoRunAcceleration";
            this.NumericUpDown_AutoRunAcceleration.Size = new System.Drawing.Size(60, 19);
            this.NumericUpDown_AutoRunAcceleration.TabIndex = 12;
            this.NumericUpDown_AutoRunAcceleration.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            this.NumericUpDown_AutoRunAcceleration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NumericUpDown_AutoRunAcceleration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_AutoRunAcceleration.Value = new decimal(new int[] {
            25,
            0,
            0,
            65536});
            // 
            // Label_AutoRunAcceleration_Unit
            // 
            this.Label_AutoRunAcceleration_Unit.AutoSize = true;
            this.Label_AutoRunAcceleration_Unit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunAcceleration_Unit.Location = new System.Drawing.Point(133, 25);
            this.Label_AutoRunAcceleration_Unit.Name = "Label_AutoRunAcceleration_Unit";
            this.Label_AutoRunAcceleration_Unit.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunAcceleration_Unit.TabIndex = 13;
            this.Label_AutoRunAcceleration_Unit.Text = "km/h/s";
            // 
            // Label_AutoRunDeceleration
            // 
            this.Label_AutoRunDeceleration.AutoSize = true;
            this.Label_AutoRunDeceleration.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunDeceleration.Location = new System.Drawing.Point(206, 25);
            this.Label_AutoRunDeceleration.Name = "Label_AutoRunDeceleration";
            this.Label_AutoRunDeceleration.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunDeceleration.TabIndex = 14;
            this.Label_AutoRunDeceleration.Text = "減速度";
            // 
            // NumericUpDown_AutoRunDeceleration
            // 
            this.NumericUpDown_AutoRunDeceleration.DecimalPlaces = 1;
            this.NumericUpDown_AutoRunDeceleration.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NumericUpDown_AutoRunDeceleration.Location = new System.Drawing.Point(266, 22);
            this.NumericUpDown_AutoRunDeceleration.Name = "NumericUpDown_AutoRunDeceleration";
            this.NumericUpDown_AutoRunDeceleration.Size = new System.Drawing.Size(60, 19);
            this.NumericUpDown_AutoRunDeceleration.TabIndex = 15;
            this.NumericUpDown_AutoRunDeceleration.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            this.NumericUpDown_AutoRunDeceleration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NumericUpDown_AutoRunDeceleration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_AutoRunDeceleration.Value = new decimal(new int[] {
            30,
            0,
            0,
            65536});
            // 
            // Label_AutoRunDeceleration_Unit
            // 
            this.Label_AutoRunDeceleration_Unit.AutoSize = true;
            this.Label_AutoRunDeceleration_Unit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunDeceleration_Unit.Location = new System.Drawing.Point(331, 25);
            this.Label_AutoRunDeceleration_Unit.Name = "Label_AutoRunDeceleration_Unit";
            this.Label_AutoRunDeceleration_Unit.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunDeceleration_Unit.TabIndex = 16;
            this.Label_AutoRunDeceleration_Unit.Text = "km/h/s";
            // 
            // Label_AutoRunMaxSpeed
            // 
            this.Label_AutoRunMaxSpeed.AutoSize = true;
            this.Label_AutoRunMaxSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunMaxSpeed.Location = new System.Drawing.Point(8, 51);
            this.Label_AutoRunMaxSpeed.Name = "Label_AutoRunMaxSpeed";
            this.Label_AutoRunMaxSpeed.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunMaxSpeed.TabIndex = 17;
            this.Label_AutoRunMaxSpeed.Text = "最高速度";
            // 
            // NumericUpDown_AutoRunMaxSpeed
            // 
            this.NumericUpDown_AutoRunMaxSpeed.Location = new System.Drawing.Point(68, 48);
            this.NumericUpDown_AutoRunMaxSpeed.Name = "NumericUpDown_AutoRunMaxSpeed";
            this.NumericUpDown_AutoRunMaxSpeed.Size = new System.Drawing.Size(60, 19);
            this.NumericUpDown_AutoRunMaxSpeed.TabIndex = 18;
            this.NumericUpDown_AutoRunMaxSpeed.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.NumericUpDown_AutoRunMaxSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_AutoRunMaxSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_AutoRunMaxSpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // Label_AutoRunMaxSpeed_Unit
            // 
            this.Label_AutoRunMaxSpeed_Unit.AutoSize = true;
            this.Label_AutoRunMaxSpeed_Unit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunMaxSpeed_Unit.Location = new System.Drawing.Point(133, 51);
            this.Label_AutoRunMaxSpeed_Unit.Name = "Label_AutoRunMaxSpeed_Unit";
            this.Label_AutoRunMaxSpeed_Unit.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunMaxSpeed_Unit.TabIndex = 19;
            this.Label_AutoRunMaxSpeed_Unit.Text = "km/h";
            // 
            // Label_AutoRunLowerSpeed
            // 
            this.Label_AutoRunLowerSpeed.AutoSize = true;
            this.Label_AutoRunLowerSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunLowerSpeed.Location = new System.Drawing.Point(206, 51);
            this.Label_AutoRunLowerSpeed.Name = "Label_AutoRunLowerSpeed";
            this.Label_AutoRunLowerSpeed.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunLowerSpeed.TabIndex = 20;
            this.Label_AutoRunLowerSpeed.Text = "下限速度";
            // 
            // NumericUpDown_AutoRunLowerSpeed
            // 
            this.NumericUpDown_AutoRunLowerSpeed.Location = new System.Drawing.Point(266, 48);
            this.NumericUpDown_AutoRunLowerSpeed.Name = "NumericUpDown_AutoRunLowerSpeed";
            this.NumericUpDown_AutoRunLowerSpeed.Size = new System.Drawing.Size(60, 19);
            this.NumericUpDown_AutoRunLowerSpeed.TabIndex = 21;
            this.NumericUpDown_AutoRunLowerSpeed.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.NumericUpDown_AutoRunLowerSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_AutoRunLowerSpeed.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // Label_AutoRunLowerSpeed_Unit
            // 
            this.Label_AutoRunLowerSpeed_Unit.AutoSize = true;
            this.Label_AutoRunLowerSpeed_Unit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunLowerSpeed_Unit.Location = new System.Drawing.Point(331, 51);
            this.Label_AutoRunLowerSpeed_Unit.Name = "Label_AutoRunLowerSpeed_Unit";
            this.Label_AutoRunLowerSpeed_Unit.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunLowerSpeed_Unit.TabIndex = 22;
            this.Label_AutoRunLowerSpeed_Unit.Text = "km/h";
            // 
            // RadioButton_AutoRunDuration
            // 
            this.RadioButton_AutoRunDuration.AutoSize = true;
            this.RadioButton_AutoRunDuration.Checked = true;
            this.RadioButton_AutoRunDuration.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RadioButton_AutoRunDuration.Location = new System.Drawing.Point(8, 76);
            this.RadioButton_AutoRunDuration.Name = "RadioButton_AutoRunDuration";
            this.RadioButton_AutoRunDuration.Size = new System.Drawing.Size(71, 16);
            this.RadioButton_AutoRunDuration.TabIndex = 23;
            this.RadioButton_AutoRunDuration.TabStop = true;
            this.RadioButton_AutoRunDuration.Text = "走行時間";
            this.RadioButton_AutoRunDuration.UseVisualStyleBackColor = true;
            this.RadioButton_AutoRunDuration.CheckedChanged += new System.EventHandler(this.RadioButton_AutoRun_CheckedChanged);
            // 
            // NumericUpDown_AutoRunMinutes
            // 
            this.NumericUpDown_AutoRunMinutes.Location = new System.Drawing.Point(88, 75);
            this.NumericUpDown_AutoRunMinutes.Name = "NumericUpDown_AutoRunMinutes";
            this.NumericUpDown_AutoRunMinutes.Size = new System.Drawing.Size(52, 19);
            this.NumericUpDown_AutoRunMinutes.TabIndex = 24;
            this.NumericUpDown_AutoRunMinutes.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.NumericUpDown_AutoRunMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericUpDown_AutoRunMinutes.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // Label_AutoRunMinutes_Unit
            // 
            this.Label_AutoRunMinutes_Unit.AutoSize = true;
            this.Label_AutoRunMinutes_Unit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunMinutes_Unit.Location = new System.Drawing.Point(143, 78);
            this.Label_AutoRunMinutes_Unit.Name = "Label_AutoRunMinutes_Unit";
            this.Label_AutoRunMinutes_Unit.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunMinutes_Unit.TabIndex = 25;
            this.Label_AutoRunMinutes_Unit.Text = "分";
            // 
            // NumericUpDown_AutoRunSeconds
            // 
            this.NumericUpDown_AutoRunSeconds.Location = new System.Drawing.Point(166, 75);
            this.NumericUpDown_AutoRunSeconds.Name = "NumericUpDown_AutoRunSeconds";
            this.NumericUpDown_AutoRunSeconds.Size = new System.Drawing.Size(42, 19);
            this.NumericUpDown_AutoRunSeconds.TabIndex = 26;
            this.NumericUpDown_AutoRunSeconds.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NumericUpDown_AutoRunSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label_AutoRunSeconds_Unit
            // 
            this.Label_AutoRunSeconds_Unit.AutoSize = true;
            this.Label_AutoRunSeconds_Unit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunSeconds_Unit.Location = new System.Drawing.Point(211, 78);
            this.Label_AutoRunSeconds_Unit.Name = "Label_AutoRunSeconds_Unit";
            this.Label_AutoRunSeconds_Unit.Size = new System.Drawing.Size(40, 12);
            this.Label_AutoRunSeconds_Unit.TabIndex = 27;
            this.Label_AutoRunSeconds_Unit.Text = "秒";
            // 
            // RadioButton_AutoRunEndTime
            // 
            this.RadioButton_AutoRunEndTime.AutoSize = true;
            this.RadioButton_AutoRunEndTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RadioButton_AutoRunEndTime.Location = new System.Drawing.Point(8, 100);
            this.RadioButton_AutoRunEndTime.Name = "RadioButton_AutoRunEndTime";
            this.RadioButton_AutoRunEndTime.Size = new System.Drawing.Size(71, 16);
            this.RadioButton_AutoRunEndTime.TabIndex = 28;
            this.RadioButton_AutoRunEndTime.Text = "終了日時";
            this.RadioButton_AutoRunEndTime.UseVisualStyleBackColor = true;
            this.RadioButton_AutoRunEndTime.CheckedChanged += new System.EventHandler(this.RadioButton_AutoRun_CheckedChanged);
            // 
            // DateTimePicker_AutoRunEndTime
            // 
            this.DateTimePicker_AutoRunEndTime.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.DateTimePicker_AutoRunEndTime.Enabled = false;
            this.DateTimePicker_AutoRunEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateTimePicker_AutoRunEndTime.Location = new System.Drawing.Point(88, 98);
            this.DateTimePicker_AutoRunEndTime.Name = "DateTimePicker_AutoRunEndTime";
            this.DateTimePicker_AutoRunEndTime.Size = new System.Drawing.Size(150, 19);
            this.DateTimePicker_AutoRunEndTime.TabIndex = 29;
            // 
            // RadioButton_AutoRunNoLimit
            // 
            this.RadioButton_AutoRunNoLimit.AutoSize = true;
            this.RadioButton_AutoRunNoLimit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RadioButton_AutoRunNoLimit.Location = new System.Drawing.Point(8, 124);
            this.RadioButton_AutoRunNoLimit.Name = "RadioButton_AutoRunNoLimit";
            this.RadioButton_AutoRunNoLimit.Size = new System.Drawing.Size(71, 16);
            this.RadioButton_AutoRunNoLimit.TabIndex = 30;
            this.RadioButton_AutoRunNoLimit.Text = "指定なし";
            this.RadioButton_AutoRunNoLimit.UseVisualStyleBackColor = true;
            this.RadioButton_AutoRunNoLimit.CheckedChanged += new System.EventHandler(this.RadioButton_AutoRun_CheckedChanged);
            // 
            // Label_AutoRunRemaining
            // 
            this.Label_AutoRunRemaining.AutoSize = true;
            this.Label_AutoRunRemaining.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_AutoRunRemaining.Location = new System.Drawing.Point(88, 126);
            this.Label_AutoRunRemaining.Name = "Label_AutoRunRemaining";
            this.Label_AutoRunRemaining.Size = new System.Drawing.Size(95, 12);
            this.Label_AutoRunRemaining.TabIndex = 31;
            this.Label_AutoRunRemaining.Text = "残り時間：--:--";
            // 
            // Button_AutoRun
            // 
            this.Button_AutoRun.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Button_AutoRun.Location = new System.Drawing.Point(290, 96);
            this.Button_AutoRun.Name = "Button_AutoRun";
            this.Button_AutoRun.Size = new System.Drawing.Size(110, 44);
            this.Button_AutoRun.TabIndex = 32;
            this.Button_AutoRun.Text = "開始";
            this.Button_AutoRun.UseVisualStyleBackColor = true;
            this.Button_AutoRun.Click += new System.EventHandler(this.Button_AutoRun_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 443);
            this.Controls.Add(this.GroupBox_AutoRun);
            this.Controls.Add(this.CheckBox_EB);
            this.Controls.Add(this.CheckBox_NotchUnLinked);
            this.Controls.Add(this.GroupBox_Infomation);
            this.Controls.Add(this.CheckBox_TopMost);
            this.Controls.Add(this.GroupBox_RunSound);
            this.Controls.Add(this.GroupBox_MotorSound);
            this.Controls.Add(this.Button_OpenFile);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "TrainCrewMotorSound V1.0.15 (Build 2026.11.13)";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_MotorVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_RunVolume)).EndInit();
            this.GroupBox_MotorSound.ResumeLayout(false);
            this.GroupBox_RunSound.ResumeLayout(false);
            this.GroupBox_Infomation.ResumeLayout(false);
            this.GroupBox_AutoRun.ResumeLayout(false);
            this.GroupBox_AutoRun.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunAcceleration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunDeceleration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunMaxSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunLowerSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AutoRunSeconds)).EndInit();
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
        private System.Windows.Forms.GroupBox GroupBox_AutoRun;
        private System.Windows.Forms.Label Label_AutoRunAcceleration;
        private System.Windows.Forms.NumericUpDown NumericUpDown_AutoRunAcceleration;
        private System.Windows.Forms.Label Label_AutoRunAcceleration_Unit;
        private System.Windows.Forms.Label Label_AutoRunDeceleration;
        private System.Windows.Forms.NumericUpDown NumericUpDown_AutoRunDeceleration;
        private System.Windows.Forms.Label Label_AutoRunDeceleration_Unit;
        private System.Windows.Forms.Label Label_AutoRunMaxSpeed;
        private System.Windows.Forms.NumericUpDown NumericUpDown_AutoRunMaxSpeed;
        private System.Windows.Forms.Label Label_AutoRunMaxSpeed_Unit;
        private System.Windows.Forms.Label Label_AutoRunLowerSpeed;
        private System.Windows.Forms.NumericUpDown NumericUpDown_AutoRunLowerSpeed;
        private System.Windows.Forms.Label Label_AutoRunLowerSpeed_Unit;
        private System.Windows.Forms.RadioButton RadioButton_AutoRunDuration;
        private System.Windows.Forms.NumericUpDown NumericUpDown_AutoRunMinutes;
        private System.Windows.Forms.Label Label_AutoRunMinutes_Unit;
        private System.Windows.Forms.NumericUpDown NumericUpDown_AutoRunSeconds;
        private System.Windows.Forms.Label Label_AutoRunSeconds_Unit;
        private System.Windows.Forms.RadioButton RadioButton_AutoRunEndTime;
        private System.Windows.Forms.DateTimePicker DateTimePicker_AutoRunEndTime;
        private System.Windows.Forms.RadioButton RadioButton_AutoRunNoLimit;
        private System.Windows.Forms.Label Label_AutoRunRemaining;
        private System.Windows.Forms.Button Button_AutoRun;
    }
}

