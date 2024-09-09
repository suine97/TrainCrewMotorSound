namespace TrainCrewMotorSound
{
    /// <summary>
    /// CustomMathクラス
    /// </summary>
    public static class CustomMath
    {
        /// <summary>
        /// 2点間の線形補間計算メソッド
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Lerp(float x0, float y0, float x1, float y1, float x)
        {
            return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
        }
    }
}
