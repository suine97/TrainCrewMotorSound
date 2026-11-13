using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TrainCrewMotorSound
{
    /// <summary>
    /// 効果音(ドア閉・ブレーキ緩解・ドア開)の設定
    /// </summary>
    [DataContract]
    public class EffectSoundConfig
    {
        /// <summary>音声ファイルのパス (未選択の場合は空文字)</summary>
        [DataMember(Name = "file", Order = 0)]
        public string File { get; set; }

        /// <summary>音量 [%]</summary>
        [DataMember(Name = "volume", Order = 1, EmitDefaultValue = false)]
        public double? Volume { get; set; }

        /// <summary>再生時間 [s]</summary>
        [DataMember(Name = "seconds", Order = 2, EmitDefaultValue = false)]
        public double? Seconds { get; set; }
    }

    /// <summary>
    /// 画面の設定内容 (JSONファイルへの保存・読込用)
    /// 読み込んだJSONに無い項目はnullになる
    /// </summary>
    [DataContract]
    public class AppConfig
    {
        /// <summary>最前面表示</summary>
        [DataMember(Name = "topMost", Order = 0, EmitDefaultValue = false)]
        public bool? TopMost { get; set; }

        /// <summary>ノッチ連動</summary>
        [DataMember(Name = "notchLinked", Order = 1, EmitDefaultValue = false)]
        public bool? NotchLinked { get; set; }

        /// <summary>非常ブレーキ時回生オフ</summary>
        [DataMember(Name = "regenerationOffAtEB", Order = 2, EmitDefaultValue = false)]
        public bool? RegenerationOffAtEB { get; set; }

        /// <summary>回生失効速度 [km/h]</summary>
        [DataMember(Name = "regenerationLimit", Order = 3, EmitDefaultValue = false)]
        public double? RegenerationLimit { get; set; }

        /// <summary>モータ音の全体音量 [%]</summary>
        [DataMember(Name = "motorVolume", Order = 4, EmitDefaultValue = false)]
        public double? MotorVolume { get; set; }

        /// <summary>走行音の全体音量 [%]</summary>
        [DataMember(Name = "runVolume", Order = 5, EmitDefaultValue = false)]
        public double? RunVolume { get; set; }

        /// <summary>車両ファイルのパス</summary>
        [DataMember(Name = "vehicleFile", Order = 6, EmitDefaultValue = false)]
        public string VehicleFile { get; set; }

        /// <summary>走行音選択 (ファイル名)</summary>
        [DataMember(Name = "runSound", Order = 7, EmitDefaultValue = false)]
        public string RunSound { get; set; }

        /// <summary>加速度 [km/h/s]</summary>
        [DataMember(Name = "acceleration", Order = 8, EmitDefaultValue = false)]
        public double? Acceleration { get; set; }

        /// <summary>減速度 [km/h/s]</summary>
        [DataMember(Name = "deceleration", Order = 9, EmitDefaultValue = false)]
        public double? Deceleration { get; set; }

        /// <summary>最高速度 [km/h]</summary>
        [DataMember(Name = "maxSpeed", Order = 10, EmitDefaultValue = false)]
        public double? MaxSpeed { get; set; }

        /// <summary>下限速度 [km/h]</summary>
        [DataMember(Name = "lowerSpeed", Order = 11, EmitDefaultValue = false)]
        public double? LowerSpeed { get; set; }

        /// <summary>ドア閉音声</summary>
        [DataMember(Name = "doorClose", Order = 12, EmitDefaultValue = false)]
        public EffectSoundConfig DoorClose { get; set; }

        /// <summary>ブレーキ緩解音声</summary>
        [DataMember(Name = "brakeRelease", Order = 13, EmitDefaultValue = false)]
        public EffectSoundConfig BrakeRelease { get; set; }

        /// <summary>ドア開までの待機時間 [s]</summary>
        [DataMember(Name = "doorOpenWaitSeconds", Order = 14, EmitDefaultValue = false)]
        public double? DoorOpenWaitSeconds { get; set; }

        /// <summary>ドア開音声</summary>
        [DataMember(Name = "doorOpen", Order = 15, EmitDefaultValue = false)]
        public EffectSoundConfig DoorOpen { get; set; }

        /// <summary>終了条件 ("duration":走行時間, "endTime":終了日時, "none":指定なし)</summary>
        [DataMember(Name = "endMode", Order = 16, EmitDefaultValue = false)]
        public string EndMode { get; set; }

        /// <summary>走行時間 (分)</summary>
        [DataMember(Name = "runMinutes", Order = 17, EmitDefaultValue = false)]
        public double? RunMinutes { get; set; }

        /// <summary>走行時間 (秒)</summary>
        [DataMember(Name = "runSeconds", Order = 18, EmitDefaultValue = false)]
        public double? RunSeconds { get; set; }

        /// <summary>
        /// JSONファイルへ保存する (インデント付き・UTF-8)
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            var serializer = new DataContractJsonSerializer(typeof(AppConfig));
            using (var stream = File.Create(filePath))
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, new UTF8Encoding(false), false, true, "  "))
            {
                serializer.WriteObject(writer, this);
            }
        }

        /// <summary>
        /// JSONファイルから読み込む (読み込めない場合は例外を投げる)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static AppConfig Load(string filePath)
        {
            var serializer = new DataContractJsonSerializer(typeof(AppConfig));
            // BOM付きのUTF-8で保存されたファイルも読めるよう、文字列として読み込んでから解析する
            byte[] bytes = new UTF8Encoding(false).GetBytes(System.IO.File.ReadAllText(filePath));
            using (var stream = new MemoryStream(bytes))
            {
                var config = serializer.ReadObject(stream) as AppConfig;
                if (config == null)
                {
                    throw new InvalidDataException("設定ファイルの内容が空です。");
                }
                return config;
            }
        }
    }
}
