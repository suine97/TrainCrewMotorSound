﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrainCrew;

namespace TrainCrewMotorSound
{
    public partial class MainForm : Form
    {
        private Timer timer;
        private Timer timer500;
        private Sound sound = new Sound();
        private readonly StringBuilder sb = new StringBuilder();
        private float fOldSpeed = 0.0f;
        private int iRegenerationLimit = 0;
        private bool IsDeceleration = false;
        private bool IsNotchLinked = true;
        private string vehicleDirectoryName = "";
        private string soundDirectoryName = "";
        private string motorNoiseDirectoryName = "";
        private string soundPath = "";
        private string motorNoisePath = "";
        private List<List<float?>> PowerVolumeData = new List<List<float?>>();
        private List<List<float?>> PowerFrequencyData = new List<List<float?>>();
        private List<List<float?>> BrakeVolumeData = new List<List<float?>>();
        private List<List<float?>> BrakeFrequencyData = new List<List<float?>>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            //最大サイズと最小サイズを現在のサイズに設定する
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            //Timer設定
            timer = InitializeTimer(50, Timer_Tick);
            timer500 = InitializeTimer(500, Timer500_Tick);

            // 音声処理スレッド生成
            sound.IsSoundThreadLoop = true;
            var _ = Task.Run(() => { sound.SoundThread(); });

            // コンボボックス初期化
            InitializeRunSoundComboBox();
            for (int i = 0; i <= 120; i++)
            {
                ComboBox_RegenerationLimit.Items.Add(i.ToString().PadLeft(5, ' '));
            }
            ComboBox_RegenerationLimit.SelectedIndex = 0;

            //初期化。起動時のみの呼び出しで大丈夫です。
            TrainCrewInput.Init();
        }

        /// <summary>
        /// Timer_Tickイベント (Interval:50ms)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                var state = TrainCrewInput.GetTrainState();
                if (state == null) { return; }

                SuspendLayout();

                // 各Motorサウンドに対して値を設定
                if (!IsDeceleration)
                {
                    // 加速(音量)
                    if (PowerVolumeData.Count > 0)
                    {
                        var strPowerVolumeValues = GetInterpolatedValuesForSpeed(PowerVolumeData, state.Speed);
                        for (int col = 0; col < strPowerVolumeValues.Count - 1; col++)
                        {
                            int index = col;
                            if (!state.Lamps[PanelLamp.EmagencyBrake] && ((IsNotchLinked && state.Pnotch > 0 && state.Speed > 0.0f) || (!IsNotchLinked && state.Speed > 0.0f)))
                                sound.SetVolume("MOTOR", index, strPowerVolumeValues[col]);
                            else
                                sound.SetVolume("MOTOR", index, 0.0f);
                        }
                    }
                    // 加速(ピッチ)
                    if (PowerFrequencyData.Count > 0)
                    {
                        var strPowerFrequencyValues = GetInterpolatedValuesForSpeed(PowerFrequencyData, state.Speed);
                        for (int col = 0; col < strPowerFrequencyValues.Count - 1; col++)
                        {
                            int index = col;
                            sound.SetPitch("MOTOR", index, strPowerFrequencyValues[col]);
                        }
                    }
                }
                else
                {
                    // 減速(音量)
                    if (BrakeVolumeData.Count > 0)
                    {
                        var strBrakeVolumeValues = GetInterpolatedValuesForSpeed(BrakeVolumeData, state.Speed);
                        for (int col = 0; col < strBrakeVolumeValues.Count - 1; col++)
                        {
                            int index = col;
                            if (!state.Lamps[PanelLamp.EmagencyBrake] && (iRegenerationLimit <= state.Speed) && ((IsNotchLinked && state.Bnotch > 0 && state.Speed > 0.0f) || (!IsNotchLinked && state.Speed > 0.0f)))
                                sound.SetVolume("MOTOR", index, strBrakeVolumeValues[col]);
                            else
                                sound.SetVolume("MOTOR", index, 0.0f);
                        }
                    }
                    // 減速(ピッチ)
                    if (BrakeFrequencyData.Count > 0)
                    {
                        var strBrakeFrequencyValues = GetInterpolatedValuesForSpeed(BrakeFrequencyData, state.Speed);
                        for (int col = 0; col < strBrakeFrequencyValues.Count - 1; col++)
                        {
                            int index = col;
                            sound.SetPitch("MOTOR", index, strBrakeFrequencyValues[col]);
                        }
                    }
                }
                // Runサウンドに対して値を設定
                var runVolume = CustomMath.Lerp(0.0f, 0.0f, 90.0f, 1.0f, state.Speed);
                var runFrequency = CustomMath.Lerp(0.0f, 0.0f, 90.0f, 1.0f, state.Speed);
                if (runVolume > 1.0f) runVolume = 1.0f;
                if (0.0f > runVolume) runVolume = 0.0f;
                if (0.0f > runFrequency) runFrequency = 0.0f;
                if (sound.currentRunSoundIndex >= 0)
                {
                    sound.SetVolume("RUN", sound.currentRunSoundIndex, runVolume);
                    sound.SetPitch("RUN", sound.currentRunSoundIndex, runFrequency);
                }

