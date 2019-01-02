using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(LineRenderer))]
public class BRS_ZoneWallManager : MonoBehaviour
{
    [Header("Zone Wall Manager")]

	[Range(0, 360)]
    [Tooltip("How many segments should the circle that appears on the mimimap be? More segments means it looks crisper, but at cost of performance.")]
    [SerializeField] private int segments = 63;

	[Range(10, 5000)]
    [Tooltip("Set the starting Radius here. Can track size during runtime.")]
    [SerializeField] private float zoneWallRadius = 1000;

    [Tooltip("How long should it take for the zone wall to shrink completely.")]
    [SerializeField] private float timeToShrink = 30.0f; //30 default; in seconds

    [Range(10, 100)]
    [Tooltip("Every shrink phase, zone radius will be reduced by this percent.")]
    [SerializeField] private int radiusShrinkFactor = 50; //default to 50%

    [Header("Shrinking Zones")]

    [Tooltip("The projecte image on the rim of ZoneWall. Not needed")]
    [SerializeField] private Projector safeZone_Circle_Projector;

    [Tooltip("How long should the delay be between shrinks?")]
    [SerializeField] private int[] shrinkTimes;

    private int shrinkTimeIndex = 0;
    private float nextShrinkTime;

    #region Private Members
    private bool Shrinking = false;  // this can be set to PUBLIC in order to troubleshoot.  It will show a checkbox in the Inspector
	private bool newCenterObtained = false;// has a new center been obtained?
    private int zoneWallNativeSize;//this is the SIZE of the zone wall object (not scale). measure it with a primitive shape to be sure. or snag the radius of attached collider
    private float distanceToMoveCenter;
    private float shrinkRadius;
    private Vector3 centerPoint;//the
    private WorldCircle currentZoneWallCircle;
    private GameObject nextZoneWallCircle;
	private LineRenderer lineRenderer;
	private Transform ZoneWallXform;
    private CapsuleCollider capsuleCollider;
    #endregion

    [Tooltip("Would the developer like to see Debug statements about what's going on during runtime?")]
    [SerializeField] private bool DEBUG = false;
    
	void Start ()
	{
        capsuleCollider = GetComponent<CapsuleCollider>();
		lineRenderer = gameObject.GetComponent<LineRenderer>();
        ZoneWallXform = this.transform;

        zoneWallNativeSize = (int)capsuleCollider.radius;

        //draw minimap zone cirlce
        currentZoneWallCircle = new WorldCircle(ref lineRenderer, segments, zoneWallNativeSize, zoneWallNativeSize);
        safeZone_Circle_Projector.transform.position = new Vector3(0, capsuleCollider.height, 0);//make sure projector is at a good height

        //apply Inspector values
        ShrinkEverything();

        //init next shrink time
        nextShrinkTime = Time.time + shrinkTimes[shrinkTimeIndex];
	}

	void Update ()
	{
        
        if (Shrinking && shrinkTimeIndex < shrinkTimes.Length)
		{
            // we need a new center point (that is within the bounds of the current zone)
            if (!newCenterObtained)
			{
			    centerPoint = NewCenterPoint(ZoneWallXform.position, zoneWallRadius, shrinkRadius, radiusShrinkFactor);
				distanceToMoveCenter = Vector3.Distance(ZoneWallXform.position, centerPoint); //this is used in the Lerp (below)
                newCenterObtained = true;

                //show on minimap where zone will shrink to
                nextZoneWallCircle = CreateLeadingCircle(centerPoint, ZoneWallXform.rotation, segments, zoneWallRadius / (100 / radiusShrinkFactor));

                if (DEBUG)
                {
                    //build new center point message
                    System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

                    stringBuilder.Append("New Center Point: ");
                    stringBuilder.Append(centerPoint);

                    Debug.Log(stringBuilder.ToString());
                }

            }

            //shrink all the things
            ShrinkEverything();
            
            //know when to stop shrinking
            HandleStopShrinking();
			
		}
        
        // have we passed the next threshold for time delay?
        else if (Time.time > nextShrinkTime)
        {
            shrinkRadius = zoneWallRadius - (zoneWallRadius / (100 / radiusShrinkFactor));  //use the ZoneRadiusFactor as a percentage
            Shrinking = true;
        }
            
        else
        {
            if (DEBUG)
            {
                //use string builder because concatentation ( + ) is expensive
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

                stringBuilder.Append("Zone Wall shrinking in ");
                stringBuilder.Append(nextShrinkTime - Time.time);

                Debug.Log(stringBuilder.ToString());
            }
        }

		
	}

