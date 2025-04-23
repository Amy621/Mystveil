using UnityEngine;
using System;

[Serializable]
public class SimpleSerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SimpleSerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public SimpleSerializableVector3()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
} 