using UnityEngine;

public static class Utils
{
    static float[] carLanes = { -0.3f, 0.3f }; // Lanes for AI cars to spawn in, relative to the center of the road, you can add more lanes if needed

    public static float[] CarLanes => carLanes;

}
