using System;
using UnityEngine;

public class UI_TargetHoverInfo : OpenableUIBase
{
    [SerializeField] Vector2 shiftedPosition;
    [SerializeField] TMPro.TextMeshProUGUI nameText;
    
    CharacterBase target;
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        InputManager.OnMouseHover += HoverInfoChange;

        //InputManager.OnMouseMove -= MoveToMouse;
        //InputManager.OnMouseMove += MoveToMouse;

        GameManager.OnUpdateObject -= MoveToTarget;
        GameManager.OnUpdateObject += MoveToTarget;

    }

    private void Update()
    {
        if(target == null) return;
        transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + (Vector3)shiftedPosition;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        //InputManager.OnMouseMove -= MoveToMouse;
        GameManager.OnUpdateObject -= MoveToTarget;

    }

    private void MoveToTarget(float deltaTime)
    {
        if (target == null) return;
        transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + (Vector3)shiftedPosition;
    }

    void HoverInfoChange(GameObject newTarget, GameObject oldTarget)
    {
        CharacterBase asCharacter = newTarget?.GetComponent<CharacterBase>();
        if (asCharacter)
        {
            nameText.SetText(newTarget.name);
            Open();
        } 
        else Close();
        target = asCharacter;
    }
    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = screenPosition + shiftedPosition;
    }
}
