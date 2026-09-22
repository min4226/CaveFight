using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Animator monsterAnim;
    [SerializeField] private Slider hpSlider;
    
    private float currentHp;
    private bool isDead;
    private void Start()
    {
        currentHp = monsterData.maxHp;

        hpSlider.maxValue = monsterData.maxHp;
        hpSlider.value = currentHp;

        Debug.Log($"[MonsterHealth] {gameObject.name} 초기 HP: {currentHp}");
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        Debug.Log($"[MonsterHealth] {gameObject.name} 피격! 받은 데미지: {damage}");

        currentHp -= damage;

        if (currentHp < 0)
            currentHp = 0;

        hpSlider.value = currentHp;

        if (currentHp <= 0)
        {
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망");
        monsterAnim.SetTrigger("Die");
        DropItem();
        StartCoroutine(DisableAfterDelay());
    }
    private void DropItem()
    {
        Debug.Log("DropItem 실행됨");

        if (monsterData.monsterEquipment == null)
        {
            Debug.LogError("dropItems가 NULL임");
            return;
        }

        if (monsterData.monsterEquipment.Length == 0)
        {
            Debug.LogError("dropItems에 아이템이 없음");
            return;
        }

        int randomIndex = Random.Range(0, monsterData.monsterEquipment.Length);

        GameObject dropItem = monsterData.monsterEquipment[randomIndex];

        Debug.Log($"드랍 아이템: {dropItem.name}");

        Instantiate(dropItem, transform.position, Quaternion.identity);
    }
    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }
}