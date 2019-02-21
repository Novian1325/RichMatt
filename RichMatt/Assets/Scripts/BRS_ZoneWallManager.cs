using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(LineRenderer))]
    public class BRS_ZoneWallManager : MonoBehaviour
    {
        [Header("Zone Wall Manager")]

        [Range(16, 360)]
        [Tooltip("How many segments should the circle that appears on the mimimap be? More segments means it looks crisper, but at cost of performance.")]
        [SerializeField] private int lineRendererSegments = 64;

        [Range(10, 10000)]
        [Tooltip("Set the starting Radius here. Can track size during runtime.")]
        [SerializeField] private float zoneWallRadius = 1000;

        [Range(10, 100)]
        [Tooltip("Every shrink phase, zone radius will be reduced by this percent. ie, if value is 50, zone wall will shrink by 50%.")]
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
        /// has a new center been obtained?
        /// </summary>
        private bool newCenterObtained = false;// 

        /// <summary>
        /// iterates through delays between each phase and speed at which each phase shrinks
        /// </summary>
        private int shrinkPhaseIndex = 0;//

        /// <summary>
        /// holds the next time in seconds that the next shrink phase will start
        /// </summary>
        private float nextShrinkTime;//

        /// <summary>
        /// this is the SIZE of the zone wall object (not scale). measure it with a primitive shape to be sure. or snag the radius of attached collider
        /// </summary>
        private int originalZoneWallRadius;//

        /// <summary>
        /// Distance to centerpoint
        /// </summary>
        private float distanceToMoveCenter;
        private float shrinkRadius;
        private Vector3 centerPoint;//
        private GameObject leadingCircle;
        private LineRenderer lineRenderer;
        private Transform ZoneWallXform;
        private CapsuleCollider capsuleCollider;
        #endregion

        [Tooltip("Would the developer like to see Debug statements about what's going on during runtime?")]
        [SerializeField] private bool DEBUG = false;

        void Start()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            lineRenderer = gameObject.GetComponent<LineRenderer>();
            //display linerenderer in space local to center of zone wall, not world
            lineRenderer.useWorldSpace = false;

            //cache transform
            ZoneWallXform = this.transform;

            //get original radius
            originalZoneWallRadius = (int)capsuleCollider.radius;

            //draw minimap zone cirlce
            ConfigureWorldCircle(lineRenderer, originalZoneWallRadius, originalZoneWallRadius, lineRendererSegments, false);
            //move projector with circle
            safeZone_Circle_Projector.transform.position = new Vector3(0, capsuleCollider.height, 0);//make sure projector is at a good height

            //init next shrink time
            nextShrinkTime = Time.time + timeBetweenEachShrinkPhase[shrinkPhaseIndex];//when will the next circle start to shrink?
            timeToShrink = secondsToShrink[shrinkPhaseIndex];//how long will the next circle take to shrink?

            //apply Inspector values
            ShrinkEverything();

        }

        void Update()
        {
            HandleShrinkingUpdate();
        }

        private void HandleShrinkingUpdate()
        {//is the zone currently in a shrinking state
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

            else if (shrinkPhaseIndex >= timeBetweenEachShrinkPhase.Length)
            {
                //do nothing if zone has run out of shrink phases
                return;
            }

            //is it time to start shrinking?
            else if (Time.time > nextShrinkTime)
            {
                shrinkRadius = zoneWallRadius - (zoneWallRadius / (100 / radiusShrinkFactor));  //use the ZoneRadiusFactor as a percentage
                Shrinking = true;
                if (DEBUG) Debug.Log("Shrinking....");
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

        /// <summary>
        /// Configure a new centerpoint for the next shrink phase.
        /// </summary>
        private void ConfigureNewCenterPoint()
        {
            centerPoint = FindNewCenterPoint(ZoneWallXform.position, zoneWallRadius, shrinkRadius, radiusShrinkFactor, DEBUG);
            distanceToMoveCenter = Vector3.Distance(ZoneWallXform.position, centerPoint); //this is used in the Lerp (below)

            //show on minimap where zone will shrink to
            leadingCircle = CreateLeadingCircle(centerPoint, zoneWallRadius / (100 / radiusShrinkFactor), originalZoneWallRadius, lineRendererSegments);

            if (DEBUG)
            {
                //build new center point message
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

                stringBuilder.Append("New Center Point: ");
                stringBuilder.Append(centerPoint);

                Debug.Log(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// Move/Shrink all components.
        /// </summary>
        private void ShrinkEverything()
        {
            // shrink the zone diameter, over time
            zoneWallRadius = Mathf.MoveTowards(zoneWallRadius, shrinkRadius, (shrinkRadius / timeToShrink) * Time.deltaTime);

            //shrink the zoneWall object and all of its children
            ZoneWallXform.localScale = new Vector3((zoneWallRadius / originalZoneWallRadius), 1, (zoneWallRadius / originalZoneWallRadius)); //set local scale of zone wall

            //move ZoneWall towards new centerpoint
            ZoneWallXform.position = Vector3.MoveTowards(ZoneWallXform.position, new Vector3(centerPoint.x, ZoneWallXform.position.y, centerPoint.z), (distanceToMoveCenter / timeToShrink) * Time.deltaTime);

            // shrink circle projector
            if (safeZone_Circle_Projector) safeZone_Circle_Projector.orthographicSize = zoneWallRadius;

        }

        /// <summary>
        /// Control when the circle should stop shrinking.
        /// </summary>
        private void HandleStopShrinking()
        {
            // MoveTowards will continue ad infinitum, so we must test that we have gotten close enough to be DONE
            if (.5f > (zoneWallRadius - shrinkRadius))//shrinking complete
            {
                Shrinking = false;
                newCenterObtained = false;
                if (DEBUG) Debug.Log("Zone Wall finished shrinking.");

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
                    if (DEBUG) Debug.Log("Zone Wall will no longer shrink.");
                }

                Destroy(leadingCircle);

            }
        }

        /// <summary>
        /// Randonly generates a new centerpoint in space.
        /// </summary>
        /// <param name="currentCenter"></param>
        /// <param name="currentRadius">New point will be within this radius</param>
        /// <param name="newRadius">Size of new radius</param>
        /// <param name="shrinkFactor"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static Vector3 FindNewCenterPoint(Vector3 currentCenter, float currentRadius, float newRadius, float shrinkFactor, bool debug = false)
        {
            Vector3 newCenterPoint = Vector3.zero;

            int attemptsUntilFailure = 500; //prevent endless loop which will kill Unity
            int attemptCounter = 0;
            bool foundSuitable = false;

            if (debug) Debug.Log("Finding a new center point....");

            while (!foundSuitable)
            {
                Vector2 randPoint = Random.insideUnitCircle * (currentRadius / (100 / shrinkFactor));
                newCenterPoint = currentCenter + new Vector3(randPoint.x, currentCenter.y, randPoint.y);
                foundSuitable = (Vector3.Distance(currentCenter, newCenterPoint) < currentRadius);

                //DEBUGS
                if (debug)
                {
                    Debug.LogFormat("RandomPoint: {0}", randPoint);
                    Debug.LogFormat("NewCenterPoint: {0}", newCenterPoint);
                    Debug.LogFormat("Distance: {0}; Current Radius: {1}", Vector3.Distance(currentCenter, newCenterPoint), currentRadius);
                }

                //to prevent infinite loop
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

        /// <summary>
        /// Create and configure a Leading Circle from scratch. One could set up a prefab with this configuration as well.
        /// </summary>
        /// <param name="circleCenterPoint">World Space coordinates of centerpoint.</param>
        /// <param name="rotation"></param>
        /// <param name="segments"></param>
        /// <param name="radius"></param>
        /// <param name="drawHeight">How high in space the points should be drawn</param>
        /// <returns></returns>
        private static GameObject CreateLeadingCircle(Vector3 circleCenterPoint, float radius, float drawHeight, int segments = 64)
        {
            //new empty game object
            GameObject leadingCircle = new GameObject();
            //set position
            leadingCircle.transform.position = circleCenterPoint;
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
            //coordinates are given in World Space, not Local Space (relative to the world, not this object)
            lr.useWorldSpace = false;
            //make first and last point connect to form a loop
            lr.loop = true;

            //create a new array
            ConfigureWorldCircle(lr, radius, drawHeight, segments, false);

            return leadingCircle;
        }

        /// <summary>
        /// Configure given line renderer's points to make a circle in space.
        /// </summary>
        /// <param name="renderer">which line renderer to use</param>
        /// <param name="radius">the radius of the circle</param>
        /// <param name="height">height of the circle</param>
        /// <param name="segments">how many segments should the circle be divided into?</param>
        /// <param name="renderInWorldSpace">Use local or world space?</param>
        static void ConfigureWorldCircle(LineRenderer renderer, float radius, float height, int segments = 64, bool renderInWorldSpace = false)
        {
            float x = 0;//x coordinate of terminal point on unit circle
            float y = height;//height circle is drawn
            float z = 0;//y coordinate of terminal point on unit circle
            float arcLength = 0;//used for trig to determine terminal point on unit circle
            float spaceBetweenPoints = 360f / segments;//if a circle has x points, this is the distance between each of those points

            renderer.positionCount = segments;//positions are vertices of circle

            //place each point an equal distance apart on the unit circle, scaled by radius
            for (int i = 0; i < segments; i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * arcLength) * radius;
                z = Mathf.Cos(Mathf.Deg2Rad * arcLength) * radius;

                renderer.SetPosition(i, new Vector3(x, y, z));

                arcLength += spaceBetweenPoints;
            }
        }
    }
}
