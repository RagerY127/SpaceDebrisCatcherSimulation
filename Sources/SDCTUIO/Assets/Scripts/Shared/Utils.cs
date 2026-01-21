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
