using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [CreateAssetMenu(fileName = "ShrinkPhaseOption_", menuName = "ScriptableObjects/ShrinkPhaseOption", order = 1)]
    public class ShrinkPhaseOptions : ScriptableObject
    {
        public ShrinkPhase[] shrinkPhases;
    }

}
