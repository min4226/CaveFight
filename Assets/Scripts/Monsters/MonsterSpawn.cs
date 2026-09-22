using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    // 몬스터가 스폰되기 전 있을 위치
    [SerializeField] private MonsterPool mushroomPool;
    [SerializeField] private MonsterPool ghostPool;
    [SerializeField] private MonsterPool spiderPool;
    [SerializeField] private MonsterPool longNosePool;

    [SerializeField] private GameObject player;

    [SerializeField] private Transform[] spawnPoints;

    private Stage currentStage;

    // 현재 스폰되어 있는 몬스터
    private List<GameObject> spawnedMonsters = new List<GameObject>();

    public void Init(Stage stageData)
    {
        if (stageData == null)
        {
            Debug.LogError("Stage 데이터가 없습니다.");
            return;
        }

        // 기존 몬스터 정리
        ClearSpawnedMonsters();

        currentStage = stageData;

        PreparePools();

        StartCoroutine(SpawnMonsterRoutine());
    }

    private void ClearSpawnedMonsters()
    {
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null)
            {
                monster.SetActive(false);
            }
        }

        spawnedMonsters.Clear();
    }

    private void PreparePools()
    {
        foreach (MonsterSpawnData data in currentStage.monsterSpawnDatas)
        {
            MonsterPool pool = GetPool(data.monsterData.monsterType);

            if (pool == null)
                continue;

            pool.Prepare(data.count);
        }
    }

    private IEnumerator SpawnMonsterRoutine()
    {
        Debug.Log("[SpawnRoutine] 시작");

        List<MonsterType> spawnList = new List<MonsterType>();

        foreach (MonsterSpawnData data in currentStage.monsterSpawnDatas)
        {
            for (int i = 0; i < data.count; i++)
            {
                spawnList.Add(data.monsterData.monsterType);
            }
        }

        while (spawnList.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnList.Count);

            MonsterType monsterType = spawnList[randomIndex];

            spawnList.RemoveAt(randomIndex);

            Debug.Log($"[SpawnRoutine] {monsterType} 스폰");

            SpawnMonster(monsterType);
        }

        yield return null;
    }

    private void SpawnMonster(MonsterType monsterType)
    {
        MonsterPool pool = GetPool(monsterType);

        if (pool == null)
        {
            Debug.LogError(
                $"Pool이 없습니다. MonsterType : {monsterType}"
            );
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Point가 없습니다.");
            return;
        }

        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject monster = pool.Get();

        if (monster == null)
            return;

        monster.transform.position = spawnPoint.position;

        // 현재 스폰된 몬스터 기록
        spawnedMonsters.Add(monster);

        // 몬스터 AI 설정
        AIController ai = monster.GetComponent<AIController>();

        if (ai != null)
        {
            ai.SetFocusTarget(player);
        }
    }

    private MonsterPool GetPool(MonsterType monsterType)
    {
        switch (monsterType)
        {
            case MonsterType.Mushroom:
                return mushroomPool;

            case MonsterType.Ghost:
                return ghostPool;

            case MonsterType.Spider:
                return spiderPool;
            case MonsterType.LongNoseMonster:
                return longNosePool;

            default:
                return null;
        }
    }
}