using System;

namespace TrainCrewMotorSound
{
    /// <summary>
    /// 自動走行モードの状態
    /// </summary>
    public enum AutoRunPhase
    {
        /// <summary>停止中(自動走行していない)</summary>
        Stopped,
        /// <summary>ドア閉 (発車前の待機)</summary>
        DoorClosing,
        /// <summary>ブレーキ緩解 (発車前の待機)</summary>
        BrakeReleasing,
        /// <summary>加速</summary>
        Accelerating,
        /// <summary>惰行</summary>
        Coasting,
        /// <summary>減速</summary>
        Braking,
        /// <summary>ドア開待機 (停車してからドア開までの待機)</summary>
        DoorOpenWaiting,
        /// <summary>ドア開 (停車後の待機)</summary>
        DoorOpening
    }

    /// <summary>
    /// 自動走行モードの設定値
    /// </summary>
    public class AutoRunSettings
    {
        /// <summary>加速度 [km/h/s]</summary>
        public double Acceleration = 2.5;
        /// <summary>減速度 [km/h/s]</summary>
        public double Deceleration = 3.0;
        /// <summary>最高速度 [km/h]</summary>
        public double MaxSpeed = 100.0;
        /// <summary>下限速度 [km/h]</summary>
        public double LowerSpeed = 80.0;
        /// <summary>ドア閉の待機時間 [s]</summary>
        public double DoorCloseSeconds = 0.0;
        /// <summary>ブレーキ緩解の待機時間 [s]</summary>
        public double BrakeReleaseSeconds = 0.0;
        /// <summary>停車してからドア開までの待機時間 [s]</summary>
        public double DoorOpenWaitSeconds = 0.0;
        /// <summary>ドア開の待機時間 [s]</summary>
        public double DoorOpenSeconds = 0.0;
        /// <summary>走行時間 [s] (加速開始から0km/hまでの時間。待機時間は含まない。指定しない場合はnull)</summary>
        public double? RunSeconds = null;
        /// <summary>終了日時 (0km/hになる日時。指定しない場合はnull)</summary>
        public DateTime? EndTime = null;
    }

    /// <summary>
    /// 自動走行モードの進行管理・速度計算クラス
    /// (ドア閉 → ブレーキ緩解 → 加速・惰行・減速 → ドア開待機 → ドア開 の順に進める。
    ///  走行中は 加速 → 最高速度で惰行 → 下限速度を下回ると再加速 を繰り返し、終了時刻にちょうど0km/hとなるように減速する)
    /// </summary>
    public class AutoRun
    {
        /// <summary>惰行中の走行抵抗 [km/h/s]</summary>
        public const double RunningResistance = 0.1;
        /// <summary>加速と減速の間に入れる惰行時間 [s]</summary>
        public const double CoastSecondsBeforeBrake = 1.0;
        /// <summary>再加速を始めるために最低限必要な加速時間 [s]</summary>
        private const double MinAccelerateSeconds = 1.0;
        /// <summary>1回の計算で進める最大時間 [s]</summary>
        private const double MaxStepSeconds = 0.05;

        private AutoRunSettings settings = new AutoRunSettings();
        private DateTime baseTime;      // 経過秒数の基準となる開始日時
        private double cursor;          // 計算済みの時刻 (baseTimeからの経過秒数)
        private double? endAt;          // 0km/hにする時刻 (baseTimeからの経過秒数)
        private double phaseElapsed;    // 待機段階に入ってからの経過秒数
        private double coastElapsed;    // 惰行を始めてからの経過秒数

        /// <summary>
        /// 段階が切り替わった時に発生するイベント (引数は新しい段階)
        /// </summary>
        public event Action<AutoRunPhase> PhaseChanged;

        /// <summary>現在速度 [km/h]</summary>
        public double Speed { get; private set; }

        /// <summary>現在の状態</summary>
        public AutoRunPhase Phase { get; private set; } = AutoRunPhase.Stopped;

        /// <summary>停止要求(停止ボタン)を受けているか</summary>
        public bool IsStopRequested { get; private set; }

        /// <summary>自動走行中か (発車前・停車後の待機を含む)</summary>
        public bool IsRunning => Phase != AutoRunPhase.Stopped;

        /// <summary>走行中(加速・惰行・減速)か</summary>
        public bool IsDriving => Phase == AutoRunPhase.Accelerating || Phase == AutoRunPhase.Coasting || Phase == AutoRunPhase.Braking;

        /// <summary>加速中か</summary>
        public bool IsAccelerating => Phase == AutoRunPhase.Accelerating;

        /// <summary>減速中か</summary>
        public bool IsBraking => Phase == AutoRunPhase.Braking;

