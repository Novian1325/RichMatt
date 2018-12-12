using UnityEngine;

public enum Axis
{
    X, Y, Z
}

public static class PPBRS_Utility {
    
    public static Quaternion ClampRotationAroundXAxis(Quaternion q, float min, float max)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, min, max);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    public static Quaternion ClampRotationAroundZAxis(Quaternion q, float min, float max)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, min, max);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }

    static public int GetDistanceToTerrain(Vector3 flyingBody)
    {
        int distanceToLanding = 999999;//just a really long distance
        RaycastHit hit;

        if (Physics.Raycast(flyingBody, Vector3.down, out hit, distanceToLanding))
        {
            if (hit.collider.CompareTag("Terrain"))//verify that the ground was hit -- ex. not a parachute right below you
            {
                distanceToLanding = (int)hit.distance;//round distance to int

            }
            //TODO
            //CHECK IF COLLIDER WAS A BUILDING, TOO
        }

        return distanceToLanding;
    }
}
