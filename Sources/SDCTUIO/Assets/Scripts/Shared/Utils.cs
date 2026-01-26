public static class Utils
{
    public static UnityEngine.Vector3 ToUnityVector3(this System.Numerics.Vector3 vec)
    {
        return new UnityEngine.Vector3(
            vec.X,
            vec.Y,
            vec.Z
        );
    }

    public static float Remap (this float from, float fromMin, float fromMax, float toMin,  float toMax)
    {
        var fromAbs  =  from - fromMin;
        var fromMaxAbs = fromMax - fromMin;       
       
        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;
       
        return to;
    }

    public static string AddTleChecksum(this string str)
    {
        int sum = 0;
        foreach (char c in str)
        {
            if (char.IsDigit(c))
            {
                sum += (int)char.GetNumericValue(c);
            }

            if (c == '-')
            {
                sum++;
            }
        }
        
        str += (sum % 10).ToString();
        return str;
    }
}
