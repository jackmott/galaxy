using UnityEngine;


public static class Utility {

	public static Vector3 UVector(this XnaGeometry.Vector3 v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }

	public static float FastDistance(XnaGeometry.Vector3 a, XnaGeometry.Vector3 b)
	{
		XnaGeometry.Vector3 r = b - a;
		return r.X * r.X + r.Y * r.Y + r.Z * r.Z;

	}

	public static float FastDistance(Vector3 a, Vector3 b)
	{
		Vector3 r = b - a;
		return r.x * r.x + r.y * r.y + r.z * r.z;

	}


	public static XnaGeometry.Vector3 XVector(this Vector3 v)
    {
        return new XnaGeometry.Vector3(v.x, v.y, v.z);
    }

    public static Quaternion UQuaternion(this XnaGeometry.Quaternion q)
    {
        return new Quaternion(q.X, q.Y, q.Z, q.W);
    }

    public static XnaGeometry.Quaternion XQuaternion(this Quaternion q)
    {
        return new XnaGeometry.Quaternion(q.x, q.y, q.z, q.w);
    }
}
