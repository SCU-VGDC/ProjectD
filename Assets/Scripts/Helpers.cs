using System;
using UnityEngine;

public static class Helpers
{
    public static bool MatchesLayerMask(GameObject objToTest, LayerMask mask)
    {
        return (mask & (1 << objToTest.layer)) != 0;
    }
}
