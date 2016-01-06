using UnityEngine;


public class Utility {

	public static Vector3 UVector(XnaGeometry.Vector3 v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }

    public static XnaGeometry.Vector3 XVector(Vector3 v)
    {
        return new XnaGeometry.Vector3(v.x, v.y, v.z);
    }

    public static Quaternion UQuaternion(XnaGeometry.Quaternion q)
    {
        return new Quaternion(q.X, q.Y, q.Z, q.W);
    }

    public static XnaGeometry.Quaternion XQuaternion(Quaternion q)
    {
        return new XnaGeometry.Quaternion(q.x, q.y, q.z, q.w);
    }
}
