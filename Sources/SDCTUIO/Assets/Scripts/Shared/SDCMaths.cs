namespace SDC
{
    public static class Maths
    {
        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float fromRange = fromMax - fromMin;
            float fromT = (value - fromMin) / fromRange;

            float toRange = toMax - toMin;
            float toT = toMin + fromT * toRange;

            return toT;
        }
    }
}
