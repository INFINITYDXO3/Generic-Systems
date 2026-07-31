using System;
using UnityEngine;

[Serializable]
public struct Aim
{
    public Vector3 OriginPoint;
    public Vector3 Direction;
    public readonly static Aim zero = new (Vector3.zero, Vector3.zero);

    public Aim(Vector3 OriginPoint, Vector3 Direction)
    {
        this.OriginPoint = OriginPoint;
        this.Direction = Direction;
    }

    public static bool operator == (Aim aim1, Aim aim2)
    {
        return aim1.Equals(aim2);
    }

    public static bool operator != (Aim aim1, Aim aim2)
    {
        return !aim1.Equals(aim2);
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is Aim other)
        {
            // Compare the Vector3 fields directly
            return this.OriginPoint == other.OriginPoint && 
                   this.Direction == other.Direction;
        }
        return false;
    }

    public override readonly int GetHashCode() => base.GetHashCode();
}
