using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UIType
{
    None, Loading, Title, Movable, Menu, Info, Option, Battle, GameQuit, CharacterSet, Stage, Inventory,
    _Length
}

public enum ScreenChageType
{ 
    None,
     FadeChanger, ScreenChanger,
    _Length
}

public delegate void PopUpEvent(string title, string context, string confirm);

public class UIManager : ManagerBase
{
    public static event PopUpEvent OnPopUp;

    readonly KeyValuePair<UIType, string>[] globalScreenArray =
    {
        new (UIType.Title, "TitleScreen"),
        // new (UIType.Battle, "BattleScreen"),
        new (UIType.Option, "OptionScreen"),
        new(UIType.CharacterSet, "CharacterSelectWindow"),
        new(UIType.Stage, "OptionWindow"),
        new(UIType.Inventory , "Inventory"),
    };

    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    UIBase _movableScreen;
    RectTransform switcherTransform;
    RectTransform createdTransform;
    RectTransform changerTransform;

    GraphicRaycaster _raycaster;
    public GraphicRaycaster Raycaster => _raycaster;

    
    Dictionary<UIType, UIBase> uiDictionary = new();
    Dictionary<ScreenChageType, UI_ScreenChanger> screenChangerDictionary = new();

    Rect _uiBoundary;
    public static Rect UIBoundary => GameManager.Instance?.UI?._uiBoundary ?? Rect.zero;
    UIType _currentScreenType = UIType.None;
    public static UIType CurrentScreen => GameManager.Instance?.UI?._currentScreenType ?? UIType.None;

    UI_ScreenChanger currentScreenChanger;

    float _uiScale = 1.0f;
    public static float UIScale => GameManager.Instance?.UI?._uiScale ?? 1.0f;

    public IEnumerator Initialize(GameManager newManager)
    {
        SetMainCanvas(GetComponentInChildren<Canvas>());
        SetUI(UIType.Loading, GetComponentInChildren<UI_LoadingScreen>());
        yield return null;
    }

    public RectTransform CreateFullScreen(string wantName)
    {
        GameObject instance = new GameObject(wantName);
        RectTransform result = instance.AddComponent<RectTransform>();
        result.SetParent(MainCanvas.transform);

        result.SetAsFirstSibling();

        result.anchorMin = Vector3.zero;
        result.anchorMax = Vector3.one;

        result.offsetMin = Vector3.zero;
        result.offsetMax = Vector3.zero;

        result.localScale = Vector3.one;

        return result;
    }
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        createdTransform = CreateFullScreen("CreatedUI");
        _movableScreen = CreateUI(UIType.Movable, "MovableScreen", MainCanvas?.transform);
        switcherTransform = CreateFullScreen("ScreenSwitcher");
        foreach (var currentPair in globalScreenArray)
        {
            UIBase created = CreateUI(currentPair.Key, currentPair.Value, switcherTransform);
            if(created is IOpenable asOpenable) asOpenable.Close();
        }
        changerTransform = CreateFullScreen("ScreenChanger");
        changerTransform.SetAsLastSibling();

        for (ScreenChageType currentChanger = (ScreenChageType)1; currentChanger < ScreenChageType._Length; currentChanger++)
        {
            GameObject instance = ObjectManager.CreateObject(currentChanger.ToString(), changerTransform);
            if (instance?.TryGetComponent(out UI_ScreenChanger asChanger) ?? false)
            {
                screenChangerDictionary.Add(currentChanger, asChanger);
            }
            instance?.SetActive(false);
        }
        
