using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class Color_Rarity : MonoBehaviour
    {
        [Header("Colors for Rarity")]
        public static readonly Color common = new Color(0.5f, 0.5f, 0.5f, 1.0f);//gray
        public static readonly Color uncommon = new Color(0.3f, 0.6f, 0.35f, 1.0f);//green
        public static readonly Color rare = new Color(0.13f, 0.45f, 1.0f, 1.0f);//blue
        public static readonly Color epic = new Color(0.85f, 0.08f, 1.0f, 1.0f);//violet
        public static readonly Color legendary = new Color(1.0f, 1.0f, 0.25f, 1.0f);//yellow
        public static readonly Color mythic = new Color(1.0f, 0.2f, 0.2f, 1.0f);//red

        public static Color GetRarityColor(ItemRarityENUM rarity)
        {
            Color rarityColor;
            switch (rarity)
            {
                case ItemRarityENUM.Common:
                    rarityColor = common;
                    break;
                case ItemRarityENUM.Uncommon:
                    rarityColor = uncommon;
                    break;
                case ItemRarityENUM.Rare:
                    rarityColor = rare;
                    break;
                case ItemRarityENUM.Epic:
                    rarityColor = epic;
                    break;
                case ItemRarityENUM.Legendary:
                    rarityColor = legendary;
                    break;
                case ItemRarityENUM.Mythic:
                    rarityColor = mythic;
                    break;
                default:
                    Debug.Log("What am I doing with my life?");
                    rarityColor = Color.white;
                    break;
            }
            return rarityColor;
        }
        
    }
    
}
