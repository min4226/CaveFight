using UnityEngine;

public abstract class AIController : ControllerBase
{
    [SerializeField] private GameObject _focusTarget = null;

    public GameObject FocusTarget => _focusTarget;

    protected abstract void Think(float deltaTime);

    public GameObject SetFocusTarget(GameObject newTarget)
    {
        if (IsFocussable(newTarget))
        {
            GameObject oldTarget = _focusTarget;

            _focusTarget = newTarget;

            OnFocusTargetChanged(oldTarget, newTarget);
        }

        return _focusTarget;
    }

    protected virtual bool IsFocussable(GameObject target)
    {
        return target != _focusTarget;
    }

    protected virtual void OnFocusTargetChanged(
        GameObject oldTarget,
        GameObject newTarget)
    {
    }
}