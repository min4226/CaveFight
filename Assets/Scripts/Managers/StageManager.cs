using System.Collections;
using UnityEngine;

public class StageManager : ManagerBase
{
    [SerializeField] private CharacterBase character;
    [SerializeField] private Transform mainCameraTransform;

    [SerializeField] private MonsterSpawn monsterSpawn;
    [SerializeField] private MonsterSpawn stage2MonsterSpawn;

    [SerializeField] private Vector3 stage1Position = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 stage2Position = new Vector3(133f, -20f, 0f);

    [SerializeField] private Stage stage2Data;

    private int currentStageNumber = 1;

    public int CurrentStageNumber => currentStageNumber;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        if (mainCameraTransform == null && Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        if (character == null)
        {
            character = FindObjectOfType<CharacterBase>();
        }

        currentStageNumber = 1;

        Debug.Log($"🔥 StageManager 연결됨 / 현재 스테이지 : {currentStageNumber}");

        yield return null;
    }

    protected override void OnDisconnected()
    {

    }

    public void MoveToStage1()
    {
        Debug.Log("🔥 MoveToStage1 호출됨");

        currentStageNumber = 1;

        MoveToStage(stage1Position);

        Stage currentStage = GameManager.Instance.StageObject;

        if (monsterSpawn == null)
        {
            Debug.LogError("❌ Stage 1 MonsterSpawn이 연결되지 않았습니다.");
            return;
        }

        monsterSpawn.Init(currentStage);

        Debug.Log("✅ Stage 1 재시작 완료");
    }

    public void MoveToStage2()
    {
        Debug.Log("🔥 MoveToStage2 호출됨");

        currentStageNumber = 2;

        MoveToStage(stage2Position);

        if (stage2MonsterSpawn == null)
        {
            Debug.LogError("❌ Stage 2 MonsterSpawn이 연결되지 않았습니다.");
            return;
        }

        stage2MonsterSpawn.Init(stage2Data);

        Debug.Log("✅ Stage 2 재시작 완료");
    }

    public void MoveToStage(Vector3 targetPos)
    {
        if (character != null)
        {
            character.transform.position = targetPos;

            MovementModule movementModule = character.GetModule<MovementModule>();

            if (movementModule != null)
            {
                movementModule.StopMovement();
            }

            PlayerController playerController = character.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.CanMove = true;
            }
        }

        if (mainCameraTransform != null)
        {
            mainCameraTransform.position =
                new Vector3(targetPos.x, targetPos.y, -10f);

            mainCameraTransform.rotation = Quaternion.identity;
        }

        Debug.Log($"[StageManager] 스테이지 이동 완료: {targetPos}");
    }

    public void RetryStageCharacter()
    {
        StartCoroutine(RetryStageRoutine());
    }

    private IEnumerator RetryStageRoutine()
    {
        Debug.Log($"🔥 RetryStage 호출됨 / 현재 스테이지 : {currentStageNumber}");

        Vector3 spawnPos = (currentStageNumber == 2) ? stage2Position : stage1Position;

        if (character != null)
        {
            GameObject model = character.transform.GetChild(0).gameObject;

            // 1. 잠시 캐릭터와 모델 끄기 (GameManager 참조 유지 목적)
            character.gameObject.SetActive(false);
            model.SetActive(false);

            // 2. 한 프레임 대기하여 유니티 렌더링 상태 정리
            yield return null;

            // 3. 위치 이동 먼저 반영
            character.transform.position = spawnPos;

            // 4. 애니메이터 강제 초기화
            Animator animator = model.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
                animator.ResetTrigger("Die");
                animator.ResetTrigger("Revive");
                animator.SetFloat("MoveSpeed", 0f);
                animator.SetFloat("MoveX", 0f);
                animator.SetFloat("MoveY", 0f);
                animator.Play("Blend Tree", 0, 0f);
            }

            // 5. 체력 초기화
            TakeDamage takeDamage = model.GetComponent<TakeDamage>();
            if (takeDamage != null)
            {
                takeDamage.ResetHp();
            }

            // 6. 모델과 캐릭터 다시 켜기
            model.SetActive(true);
            character.gameObject.SetActive(true);
        }

        // 7. 위치 이동 및 카메라 맞추기
        MoveToStage(spawnPos);

        // 8. 현재 스테이지 재시작 (몬스터 스폰 초기화 등)
        if (currentStageNumber == 1)
        {
            MoveToStage1();
        }
        else if (currentStageNumber == 2)
        {
            MoveToStage2();
        }
    }
}