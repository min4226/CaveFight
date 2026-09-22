using UnityEngine;
using UnityEngine.EventSystems;

public delegate void DragStartEvent(UI_DraggableWindow dragTarget, Vector2 startPosition);

public class UI_DraggableWindow : UIBase, IPointerDownHandler
{
    public event DragStartEvent OnDragStart;

    //드래그하면 어떤 트랜스폼을 움직여야 할까?
    [SerializeField] RectTransform rootTransform;

    /// <summary> 마지막으로 수신받은 마우스의 위치 </summary>
    Vector2 currentScreenPosition;
    Vector2 shiftedPosition;
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDragStart?.Invoke(this, eventData.position);
    }

    public void SetMouseStartPosition(Vector2 screenPosition)
    {
        currentScreenPosition = screenPosition;
        shiftedPosition = Vector2.zero;
    }

    public void SetMouseCurrentPosition(Vector2 screenPosition)
    {
        
        Vector2 screenDelta = screenPosition - currentScreenPosition;
        currentScreenPosition = screenPosition;

        if (shiftedPosition.x * screenDelta.x > 0.0f)
        {
            float counter = Mathf.Min(Mathf.Abs(screenDelta.x), Mathf.Abs(shiftedPosition.x));
            counter *= Mathf.Sign(shiftedPosition.x);
            shiftedPosition.x -= counter;
            screenDelta.x -= counter;
        }
        if (shiftedPosition.y * screenDelta.y > 0.0f)
        {
            float counter = Mathf.Min(Mathf.Abs(screenDelta.y), Mathf.Abs(shiftedPosition.y));
            counter *= Mathf.Sign(shiftedPosition.y);
            shiftedPosition.y -= counter;
            screenDelta.y -= counter;
        }
        if (screenDelta.sqrMagnitude == 0.0f) return;
        Rect rootRect = rootTransform.rect;

        
        rootRect.position += (Vector2)(rootTransform.localPosition / UIManager.UIScale) + screenDelta;
        
        Vector2 overScreen = rootRect.InversedAABB(UIManager.UIBoundary);
        shiftedPosition += overScreen;
        screenDelta += overScreen;

        Vector3 positionDelta = (Vector3)screenDelta;

        if (UIManager.UIScale > 0.0f) positionDelta /= UIManager.UIScale;

        rootTransform.localPosition += positionDelta;
        
    }
}