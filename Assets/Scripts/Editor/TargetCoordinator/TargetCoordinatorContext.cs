using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TargetCoordinatorContext
{
    public static bool SuppressCoordinatorPanel { get; set; } = false;
    public static bool SuppressAreaConfig { get; set; } = false;

    public static void Reset()
    {
        SuppressCoordinatorPanel = false;
        SuppressAreaConfig = false;
    }
}
