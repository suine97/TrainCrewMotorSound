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
        /// <summary>加速</summary>
        Accelerating,
        /// <summary>惰行</summary>
        Coasting,
        /// <summary>減速</summary>
        Braking
    }

    /// <summary>
    /// 自動走行モードの速度計算クラス
    /// (加速 → 最高速度で惰行 → 下限速度を下回ると再加速 を繰り返し、終了時刻にちょうど0km/hとなるように減速する)
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

        private double acceleration;
        private double deceleration;
        private double maxSpeed;
        private double lowerSpeed;
        private DateTime? endTime;
        private DateTime lastUpdateTime;
        private double coastElapsed;

        /// <summary>現在速度 [km/h]</summary>
        public double Speed { get; private set; }

        /// <summary>現在の状態</summary>
        public AutoRunPhase Phase { get; private set; } = AutoRunPhase.Stopped;

        /// <summary>停止要求(停止ボタン)を受けているか</summary>
        public bool IsStopRequested { get; private set; }

        /// <summary>自動走行中か</summary>
        public bool IsRunning => Phase != AutoRunPhase.Stopped;

        /// <summary>加速中か</summary>
        public bool IsAccelerating => Phase == AutoRunPhase.Accelerating;

        /// <summary>減速中か</summary>
        public bool IsBraking => Phase == AutoRunPhase.Braking;

        /// <summary>
        /// 自動走行を開始する
        /// </summary>
        /// <param name="now">現在日時</param>
        /// <param name="acceleration">加速度 [km/h/s]</param>
        /// <param name="deceleration">減速度 [km/h/s]</param>
        /// <param name="maxSpeed">最高速度 [km/h]</param>
        /// <param name="lowerSpeed">下限速度 [km/h]</param>
        /// <param name="endTime">終了日時 (nullの場合は停止要求があるまで走行を続ける)</param>
        public void Start(DateTime now, double acceleration, double deceleration, double maxSpeed, double lowerSpeed, DateTime? endTime)
        {
            if (acceleration <= 0) throw new ArgumentOutOfRangeException(nameof(acceleration));
            if (deceleration <= 0) throw new ArgumentOutOfRangeException(nameof(deceleration));
            if (maxSpeed <= 0) throw new ArgumentOutOfRangeException(nameof(maxSpeed));
            if (lowerSpeed < 0 || lowerSpeed >= maxSpeed) throw new ArgumentOutOfRangeException(nameof(lowerSpeed));

            this.acceleration = acceleration;
            this.deceleration = deceleration;
            this.maxSpeed = maxSpeed;
            this.lowerSpeed = lowerSpeed;
            this.endTime = endTime;

            lastUpdateTime = now;
            coastElapsed = 0.0;
            Speed = 0.0;
            IsStopRequested = false;
            Phase = AutoRunPhase.Accelerating;
        }

        /// <summary>
        /// 停止要求 (加速中は惰行を挟み、指定の減速度で0km/hまで減速して終了する)
        /// </summary>
        public void RequestStop()
        {
            if (IsRunning) IsStopRequested = true;
        }

        /// <summary>
        /// 自動走行を即時に打ち切る
        /// </summary>
        public void Abort()
        {
            Speed = 0.0;
            IsStopRequested = false;
            Phase = AutoRunPhase.Stopped;
        }

        /// <summary>
        /// 終了日時までの残り時間を取得する (終了日時の指定が無い場合はnull)
        /// </summary>
        /// <param name="now">現在日時</param>
        /// <returns></returns>
        public TimeSpan? GetRemainingTime(DateTime now)
        {
            if (!IsRunning || !endTime.HasValue) return null;
            TimeSpan remaining = endTime.Value - now;
            return remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
        }

        /// <summary>
        /// 現在日時まで速度計算を進める
        /// </summary>
        /// <param name="now">現在日時</param>
        public void Update(DateTime now)
        {
            if (!IsRunning) return;

            double elapsed = (now - lastUpdateTime).TotalSeconds;
            if (elapsed <= 0.0)
            {
                // 時計が戻された場合は基準時刻だけ合わせる
                lastUpdateTime = now;
                return;
            }

            // 終了日時を過ぎた分は計算しない
            double remaining = endTime.HasValue ? (endTime.Value - lastUpdateTime).TotalSeconds : double.PositiveInfinity;
            lastUpdateTime = now;

            // 処理が遅れた場合でも判定がずれないよう、細かい時間刻みに分けて計算する
            while (elapsed > 0.0 && IsRunning)
            {
                if (remaining <= 0.0)
                {
                    Abort();
                    break;
                }

                double dt = Math.Min(Math.Min(elapsed, MaxStepSeconds), remaining);
                Step(dt, remaining);
                elapsed -= dt;
                remaining -= dt;
            }

            // 終了日時に到達
            if (IsRunning && remaining <= 0.0)
            {
                Abort();
            }
        }

        /// <summary>
        /// 1ステップ分の速度計算
        /// </summary>
        /// <param name="dt">進める時間 [s]</param>
        /// <param name="remaining">ステップ開始時点での終了日時までの残り時間 [s] (指定なしの場合は無限大)</param>
        private void Step(double dt, double remaining)
        {
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

                    Speed += acceleration * dt;
                    if (Speed >= maxSpeed)
                    {
                        Speed = maxSpeed;
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
                            Phase = AutoRunPhase.Braking;
                        }
                        break;
                    }

                    // 下限速度を下回ったら再加速 (すぐに加速を中断することになる場合は惰行を続ける)
                    bool isBelowLower = Speed < lowerSpeed || Speed <= 0.0;
                    bool hasTimeToAccelerate = remaining - dt > MinAccelerateSeconds + CoastSecondsBeforeBrake + Speed / deceleration;
                    if (isBelowLower && hasTimeToAccelerate)
                    {
                        Phase = AutoRunPhase.Accelerating;
                    }
                    break;

                case AutoRunPhase.Braking:
                    // 終了日時にちょうど0km/hとなる減速度を毎回計算し直す
                    double brake = double.IsInfinity(remaining) ? deceleration : Speed / remaining;
                    if (IsStopRequested)
                    {
                        brake = Math.Max(brake, deceleration);
                    }

                    Speed -= brake * dt;
                    if (Speed <= 1e-6)
                    {
                        Abort();
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
            Phase = AutoRunPhase.Coasting;
        }
    }
}
