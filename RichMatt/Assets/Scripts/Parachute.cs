using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class Parachute : MonoBehaviour
    {

        private Animator anim;
        private MeshRenderer meshRenderer;

        public void DeployParachute()
        {
            meshRenderer.enabled = true;
            anim.SetTrigger("DeployChute");
            //play sound
            //let other players know teammate deployed 'chute  

        }

        public void DestroyParachute()
        {
            //don't destroy right away
            //maybe play a final animation or something
            //meshRenderer.enabled = false;
            Destroy(this.gameObject);
        }


        // Use this for initialization
        void Start()
        {
            anim = this.gameObject.GetComponent<Animator>();
            meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
