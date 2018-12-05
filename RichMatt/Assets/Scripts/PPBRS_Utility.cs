using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PPBRS_Utility {

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