        /// <summary>
        /// 自動走行を開始する
        /// </summary>
        /// <param name="now">現在日時</param>
        /// <param name="settings">設定値</param>
        public void Start(DateTime now, AutoRunSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (settings.Acceleration <= 0) throw new ArgumentOutOfRangeException(nameof(settings.Acceleration));
            if (settings.Deceleration <= 0) throw new ArgumentOutOfRangeException(nameof(settings.Deceleration));
            if (settings.MaxSpeed <= 0) throw new ArgumentOutOfRangeException(nameof(settings.MaxSpeed));
            if (settings.LowerSpeed < 0 || settings.LowerSpeed >= settings.MaxSpeed) throw new ArgumentOutOfRangeException(nameof(settings.LowerSpeed));

            this.settings = settings;
            baseTime = now;
            cursor = 0.0;
            // 終了日時指定の場合は開始時点で終了時刻が決まる (走行時間指定の場合は加速開始時に決める)
            endAt = settings.EndTime.HasValue ? (double?)(settings.EndTime.Value - now).TotalSeconds : null;
            coastElapsed = 0.0;
            Speed = 0.0;
            IsStopRequested = false;

            EnterPhase(AutoRunPhase.DoorClosing);
        }

        /// <summary>
        /// 停止要求
        /// (走行中は、加速中なら惰行を挟み、指定の減速度で0km/hまで減速してからドア開待機へ進む。待機中は即時に打ち切る)
        /// </summary>
        public void RequestStop()
        {
            if (IsDriving) IsStopRequested = true;
            else Abort();
        }

        /// <summary>
        /// 自動走行を即時に打ち切る
        /// </summary>
        public void Abort()
        {
            Speed = 0.0;
            IsStopRequested = false;
            if (Phase != AutoRunPhase.Stopped)
            {
                EnterPhase(AutoRunPhase.Stopped);
            }
        }

        /// <summary>
        /// 0km/hになるまでの残り時間を取得する (終了条件の指定が無い場合はnull)
        /// </summary>
        /// <param name="now">現在日時</param>
        /// <returns></returns>
        public TimeSpan? GetRemainingTime(DateTime now)
        {
            if (!IsRunning) return null;

            double seconds;
            if (Phase == AutoRunPhase.DoorOpenWaiting || Phase == AutoRunPhase.DoorOpening)
            {
                if (!settings.RunSeconds.HasValue && !settings.EndTime.HasValue) return null;
                seconds = 0.0;
            }
            else if (endAt.HasValue)
            {
                seconds = endAt.Value - (now - baseTime).TotalSeconds;
            }
            else if (settings.RunSeconds.HasValue)
            {
                // 発車前の待機中は走行時間をそのまま返す
                seconds = settings.RunSeconds.Value;
            }
            else
            {
                return null;
            }
            return TimeSpan.FromSeconds(Math.Max(0.0, seconds));
        }

        /// <summary>
        /// 現在日時まで計算を進める
        /// </summary>
        /// <param name="now">現在日時</param>
        public void Update(DateTime now)
        {
            if (!IsRunning) return;

            double target = (now - baseTime).TotalSeconds;

            // 処理が遅れた場合でも判定がずれないよう、細かい時間刻みに分けて計算する
            while (IsRunning && cursor < target)
            {
                double elapsed = target - cursor;

                if (IsDriving)
                {
                    double remaining = endAt.HasValue ? endAt.Value - cursor : double.PositiveInfinity;
                    if (remaining <= 0.0)
                    {
                        FinishDriving();
                        continue;
                    }

                    double dt = Math.Min(Math.Min(elapsed, MaxStepSeconds), remaining);
                    Step(dt, remaining);
                    cursor += dt;

                    // 終了時刻に到達
                    if (IsDriving && endAt.HasValue && endAt.Value - cursor <= 1e-9)
                    {
                        FinishDriving();
                    }
                }
                else
                {
                    // 待機段階：指定時間が経過したら次の段階へ進む
                    double left = GetWaitSeconds(Phase) - phaseElapsed;
                    if (elapsed < left)
                    {
                        phaseElapsed += elapsed;
                        cursor = target;
                    }
                    else
                    {
                        cursor += Math.Max(0.0, left);
                        FinishWaiting();
                    }
                }
            }
        }

