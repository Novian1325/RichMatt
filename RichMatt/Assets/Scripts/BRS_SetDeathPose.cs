using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_SetDeathPose : MonoBehaviour
    {
        private Animator anim;
        public int DeathPose;


        // Use this for initialization
        void Start()
        {
            anim = gameObject.GetComponent<Animator>();
            anim.SetInteger("DeathPose", DeathPose);
        }
    }
}
