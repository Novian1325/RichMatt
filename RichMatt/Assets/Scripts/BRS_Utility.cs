using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    /// <summary>
    /// Contains a number of functions that multiple classes reference. During development, any time two systems needed to perform the same func, it would get added here instead.
    /// </summary>
    public static class BRS_Utility
    {

        /// <summary>
        /// Clamps rotation around the X axis to the given constraints.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Clamps rotation around the Z axis to the given constraints.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a non-precise distance from an object straight down
        /// </summary>
        /// <param name="flyingBody">WorldSpace coordinates of object to test.</param>
        /// <returns></returns>
        static public int GetDistanceToTerrain(Vector3 flyingBody)
        {
            int distanceToLanding = int.MaxValue;//just a really long distance
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

        /// <summary>
        /// Get pitch of object compared to horizon line
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static public float GetPitch(Quaternion q)
        {
            //Quaternion q = characterSwoopTransform.localRotation;
            //normalize
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            //hooray trigonometry!
            return 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        }
    }


}
