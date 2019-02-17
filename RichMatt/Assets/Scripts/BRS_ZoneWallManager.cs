using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(LineRenderer))]
public class BRS_ZoneWallManager : MonoBehaviour
{
    [Header("Zone Wall Manager")]

	[Range(0, 359)]
    [Tooltip("How many segments should the circle that appears on the mimimap be? More segments means it looks crisper, but at cost of performance.")]
    [SerializeField] private int lineRendererSegments = 63;

	[Range(10, 10000)]
    [Tooltip("Set the starting Radius here. Can track size during runtime.")]
    [SerializeField] private float zoneWallRadius = 1000;
    
    [Range(10, 100)]
    [Tooltip("Every shrink phase, zone radius will be reduced by this percent.")]
    [SerializeField] private int radiusShrinkFactor = 50; //default to 50%

    [Header("Shrinking Zones")]

    [Tooltip("The projecte image on the rim of ZoneWall. Not needed")]
    [SerializeField] private Projector safeZone_Circle_Projector;

    [Tooltip("How long the should delay be between shrinks.")]
    [SerializeField] private int[] timeBetweenEachShrinkPhase;

    [Tooltip("How many seconds it will take to shrink each phase. If more phases exist, will repeat last.")]
    [SerializeField] private int[] secondsToShrink;

    #region Private Members
    /// <summary>
    /// how long this phase will take to shrink
    /// </summary>
    private float timeToShrink = 1;//

    /// <summary>
    /// this can be set to PUBLIC in order to troubleshoot.  It will show a checkbox in the Inspector
    /// </summary>
    private bool Shrinking = false;  // 

    /// <summary>
    /// iterates through delays between each phase and speed at which each phase shrinks
    /// </summary>
    private int shrinkPhaseIndex = 0;//

    /// <summary>
    /// holds the next time in seconds that the next shrink phase will start
    /// </summary>
    private float nextShrinkTime;//

    /// <summary>
    /// has a new center been obtained?
    /// </summary>
    private bool newCenterObtained = false;// 

    /// <summary>
    /// this is the SIZE of the zone wall object (not scale). measure it with a primitive shape to be sure. or snag the radius of attached collider
    /// </summary>
    private int zoneWallNativeSize;//

    /// <summary>
    /// Distance to center
    /// </summary>
    private float distanceToMoveCenter;
    private float shrinkRadius;
    private Vector3 centerPoint;//
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
        WorldCircle.ConfigureWorldCircle(lineRenderer, zoneWallNativeSize, zoneWallNativeSize, lineRendererSegments, false); 
        //move projector with circle
        safeZone_Circle_Projector.transform.position = new Vector3(0, capsuleCollider.height, 0);//make sure projector is at a good height

        //init next shrink time
        nextShrinkTime = Time.time + timeBetweenEachShrinkPhase[shrinkPhaseIndex];//when will the next circle start to shrink?
        timeToShrink = secondsToShrink[shrinkPhaseIndex];//how long will the next circle take to shrink?

