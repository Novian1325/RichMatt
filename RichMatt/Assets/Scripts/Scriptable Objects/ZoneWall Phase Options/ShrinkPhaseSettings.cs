using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [CreateAssetMenu(fileName = "ShrinkPhaseOption_", menuName = "ScriptableObjects/ShrinkPhaseOption", order = 1)]
    public class ShrinkPhaseSettings : ScriptableObject
    {
        public ShrinkPhase[] shrinkPhases;
    }

}
