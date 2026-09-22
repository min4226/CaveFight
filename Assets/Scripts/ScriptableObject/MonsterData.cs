using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
  
public class MonsterData : ScriptableObject
{
    public MonsterType monsterType;

    public float detectionRange;
    public float moveSpeed;

    public float attackRange;
    public int attackDamage;

    public int maxHp;

    public GameObject[] monsterEquipment;
}
