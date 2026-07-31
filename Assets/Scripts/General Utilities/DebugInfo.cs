using UnityEngine;

public static class DebugInfo
{
    public static float GetDetailedCurrentFPS()
    {
        return 1f / Time.unscaledDeltaTime;
    }

    public static int GetCurrentFPS()
    {
        return Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
    }
}
