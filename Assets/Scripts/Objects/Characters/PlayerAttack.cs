using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private float attackRange = 1f;

    public void AttackHit()
    {
        Debug.Log("[PlayerAttack] 공격 판정!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange
        );

        foreach (Collider2D hit in hits)
        {
            MonsterHealth monster = hit.GetComponentInParent<MonsterHealth>();

            if (monster != null)
            {
                Debug.Log($"[PlayerAttack] {hit.gameObject.name}에게 데미지!");

                monster.TakeDamage(damage);
            }
        }
    }
}