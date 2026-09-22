using UnityEngine;

public class AnimationModule : CharacterModule
{
    [SerializeField] private Animator anim;
    [SerializeField] private bool isRotationByMovement;

    // 실제 캐릭터가 보이는 Model
    [SerializeField] private Transform model;

    // 현재 바라보는 방향
    private Vector3 currentLookRotation;

    // 공격 중인지
    private bool isAttacking;
    private Quaternion beforeAttackRotation;
    public sealed override System.Type RegistrationType => typeof(AnimationModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        newOwner.OnLookAt -= AnimationByLookRotation;
        newOwner.OnLookAt += AnimationByLookRotation;

        newOwner.OnMovement -= AnimationByMovement;
        newOwner.OnMovement += AnimationByMovement;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);

        oldOwner.OnLookAt -= AnimationByLookRotation;
        oldOwner.OnMovement -= AnimationByMovement;
    }

    public void AnimationByLookRotation(Vector3 lookRotation)
    {
        currentLookRotation = lookRotation;

        // 걷기 방향을 바라보는 순간 Model은 원래 방향으로 복구
        if (lookRotation.x != 0)
        {
            model.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        anim.SetFloat("MoveX", lookRotation.x);
        anim.SetFloat("MoveY", lookRotation.y);
    }

    public void AnimationByMovement(Vector3 moveDelta)
    {
        if (!anim) return;

        if (moveDelta.sqrMagnitude > 0)
        {
            // 걷기 시작하면 Model 회전 0
            model.localRotation = Quaternion.Euler(0f, 0f, 0f);

            if (isRotationByMovement)
            {
                AnimationByLookRotation(moveDelta);
            }
        }

        anim.SetFloat(
            "MoveSpeed",
            moveDelta.magnitude / Time.fixedDeltaTime
        );
    }

    public void PlayAttack()
    {
        if (!anim || isAttacking) return;

        isAttacking = true;

        // 공격하기 직전 회전 저장
        beforeAttackRotation = model.localRotation;

        if (currentLookRotation.x < 0)
        {
            model.localRotation = Quaternion.Euler(0f, 180f, 0f);
            Debug.Log("왼쪽 공격");
        }
        else if (currentLookRotation.x > 0)
        {
            model.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Debug.Log("오른쪽 공격");
        }

        anim.SetTrigger("Attack");
    }

    // Attack 애니메이션 마지막 프레임의 Animation Event
    public void ResetAttackRotation()
    {
        if (currentLookRotation.x < 0)
        {
            model.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            model.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        isAttacking = false;
    }
    public void ResetModelRotation()
    {
        model.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Debug.Log("걷기 상태 → Rotation 0");
    }
}