    private void ShrinkEverything()
    {
        //shrink the zoneWall object and all of its children
        ZoneWallXform.localScale = new Vector3((zoneWallRadius / zoneWallNativeSize), 1, (zoneWallRadius / zoneWallNativeSize)); //set local scale of zone wall

        // shrink the zone diameter, over time
        zoneWallRadius = Mathf.MoveTowards(zoneWallRadius, shrinkRadius, (shrinkRadius / timeToShrink) * Time.deltaTime);

        //move ZoneWall towards new centerpoint
        ZoneWallXform.position = Vector3.MoveTowards(ZoneWallXform.position, new Vector3(centerPoint.x, ZoneWallXform.position.y, centerPoint.z), (distanceToMoveCenter / timeToShrink) * Time.deltaTime);

        // shrink circle projector
        if (safeZone_Circle_Projector) safeZone_Circle_Projector.orthographicSize = zoneWallRadius;
        
    }

    private void HandleStopShrinking()
    {
        // MoveTowards will continue ad infinitum, so we must test that we have gotten close enough to be DONE
        if (1 > (zoneWallRadius - shrinkRadius))//shrinking complete
        {
            Shrinking = false;
            newCenterObtained = false;

            //is there more shrinking to do?
            if (++shrinkTimeIndex < shrinkTimes.Length) 
            {//set next shrink time
                nextShrinkTime = Time.time + shrinkTimes[shrinkTimeIndex];
                
            }
            else
            {
                Debug.Log("Zone Wall has finished shrinking.");
            }

            Destroy(nextZoneWallCircle);

        }

    }

	// ***********************************
	// PRIVATE static (helper) FUNCTIONS
	// ***********************************
	private static Vector3 NewCenterPoint(Vector3 currentCenter, float currentRadius, float newRadius, float shrinkFactor)
	{
		Vector3 newCenterPoint = Vector3.zero;

		int attemptsUntilFailure = 500; //prevent endless loop which will kill Unity
        int attemptCounter = 0;
		bool foundSuitable = false;
        
		while (!foundSuitable)
		{
			Vector2 randPoint = Random.insideUnitCircle * (currentRadius / (100 / shrinkFactor));
			newCenterPoint = currentCenter + new Vector3(randPoint.x, currentCenter.y, randPoint.y);
			foundSuitable = (Vector3.Distance(currentCenter, newCenterPoint) < currentRadius);

            //DEBUGS
            //Debug.Log("RandomPoint: " + randPoint);
            //Debug.Log("NewCenterPoint: " + newCenterPoint);
            //Debug.Log("Distance: " + Vector3.Distance(currentCenter, newCenterPoint) + " Current Radius: " + currentRadius);
            
			if (++attemptCounter > attemptsUntilFailure)
            {
                //build error message
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

                stringBuilder.Append("ERROR! New Center point could not be found after ");
                stringBuilder.Append(attemptsUntilFailure);
                stringBuilder.Append(" attempts.");

                Debug.LogError(stringBuilder.ToString());//display error message

                newCenterPoint = currentCenter;//return same position to keep the game moving

                break;//break out of while loop
            }
		}
		return newCenterPoint;
    }

    private static GameObject CreateLeadingCircle(Vector3 circleCenterPoint, Quaternion rotation, int segments, float radius)
    {
        GameObject leadingCircle = new GameObject();
        leadingCircle.transform.position = circleCenterPoint;
        leadingCircle.transform.rotation = rotation;
        leadingCircle.name = "Next Zone Wall Boundary Marker";

        LineRenderer lr = leadingCircle.AddComponent<LineRenderer>() as LineRenderer;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.allowOcclusionWhenDynamic = false;

        new WorldCircle(ref lr, segments, radius, radius);

        return leadingCircle;
    }

}
