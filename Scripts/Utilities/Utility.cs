using System;
using System.Diagnostics;

namespace ByTheSword.Scripts.Utilities;

public static class Utility
{
    public static uint GetCollisionValueFromIndices(params uint[] layerIndexArr)
    {
        uint result = 0;
        foreach(uint layerIndex in layerIndexArr)
        {
            if (layerIndex == 1)
            {
                result += 1;
            }
            else
            {
                // Layer indexes start at 1.
                result += (uint)Math.Pow(2, layerIndex - 1);
            }
        }
        return result;
    }
}