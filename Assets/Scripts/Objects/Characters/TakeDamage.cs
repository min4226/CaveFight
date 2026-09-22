using UnityEngine;
using UnityEngine.UI;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject deadUI;
    private int currentHp;
    private int maxHp = 100;

    private void Awake()
    {
        currentHp = maxHp;

        hpSlider.maxValue = maxHp;
        hpSlider.value = currentHp;
    }

    public void TakePlayerDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp < 0)
            currentHp = 0;

        hpSlider.value = currentHp;

        Debug.Log($"플레이어hp : {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");

        PlayerController playerController = GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.CanMove = false;
        }

        animator.SetTrigger("Die");
    }

    public void DisablePlayer()
    { 
        gameObject.SetActive(false);
        hpSlider.gameObject.SetActive(false);
        deadUI.SetActive(true);
    }
    public void ResetHp()
    {
        currentHp = maxHp;

        hpSlider.gameObject.SetActive(true);
        hpSlider.maxValue = maxHp;
        hpSlider.value = currentHp;
    }
}