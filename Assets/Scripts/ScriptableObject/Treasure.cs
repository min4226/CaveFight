using UnityEngine;

public enum TreasureItemType
{ 
     None, Potion, Equipment,
     _Length
}

public enum TreasureEffectItemType
{ 
    Health, None, Attack, Defense,
    _Length
}
[CreateAssetMenu(fileName = "Treasure", menuName = "Scriptable Objects/Treasure")]
public class Treasure : ScriptableObject
{
    public GameObject treasureItemPrefab;
    public string treasureItemName;
    public string treasureItemDescription;
    public TreasureItemType treasureItemType;
    public TreasureEffectItemType treasureItemEffect;
    public float treasureItemEffectValue;
}
