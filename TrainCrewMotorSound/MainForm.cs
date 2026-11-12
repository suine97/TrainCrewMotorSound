using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
        // コンボボックスの選択位置 → [Run]セクションの番号 の対応表
        private readonly List<int> runSoundIndexMap = new List<int>();

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
                    var volumeData = IsDeceleration ? BrakeVolumeData : PowerVolumeData;
                    var frequencyData = IsDeceleration ? BrakeFrequencyData : PowerFrequencyData;
                    int motorCount = sound.motorSoundSource.Count;

                    // 音量 (テーブルが無い側・テーブルに列が無い音は音量0にする)
                    List<float> volumeValues = (volumeData != null && volumeData.Count > 0)
                        ? GetInterpolatedValuesForSpeed(volumeData, speed)
                        : null;
                    for (int index = 0; index < motorCount; index++)
                    {
                        float volume = (volumeValues != null && index < volumeValues.Count) ? volumeValues[index] : 0.0f;
                        sound.SetVolume("MOTOR", index, volume);
                    }

                    // ピッチ
                    if (frequencyData != null && frequencyData.Count > 0)
                    {
                        var frequencyValues = GetInterpolatedValuesForSpeed(frequencyData, speed);
                        for (int index = 0; index < frequencyValues.Count; index++)
                        {
                            sound.SetPitch("MOTOR", index, frequencyValues[index]);
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
                    ReadVehicleFile(filePath, out bool isSoundFileLoaded, out bool isMotorNoiseFileLoaded);

                    // 音声ファイル読み込み
                    sound.LoadSoundFiles();

                    // Motorサウンドを再生 (Runサウンドはコンボボックス選択時に再生する)
                    sound.SoundAllPlay(-1);

                    // コンボボックス更新
                    InitializeRunSoundComboBox();

                    // 各CSVデータ読込・整形処理
                    if (sound.IsMotorSoundFileLoaded)
                    {
                        PowerVolumeData = sound.PowerVolumePath != null ? ConvertCSVData(sound.PowerVolumePath) : null;
                        PowerFrequencyData = sound.PowerFrequencyPath != null ? ConvertCSVData(sound.PowerFrequencyPath) : null;
                        BrakeVolumeData = sound.BrakeVolumePath != null ? ConvertCSVData(sound.BrakeVolumePath) : null;
                        BrakeFrequencyData = sound.BrakeFrequencyPath != null ? ConvertCSVData(sound.BrakeFrequencyPath) : null;

                        //// CSVファイル書き出し
                        //WriteCsv("PowerVolumeData.csv", PowerVolumeData);
                        //WriteCsv("PowerFrequencyData.csv", PowerFrequencyData);
                        //WriteCsv("BrakeVolumeData.csv", BrakeVolumeData);
                        //WriteCsv("BrakeFrequencyData.csv", BrakeFrequencyData);
                    }

                    // ファイル読込判定 (全ての読み込みが終わってから表示する)
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
                        MessageBox.Show("車両ファイルの読み込みに失敗しました。\n\nVehicleファイルの記述がBVE5/BVE6形式になっている事を確認してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    // ファイルデータ初期化
                    ClearAllFileData();

                    MessageBox.Show("車両ファイルの読み込みに失敗しました。\n\n" + ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            int comboIndex = ComboBox_RunSound.SelectedIndex;
            if (comboIndex >= 0 && comboIndex < runSoundIndexMap.Count)
            {
                // コンボボックスの選択位置を[Run]セクションの番号に変換
                int selectedRunSoundIndex = runSoundIndexMap[comboIndex];

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
            runSoundIndexMap.Clear();
            for (int i = 0; i < sound.runSoundSource.Count; i++)
            {
                // nullチェック
                if (sound.runSoundSource[i] != null)
                {
                    ComboBox_RunSound.Items.Add(Path.GetFileName(sound.runSoundData[i]));
                    runSoundIndexMap.Add(i);
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

            PowerVolumeData?.Clear();
            PowerFrequencyData?.Clear();
            BrakeVolumeData?.Clear();
            BrakeFrequencyData?.Clear();

            ComboBox_RunSound.Items.Clear();
            ComboBox_RunSound.SelectedIndex = -1;
            runSoundIndexMap.Clear();

            sound.ClearSoundData();
            sound.currentRunSoundIndex = -1;
            sound.PowerVolumePath = null;
            sound.PowerFrequencyPath = null;
            sound.BrakeVolumePath = null;
            sound.BrakeFrequencyPath = null;
            sound.motorSoundData.Clear();
            sound.runSoundData.Clear();
        }

        /// <summary>
        /// BVEの設定ファイルを読み込むメソッド
        /// (1行目のヘッダーに ":文字コード名" があればその文字コードで、無ければUTF-8で読み込む)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string[] ReadBveTextFile(string filePath)
        {
            Encoding encoding = Encoding.UTF8;
            string firstLine = File.ReadLines(filePath).FirstOrDefault();

            // "BveTs ～" のヘッダー行が存在し、エンコーディング情報が含まれているか確認
            if (firstLine != null && firstLine.TrimStart().StartsWith("BVETS", StringComparison.OrdinalIgnoreCase))
            {
                int pos = firstLine.IndexOf(':');
                if (pos >= 0)
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(StripComment(firstLine.Substring(pos + 1)));
                    }
                    catch (ArgumentException)
                    {
                        encoding = Encoding.UTF8;
                    }
                }
            }

            return File.ReadAllLines(filePath, encoding);
        }

        /// <summary>
        /// コメントと前後の空白を取り除くメソッド
        /// (";" 以降、および行頭が "#" の行をコメントとして扱う)
        /// </summary>
        /// <param name="rawLine"></param>
        /// <returns></returns>
        private static string StripComment(string rawLine)
        {
            string line = rawLine.Split(';')[0].Trim();
            return line.StartsWith("#") ? "" : line;
        }

        /// <summary>
        /// "[セクション名]" の行からセクション名を取得するメソッド
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        private static bool TryParseSection(string line, out string sectionName)
        {
            sectionName = null;
            if (line.Length >= 2 && line.StartsWith("[") && line.EndsWith("]"))
            {
                sectionName = line.Substring(1, line.Length - 2).Trim();
                return true;
            }
            return false;
        }

        /// <summary>
        /// "キー = 値" の行からキーと値を取得するメソッド
        /// </summary>
        /// <param name="line"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryParseKeyValue(string line, out string key, out string value)
        {
            key = null;
            value = null;
            int pos = line.IndexOf('=');
            if (pos <= 0) return false;

            key = line.Substring(0, pos).Trim();
            value = line.Substring(pos + 1).Trim();
            return key.Length > 0;
        }

        /// <summary>
        /// 相対パスからフルパスに変換するメソッド
        /// (値が空、またはパスとして不正な場合はnullを返す)
        /// </summary>
        /// <param name="baseDirectory"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string ConvertToFullPathIfRelative(string baseDirectory, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return null;

            try
            {
                // 相対パスならフルパスに変換、すでにフルパスの場合はそのまま返す
                return Path.IsPathRooted(filePath) ? filePath : Path.GetFullPath(Path.Combine(baseDirectory, filePath));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                return null;
            }
        }

        /// <summary>
        /// Vehicleファイル読み込みメソッド
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isSoundFileLoaded">Soundファイルを読み込めたか</param>
        /// <param name="isMotorNoiseFileLoaded">MotorNoiseファイルを読み込めたか</param>
        private void ReadVehicleFile(string filePath, out bool isSoundFileLoaded, out bool isMotorNoiseFileLoaded)
        {
            isSoundFileLoaded = false;
            isMotorNoiseFileLoaded = false;

            // ヘッダーの文字コード指定に従ってファイルを読み込む
            string[] lines = ReadBveTextFile(filePath);

            // "Bvets Vehicle バージョン番号" の形式になっているか確認
            string[] firstLineParts = lines.Length == 0
                ? new string[0]
                : lines[0].Split(':')[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (firstLineParts.Length < 3
                || !firstLineParts[0].Equals("BVETS", StringComparison.OrdinalIgnoreCase)
                || !firstLineParts[1].Equals("VEHICLE", StringComparison.OrdinalIgnoreCase)
                || !double.TryParse(firstLineParts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                throw new InvalidDataException(Path.GetFileName(filePath) + " はBVE5以降の車両ファイルではありません。");
            }

            // SoundやMotorNoiseのパスを抽出
            foreach (string rawLine in lines)
            {
                if (!TryParseKeyValue(StripComment(rawLine), out string key, out string value)) continue;

                if (key.Equals("Sound", StringComparison.OrdinalIgnoreCase))
                {
                    string fullPath = ConvertToFullPathIfRelative(vehicleDirectoryName, value);
                    if (fullPath != null)
                    {
                        soundPath = fullPath;
                        soundDirectoryName = Path.GetDirectoryName(soundPath) + "\\";
                    }
                }
                else if (key.Equals("MotorNoise", StringComparison.OrdinalIgnoreCase))
                {
                    string fullPath = ConvertToFullPathIfRelative(vehicleDirectoryName, value);
                    if (fullPath != null)
                    {
                        motorNoisePath = fullPath;
                        motorNoiseDirectoryName = Path.GetDirectoryName(motorNoisePath) + "\\";
                    }
                }
            }

            // Soundファイル読み込み
            if (!string.IsNullOrEmpty(soundPath))
            {
                if (!File.Exists(soundPath))
                {
                    throw new FileNotFoundException("Soundファイルが見つかりません:\n" + soundPath);
                }
                isSoundFileLoaded = ReadSoundFile(soundPath);
            }

            // MotorNoiseファイル読み込み (ファイルが無い場合は走行音のみ読み込む)
            if (!string.IsNullOrEmpty(motorNoisePath) && File.Exists(motorNoisePath))
            {
                isMotorNoiseFileLoaded = ReadMotorNoiseFile(motorNoisePath);
            }
        }

        /// <summary>
        /// Soundファイル読み込みメソッド
        /// </summary>
        /// <param name="filePath"></param>
        private bool ReadSoundFile(string filePath)
        {
            try
            {
                // ファイルの読み込み
                var motorData = new Dictionary<int, string>();
                var runData = new Dictionary<int, string>();
                string[] lines = ReadBveTextFile(filePath);

                Dictionary<int, string> currentSection = null;

                foreach (string rawLine in lines)
                {
                    string line = StripComment(rawLine);

                    // セクションの開始を確認 (大文字小文字は区別しない)
                    if (TryParseSection(line, out string sectionName))
                    {
                        if (sectionName.Equals("Motor", StringComparison.OrdinalIgnoreCase)) currentSection = motorData;
                        else if (sectionName.Equals("Run", StringComparison.OrdinalIgnoreCase)) currentSection = runData;
                        else currentSection = null; // 別セクション
                        continue;
                    }
                    if (currentSection == null) continue;

                    // "[Motor]" "[Run]" セクションのデータを辞書に追加 (同じ番号は後の行を優先)
                    if (TryParseKeyValue(line, out string key, out string value)
                        && int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index)
                        && index >= 0)
                    {
                        string fullPath = ConvertToFullPathIfRelative(soundDirectoryName, value);
                        if (fullPath != null)
                        {
                            currentSection[index] = fullPath;
                        }
                    }
                }

                // Dictionaryに保存
                sound.motorSoundData = motorData;
                sound.runSoundData = runData;

                return true;
            }
            catch
            {
                MessageBox.Show("Soundファイルの読み込みに失敗しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        /// <summary>
        /// MotorNoiseファイル読み込みメソッド
        /// </summary>
        /// <param name="filePath"></param>
        private bool ReadMotorNoiseFile(string filePath)
        {
            // 変数初期化
            sound.PowerVolumePath = null;
            sound.PowerFrequencyPath = null;
            sound.BrakeVolumePath = null;
            sound.BrakeFrequencyPath = null;

            try
            {
                // ファイルの読み込み
                string[] lines = ReadBveTextFile(filePath);

                // 現在のセクション (null:セクション無し, true:[Power], false:[Brake])
                bool? isPowerSection = null;
                bool isUnknownSection = false;

                foreach (string rawLine in lines)
                {
                    string line = StripComment(rawLine);

                    // セクションの開始を確認 (大文字小文字は区別しない)
                    if (TryParseSection(line, out string sectionName))
                    {
                        isUnknownSection = false;
                        if (sectionName.Equals("Power", StringComparison.OrdinalIgnoreCase)) isPowerSection = true;
                        else if (sectionName.Equals("Brake", StringComparison.OrdinalIgnoreCase)) isPowerSection = false;
                        else isUnknownSection = true;
                        continue;
                    }
                    if (isUnknownSection) continue;

                    // キーと値を取得
                    if (!TryParseKeyValue(line, out string key, out string value)) continue;

                    bool isVolume = key.Equals("Volume", StringComparison.OrdinalIgnoreCase);
                    bool isFrequency = key.Equals("Frequency", StringComparison.OrdinalIgnoreCase);
                    if (!isVolume && !isFrequency) continue;

                    string fullPath = ConvertToFullPathIfRelative(motorNoiseDirectoryName, value);
                    if (fullPath == null) continue;

                    // セクションが無いファイルは、1つ目を力行・2つ目を制動として扱う
                    bool isPower = isPowerSection
                        ?? (isVolume ? sound.PowerVolumePath == null : sound.PowerFrequencyPath == null);

                    if (isVolume)
                    {
                        if (isPower) sound.PowerVolumePath = fullPath;
                        else sound.BrakeVolumePath = fullPath;
                    }
                    else
                    {
                        if (isPower) sound.PowerFrequencyPath = fullPath;
                        else sound.BrakeFrequencyPath = fullPath;
                    }
                }
                return true;
            }
            catch
            {
                MessageBox.Show("MotorNoiseファイルの読み込みに失敗しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        /// <summary>
        /// CSVデータ整形メソッド
        /// </summary>
        /// <param name="filePath"></param>
        private List<List<float?>> ConvertCSVData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("モータ音テーブルファイルが見つかりません:\n" + filePath);
            }

            try
            {
                // CSVファイル読込
                var data = ReadCSVFile(filePath);

                // 1列目がマイナス値の行を削除
                RemoveNegativeRows(data);

                // データ行が無い場合はテーブル無しとして扱う
                if (data.Count == 0)
                {
                    return data;
                }

                // 全行の列数を最大列数に揃える
                NormalizeColumnCount(data);

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
                throw new InvalidDataException(Path.GetFileName(filePath) + " の読み込みに失敗しました。\n" + ex.Message, ex);
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

            foreach (var rawLine in File.ReadLines(filePath))
            {
                string line = rawLine.Trim();

                // ヘッダー行を無視
                if (line.StartsWith("BVETS", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // コメント行を無視
                if (line.StartsWith("#") || line.StartsWith(";"))
                {
                    continue;
                }

                // 数値はPCの地域設定に依存しない形式(小数点は".")で解釈する
                var values = line.Split(',')
                                 .Select(v => float.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var num) ? (float?)num : null)
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
        /// 全行の列数を最大列数に揃える (足りない列は空白として扱う)
        /// </summary>
        /// <param name="data"></param>
        private void NormalizeColumnCount(List<List<float?>> data)
        {
            int maxColumnCount = data.Max(row => row.Count);
            foreach (var row in data)
            {
                while (row.Count < maxColumnCount)
                {
                    row.Add(null);
                }
            }
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
                            // 値が 0 の場合、または速度が同じで予測できない場合は、そのままの値を使う
                            if (lowerValue.Value == 0 || secondLowerSpeed == lowerSpeed)
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
            // (速度が最終行を超える場合は最終行の値を使う)
            int lowerIndex = data.Count - 1;
            int upperIndex = data.Count - 1;

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
