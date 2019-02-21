using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 rotateSpeed;

        private Transform xform;//cache for performance

        // Use this for initialization
        void Start()
        {
            xform = this.transform;
        }

        // Update is called once per frame
        void Update()
        {
            xform.Rotate(rotateSpeed * Time.deltaTime);

        }
    }

}
