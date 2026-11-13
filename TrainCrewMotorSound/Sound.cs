using SharpDX.XAudio2;
using SharpDX.Multimedia;
using System;
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
        // ピッチ(再生速度の倍率)の上限・下限
        private const float MaxFrequencyRatio = 8.0f;
        private const float MinFrequencyRatio = 1.0f / 1024.0f;

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

        /// <summary>効果音(1回だけ再生する音声)の数</summary>
        public const int EffectSoundCount = 3;
        private readonly SourceVoice[] effectSoundSource = new SourceVoice[EffectSoundCount];
        private readonly AudioBuffer[] effectSoundBuffer = new AudioBuffer[EffectSoundCount];
        private readonly WaveFormat[] effectSoundFormat = new WaveFormat[EffectSoundCount];
        private readonly float[] effectSoundVolume = new float[] { 1.0f, 1.0f, 1.0f };

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
            LoadSoundGroup(motorSoundData, motorSoundSource, motorSoundBuffer, "Motor");
            // サウンド読み込み判定
            IsMotorSoundFileLoaded = (motorSoundSource.Count > 0);

            // Runサウンド読み込み
            LoadSoundGroup(runSoundData, runSoundSource, runSoundBuffer, "Run");
            // サウンド読み込み判定
            IsRunSoundFileLoaded = (runSoundSource.Count > 0);
        }

        /// <summary>
        /// 音声ファイル群を読み込み、番号に対応する位置へ格納するメソッド
        /// </summary>
        /// <param name="soundData">番号と音声ファイルパスの対応</param>
        /// <param name="sources">格納先のSourceVoiceリスト</param>
        /// <param name="buffers">格納先のAudioBufferリスト</param>
        /// <param name="typeName">エラー表示用の音声種類名</param>
        private void LoadSoundGroup(Dictionary<int, string> soundData, List<SourceVoice> sources, List<AudioBuffer> buffers, string typeName)
        {
            foreach (var entry in soundData)
            {
                int index = entry.Key;
                string filePath = entry.Value;

                if (index < 0) continue;

                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"{typeName}サウンドファイルが見つかりません:\n {filePath}", "エラー");
                    continue;
                }

                AudioBuffer buffer = null;
                SourceVoice sourceVoice = null;
                try
                {
                    // サウンドファイルを読み込む
                    using (var stream = new SoundStream(File.OpenRead(filePath)))
                    {
                        var waveFormat = stream.Format;
                        buffer = new AudioBuffer
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
                        sourceVoice = new SourceVoice(xAudio2, waveFormat, VoiceFlags.None, maxFrequencyRatio: MaxFrequencyRatio);
                    }
                }
                catch (Exception ex)
                {
                    // 読み込めない音声ファイルはスキップして続行する
                    buffer?.Stream?.Dispose();
                    MessageBox.Show($"{typeName}サウンドファイルを読み込めませんでした:\n {filePath}\n\n{ex.Message}", "エラー");
                    continue;
                }

                // リストのサイズをsoundFileDataのキーに合わせる
                while (sources.Count <= index)
                {
                    sources.Add(null);
                }
                while (buffers.Count <= index)
                {
                    buffers.Add(null);
                }

                // 指定されたインデックスにSourceVoiceとAudioBufferを代入
                sources[index] = sourceVoice;
                buffers[index] = buffer;
            }
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

            // 音声データのメモリを解放 (SourceVoiceの解放後に行う)
            foreach (var buffer in motorSoundBuffer)
            {
                buffer?.Stream?.Dispose();
            }
            foreach (var buffer in runSoundBuffer)
            {
                buffer?.Stream?.Dispose();
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
                if (runIndex >= 0 && runIndex < runSoundSource.Count && runSoundSource[runIndex] != null)
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
                if (float.IsNaN(volume) || volume < 0) volume = 0;
                motorSoundSource[index].SetVolume(fMotorMasterVolume * fFadeVolume * volume);
            }
            else if (type.ToUpper() == "RUN")
            {
                if (index < 0 || index >= runSoundSource.Count || runSoundSource[index] == null) return;
                if (float.IsNaN(volume) || volume < 0) volume = 0;
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
                motorSoundSource[index].SetFrequencyRatio(ClampPitch(pitch));
            }
            else if (type.ToUpper() == "RUN")
            {
                if (index < 0 || index >= runSoundSource.Count || runSoundSource[index] == null) return;
                runSoundSource[index].SetFrequencyRatio(ClampPitch(pitch));
            }
        }

        /// <summary>
        /// ピッチを設定可能な範囲に収めるメソッド
        /// </summary>
        /// <param name="pitch"></param>
        /// <returns></returns>
        private static float ClampPitch(float pitch)
        {
            if (float.IsNaN(pitch) || pitch < MinFrequencyRatio) return MinFrequencyRatio;
            if (pitch > MaxFrequencyRatio) return MaxFrequencyRatio;
            return pitch;
        }

        /// <summary>
        /// 効果音ファイル読み込みメソッド (読み込めない場合は例外を投げる)
        /// </summary>
        /// <param name="index">効果音のインデックス</param>
        /// <param name="filePath">音声ファイルのパス</param>
        public void LoadEffectSound(int index, string filePath)
        {
            if (index < 0 || index >= EffectSoundCount) return;

            AudioBuffer buffer;
            WaveFormat waveFormat;
            using (var stream = new SoundStream(File.OpenRead(filePath)))
            {
                waveFormat = stream.Format;
                buffer = new AudioBuffer
                {
                    Stream = stream.ToDataStream(),
                    AudioBytes = (int)stream.Length,
                    LoopCount = 0,
                    Flags = BufferFlags.EndOfStream
                };
            }

            // 読み込みに成功してから既存の効果音と入れ替える
            UnloadEffectSound(index);
            effectSoundBuffer[index] = buffer;
            effectSoundFormat[index] = waveFormat;
        }

        /// <summary>
        /// 効果音の解除メソッド
        /// </summary>
        /// <param name="index">効果音のインデックス</param>
        public void UnloadEffectSound(int index)
        {
            if (index < 0 || index >= EffectSoundCount) return;

            StopEffectSound(index);
            effectSoundBuffer[index]?.Stream?.Dispose();
            effectSoundBuffer[index] = null;
            effectSoundFormat[index] = null;
        }

        /// <summary>
        /// 効果音が読み込まれているか
        /// </summary>
        /// <param name="index">効果音のインデックス</param>
        /// <returns></returns>
        public bool IsEffectSoundLoaded(int index)
        {
            return index >= 0 && index < EffectSoundCount && effectSoundBuffer[index] != null;
        }

        /// <summary>
        /// 効果音を先頭から1回だけ再生するメソッド
        /// </summary>
        /// <param name="index">効果音のインデックス</param>
        public void PlayEffectSound(int index)
        {
            if (!IsEffectSoundLoaded(index)) return;

            // 再生中の場合は止めて、先頭から再生し直す
            StopEffectSound(index);

            var sourceVoice = new SourceVoice(xAudio2, effectSoundFormat[index], VoiceFlags.None, maxFrequencyRatio: 1.0f);
            sourceVoice.SubmitSourceBuffer(effectSoundBuffer[index], null);
            sourceVoice.SetVolume(effectSoundVolume[index]);
            sourceVoice.Start();
            effectSoundSource[index] = sourceVoice;
        }

        /// <summary>
        /// 効果音停止メソッド
        /// </summary>
        /// <param name="index">効果音のインデックス</param>
        public void StopEffectSound(int index)
        {
            if (index < 0 || index >= EffectSoundCount || effectSoundSource[index] == null) return;

            effectSoundSource[index].Stop();
            effectSoundSource[index].DestroyVoice();
            effectSoundSource[index] = null;
        }

        /// <summary>
        /// 全効果音停止メソッド
        /// </summary>
        public void StopAllEffectSounds()
        {
            for (int i = 0; i < EffectSoundCount; i++)
            {
                StopEffectSound(i);
            }
        }

        /// <summary>
        /// 効果音の音量設定メソッド (再生中の音声にもすぐ反映する)
        /// </summary>
        /// <param name="index">効果音のインデックス</param>
        /// <param name="volume">音量(0.0～1.0)</param>
        public void SetEffectVolume(int index, float volume)
        {
            if (index < 0 || index >= EffectSoundCount) return;
            if (float.IsNaN(volume) || volume < 0) volume = 0;

            effectSoundVolume[index] = volume;
            effectSoundSource[index]?.SetVolume(volume);
        }

        /// <summary>
        /// リソース解放
        /// </summary>
        public void Dispose()
        {
            // 既存の音声データをクリア
            ClearSoundData();
            for (int i = 0; i < EffectSoundCount; i++)
            {
                UnloadEffectSound(i);
            }

            // リソースを解放
            masteringVoice.Dispose();
            xAudio2.Dispose();
        }
    }
}
