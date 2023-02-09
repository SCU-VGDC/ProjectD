using UnityEngine;

public static class Vector_Extension
{
    public static Vector3 WithAxis(this Vector3 vector, Axis axis, float value)
    {
        return new Vector3(
            x: axis == Axis.X ? value : vector.x,
            y: axis == Axis.Y ? value : vector.y,
            z: axis == Axis.Z ? value : vector.z
        );
    }
}

public enum Axis
{
    X, Y, Z
}