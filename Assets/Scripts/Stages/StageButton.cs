using UnityEngine;

public class StageButton : MonoBehaviour
{
    public void SelectStage1()
    {
        GameManager.Instance.Stage.MoveToStage1();
        UIManager.ClaimCloseUI(UIType.Stage);
    }
    public void SelectStage2()
    {
        GameManager.Instance.Stage.MoveToStage2();
        UIManager.ClaimCloseUI(UIType.Stage);
    }
}
