using UnityEngine;

public class ChaseAIController : AIController
{
    protected override void OnPossess(CharacterBase newCharacter)
    {
        Debug.Log($"[ChaseAIController] OnPossess 호출 : {newCharacter}");

        GameManager.OnUpdateController -= Think;
        GameManager.OnUpdateController += Think;

       
    }
    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        GameManager.OnUpdateController -= Think;

    }
    protected override void Think(float deltaTime)
    {
        Debug.Log($"Think 실행 / Target : {FocusTarget}");

        if (!FocusTarget) return;

        CommandMoveToDestination(
            FocusTarget.transform.position,
            1.0f
        );
    }
}
