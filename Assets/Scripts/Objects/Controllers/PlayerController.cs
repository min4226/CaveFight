using UnityEngine;

public class PlayerController : ControllerBase
{
    public bool CanMove { get; set; }

    [SerializeField] private Transform model;

    protected override void OnPossess(CharacterBase newCharacter)
    {
        base.OnPossess(newCharacter);

        InputManager.OnMouseRightButton -= MoveToMousePosition;
        InputManager.OnMouseRightButton += MoveToMousePosition;

        InputManager.OnMove -= MoveToDirection;
        InputManager.OnMove += MoveToDirection;

        InputManager.OnAttack -= Attack;
        InputManager.OnAttack += Attack;
    }

    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        base.OnUnpossess(oldCharacter);

        InputManager.OnMouseRightButton -= MoveToMousePosition;
        InputManager.OnMove -= MoveToDirection;
        InputManager.OnAttack -= Attack;
    }

    public void MoveToMousePosition(
        bool value,
        Vector2 screenPosition,
        Vector3 worldPosition)
    {
        if (!CanMove) return;

        if (value)
        {
            CommandMoveToDestination(worldPosition, 0.0f);
        }
    }

    public void MoveToDirection(Vector2 value)
    {
        if (!CanMove) return;

        CommandMoveToDirection(value);
    }

    private void Attack(bool value)
    {
        if (!value) return;
        if (!CanMove) return;

        Debug.Log("Attack ÇÔ¼ö ½ÇÇàµÊ");

        CommandAttack();
    }

    public void SetDead()
    {
        CanMove = false;
    }
}