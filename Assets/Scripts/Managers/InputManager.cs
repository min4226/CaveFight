using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public delegate void MouseMoveEvent(Vector2 screenPosition, Vector3 worldPosition);
public delegate void MouseButtonEvent(bool value, Vector2 screenPosition, Vector3 worldPosition);
public delegate void MouseHoverEvent(GameObject newTarget, GameObject oldTarget);
public delegate void ButtonEvent(bool value);
public delegate void VectorEvent(Vector2 value);
public delegate void AxisEvent(float value);
public delegate void AttackEvent(bool value);

[RequireComponent(typeof(PlayerInput))]
public class InputManager : ManagerBase
{
    public static event MouseButtonEvent OnMouseLeftButton;
    public static event MouseButtonEvent OnMouseRightButton;
    public static event MouseMoveEvent OnMouseMove;
    public static event MouseHoverEvent OnMouseHover;
    public static event ButtonEvent OnCancel;
    public static event ButtonEvent OnShowStatus;
    public static event VectorEvent OnMove;
    public static event Action OnAnyKey;
    public static event AttackEvent OnAttack;

    PlayerInput targetInput;
    Dictionary<string, InputAction> actionDictionary = new();
    List<RaycastResult> cursorHitList = new();

    GameObject cursorHoverObject;
    Vector2 cursorScreenPosition;
    Vector3 cursorWorldPosition;

    

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        targetInput = GetComponent<PlayerInput>();

        LoadAllActions();
        InitializeAllActions();

        
        GameManager.OnUpdateManager -= UpdateEvent; 
        GameManager.OnUpdateManager += UpdateEvent;
        yield return null;
    }

    protected override void OnDisconnected()
    {
        GameManager.OnUpdateManager -= UpdateEvent;
    }

    public void UpdateEvent(float deltaTime)
    {
        RefreshGameObjectUnderCursor(cursorScreenPosition);
    }

    void RefreshGameObjectUnderCursor(Vector2 screenPosition)
    {
        cursorHitList.Clear();

        GameManager.Instance.Camera.GetRaycastResult(screenPosition, cursorHitList);

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GameObject firstObject = null;

        // 마우스에 닿은 오브젝트가 없으면 종료
        if (cursorHitList.Count == 0)
        {
            GameObject lastHoverObject = cursorHoverObject;

            cursorScreenPosition = screenPosition;
            cursorWorldPosition = worldPosition;
            cursorHoverObject = null;

            if (lastHoverObject != null)
            {
                OnMouseHover?.Invoke(null, lastHoverObject);
            }

            return;
        }

        if (GameManager.is2D)
        {
            worldPosition.z = 0;

            float GetValue(RaycastResult target)
            {
                return target.sortingOrder + target.sortingLayer * 100000;
            }

            RaycastResult nearest = cursorHitList.GetMaximum<RaycastResult>(GetValue);
            firstObject = nearest.gameObject;
        }
        else
        {
            float GetDistance(RaycastResult target)
            {
                return target.distance;
            }

            RaycastResult nearest = cursorHitList.GetMinimum<RaycastResult>(GetDistance);

            firstObject = nearest.gameObject;
            worldPosition = nearest.worldPosition;
        }

        GameObject lastHoverObject2 = cursorHoverObject;

        cursorScreenPosition = screenPosition;
        cursorWorldPosition = worldPosition;
        cursorHoverObject = firstObject;

        if (lastHoverObject2 != firstObject)
        {
            OnMouseHover?.Invoke(firstObject, lastHoverObject2);
        }
    }

    public GameObject GetGameObjectUnderCursor()
    {
        //마우스에 닿은것의 개수가 0이라면 => 없으니까 돌아가라
        if (cursorHitList.Count == 0) return null;
        return cursorHitList[0].gameObject; //일단 지금은 임시로 첫 번째 오브젝트 돌려주기!
    }

    void LoadAllActions()
    {
        foreach (InputAction currentAction in targetInput.actions)
        {
            actionDictionary.TryAdd(currentAction.name, currentAction);
        }
    }

    void InitializeAllActions()
    {
        if (actionDictionary == null || actionDictionary.Count == 0) return;

        InitializeAction("CursorPositionChanged", (context) => CursorPositionChanged(GetVector2Value(context)));
        InitializeAction("Move", (context) => OnMove?.Invoke(GetVector2Value(context))
                               , (context) => OnMove?.Invoke(Vector2.zero));

        InitializeAction("MouseLeftButton",
                                 (context) => OnMouseLeftButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition),
                                 (context) => OnMouseLeftButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition));

        InitializeAction("MouseRightButton",
                                 (context) => OnMouseRightButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition),
                                 (context) => OnMouseRightButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition));

        InitializeAction("ShowStatusButton", (context) => OnShowStatus?.Invoke(true)
                                               , (context) => OnShowStatus?.Invoke(false));

        InitializeAction("Cancel", (context) => OnCancel?.Invoke(true));
        InitializeAction("AnyKey", (context) => OnAnyKey?.Invoke());
        InitializeAction("Attack", (context) => OnAttack?.Invoke(true));
    }

    void InitializeAction(string actionName, Action<InputAction.CallbackContext> actionMethod, Action<InputAction.CallbackContext> cancelMethod = null)
    {
        if (actionDictionary == null) return;
        if (actionDictionary.TryGetValue(actionName, out InputAction currentInput))
        {
            if(actionMethod is not null) currentInput.performed += actionMethod;
            if(cancelMethod is not null) currentInput.canceled += cancelMethod;
        }
    }

    T GetInputValue<T>(InputAction.CallbackContext context) where T : struct
    {
        if (context.valueType != typeof(T)) return default;
        return context.ReadValue<T>();
    }

    Vector2 GetVector2Value(InputAction.CallbackContext context) => GetInputValue<Vector2>(context);

    void CursorPositionChanged(Vector2 screenPosition)
    {
        
        RefreshGameObjectUnderCursor(screenPosition);

        



        //대리자는 모든 스킬을 한 번에 사용할 수 있는 친구 => 사기캐
        //....배운 스킬이 없으면?
        OnMouseMove?.Invoke(cursorScreenPosition, cursorWorldPosition);
    }
}
