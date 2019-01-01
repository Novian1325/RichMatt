using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BRS_ChangeCircle : MonoBehaviour
{
	[Range(0, 360)]
	public int Segments;
	[Range(0,5000)]
	public float Radius;

    public float timeToShrink = 30.0f; //30 default; in seconds

    [Range(10, 100)]
    public int ZoneRadiusFactor = 50; //default to 50%

    [Header("Shrinking Zones")]
    public Projector safeZone_Circle_Projector;
    public int[] ZoneTimes;

	#region Private Members
	private bool Shrinking;  // this can be set to PUBLIC in order to troubleshoot.  It will show a checkbox in the Inspector
	private int countdownPrecall = 10;  //this MIGHT be public, but it should not need to be changed
	private int count = 0;
	private bool newCenterObtained = false;
	private Vector3 centerPoint = new Vector3(0, -100, 0);
	private float distanceToMoveCenter;
	private WorldCircle circle;
	private LineRenderer lineRenderer;
	private GameObject ZoneWall;
	private float shrinkRadius;
	private int zoneTimesIndex = 0;
	private float timePassed;
	#endregion

	void Start ()
	{
		lineRenderer = gameObject.GetComponent<LineRenderer>();
		circle = new WorldCircle(ref lineRenderer, Segments, Radius, Radius);
        ZoneWall = GameObject.FindGameObjectWithTag("ZoneWall");

        
        
		ZoneWall.transform.position = new Vector3(transform.position.x, ZoneWall.transform.position.y, transform.position.z);

        timePassed = Time.deltaTime;
	}


	void Update ()
	{
        ZoneWall.transform.localScale = new Vector3((Radius * 0.01f), 1, (Radius * 0.01f));
        if (Shrinking)
		{
			// we need a new center point (that is within the bounds of the current zone)
			if (!newCenterObtained)
			{
			    centerPoint = NewCenterPoint(ZoneWall.transform.position, Radius, shrinkRadius);
				distanceToMoveCenter = Vector3.Distance(ZoneWall.transform.position, centerPoint); //this is used in the Lerp (below)
				newCenterObtained = (centerPoint != new Vector3(0, -100, 0));
		    }

            //Debug.Log("New Center Point is " + centerPoint);

			// move the center point, over time
			transform.position = Vector3.MoveTowards(transform.position, centerPoint, (distanceToMoveCenter / timeToShrink) * Time.deltaTime );
			// shrink the zone diameter, over time
			Radius = Mathf.MoveTowards(Radius, shrinkRadius, (shrinkRadius / timeToShrink) * Time.deltaTime);
            //move ZoneWall towards new centerpoint
            ZoneWall.transform.position = Vector3.MoveTowards(ZoneWall.transform.position, new Vector3(centerPoint.x, ZoneWall.transform.position.y, centerPoint.z), (distanceToMoveCenter / timeToShrink) * Time.deltaTime);
            // shrink circle projector
            safeZone_Circle_Projector.orthographicSize = Radius;

            circle.Draw(Segments, Radius, Radius);

			// MoveTowards will continue infinitum, so we must test that we have gotten close enough to be DONE
			if (1 > (Radius - shrinkRadius))
			{
			  timePassed = Time.deltaTime;
				Shrinking = false;
				newCenterObtained = false;
			}
		} else {
			timePassed += Time.deltaTime; // increment clock time
		}

		// have we passed the next threshold for time delay?
		if (((int) timePassed)  > ZoneTimes[zoneTimesIndex])
		{
			shrinkRadius = Radius - (float)(Radius * (ZoneRadiusFactor * 0.01));  //use the ZoneRadiusFactor as a percentage
			Shrinking = true;
			timePassed = Time.deltaTime;  //reset the time so other operations are halted.
			NextZoneTime();
		}

		// COUNT DOWN
		if (timePassed > (ZoneTimes[zoneTimesIndex] - countdownPrecall)) {  // we need to begin counting down
			if (ZoneTimes[zoneTimesIndex] - (int) timePassed != count)
			{
				count = Mathf.Clamp(ZoneTimes[zoneTimesIndex] - (int) timePassed, 1, 1000);  // this ensures our value never falls below zero

				//FILL IN APPROPRIATE UI CALLS HERE FOR THE COUNTDOWN
				//Debug.Log("Shrinking in " + count + " seconds.");
			}
		}
	}

	// ***********************************
	// PRIVATE (helper) FUNCTIONS
	// ***********************************
	private Vector3 NewCenterPoint(Vector3 currentCenter, float currentRadius, float newRadius)
	{
		Vector3 newPoint = Vector3.zero;

		int totalCountDown = 30000; //prevent endless loop which will kill Unity
		bool foundSuitable = false;
		while (!foundSuitable)
		{
			 --totalCountDown;
			 Vector2 randPoint = Random.insideUnitCircle * (currentRadius / 2);
			 newPoint = currentCenter + new Vector3(randPoint.x, 0, randPoint.y);
			 foundSuitable = (Vector3.Distance(currentCenter, newPoint) < currentRadius);
			 if (totalCountDown < 1)
			   return new Vector3(0, -100, 0);  //explicitly define an error has occured.  In this case we did not locate a reasonable point
		}
		return newPoint;
	}

	private int NextZoneTime()
	{
        //if we have exceeded the count, just start over
        return ++zoneTimesIndex > ZoneTimes.Length ? -1 : ZoneTimes[zoneTimesIndex];
	}
}
