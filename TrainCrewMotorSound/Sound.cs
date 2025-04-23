using SharpDX.XAudio2;
using SharpDX.Multimedia;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TrainCrewMotorSound
{
    /// <summary>
    /// Soundクラス
    /// </summary>
    public class Sound
    {
        private XAudio2 xAudio2;
        private MasteringVoice masteringVoice;
        public List<SourceVoice> motorSoundSource = new List<SourceVoice>();
        public List<AudioBuffer> motorSoundBuffer = new List<AudioBuffer>();
        public List<SourceVoice> runSoundSource = new List<SourceVoice>();
        public List<AudioBuffer> runSoundBuffer = new List<AudioBuffer>();
        public Dictionary<int, string> motorSoundData = new Dictionary<int, string>();
        public Dictionary<int, string> runSoundData = new Dictionary<int, string>();
        public int currentRunSoundIndex = -1;
        public bool IsMotorSoundFileLoaded = false;
        public bool IsRunSoundFileLoaded = false;
        public float fMotorMasterVolume = 1.0f;
        public float fRunMasterVolume = 1.0f;
        public float fFadeVolume = 1.0f;
        public string PowerVolumePath = null;
        public string PowerFrequencyPath = null;
        public string BrakeVolumePath = null;
        public string BrakeFrequencyPath = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Sound()
        {
            try
            {
                // XAudio2とMasteringVoiceを初期化
                xAudio2 = new XAudio2();
                masteringVoice = new MasteringVoice(xAudio2);
            }
            catch
            {
                MessageBox.Show("サウンドデバイスの生成に失敗しました。", "エラー");
            }
        }

        /// <summary>
        /// 音声ファイル読み込みメソッド
        /// </summary>
        public void LoadSoundFiles()
        {
            // 既存のSourceVoiceとAudioBufferを解放してクリア
            ClearSoundData();

            // Motorサウンド読み込み
            foreach (var entry in motorSoundData)
            {
                int index = entry.Key;
                string filePath = entry.Value;

                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Motorサウンドファイルが見つかりません: {filePath}", "エラー");
                    continue;
                }

                // サウンドファイルを読み込む
                using (var stream = new SoundStream(File.OpenRead(filePath)))
                {
                    var waveFormat = stream.Format;
                    var buffer = new AudioBuffer
                    {
                        Stream = stream.ToDataStream(),
                        AudioBytes = (int)stream.Length,
                        LoopCount = 255,
                        LoopBegin = 0,
                        LoopLength = 0,
                        PlayBegin = 0,
                        PlayLength = 0,
                        Flags = BufferFlags.EndOfStream
                    };

                    // SourceVoiceを作成
                    var sourceVoice = new SourceVoice(xAudio2, waveFormat, VoiceFlags.None, maxFrequencyRatio: 4.0f);

                    // リストのサイズをsoundFileDataのキーに合わせる
                    while (motorSoundSource.Count <= index)
                    {
                        motorSoundSource.Add(null);
                    }
                    while (motorSoundBuffer.Count <= index)
                    {
                        motorSoundBuffer.Add(null);
                    }

                    // 指定されたインデックスにSourceVoiceとAudioBufferを代入
                    motorSoundSource[index] = sourceVoice;
                    motorSoundBuffer[index] = buffer;
                }
            }
            // サウンド読み込み判定
            IsMotorSoundFileLoaded = (motorSoundSource.Count > 0);

            // Runサウンド読み込み
            foreach (var entry in runSoundData)
            {
                int index = entry.Key;
                string filePath = entry.Value;

                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Runサウンドファイルが見つかりません:\n {filePath}", "エラー");
                    continue;
                }

                // サウンドファイルを読み込む
                using (var stream = new SoundStream(File.OpenRead(filePath)))
                {
                    var waveFormat = stream.Format;
                    var buffer = new AudioBuffer
                    {
                        Stream = stream.ToDataStream(),
                        AudioBytes = (int)stream.Length,
                        LoopCount = 255,
                        LoopBegin = 0,
                        LoopLength = 0,
                        PlayBegin = 0,
                        PlayLength = 0,
                        Flags = BufferFlags.EndOfStream
                    };

                    // SourceVoiceを作成
                    var sourceVoice = new SourceVoice(xAudio2, waveFormat, VoiceFlags.None, maxFrequencyRatio: 4.0f);

                    // リストのサイズをsoundFileDataのキーに合わせる
                    while (runSoundSource.Count <= index)
                    {
                        runSoundSource.Add(null);
                    }
                    while (runSoundBuffer.Count <= index)
                    {
                        runSoundBuffer.Add(null);
                    }

                    // 指定されたインデックスにSourceVoiceとAudioBufferを代入
                    runSoundSource[index] = sourceVoice;
                    runSoundBuffer[index] = buffer;
                }
            }
            // サウンド読み込み判定
            IsRunSoundFileLoaded = (runSoundSource.Count > 0);
        }

        /// <summary>
        /// 既存の音声データをクリア
        /// </summary>
        public void ClearSoundData()
        {
            // すべてのSourceVoiceを停止して解放
            foreach (var voice in motorSoundSource)
            {
                if (voice != null)
                {
                    voice.Stop();
                    voice.DestroyVoice();
                }
            }
            foreach (var voice in runSoundSource)
            {
                if (voice != null)
                {
                    voice.Stop();
                    voice.DestroyVoice();
                }
            }
            motorSoundSource.Clear();
            motorSoundBuffer.Clear();
            runSoundSource.Clear();
            runSoundBuffer.Clear();
        }

        /// <summary>
        /// 全音声再生メソッド
        /// </summary>
        /// <param name="runIndex">Runサウンドインデックス</param>
        public void SoundAllPlay(int runIndex)
        {
            // Motorサウンドを再生
            for (int i = 0; i < motorSoundSource.Count; i++)
            {
                if (motorSoundSource[i] != null)
                {
                    // バッファをソースに渡して再生開始
                    motorSoundSource[i].SubmitSourceBuffer(motorSoundBuffer[i], null);
                    motorSoundSource[i].SetVolume(0.0f); // 音量を0に設定
                    motorSoundSource[i].Start();
                }
            }

            // Runサウンドを再生
            if (runSoundSource.Count > 0)
            {
                if (runSoundSource[runIndex] != null)
                {
                    // バッファをソースに渡して再生開始
                    runSoundSource[runIndex].SubmitSourceBuffer(runSoundBuffer[runIndex], null);
                    runSoundSource[runIndex].SetVolume(0.0f); // 音量を0に設定
                    runSoundSource[runIndex].Start();
                }
            }
        }

        /// <summary>
        /// 音声再生メソッド
        /// </summary>
        /// <param name="type">音声種類[Motor, Run]</param>
        /// <param name="index">再生する音声のインデックス</param>
        public void SoundPlay(string type, int index)
        {
            if (type.ToUpper() == "MOTOR")
            {
                if (index < 0 || index >= motorSoundSource.Count || motorSoundSource[index] == null) return;

                var sourceVoice = motorSoundSource[index];
                var buffer = motorSoundBuffer[index];

                // バッファをソースに渡して再生開始
                sourceVoice.SubmitSourceBuffer(buffer, null);
                sourceVoice.Start();
            }
            else if (type.ToUpper() == "RUN")
            {
                if (index < 0 || index >= runSoundSource.Count || runSoundSource[index] == null) return;

                var sourceVoice = runSoundSource[index];
                var buffer = runSoundBuffer[index];

                // バッファをソースに渡して再生開始
                sourceVoice.SubmitSourceBuffer(buffer, null);
                sourceVoice.Start();
            }
        }

        /// <summary>
        /// 全音声停止メソッド
        /// </summary>
        /// <param name="runIndex"></param>
        public void SoundAllStop(int runIndex)
        {
            // Motorサウンドを停止
            for (int i = 0; i < motorSoundSource.Count; i++)
            {
                if (motorSoundSource[i] != null)
                {
                    motorSoundSource[i].Stop();
                    motorSoundSource[i].FlushSourceBuffers();
                }
            }

            // Runサウンドを停止
            if (runSoundSource.Count > 0)
            {
                if (runIndex >= 0 && runIndex < runSoundSource.Count && runSoundSource[runIndex] != null)
                {
                    runSoundSource[runIndex].Stop();
                    runSoundSource[runIndex].FlushSourceBuffers();
                }
            }
        }

        /// <summary>
        /// 音声停止メソッド
        /// </summary>
        /// <param name="type">音声種類[Motor, Run]</param>
        /// <param name="index"></param>
        public void SoundStop(string type, int index)
        {
            if (type.ToUpper() == "MOTOR")
            {
                if (index < 0 || index >= motorSoundSource.Count || motorSoundSource[index] == null) return;
                motorSoundSource[index].Stop();
            }
            else if (type.ToUpper() == "RUN")
            {
                if (index < 0 || index >= runSoundSource.Count || runSoundSource[index] == null) return;
                runSoundSource[index].Stop();
            }
        }

        /// <summary>
        /// 音量設定メソッド
        /// </summary>
        /// <param name="index"></param>
        /// <param name="volume"></param>
        public void SetVolume(string type, int index, float volume)
        {
            if (type.ToUpper() == "MOTOR")
            {
                if (index < 0 || index >= motorSoundSource.Count || motorSoundSource[index] == null) return;
                if (volume < 0) volume = 0;
                motorSoundSource[index].SetVolume(fMotorMasterVolume * fFadeVolume * volume);
            }
            else if (type.ToUpper() == "RUN")
            {
                if (index < 0 || index >= runSoundSource.Count || runSoundSource[index] == null) return;
                if (volume < 0) volume = 0;
                runSoundSource[index].SetVolume(fRunMasterVolume * volume);
            }
        }

        /// <summary>
        /// ピッチ設定メソッド
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pitch"></param>
        public void SetPitch(string type, int index, float pitch)
        {
            if (type.ToUpper() == "MOTOR")
            {
                if (index < 0 || index >= motorSoundSource.Count || motorSoundSource[index] == null) return;
                motorSoundSource[index].SetFrequencyRatio(pitch);
            }
            else if (type.ToUpper() == "RUN")
            {
                if (index < 0 || index >= runSoundSource.Count || runSoundSource[index] == null) return;
                runSoundSource[index].SetFrequencyRatio(pitch);
            }
        }

        /// <summary>
        /// リソース解放
        /// </summary>
        public void Dispose()
        {
            // 既存の音声データをクリア
            ClearSoundData();

            // リソースを解放
            masteringVoice.Dispose();
            xAudio2.Dispose();
        }
    }
}
