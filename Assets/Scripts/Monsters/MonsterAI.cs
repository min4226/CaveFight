using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private MonsterData monsterData;

    [Header("배회")]
    [SerializeField] private float wanderRadius = 2f;
    [SerializeField] private float wanderChangeTime = 2f;

    private Animator mushroomAnimator;
    private Transform player;

    private bool isAttacking = false;

    private Vector2 wanderTarget;
    private float wanderTimer;

    private void Awake()
    {
        mushroomAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Debug.Log($"Animator : {mushroomAnimator}");
        Debug.Log($"SpriteRenderer : {spriteRenderer}");
        Debug.Log($"MonsterData : {monsterData}");
    }

    private void OnEnable()
    {
        Debug.Log($"[MonsterAI] 활성화 : {gameObject.name}");

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Character");

        if (playerObject == null)
        {
            Debug.LogError("[MonsterAI] Player를 찾지 못함!");
            return;
        }

        player = playerObject.transform;

        // 처음에는 현재 위치 주변으로 배회 목표 설정
        SetWanderTarget();

        Debug.Log($"[MonsterAI] Player 찾음 : {player.name}");
    }

    private void Update()
    {
        if (player == null || monsterData == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        // =========================
        // 공격 범위
        // =========================
        if (distance <= monsterData.attackRange)
        {
            LookAtPlayer();

            mushroomAnimator.SetBool("IsRunning", false);

            if (!isAttacking)
            {
                isAttacking = true;
                mushroomAnimator.SetTrigger("Attack");
            }

            return;
        }

        // =========================
        // 추적 범위
        // =========================
        if (distance <= monsterData.detectionRange)
        {
            Vector2 direction =
                (player.position - transform.position).normalized;

            LookAtPlayer();

            transform.position +=
                (Vector3)direction *
                monsterData.moveSpeed *
                Time.deltaTime;

            mushroomAnimator.SetBool("IsRunning", true);
        }
        // =========================
        // 추적 범위 밖 → 배회
        // =========================
        else
        {
            Wander();
        }
    }

    // =========================
    // 몬스터 배회
    // =========================
    private void Wander()
    {
        wanderTimer -= Time.deltaTime;

        Vector2 direction =
            (wanderTarget - (Vector2)transform.position).normalized;

        // 목표 지점으로 이동
        if (direction.magnitude > 0.1f)
        {
            transform.position +=
                (Vector3)direction *
                (monsterData.moveSpeed * 0.5f) *
                Time.deltaTime;

            // 이동 방향 바라보기
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }

            mushroomAnimator.SetBool("IsRunning", true);
        }
        else
        {
            mushroomAnimator.SetBool("IsRunning", false);
        }

        // 일정 시간이 지나면 새로운 위치 선택
        if (wanderTimer <= 0f ||
            Vector2.Distance(transform.position, wanderTarget) < 0.1f)
        {
            SetWanderTarget();
        }
    }

    // 새로운 배회 위치 선택
    private void SetWanderTarget()
    {
        Vector2 randomPosition =
            Random.insideUnitCircle * wanderRadius;

        wanderTarget =
            (Vector2)transform.position + randomPosition;

        wanderTimer = wanderChangeTime;
    }

    // 플레이어 방향 바라보기
    private void LookAtPlayer()
    {
        if (player == null || spriteRenderer == null)
            return;

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void DealDamage()
    {
        Debug.Log("[MonsterAI] DealDamage 실행!");

        if (player == null)
        {
            Debug.LogError("[MonsterAI] player가 null!");
            return;
        }

        TakeDamage takeDamage =
            player.GetComponent<TakeDamage>();

        if (takeDamage == null)
        {
            Debug.LogError(
                "[MonsterAI] Player에서 TakeDamage를 찾지 못함!"
            );
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > monsterData.attackRange)
        {
            Debug.Log(
                "[MonsterAI] 플레이어가 공격 범위 밖으로 이동함"
            );
            return;
        }

        Debug.Log(
            $"[MonsterAI] 플레이어에게 데미지 {monsterData.attackDamage} 적용!"
        );

        takeDamage.TakePlayerDamage(
            monsterData.attackDamage
        );
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
}