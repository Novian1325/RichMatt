using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(LineRenderer))]
    public class BRS_ZoneWallManager : MonoBehaviour
    {
        [Header("---Zone Wall Manager---")]

        [Range(16, 360)]
        [Tooltip("How many segments should the circle that appears on the mimimap be? More segments means it looks crisper, but at cost of performance.")]
        [SerializeField] private int lineRendererSegments = 64;

        [Tooltip("The projecte image on the rim of ZoneWall. Not needed")]
        [SerializeField] private Projector safeZone_Circle_Projector;

        [Header("---Shrinking Zones---")]
        
        [Tooltip("Set the starting Radius here. Can track size during runtime.")]
        [SerializeField] private float startingZoneWallRadius = 1000;

        [Tooltip("How long the should delay be between shrinks.")]
        [SerializeField] private ShrinkPhase[] shrinkPhases;

        #region Private Members

        /// <summary>
        /// this can be set to PUBLIC in order to troubleshoot.  It will show a checkbox in the Inspector.
        /// </summary>
        private bool shrinking = false;  // 

        /// <summary>
        /// iterates through delays between each phase and speed at which each phase shrinks.
        /// </summary>
        private int shrinkPhaseIndex = 0;//

        /// <summary>
        /// Holds the next Time that the next shrink phase will start.
        /// </summary>
        private float nextShrinkTime;//

        /// <summary>
        /// this is the SIZE of the zone wall object (not scale) in Meters. measure it with a primitive shape to be sure. or snag the radius of attached collider.
        /// </summary>
        private int originalZoneWallRadius;//

        /// <summary>
        /// Current size in Meters of the Zone Wall Radius.
        /// </summary>
        private float currentZoneWallRadius = 1000;

        /// <summary>
        /// Distance to centerpoint
        /// </summary>
        private float distanceToMoveCenter;

        /// <summary>
        /// The Zone Wall will become smaller to match its radius with this target radius.
        /// </summary>
        private float targetShrunkenRadius;

        /// <summary>
        /// Point in space that the Zone Wall orbits around.
        /// </summary>
        private Vector3 centerPoint;//

        /// <summary>
        /// Reference to the UI circle that shows the future bounds of the Zone Wall.
        /// </summary>
        private GameObject leadingCircle;

        /// <summary>
        /// Line Renderer that draws the bounds of the Zone Wall.
        /// </summary>
        private LineRenderer lineRenderer;

        /// <summary>
        /// Cached Transform of the parent Object.
        /// </summary>
        private Transform ZoneWallXform;

        /// <summary>
        /// Capsule Collider attached to this GameObject. Used as a reference for Scale.
        /// </summary>
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
            currentZoneWallRadius = startingZoneWallRadius;

            //draw minimap zone cirlce
            ConfigureWorldCircle(lineRenderer, originalZoneWallRadius, originalZoneWallRadius, lineRendererSegments, false);

            //move projector with circle
            safeZone_Circle_Projector.transform.position = new Vector3(0, capsuleCollider.height, 0);//make sure projector is at a good height

            InitNextShrink();
            
            //apply Inspector values
            ShrinkEverything();

        }

        void Update()
        {
            HandleShrinkingUpdate();
        }

        private void HandleShrinkingUpdate()
        {//is the zone currently in a shrinking state
            if (shrinking)
            {

                //shrink all the things
                ShrinkEverything();

                //know when to stop shrinking
                HandleStopShrinking();

            }

            else if (shrinkPhaseIndex >= shrinkPhases.Length)
            {
                //do nothing if zone has run out of shrink phases
                return;
            }

            //is it time to start shrinking?
            else if (Time.time > nextShrinkTime)
            {
                shrinking = true;
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
            centerPoint = FindNewCenterPoint(ZoneWallXform.position, currentZoneWallRadius, targetShrunkenRadius, DEBUG);
            distanceToMoveCenter = Vector3.Distance(ZoneWallXform.position, centerPoint); //this is used in the Lerp (below)

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
            currentZoneWallRadius = Mathf.MoveTowards(currentZoneWallRadius, targetShrunkenRadius, (targetShrunkenRadius / shrinkPhases[shrinkPhaseIndex].secondsToFullyShrink) * Time.deltaTime);

            //shrink the zoneWall object and all of its children
            ZoneWallXform.localScale = new Vector3((currentZoneWallRadius / originalZoneWallRadius), 1, (currentZoneWallRadius / originalZoneWallRadius)); //set local scale of zone wall

            //move ZoneWall towards new centerpoint
            ZoneWallXform.position = Vector3.MoveTowards(ZoneWallXform.position, new Vector3(centerPoint.x, ZoneWallXform.position.y, centerPoint.z), (distanceToMoveCenter / shrinkPhases[shrinkPhaseIndex].secondsToFullyShrink) * Time.deltaTime);

            // shrink circle projector
            if (safeZone_Circle_Projector) safeZone_Circle_Projector.orthographicSize = currentZoneWallRadius;
        }

        /// <summary>
        /// Set timers, choose new shrink point, draw new leading circle.
        /// </summary>
        private void InitNextShrink()
        {
            //this shrink phase, shrink from current radius to this smaller radius. 
            targetShrunkenRadius = shrinkPhases[shrinkPhaseIndex].shrinkToRadius;  //use the ZoneRadiusFactor as a percentage

            //set next shrink time
            nextShrinkTime = Time.time + shrinkPhases[shrinkPhaseIndex].secondsUntilShrinkBegins;
                        
            //get a new centerpoint for the zone wall to shrink around
            ConfigureNewCenterPoint();

            //show on minimap where zone will shrink to
            leadingCircle = CreateLeadingCircle(centerPoint, targetShrunkenRadius, originalZoneWallRadius, lineRendererSegments);
            //increment to next phase
            ++shrinkPhaseIndex;
        }

        /// <summary>
        /// Control when the circle should stop shrinking.
        /// </summary>
        private void HandleStopShrinking()
        {
            // MoveTowards will continue ad infinitum, so we must test that we have gotten CLOSE ENOUGH to be DONE
            if (.5f > (currentZoneWallRadius - targetShrunkenRadius))//shrinking complete
            {
                Destroy(leadingCircle);

                shrinking = false;
                if (DEBUG) Debug.Log("Zone Wall finished shrinking.");

                //is there more shrinking to do?
                if (shrinkPhaseIndex  + 1 < shrinkPhases.Length)
                {
                    InitNextShrink();
                }
                else
                {
                    if (DEBUG) Debug.Log("Zone Wall will no longer shrink.");
                }
            }
        }

        /// <summary>
        /// Randonly generates a new centerpoint in space.
        /// </summary>
        /// <param name="currentCenter"></param>
        /// <param name="currentRadius">New point will be within this radius</param>
        /// <param name="newRadius">Size of new radius</param>
        /// <param name="shrinkFactor"></param>
        /// <param name="DEBUG"></param>
        /// <returns></returns>
        private static Vector3 FindNewCenterPoint(Vector3 currentCenter, float currentRadius, float newRadius, bool DEBUG = false)
        {
            Vector3 newCenterPoint = Vector3.zero;

            int attemptsUntilFailure = 500; //prevent endless loop which will kill Unity
            int attemptCounter = 0;
            bool foundSuitable = false;

            if (DEBUG) Debug.Log("Finding a new center point....");

            while (!foundSuitable)
            {
                Vector2 randPoint = Random.insideUnitCircle * newRadius;
                newCenterPoint = currentCenter + new Vector3(randPoint.x, currentCenter.y, randPoint.y);
                foundSuitable = (Vector3.Distance(currentCenter, newCenterPoint) < currentRadius);

                //DEBUGS
                if (DEBUG)
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
            var leadingCircle = new GameObject();
            //set position
            leadingCircle.transform.position = circleCenterPoint;
            //set layer to make sure is on top of other objects
            leadingCircle.layer = 10;// Minimap Icon layer
            //name it so developer can identify it
            leadingCircle.name = "Next Zone Wall Boundary Marker";

            //configure line renderer
            //create new
            var lr = leadingCircle.AddComponent<LineRenderer>() as LineRenderer;
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
            var x = 0.0f;//x coordinate of terminal point on unit circle
            var y = height;//height circle is drawn
            var z = 0.0f;//y coordinate of terminal point on unit circle
            var arcLength = 0.0f;//used for trig to determine terminal point on unit circle
            var spaceBetweenPoints = 360f / segments;//if a circle has x points, this is the distance between each of those points

            renderer.positionCount = segments;//positions are vertices of circle

            //place each point an equal distance apart on the unit circle, scaled by radius
            for (var i = 0; i < segments; i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * arcLength) * radius;
                z = Mathf.Cos(Mathf.Deg2Rad * arcLength) * radius;

                renderer.SetPosition(i, new Vector3(x, y, z));

                arcLength += spaceBetweenPoints;
            }
        }
    }
}
