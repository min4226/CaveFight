using System;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

[Serializable]
public struct UIClaim
{
    public string prefabName;
    public UIType uiType;
    public bool isOpen;

    public UIBase Execute()
    {
        UIBase result = UIManager.ClaimGetUI(uiType);
        if (!result) result = UIManager.ClaimCreateUI(uiType, prefabName);
        if (!result) return result;

        if (result is IOpenable openTarget)
        { 
            if(isOpen) openTarget.Open();
            else openTarget.Close();
        }

        return result;
    }
}
public class UI_ScreenBase : OpenableUIBase
{
    [SerializeField] UIClaim[] requiredUI;
    [SerializeField] protected UIType[] closeWithScreen;
    /*public bool IsOpen => gameObject.activeSelf;
    public void Close() => gameObject.SetActive(false);
    public void Open() => gameObject.SetActive(true);
    public void Toggle() => gameObject.SetActive(!IsOpen);*/
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        if (requiredUI is null) return;
        foreach (UIClaim currentClaim in requiredUI)
        {
            currentClaim.Execute();
        }
    }
    public override void Close()
    {
        base.Close();
        if (closeWithScreen != null)
        {
            foreach (UIType currentUI in closeWithScreen) UIManager.ClaimCloseUI(currentUI);
        }
    }
    private void OnEnable()
    {
        InputManager.OnCancel -= ToggleMenu;
        InputManager.OnCancel += ToggleMenu;
    }

    private void OnDisable()
    {
        InputManager.OnCancel -= ToggleMenu;
    }
    public virtual bool CloseInnerUI()
    {
        if (closeWithScreen == null || closeWithScreen.Length == 0)
            return false;

        foreach (UIType currentUI in closeWithScreen)
        {
            UIManager.ClaimCloseUI(currentUI);
        }

        return true;
    }
    void ToggleMenu(bool value) => UIManager.ClaimToggleUI(UIType.Menu);
}
