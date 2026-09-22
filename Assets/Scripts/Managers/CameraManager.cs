using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraManager : ManagerBase
{
    public Camera MainCamera { get; private set; }
    
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        SetMainCamera(Camera.main);
        yield return null;
    }

    protected override void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void SetMainCamera(Camera wantCamera)
    { 
        MainCamera = wantCamera;
        
    }

    public void GetRaycastResult(Vector2 screenPosition, List<RaycastResult> outResult)
    {
        EventSystem currentEvent = EventSystem.current;
        PointerEventData eventData = new(currentEvent);
        eventData.position = screenPosition;
        currentEvent.RaycastAll(eventData, outResult);
        
    }
    
}
