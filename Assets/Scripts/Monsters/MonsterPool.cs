using System.Collections.Generic;
using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    [SerializeField] private GameObject monsterPrefab;

    private Queue<GameObject> pool = new Queue<GameObject>();

    // 스테이지에서 필요한 수만큼 Pool 준비
    public void Prepare(int count)
    {
        while (pool.Count < count)
        {
            CreateMonster();
        }
    }

    private void CreateMonster()
    {
        GameObject monster = Instantiate(monsterPrefab, transform);

        monster.SetActive(false);

        pool.Enqueue(monster);
    }

    public GameObject Get()
    {
        Debug.Log($"[Pool Get] {gameObject.name} Get 호출");

        if (pool.Count == 0)
        {
            Debug.LogWarning($"[Pool Get] {gameObject.name} Pool이 비어있음");
            return null;
        }

        GameObject monster = pool.Dequeue();

        Debug.Log($"[Pool Get] 꺼낸 몬스터 : {monster.name}");
        Debug.Log($"[Pool Get] 활성화 전 : {monster.activeSelf}");

        monster.SetActive(true);

        Debug.Log($"[Pool Get] 활성화 후 : {monster.activeSelf}");

        return monster;
    }

    public void Return(GameObject monster)
    {
        monster.SetActive(false);
        monster.transform.SetParent(transform);

        pool.Enqueue(monster);
    }
}