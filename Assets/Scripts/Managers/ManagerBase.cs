using System.Collections;
using UnityEngine;

public abstract class ManagerBase : MonoBehaviour
{
    GameManager _connectedManager;
    public virtual int LoadCount => 1;
    public IEnumerator Connect(GameManager newManager)
    {
        if(_connectedManager != null) Disconnect();
        _connectedManager = newManager;
        yield return OnConnected(newManager);
    }

    protected abstract IEnumerator OnConnected(GameManager newManager);

    public void Disconnect()
    {
        _connectedManager = null;
        OnDisconnected();
    }

    protected abstract void OnDisconnected();
}
