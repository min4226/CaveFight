using UnityEngine;


[System.Serializable]
public class MonsterSpawnData
{
    public MonsterData monsterData;
    public int count;
}

[CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Objects/Stage")]
public class Stage : ScriptableObject
{
    public StageType stageType;
    public MonsterSpawnData[] monsterSpawnDatas;
    public Treasure[] stageTreasures;
}