                // Text更新
                sb.Clear();
                sb.AppendLine("車両ファイル読込：" + (sound.IsSoundFileLoaded ? "完了" : "未読込") + "");
                if (vehicleDirectoryName != "")
                    sb.AppendLine("読込フォルダ　　：" + (sound.IsSoundFileLoaded ? Path.GetFileName(vehicleDirectoryName.Substring(0, vehicleDirectoryName.Length - 1)) : "未読込") + "\n");
                else
                    sb.AppendLine("読込フォルダ　　：未読込\n");
                sb.AppendLine("現在速度　　　　：" + state.Speed.ToString("F2") + "km/h");
                sb.AppendLine("加速・減速判定　：" + (IsDeceleration ? "減速" : "加速"));
                Label_Parameters.Text = sb.ToString();

                ResumeLayout();
            }
            catch {}
        }

        /// <summary>
        /// Timer500_Tickイベント (Interval:500ms)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer500_Tick(object sender, EventArgs e)
        {
            try
            {
                var state = TrainCrewInput.GetTrainState();
                if (state == null) { return; }

                // 加速・減速判定
                if (fOldSpeed > Math.Abs(state.Speed)) IsDeceleration = true;
                if (Math.Abs(state.Speed) > fOldSpeed) IsDeceleration = false;
                fOldSpeed = Math.Abs(state.Speed);

            }
            catch {}
        }

        /// <summary>
        /// Button_OpenFile_Clickイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_OpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Vehicle Files (*.txt)|*.txt"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // ファイルデータ初期化
                    ClearAllFileData();

                    // Vehicleファイル読み込み
                    string filePath = openFileDialog.FileName;
                    vehicleDirectoryName = Path.GetDirectoryName(filePath) + "\\";
                    // カレンドディレクトリに設定
                    Environment.CurrentDirectory = Path.GetDirectoryName(filePath);
                    ReadVehicleFile(filePath);

                    // 音声ファイル読み込み
                    sound.LoadSoundFiles();

                    // 全音声を再生
                    sound.SoundAllPlay(0);

                    // コンボボックス更新
                    InitializeRunSoundComboBox();

                    // 各CSVデータ読込・整形処理
                    PowerVolumeData = ConvertCSVData(sound.PowerVolumePath);
                    PowerFrequencyData = ConvertCSVData(sound.PowerFrequencyPath);
                    BrakeVolumeData = ConvertCSVData(sound.BrakeVolumePath);
                    BrakeFrequencyData = ConvertCSVData(sound.BrakeFrequencyPath);
                }
                catch
                {
                    // ファイルデータ初期化
                    ClearAllFileData();
                }
            }
        }

        /// <summary>
        /// ComboBox_RunSound_SelectedIndexChangedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_RunSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_RunSound.SelectedIndex != -1)
            {
                int selectedRunSoundIndex = ComboBox_RunSound.SelectedIndex;

                // 現在再生中のrunSoundSourceを停止
                if (sound.currentRunSoundIndex != -1 && sound.currentRunSoundIndex != selectedRunSoundIndex)
                {
                    sound.SoundStop("RUN", sound.currentRunSoundIndex);
                }

                // 新しいrunSoundSourceを再生
                sound.SoundPlay("RUN", selectedRunSoundIndex);

                // 現在再生中のインデックスを更新
                sound.currentRunSoundIndex = selectedRunSoundIndex;
            }
        }

        /// <summary>
        /// TrackBar_MotorVolume_Scrollイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackBar_MotorVolume_Scroll(object sender, EventArgs e)
        {
            Label_MotorVolume.Text = "全体音量：" + TrackBar_MotorVolume.Value.ToString().PadLeft(3, ' ') + "%";
            sound.fMotorMasterVolume = (float)(TrackBar_MotorVolume.Value / 100f);
        }

        /// <summary>
        /// TrackBar_RunVolume_Scrollイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackBar_RunVolume_Scroll(object sender, EventArgs e)
        {
            Label_RunVolume.Text = "全体音量：" + TrackBar_RunVolume.Value.ToString().PadLeft(3, ' ') + "%";
            sound.fRunMasterVolume = (float)(TrackBar_RunVolume.Value / 100f);
        }

        /// <summary>
        /// CheckBox_TopMost_CheckedChangedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_TopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = CheckBox_TopMost.Checked;
        }

        /// <summary>
        /// CheckBox_NotchUnLinked_CheckedChangedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_NotchUnLinked_CheckedChanged(object sender, EventArgs e)
        {
            IsNotchLinked = CheckBox_NotchUnLinked.Checked;
        }

        /// <summary>
        /// ComboBox_RegenerationLimit_SelectedIndexChangedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_RegenerationLimit_SelectedIndexChanged(object sender, EventArgs e)
        {
            iRegenerationLimit = int.Parse(ComboBox_RegenerationLimit.SelectedItem.ToString().Trim());
        }

        /// <summary>
        /// MainForm_FormClosingイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            sound.IsSoundThreadLoop = false;
            sound.Dispose();
            TrainCrewInput.Dispose();
        }

        /// <summary>
        /// Timer初期化メソッド
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="tickEvent"></param>
        /// <returns></returns>
        private Timer InitializeTimer(int interval, EventHandler tickEvent)
        {
            var timer = new Timer
            {
                Interval = interval
            };
            timer.Tick += tickEvent;
            timer.Start();
            return timer;
        }

        /// <summary>
        /// コンボボックスにアイテムを追加するメソッド
        /// </summary>
        private void InitializeRunSoundComboBox()
        {
            // コンボボックスにアイテムを追加
            ComboBox_RunSound.Items.Clear();
            for (int i = 0; i < sound.runSoundSource.Count; i++)
            {
                // nullチェック
                if (sound.runSoundSource[i] != null)
                {
                    ComboBox_RunSound.Items.Add(Path.GetFileName(sound.runSoundData[i]));
                }
            }

            // コンボボックス初期選択
            if (ComboBox_RunSound.Items.Count > 0)
            {
                ComboBox_RunSound.SelectedIndex = 0;
            }
            else
            {
                ComboBox_RunSound.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// ファイルデータ初期化
        /// </summary>
        private void ClearAllFileData()
        {
            vehicleDirectoryName = "";
            soundDirectoryName = "";
            motorNoiseDirectoryName = "";
            soundPath = "";
            motorNoisePath = "";

            PowerVolumeData.Clear();
            PowerFrequencyData.Clear();
            BrakeVolumeData.Clear();
            BrakeFrequencyData.Clear();

            ComboBox_RunSound.Items.Clear();
            ComboBox_RunSound.SelectedIndex = -1;

            sound.ClearSoundData();
            sound.motorSoundData.Clear();
            sound.runSoundData.Clear();
        }

        /// <summary>
        /// Vehicleファイル読み込みメソッド
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadVehicleFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                // 1行目が "Bvets Vehicle" で始まっているか確認
                if (lines.Length == 0 || !lines[0].ToUpper().StartsWith("BVETS VEHICLE"))
                {
                    MessageBox.Show(Path.GetFileName(filePath) + "は\nBVE5の車両ファイルではありません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 正規表現でSoundやMotorNoiseのパスを抽出
                foreach (string line in lines)
                {
                    var soundMatch = Regex.Match(line, @"^Sound\s*=\s*(.+)$");
                    var motorNoiseMatch = Regex.Match(line, @"^MotorNoise\s*=\s*(.+)$");

                    if (soundMatch.Success)
                    {
                        string extractedPath = soundMatch.Groups[1].Value.Trim();
                        soundPath = ConvertToFullPathIfRelative(vehicleDirectoryName, extractedPath);
                        soundDirectoryName = Path.GetDirectoryName(soundPath) + "\\";
                    }
                    else if (motorNoiseMatch.Success)
                    {
                        string extractedPath = motorNoiseMatch.Groups[1].Value.Trim();
                        motorNoisePath = ConvertToFullPathIfRelative(vehicleDirectoryName, extractedPath);
                        motorNoiseDirectoryName = Path.GetDirectoryName(motorNoisePath) + "\\";
                    }
                }

                // Soundファイル読み込み
                if (!string.IsNullOrEmpty(soundPath))
                {
                    ReadSoundFile(soundPath);
                }

                // MotorNoiseファイル読み込み
                if (!string.IsNullOrEmpty(motorNoisePath))
                {
                    ReadMotorNoiseFile(motorNoisePath);
                }
            }
            catch
            {
                MessageBox.Show(Path.GetFileName(filePath) + "を読み込めませんでした。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw;
            }
        }

        /// <summary>
        /// 相対パスからフルパスに変換するメソッド
        /// </summary>
        /// <param name="baseDirectory"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string ConvertToFullPathIfRelative(string baseDirectory, string filePath)
        {
            if (!Path.IsPathRooted(filePath))
            {
                // 相対パスなのでフルパスに変換
                return Path.GetFullPath(Path.Combine(baseDirectory, filePath));
            }
            return filePath;  // すでにフルパスの場合はそのまま返す
        }

        /// <summary>
        /// Soundファイル読み込みメソッド
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadSoundFile(string filePath)
        {
            // ファイルのエンコーディング設定
            Encoding encoding = Encoding.UTF8;
            string firstLine = File.ReadLines(filePath).FirstOrDefault();

            if (firstLine != null && firstLine.StartsWith("Bvets Vehicle"))
            {
                string[] parts = firstLine.Split(':');
                if (parts.Length == 2)
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(parts[1].Trim());
                    }
                    catch (ArgumentException)
                    {
                        encoding = Encoding.UTF8;
                    }
                }
            }

            // ファイルの読み込み
            var motorData = new Dictionary<int, string>();
            var runData = new Dictionary<int, string>();
            string[] lines = File.ReadAllLines(filePath, encoding);

            bool isMotorSection = false;
            bool isRunSection = false;

            try
            {
                foreach (string rawLine in lines)
                {
                    // 行内の ";" 以降を無視
                    string line = rawLine.Split(';')[0].Trim();

                    // セクションの開始を確認
                    if (line == "[Motor]")
                    {
                        isMotorSection = true;
                        isRunSection = false;
                        continue;
                    }
                    else if (line == "[Run]")
                    {
                        isMotorSection = false;
                        isRunSection = true;
                        continue;
                    }
                    // 空白行または別セクションならスキップし、セクション終了を示す
                    else if (string.IsNullOrWhiteSpace(line) || (line.StartsWith("[") && line.EndsWith("]")))
                    {
                        isMotorSection = false;
                        isRunSection = false;
                        continue;
                    }

                    // "[Motor]"セクションのデータを辞書に追加
                    if (isMotorSection)
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int index))
                        {
                            string filePathValue = parts[1].Trim();
                            motorData.Add(index, soundDirectoryName + filePathValue);
                        }
                    }

                    // "[Run]"セクションのデータを辞書に追加
                    if (isRunSection)
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int index))
                        {
                            string filePathValue = parts[1].Trim();
                            runData.Add(index, soundDirectoryName + filePathValue);
                        }
                    }
                }

                // Dictionary初期化
                sound.motorSoundData.Clear();
                sound.runSoundData.Clear();

                // Dictionaryに保存
                sound.motorSoundData = motorData;
                sound.runSoundData = runData;
            }
            catch
            {
                MessageBox.Show("Soundファイルの読み込みに失敗しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw;
            }
        }

        /// <summary>
        /// MotorNoiseファイル読み込みメソッド
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadMotorNoiseFile(string filePath)
        {
            // ファイルのエンコーディング設定
            Encoding encoding = Encoding.UTF8;

            // 変数初期化
            sound.PowerVolumePath = null;
            sound.PowerFrequencyPath = null;
            sound.BrakeVolumePath = null;
            sound.BrakeFrequencyPath = null;

            // ファイルの読み込み
            string[] lines = File.ReadAllLines(filePath, encoding);

            try
            {
                foreach (string rawLine in lines)
                {
                    // 行内の ";" 以降を無視
                    string trimmedLine = rawLine.Split(';')[0].Trim();

                    // セクション名をスキップ
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        continue;
                    }

                    // 空白行を無視
                    if (string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        continue;
                    }

                    // キーと値を取得して適切な変数に格納
                    var parts = trimmedLine.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = motorNoiseDirectoryName + parts[1].Trim();

                        switch (key)
                        {
                            case "Volume" when sound.PowerVolumePath == null:
                                sound.PowerVolumePath = value;
                                break;
                            case "Frequency" when sound.PowerFrequencyPath == null:
                                sound.PowerFrequencyPath = value;
                                break;
                            case "Volume" when sound.PowerVolumePath != null:
                                sound.BrakeVolumePath = value;
                                break;
                            case "Frequency" when sound.PowerFrequencyPath != null:
                                sound.BrakeFrequencyPath = value;
                                break;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("MotorNoiseファイルの読み込みに失敗しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw;
            }
        }

        /// <summary>
        /// CSVデータ整形メソッド
        /// </summary>
        /// <param name="filePath"></param>
        private List<List<float?>> ConvertCSVData(string filePath)
        {
            try
            {
                // CSVファイル読込
                var data = ReadCSVFile(filePath);

                // 各列の1行目が空白の場合、0を代入
                FillFirstRowWithZero(data);

                // 線形補間によるCSVファイル整形処理
                InterpolateAllData(data);

                // 最終行に最大速度情報を追加
                AddRowWithMaxSpeed(data);

                return data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Path.GetFileName(filePath) + " の読み込みに失敗しました。\n\n" + ex.ToString(), "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw;
            }
        }

        /// <summary>
        /// CSVファイル読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<List<float?>> ReadCSVFile(string filePath)
        {
            var data = new List<List<float?>>();
            bool isFirstLine = true; // ヘッダー行の判定

            foreach (var line in File.ReadLines(filePath))
            {
                // ヘッダー行を無視
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                // コメント行を無視
                if (line.StartsWith("#"))
                {
                    continue;
                }

                var values = line.Split(',')
                                 .Select(v => float.TryParse(v, out var num) ? (float?)num : null)
                                 .ToList();
                data.Add(values);
            }
            return data;
        }

        /// <summary>
        /// 各列の1行目が空白の場合、0を代入
        /// </summary>
        /// <param name="data"></param>
        private void FillFirstRowWithZero(List<List<float?>> data)
        {
            for (int col = 1; col < data[0].Count; col++)
            {
                // 最初の行が空白なら0を代入
                if (!data[0][col].HasValue)
                {
                    data[0][col] = 0f;
                }
            }
        }

        /// <summary>
        /// 線形補間によるCSVファイル整形処理
        /// </summary>
        /// <param name="data"></param>
        private void InterpolateAllData(List<List<float?>> data)
        {
            int rowCount = data.Count;

            for (int col = 1; col < data[0].Count; col++)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    if (!data[i][col].HasValue && data[i][0].HasValue)
                    {
                        // 前後の有効な値を見つけるためのフラグと変数
                        bool foundLower = false, foundUpper = false;
                        float lowerSpeed = 0, upperSpeed = 0;
                        float? lowerValue = null, upperValue = null;

                        // 前方に最も近い有効な値を見つける
                        float? secondLowerValue = null;
                        float secondLowerSpeed = 0;
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (data[j][col].HasValue && data[j][0].HasValue)
                            {
                                if (!foundLower)
                                {
                                    lowerValue = data[j][col];
                                    lowerSpeed = data[j][0].Value;
                                    foundLower = true;
                                }
                                else if (secondLowerValue == null)
                                {
                                    secondLowerValue = data[j][col];
                                    secondLowerSpeed = data[j][0].Value;
                                    break;
                                }
                            }
                        }

                        // 後方に最も近い有効な値を見つける
                        for (int j = i + 1; j < rowCount; j++)
                        {
                            if (data[j][col].HasValue && data[j][0].HasValue)
                            {
                                upperValue = data[j][col];
                                upperSpeed = data[j][0].Value;
                                foundUpper = true;
                                break;
                            }
                        }

                        // 前後に有効な値が見つかった場合に線形補間を行う
                        if (foundLower && foundUpper && lowerValue.HasValue && upperValue.HasValue)
                        {
                            data[i][col] = CustomMath.Lerp(lowerSpeed, lowerValue.Value, upperSpeed, upperValue.Value, data[i][0].Value);
                        }
                        // 前方のみ有効な値が見つかり、2番目の前方の値も見つかった場合
                        else if (foundLower && secondLowerValue.HasValue)
                        {
                            // 値が 0 の場合、そのまま 0 を使う
                            if (lowerValue.Value == 0)
                            {
                                data[i][col] = lowerValue.Value;  // 値が 0 の場合はそのまま使用
                            }
                            else
                            {
                                // 2つの前方の値を使って予測して線形補間を行う
                                data[i][col] = CustomMath.Lerp(secondLowerSpeed, secondLowerValue.Value, lowerSpeed, lowerValue.Value, data[i][0].Value);
                            }
                        }
                        // 前方のみ有効な値が見つかった場合は、その値を使用
                        else if (foundLower && lowerValue.HasValue)
                        {
                            // 値が 0 の場合、そのまま 0 を使う
                            if (lowerValue.Value == 0)
                            {
                                data[i][col] = lowerValue.Value;  // 値が 0 の場合はそのまま使用
                            }
                            else
                            {
                                // 予測せずにその値を使う
                                data[i][col] = lowerValue.Value;
                            }
                        }
                        // 後方のみ有効な値が見つかった場合は後の値を使用
                        else if (foundUpper && upperValue.HasValue)
                        {
                            data[i][col] = upperValue.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 最終行に最大速度情報を追加
        /// </summary>
        /// <param name="data"></param>
        private void AddRowWithMaxSpeed(List<List<float?>> data)
        {
            int lastRowIndex = data.Count - 1;
            var newRow = new List<float?> { 500f };
            newRow.AddRange(new float?[data[0].Count - 1]);
            data.Add(newRow);

            // 2列目以降の処理
            for (int col = 1; col < data[0].Count; col++)
            {
                float? lastValue = data[lastRowIndex][col];
                if (lastValue == 0) // 直前が0なら0を設定
                {
                    data[data.Count - 1][col] = 0;
                }
                else if (lastValue.HasValue) // それ以外なら線形補間
                {
                    int secondLastIndex = Math.Max(0, lastRowIndex - 1); // 2つ前の行まで遡る
                    float? secondLastValue = data[secondLastIndex][col];

                    if (secondLastValue.HasValue && secondLastIndex != lastRowIndex)
                    {
                        float velocity1 = data[secondLastIndex][0] ?? 0;
                        float velocity2 = data[lastRowIndex][0] ?? 0;
                        float velocity3 = 500;

                        float interpolatedValue = CustomMath.Lerp(velocity1, secondLastValue.Value, velocity2, lastValue.Value, velocity3);
                        data[data.Count - 1][col] = interpolatedValue;
                    }
                    else
                    {
                        data[data.Count - 1][col] = lastValue;
                    }
                }
            }
        }

        /// <summary>
        /// 指定された速度に対する線形補間値を計算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        private List<float> GetInterpolatedValuesForSpeed(List<List<float?>> data, float speed)
        {
            // 速度列(1列目)から対応する前後のインデックスを取得
            int lowerIndex = 0;
            int upperIndex = 0;

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i][0] >= speed)
                {
                    upperIndex = i;
                    lowerIndex = i == 0 ? 0 : i - 1;
                    break;
                }
            }

            // 境界の速度値を取得
            float lowerSpeed = data[lowerIndex][0] ?? 0;
            float upperSpeed = data[upperIndex][0] ?? 500; // 最終行までの範囲

            // 線形補間結果を格納するリスト
            var interpolatedValues = new List<float>();

            // 各音声ファイル列（2列目以降）に対して線形補間を計算
            for (int col = 1; col < data[0].Count; col++)
            {
                float? lowerValue = data[lowerIndex][col];
                float? upperValue = data[upperIndex][col];

                // 値がどちらもnullでない場合に線形補間を行う
                if (lowerValue.HasValue && upperValue.HasValue && lowerSpeed != upperSpeed)
                {
                    // 線形補間
                    float interpolatedValue = CustomMath.Lerp(lowerSpeed, lowerValue.Value, upperSpeed, upperValue.Value, speed);
                    interpolatedValues.Add(interpolatedValue);
                }
                else if (lowerValue.HasValue)
                {
                    interpolatedValues.Add(lowerValue.Value); // 下限値のみ存在する場合はそのまま代入
                }
                else if (upperValue.HasValue)
                {
                    interpolatedValues.Add(upperValue.Value); // 上限値のみ存在する場合はそのまま代入
                }
                else
                {
                    interpolatedValues.Add(0); // どちらもnullの場合は0を代入
                }
            }

            return interpolatedValues;
        }
    }
}