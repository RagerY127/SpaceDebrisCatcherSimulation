using UnityEngine;
using One_Sgp4;

public static class Utils
{
    public static Vector3 GetUnityPosition(Tle tle, EpochTime time, float scaleFactor)
    {
        Sgp4Data sgp4DebrisData = SatFunctions.getSatPositionAtTime(
            tle,
            time,
            Sgp4.wgsConstant.WGS_84
        );

        // ATTENTION: Unity utilise un systeme de coordonnees Y-up tandis que SGP4 utilise Z-up
        Point3d realPositionKm = sgp4DebrisData.getPositionData();
        return new Vector3(
            (float)realPositionKm.x,
            (float)realPositionKm.z,
            (float)realPositionKm.y
        ) * scaleFactor;
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