        //apply Inspector values
        ShrinkEverything();

	}

    void Update ()
	{
        //is the zone currently in a shrinking state
        if (Shrinking && shrinkPhaseIndex < timeBetweenEachShrinkPhase.Length)
		{
            // we need a new center point (that is within the bounds of the current zone)
            if (!newCenterObtained)
			{
                ConfigureNewCenterPoint();
                newCenterObtained = true;

            }

            //shrink all the things
            ShrinkEverything();
            
            //know when to stop shrinking
            HandleStopShrinking();
			
		}
        
        //is it time to start shrinking?
        else if (Time.time > nextShrinkTime)
        {
            //when all shrinking is done, behavior runs this ad infinitum.  this update() could be slightly restructured to avoid unnecessary operations
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

    private void ConfigureNewCenterPoint()
    {
        centerPoint = NewCenterPoint(ZoneWallXform.position, zoneWallRadius, shrinkRadius, radiusShrinkFactor);
        distanceToMoveCenter = Vector3.Distance(ZoneWallXform.position, centerPoint); //this is used in the Lerp (below)

        //show on minimap where zone will shrink to
        nextZoneWallCircle = CreateLeadingCircle(centerPoint, ZoneWallXform.rotation, lineRendererSegments, zoneWallRadius / (100 / radiusShrinkFactor), zoneWallNativeSize);

        if (DEBUG)
        {
            //build new center point message
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            stringBuilder.Append("New Center Point: ");
            stringBuilder.Append(centerPoint);

            Debug.Log(stringBuilder.ToString());
        }

    }

    private void ShrinkEverything()
    {
        // shrink the zone diameter, over time
        zoneWallRadius = Mathf.MoveTowards(zoneWallRadius, shrinkRadius, (shrinkRadius / timeToShrink) * Time.deltaTime);

        //shrink the zoneWall object and all of its children
        ZoneWallXform.localScale = new Vector3((zoneWallRadius / zoneWallNativeSize), 1, (zoneWallRadius / zoneWallNativeSize)); //set local scale of zone wall

        //move ZoneWall towards new centerpoint
        ZoneWallXform.position = Vector3.MoveTowards(ZoneWallXform.position, new Vector3(centerPoint.x, ZoneWallXform.position.y, centerPoint.z), (distanceToMoveCenter / timeToShrink) * Time.deltaTime);

        // shrink circle projector
        if (safeZone_Circle_Projector) safeZone_Circle_Projector.orthographicSize = zoneWallRadius;
        
    }

    private void HandleStopShrinking()
    {
        // MoveTowards will continue ad infinitum, so we must test that we have gotten close enough to be DONE
        if (.5f > (zoneWallRadius - shrinkRadius))//shrinking complete
        {
            Shrinking = false;
            newCenterObtained = false;

            //is there more shrinking to do?
            if (++shrinkPhaseIndex < timeBetweenEachShrinkPhase.Length) 
            {
                //set next shrink time
                nextShrinkTime = Time.time + timeBetweenEachShrinkPhase[shrinkPhaseIndex];

                //repeat last index if there's more shrink phases
                timeToShrink = secondsToShrink[(shrinkPhaseIndex >= secondsToShrink.Length ? secondsToShrink.Length - 1 : shrinkPhaseIndex)];
                
            }
            else
            {
                Debug.Log("Zone Wall has finished shrinking.");
            }

            Destroy(nextZoneWallCircle);

        }

    }

	private static Vector3 NewCenterPoint(Vector3 currentCenter, float currentRadius, float newRadius, float shrinkFactor, bool debug = false)
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
            if (debug)
            {
                Debug.Log("RandomPoint: " + randPoint);
                Debug.Log("NewCenterPoint: " + newCenterPoint);
                Debug.Log("Distance: " + Vector3.Distance(currentCenter, newCenterPoint) + " Current Radius: " + currentRadius);

            }

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

    private static GameObject CreateLeadingCircle(Vector3 circleCenterPoint, Quaternion rotation, int segments, float radius, float drawHeight)
    {
        //new empty game object
        GameObject leadingCircle = new GameObject();
        //set position
        leadingCircle.transform.position = circleCenterPoint;
        //set rotation
        leadingCircle.transform.rotation = rotation;
        //set layer to make sure is on top of other objects
        leadingCircle.layer = 10;// Minimap Icon layer
        //name it so developer can identify it
        leadingCircle.name = "Next Zone Wall Boundary Marker";

        //configure line renderer
        //create new
        LineRenderer lr = leadingCircle.AddComponent<LineRenderer>() as LineRenderer;
        //do not cast shadows
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //do not receive shadows
        lr.receiveShadows = false;
        //do this because I said so... I had a reason once. what was it? The lesson here is: comment as you go
        lr.allowOcclusionWhenDynamic = false;

        //create a new array
        WorldCircle.ConfigureWorldCircle(lr, radius, drawHeight, segments, false);

        return leadingCircle;
    }

}
