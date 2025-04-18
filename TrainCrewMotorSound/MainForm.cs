using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrainCrew;
using static System.Windows.Forms.AxHost;

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
        private bool IsFadeIn = false;
        private bool IsDeceleration = false;
        private bool IsNotchLinked = true;
        private bool IsRegenerationOffAtEB = true;
        private bool wasSoundPlaying = false;
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

                float speed = 0.0f;
                bool isSMEECar = false;
                bool isSMEECarEB = false;
                bool isSMEEBrake = false;
                bool isReverserOff = false;
                bool isPower = false;
                bool isRegenerater = false;
                bool isBCPress = false;
                bool isPNotch = false;
                bool isBNotch = false;
                bool isMute = true;
                bool isAcc = false;
                bool isDec = false;

                //運転画面遷移なら処理
                if (TrainCrewInput.gameState.gameScreen == GameScreen.MainGame
                    || TrainCrewInput.gameState.gameScreen == GameScreen.MainGame_Pause
                    || TrainCrewInput.gameState.gameScreen == GameScreen.MainGame_Loading)
                {
                    if (state.CarStates.Count > 0)
                    {
                        speed = state.Speed;
                        isSMEECar = (state.CarStates[0].CarModel == "3000") || (state.CarStates[0].CarModel == "3020");
                        isSMEECarEB = (state.Bnotch == 9);
                        isReverserOff = state.Reverser == 0;
                        isPower = state.CarStates.Average(x => x.Ampare) > 0;
                        isRegenerater = state.CarStates.Average(x => x.Ampare) < 0;
                        isBCPress = state.CarStates.Average(x => x.BC_Press) > 1;
                        isSMEEBrake = state.CarStates.Average(x => x.Ampare) > 0 && isBCPress;
                        isPNotch = (state.Pnotch != 0 && speed > 0.0f);
                        isBNotch = (state.Bnotch != 0 && speed > 0.0f);
                        isMute = IsRegenerationOffAtEB
                            && ((isSMEECar && isSMEECarEB)
                            || (!isSMEECar && state.Lamps[PanelLamp.EmagencyBrake]));
                        isAcc = !isMute
                            && ((IsNotchLinked && !isReverserOff && isPNotch)
                            || (!IsNotchLinked && ((isSMEECar && !isSMEEBrake && isPower) || (!isSMEECar && isPower)) && speed > 0.0f));
                        isDec = !isMute
                            && (iRegenerationLimit <= speed) && ((IsNotchLinked && !isReverserOff && isBNotch)
                            || (!IsNotchLinked && (isRegenerater || isBCPress) && speed > 0.0f));
                    }
                }

                SuspendLayout();

                // 速度が0の場合、音声を停止
                if (speed <= 0.0f)
                {
                    if (wasSoundPlaying)
                    {
                        sound.SoundAllStop(sound.currentRunSoundIndex);
                        wasSoundPlaying = false;
                    }
                }
                // 速度が0以外で音声が停止状態の場合、1回だけ再生
                else
                {
                    if (!wasSoundPlaying)
                    {
                        sound.SoundAllPlay(sound.currentRunSoundIndex);
                        wasSoundPlaying = true;
                    }
                }

                // Motorサウンド音量フェード処理
                if (IsFadeIn && (isAcc || isDec))
                {
                    _ = FadeAsync(true, 0.2f);
                    IsFadeIn = false;
                }
                else if (!IsFadeIn && !isAcc && !isDec)
                {
                    _ = FadeAsync(false, 0.2f);
                    IsFadeIn = true;
                }

                // 各Motorサウンドに対して値を設定
                if (sound.IsMotorSoundFileLoaded)
                {
                    if (!IsDeceleration)
                    {
                        // 加速(音量)
                        if (PowerVolumeData.Count > 0)
                        {
                            var strPowerVolumeValues = GetInterpolatedValuesForSpeed(PowerVolumeData, speed);
                            for (int col = 0; col <= strPowerVolumeValues.Count - 1; col++)
                            {
                                int index = col;
                                sound.SetVolume("MOTOR", index, strPowerVolumeValues[col]);
                            }
                        }
                        // 加速(ピッチ)
                        if (PowerFrequencyData.Count > 0)
                        {
                            var strPowerFrequencyValues = GetInterpolatedValuesForSpeed(PowerFrequencyData, speed);
                            for (int col = 0; col <= strPowerFrequencyValues.Count - 1; col++)
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
                            var strBrakeVolumeValues = GetInterpolatedValuesForSpeed(BrakeVolumeData, speed);
                            for (int col = 0; col <= strBrakeVolumeValues.Count - 1; col++)
                            {
                                int index = col;
                                sound.SetVolume("MOTOR", index, strBrakeVolumeValues[col]);
                            }
                        }
                        // 減速(ピッチ)
                        if (BrakeFrequencyData.Count > 0)
                        {
                            var strBrakeFrequencyValues = GetInterpolatedValuesForSpeed(BrakeFrequencyData, speed);
                            for (int col = 0; col <= strBrakeFrequencyValues.Count - 1; col++)
                            {
                                int index = col;
                                sound.SetPitch("MOTOR", index, strBrakeFrequencyValues[col]);
                            }
                        }
                    }
                }

                // Runサウンドに対して値を設定
                if (sound.IsRunSoundFileLoaded)
                {
                    var runVolume = CustomMath.Lerp(0.0f, 0.0f, 90.0f, 1.0f, speed);
                    var runFrequency = CustomMath.Lerp(0.0f, 0.0f, 90.0f, 1.0f, speed);
                    if (runVolume > 1.0f) runVolume = 1.0f;
                    if (0.0f > runVolume) runVolume = 0.0f;
                    if (0.0f > runFrequency) runFrequency = 0.0f;
                    if (sound.currentRunSoundIndex >= 0)
                    {
                        sound.SetVolume("RUN", sound.currentRunSoundIndex, runVolume);
                        sound.SetPitch("RUN", sound.currentRunSoundIndex, runFrequency);
                    }
                }

                // Text更新
                sb.Clear();
                sb.AppendLine("モータ音読込　：" + (sound.IsMotorSoundFileLoaded ? "完了" : "未読込") + "");
                sb.AppendLine("走行音読込　　：" + (sound.IsRunSoundFileLoaded ? "完了" : "未読込") + "");
                if (vehicleDirectoryName != "")
                    sb.AppendLine("読込フォルダ　：" + ((sound.IsMotorSoundFileLoaded || sound.IsRunSoundFileLoaded) ? Path.GetFileName(vehicleDirectoryName.Substring(0, vehicleDirectoryName.Length - 1)) : "未読込") + "\n");
                else
                    sb.AppendLine("読込フォルダ　：未読込\n");
                sb.AppendLine("現在速度　　　：" + speed.ToString("F2") + "km/h");
                sb.AppendLine("加速・減速判定：" + (IsDeceleration ? "減速" : "加速"));
                Label_Parameters.Text = sb.ToString();

                ResumeLayout();
            }
            catch
            {
                // Text更新
                sb.Clear();
                sb.AppendLine("モータ音読込　：" + (sound.IsMotorSoundFileLoaded ? "完了" : "未読込") + "");
                sb.AppendLine("走行音読込　　：" + (sound.IsRunSoundFileLoaded ? "完了" : "未読込") + "");
                if (vehicleDirectoryName != "")
                    sb.AppendLine("読込フォルダ　：" + ((sound.IsMotorSoundFileLoaded || sound.IsRunSoundFileLoaded) ? Path.GetFileName(vehicleDirectoryName.Substring(0, vehicleDirectoryName.Length - 1)) : "未読込") + "\n");
                else
                    sb.AppendLine("読込フォルダ　：未読込\n");
                sb.AppendLine("現在速度　　　：" + "0.00km/h");
                sb.AppendLine("加速・減速判定：" + (IsDeceleration ? "減速" : "加速"));
                Label_Parameters.Text = sb.ToString();
            }
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

                float speed = 0.0f;

                //運転画面遷移なら処理
                if (TrainCrewInput.gameState.gameScreen == GameScreen.MainGame
                    || TrainCrewInput.gameState.gameScreen == GameScreen.MainGame_Pause
                    || TrainCrewInput.gameState.gameScreen == GameScreen.MainGame_Loading)
                {
                    speed = state.Speed;
                }

                // 加速・減速判定
                if (IsNotchLinked)
                {
                    if (state.Bnotch > 0) IsDeceleration = true;
                    else if (state.Pnotch > 0) IsDeceleration = false;
                }
                else
                {
                    if (fOldSpeed > Math.Abs(speed)) IsDeceleration = true;
                    else if (Math.Abs(speed) > fOldSpeed) IsDeceleration = false;
                }
                fOldSpeed = Math.Abs(speed);
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
                    if (sound.IsMotorSoundFileLoaded)
                    {
                        PowerVolumeData = ConvertCSVData(sound.PowerVolumePath);
                        PowerFrequencyData = ConvertCSVData(sound.PowerFrequencyPath);
                        BrakeVolumeData = ConvertCSVData(sound.BrakeVolumePath);
                        BrakeFrequencyData = ConvertCSVData(sound.BrakeFrequencyPath);

                        //// CSVファイル書き出し
                        //WriteCsv("PowerVolumeData.csv", PowerVolumeData);
                        //WriteCsv("PowerFrequencyData.csv", PowerFrequencyData);
                        //WriteCsv("BrakeVolumeData.csv", BrakeVolumeData);
                        //WriteCsv("BrakeFrequencyData.csv", BrakeFrequencyData);
                    }
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
                sound.SetVolume("RUN", selectedRunSoundIndex, 0.0f);
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
        /// CheckBox_EB_CheckedChangedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_EB_CheckedChanged(object sender, EventArgs e)
        {
            IsRegenerationOffAtEB = CheckBox_EB.Checked;
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
                // ファイルのエンコーディング設定
                Encoding encoding = Encoding.UTF8;
                string firstLine = File.ReadLines(filePath).FirstOrDefault();

                // "BVETS VEHICLE" の行が存在し、エンコーディング情報が含まれているか確認
                if (firstLine != null && firstLine.ToUpper().StartsWith("BVETS VEHICLE"))
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

                // エンコーディングを使ってファイルを読み込む
                string[] lines = File.ReadAllLines(filePath, encoding);
                string[] firstLineParts = lines[0]
                        .Replace(":", " ")
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // "Bvets Vehicle バージョン番号" の形式になっているか確認
                if (lines.Length == 0 || !lines[0].ToUpper().StartsWith("BVETS VEHICLE") || firstLineParts.Length < 3 || !double.TryParse(firstLineParts[2], out _))
                {
                    MessageBox.Show(Path.GetFileName(filePath) + "は\nBVE5の車両ファイルではありません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    throw new Exception();
                }

                // 正規表現でSoundやMotorNoiseのパスを抽出
                foreach (string line in lines)
                {
                    var soundMatch = Regex.Match(line, @"^Sound\s*=\s*(.+)$", RegexOptions.IgnoreCase);
                    var motorNoiseMatch = Regex.Match(line, @"^MotorNoise\s*=\s*(.+)$", RegexOptions.IgnoreCase);

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

                bool isSoundFileLoaded = false;
                bool isMotorNoiseFileLoaded = false;

                // Soundファイル読み込み
                if (!string.IsNullOrEmpty(soundPath))
                {
                    isSoundFileLoaded = ReadSoundFile(soundPath);
                }

                // MotorNoiseファイル読み込み
                if (!string.IsNullOrEmpty(motorNoisePath))
                {
                    isMotorNoiseFileLoaded = ReadMotorNoiseFile(motorNoisePath);
                }

                // ファイル読込判定
                if (isSoundFileLoaded && isMotorNoiseFileLoaded)
                {
                    MessageBox.Show("車両ファイルを正常に読み込みました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (isSoundFileLoaded)
                {
                    MessageBox.Show("音声ファイルの読み込みには成功しましたが、MotorNoiseファイルが見つかりませんでした。\n\n走行音のみ再生可能です。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("車両ファイルの読み込みに失敗しました。\n\nVehicleファイルの記述がBVE5形式になっている事を確認してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
                MessageBox.Show("車両ファイルの読み込みに失敗しました。\n\nVehicleファイルの記述がBVE5形式になっている事を確認してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        private bool ReadSoundFile(string filePath)
        {
            // ファイルのエンコーディング設定
            Encoding encoding = Encoding.UTF8;
            string firstLine = File.ReadLines(filePath).FirstOrDefault();

            // "BVETS VEHICLE" の行が存在し、エンコーディング情報が含まれているか確認
            if (firstLine != null && firstLine.ToUpper().StartsWith("BVETS VEHICLE"))
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
                    // 別セクションならスキップし、セクション終了を示す
                    else if ((line.StartsWith("[") && line.EndsWith("]")))
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

                return true;
            }
            catch
            {
                MessageBox.Show("Soundファイルの読み込みに失敗しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
                throw;
            }
        }

        /// <summary>
        /// MotorNoiseファイル読み込みメソッド
        /// </summary>
        /// <param name="filePath"></param>
        private bool ReadMotorNoiseFile(string filePath)
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
                return true;
            }
            catch
            {
                MessageBox.Show("MotorNoiseファイルの読み込みに失敗しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
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

                // 1列目がマイナス値の行を削除
                RemoveNegativeRows(data);

                // 1列目で昇順にソートする
                SortByFirstColumn(data);

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

                // 1列目が空白の場合は無視
                if (!values.Any() || values[0] == null)
                {
                    continue;
                }

                data.Add(values);
            }
            return data;
        }

        /// <summary>
        /// 1列目がマイナス値の行を削除する
        /// </summary>
        /// <param name="data"></param>
        private void RemoveNegativeRows(List<List<float?>> data)
        {
            data.RemoveAll(row => row[0].HasValue && row[0].Value < 0);
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
                            if (Math.Abs(upperSpeed - lowerSpeed) < float.Epsilon)
                            {
                                // 速度が同じ場合は、補間せずにその値を使う
                                data[i][col] = lowerValue.Value;
                            }
                            else
                            {
                                data[i][col] = CustomMath.Lerp(lowerSpeed, lowerValue.Value, upperSpeed, upperValue.Value, data[i][0].Value);
                            }
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

        /// <summary>
        /// List<List<float?>> 型のデータを1列目の値で昇順にソートするメソッド
        /// </summary>
        /// <param name="data">ソート対象のデータ</param>
        private void SortByFirstColumn(List<List<float?>> data)
        {
            data.Sort((row1, row2) =>
            {
                float? value1 = row1[0];
                float? value2 = row2[0];

                // nullの場合の処理（nullは最後に並ぶようにする）
                if (value1 == null && value2 == null) return 0;
                if (value1 == null) return 1;
                if (value2 == null) return -1;

                // 昇順で比較
                return value1.Value.CompareTo(value2.Value);
            });
        }

        /// <summary>
        /// フェードイン・フェードアウト演算メソッド
        /// </summary>
        /// <param name="fadeIn"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private async Task FadeAsync(bool fadeIn, float duration)
        {
            // 開始と終了の値を設定
            float startValue = fadeIn ? 0.0f : 1.0f;
            float endValue = fadeIn ? 1.0f : 0.0f;
            float currentValue = startValue;

            // 徐々に変化させるループ
            float stepTime = 0.01f; // ステップの間隔 (秒)
            int steps = (int)(duration / stepTime);

            for (int i = 0; i <= steps; i++)
            {
                // 現在の時間を計算 (0.0f ～ 1.0f)
                float currentTime = i * stepTime;

                // Lerpを用いて線形補間
                currentValue = CustomMath.Lerp(0.0f, startValue, duration, endValue, currentTime);
                ApplyFade(currentValue);

                // ステップ間隔だけ待機
                await Task.Delay(TimeSpan.FromSeconds(stepTime));
            }
        }
        /// <summary>
        /// フェード処理メソッド
        /// </summary>
        /// <param name="value"></param>
        private void ApplyFade(float value)
        {
            //Console.WriteLine($"Current fade value: {value}");
            sound.fFadeVolume = value;
        }

        /// <summary>
        /// CSVファイル書き出しメソッド(デバッグ用)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        static void WriteCsv(string filePath, List<List<float?>> data)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var row in data)
                {
                    // 各行のデータをカンマで区切り、nullは空文字列として扱う
                    var line = string.Join(",", row.Select(x => x.HasValue ? x.ToString() : ""));
                    writer.WriteLine(line);
                }
            }
        }
    }
}