        /// <summary>
        /// 待機段階の待機時間を取得する
        /// </summary>
        /// <param name="phase"></param>
        /// <returns></returns>
        private double GetWaitSeconds(AutoRunPhase phase)
        {
            switch (phase)
            {
                case AutoRunPhase.DoorClosing: return Math.Max(0.0, settings.DoorCloseSeconds);
                case AutoRunPhase.BrakeReleasing: return Math.Max(0.0, settings.BrakeReleaseSeconds);
                case AutoRunPhase.DoorOpenWaiting: return Math.Max(0.0, settings.DoorOpenWaitSeconds);
                case AutoRunPhase.DoorOpening: return Math.Max(0.0, settings.DoorOpenSeconds);
                default: return 0.0;
            }
        }

        /// <summary>
        /// 待機段階を終えて次の段階へ進む
        /// </summary>
        private void FinishWaiting()
        {
            switch (Phase)
            {
                case AutoRunPhase.DoorClosing:
                    EnterPhase(AutoRunPhase.BrakeReleasing);
                    break;

                case AutoRunPhase.BrakeReleasing:
                    // 走行時間指定の場合は、加速開始から数えて終了時刻を決める
                    if (settings.RunSeconds.HasValue)
                    {
                        endAt = cursor + settings.RunSeconds.Value;
                    }
                    Speed = 0.0;
                    coastElapsed = 0.0;
                    EnterPhase(AutoRunPhase.Accelerating);
                    break;

                case AutoRunPhase.DoorOpenWaiting:
                    EnterPhase(AutoRunPhase.DoorOpening);
                    break;

                default:
                    Abort();
                    break;
            }
        }

        /// <summary>
        /// 走行(加速・惰行・減速)を終えてドア開待機へ進む
        /// </summary>
        private void FinishDriving()
        {
            Speed = 0.0;
            IsStopRequested = false;
            EnterPhase(AutoRunPhase.DoorOpenWaiting);
        }

        /// <summary>
        /// 段階を切り替える
        /// </summary>
        /// <param name="phase"></param>
        private void EnterPhase(AutoRunPhase phase)
        {
            Phase = phase;
            phaseElapsed = 0.0;
            PhaseChanged?.Invoke(phase);
        }

        /// <summary>
        /// 走行中の1ステップ分の速度計算
        /// </summary>
        /// <param name="dt">進める時間 [s]</param>
        /// <param name="remaining">ステップ開始時点での終了時刻までの残り時間 [s] (指定なしの場合は無限大)</param>
        private void Step(double dt, double remaining)
        {
            double deceleration = settings.Deceleration;

            switch (Phase)
            {
                case AutoRunPhase.Accelerating:
                    // 惰行(1秒)とその間の速度低下を見込んで、減速が必要になる前に加速を中断する
                    double speedAfterCoast = Math.Max(0.0, Speed - RunningResistance * CoastSecondsBeforeBrake);
                    if (IsStopRequested || remaining <= CoastSecondsBeforeBrake + speedAfterCoast / deceleration)
                    {
                        StartCoasting();
                        break;
                    }

                    Speed += settings.Acceleration * dt;
                    if (Speed >= settings.MaxSpeed)
                    {
                        Speed = settings.MaxSpeed;
                        StartCoasting();
                    }
                    break;

                case AutoRunPhase.Coasting:
                    coastElapsed += dt;
                    Speed = Math.Max(0.0, Speed - RunningResistance * dt);

                    // 減速開始 (加速との間に必ず1秒の惰行を入れる)
                    bool isBrakeNeeded = IsStopRequested || remaining - dt <= Speed / deceleration;
                    if (isBrakeNeeded)
                    {
                        if (coastElapsed >= CoastSecondsBeforeBrake)
                        {
                            EnterPhase(AutoRunPhase.Braking);
                        }
                        break;
                    }

                    // 下限速度を下回ったら再加速 (すぐに加速を中断することになる場合は惰行を続ける)
                    bool isBelowLower = Speed < settings.LowerSpeed || Speed <= 0.0;
                    bool hasTimeToAccelerate = remaining - dt > MinAccelerateSeconds + CoastSecondsBeforeBrake + Speed / deceleration;
                    if (isBelowLower && hasTimeToAccelerate)
                    {
                        EnterPhase(AutoRunPhase.Accelerating);
                    }
                    break;

                case AutoRunPhase.Braking:
                    // 終了時刻にちょうど0km/hとなる減速度を毎回計算し直す
                    double brake = double.IsInfinity(remaining) ? deceleration : Speed / remaining;
                    if (IsStopRequested)
                    {
                        brake = Math.Max(brake, deceleration);
                    }

                    Speed -= brake * dt;
                    if (Speed <= 1e-6)
                    {
                        FinishDriving();
                    }
                    break;
            }
        }

        /// <summary>
        /// 惰行を開始する
        /// </summary>
        private void StartCoasting()
        {
            coastElapsed = 0.0;
            EnterPhase(AutoRunPhase.Coasting);
        }
    }
}
