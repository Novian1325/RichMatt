using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [System.Serializable]//makes visible in Inspector!
    public struct ShrinkPhase 
    {
        /// <summary>
        /// How many seconds will pass until the Zone Wall begins shrinking.
        /// </summary>
        [Tooltip("How many seconds will pass until the Zone Wall begins shrinking.")]
        public int secondsUntilShrinkBegins;

        /// <summary>
        /// How many seconds it takes for the Zone Wall to completely shrink through this shrink phase.
        /// </summary>
        [Tooltip("How many seconds it takes for the Zone Wall to completely shrink through this shrink phase.")]
        public int secondsToFullyShrink;

        /// <summary>
        /// What radius should the Zone Wall stop shrinking at this phase?
        /// </summary>
        [Tooltip("What radius should the Zone Wall stop shrinking at this phase?")]
        public int shrinkToRadius;

        /// <summary>
        /// The frequency of the damage ticks per second.
        /// </summary>
        [Tooltip("The frequency of the damage ticks per second.")]
        public int ticksPerSecond;

        /// <summary>
        /// The amount of damage that is given every one time damage is dealt.
        /// </summary>
        [Tooltip("The amount of damage that is given every one time damage is dealt.")]
        public float damagePerTick;
    }
}
