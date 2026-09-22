using UnityEngine;

public class RetryButton : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    public void RetryStage()
    {
        Debug.Log($"RetryStage 호출됨 / 현재 스테이지 : {GameManager.Instance.Stage.CurrentStageNumber}");

        // 게임 오버 창 닫기
        gameOverPanel.SetActive(false);

        // 현재 스테이지 재시작
        GameManager.Instance.Stage.RetryStageCharacter();
    }
}