        yield return null;
    }

    protected override void OnDisconnected()
    {
        UnSetAllUI();
    }

    protected void SetMainCanvas(Canvas newCanvas)
    {
        _mainCanvas = newCanvas;
        if (MainCanvas)
        {
            _raycaster = MainCanvas.GetComponent<GraphicRaycaster>();

            if (MainCanvas.transform is RectTransform mainRectTransform)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(mainRectTransform);
                _uiScale = mainRectTransform.lossyScale.x;
                _uiBoundary = mainRectTransform.rect;
                _uiBoundary.size *= _uiScale;
                _uiBoundary.position *= _uiScale / 1.0f;
            }
        }
        else
        {
            _raycaster = null;
        }
    }

    protected UIBase CreateUI(UIType wantType, string wantName, Transform parent)
    {
        GameObject instance = ObjectManager.CreateObject(wantName, parent);
        UIBase result = instance?.GetComponent<UIBase>();
        return SetUI(wantType, result);
    }

    protected UIBase CreateUI(UIType wantType, string wantName)
    {
        UIBase result = CreateUI(wantType, wantName, createdTransform ?? MainCanvas?.transform);
        if (result?.GetComponentInChildren<UI_DraggableWindow>())
        {
            _movableScreen?.SetChild(result.gameObject);
        }
        return result;
    }
    public static UIBase ClaimCreateUI(UIType wantType, string wantName) => GameManager.Instance?.UI?.CreateUI(wantType, wantName);
    protected void UnSetAllUI() // 싹 다 해고야
    {
        foreach (UIBase ui in uiDictionary.Values) //애들 전부 돌면서
        {
            UnsetUI(ui);
        }
        
        uiDictionary.Clear();
    }
    protected void UnsetUI(UIType wantType) //담당 공무원의 부서랑 직책만 알고 있는 경우
    {
        
        if (uiDictionary.TryGetValue(wantType, out UIBase found))
        {
            //처리하고
            UnsetUI(found);
            //너 해고야.
            uiDictionary.Remove(wantType);
        }
    }
    protected void UnsetUI(UIBase wantUI)
    {
        if (!wantUI) return;

        wantUI.Unregistration(this);
    }
    public static void ClaimUnsetUI(UIBase wantUI) => GameManager.Instance?.UI?.UnsetUI(wantUI);
    public static void ClaimUnsetUI(GameObject wantObject) => ClaimUnsetUI(wantObject?.GetComponent<UIBase>());

    protected UIBase SetUI(UIBase wantUI)
    {
        wantUI?.Registration(this);
        return wantUI;
    }
    protected UIBase SetUI(UIType wantType, UIBase wantUI)
    {
        
        if (wantUI == null) return null; 

        
        if (uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin;

        
        uiDictionary.Add(wantType, wantUI);
        
        return SetUI(wantUI);
    }
    public static UIBase ClaimSetUI(UIBase wantUI) => GameManager.Instance?.UI?.SetUI(wantUI);
    public static UIBase ClaimSetUI(GameObject wantObject) => ClaimSetUI(wantObject?.GetComponent<UIBase>());
    public static UIBase ClaimSetUI(UIType wantType, UIBase wantUI) => GameManager.Instance?.UI?.SetUI(wantType, wantUI);

    protected UIBase GetUI(UIType wantType)
    {
        if (uiDictionary.TryGetValue(wantType, out UIBase result)) return result; //있으면 result반환
        else return null; //없으면 null
    }
    public static UIBase ClaimGetUI(UIType wantType) => GameManager.Instance?.UI?.GetUI(wantType);

    protected UIBase OpenUI(UIType wantType)
    {
        
        UIBase result = GetUI(wantType);
        
        if (result is IOpenable asOpenable) asOpenable.Open();

        if (result) EventSystem.current.SetSelectedGameObject(result.gameObject);
        
        return result;
    }
    public static UIBase ClaimOpenUI(UIType wantType) => GameManager.Instance?.UI?.OpenUI(wantType);

    protected UIBase CloseUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        //             자료형    이름   =>  변수 생성
        if (result is IOpenable asOpenable) asOpenable.Close();
        return result;
    }
    public static UIBase ClaimCloseUI(UIType wantType) => GameManager.Instance?.UI?.CloseUI(wantType);

    protected UIBase ToggleUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Toggle();
        return result;
    }

    protected UIBase OpenScreen(UIType wantType)
    {
        CloseUI(CurrentScreen);
        _currentScreenType = wantType;
        return OpenUI(wantType);
    }

    public static UIBase ClaimOpenScreen(UIType wantType) => GameManager.Instance?.UI?.OpenScreen(wantType);
    public static void ClaimOpenScreen(UIType wantScreen, ScreenChageType changeType)
        => GameManager.Instance?.UI?.OpenScreen(wantScreen, changeType);
    protected void OpenScreen(UIType wantScreen, ScreenChageType changeType)
    {
        ClaimScreenChangeEffect(changeType, () => OpenScreen(wantScreen));
    }
    protected void ScreenChangeEffectStart(ScreenChageType wantType, System.Action endFunction = null)
    {
        if (currentScreenChanger) return;
        if (screenChangerDictionary.TryGetValue(wantType, out UI_ScreenChanger result))
        {
            if (!result)
            { 
                endFunction?.Invoke();
                return;
            } 
            result.gameObject.SetActive(true);
            result?.ChangeStart(endFunction);
            currentScreenChanger = result;
        }
        else 
        {
            endFunction?.Invoke();
        }
    }
    public static void ClaimScreenChangeEffectStart(ScreenChageType wantType, System.Action endFunction = null)
        => GameManager.Instance?.UI?.ScreenChangeEffectStart(wantType, endFunction);

    public static void ClaimScreenChangeEffect(ScreenChageType wantType, System.Action endFunction = null)
        => GameManager.Instance?.UI?.ScreenChangeEffectStart(wantType, endFunction + ClaimScreenChangeEffectEnd);
    protected void ScreenChangeEffectEnd()
    {
        if (currentScreenChanger == null) return;
        GameObject targetObject = currentScreenChanger.gameObject;
        currentScreenChanger.ChangeEnd(() => targetObject.SetActive(false));
        currentScreenChanger = null;
    }
    public static void ClaimScreenChangeEffectEnd() => GameManager.Instance?.UI?.ScreenChangeEffectEnd();
    public static UIBase ClaimToggleUI(UIType wantType) => GameManager.Instance?.UI?.ToggleUI(wantType);

    public static void ClaimPopUp(string title, string context, string confirm)
    {
        OnPopUp?.Invoke(title, context, confirm);
    }
    public static void ClaimErrorMessage(string context)
    {
        OnPopUp?.Invoke("Error", context, "Confirm");
    }
}
