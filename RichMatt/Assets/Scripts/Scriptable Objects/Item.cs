using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "ScriptableObjects/Item", order = 0)]
public class Item : ScriptableObject {
    [Header("Item")]
    public string itemName;
    public ItemRarityENUM itemRarity;
    public int stackQuantity; // <= 1 means cannot stack
    public Sprite itemIcon;
    public GameObject itemModel;
    [Range(0, 1)]
    public float itemCondition = 1.0f;
}
