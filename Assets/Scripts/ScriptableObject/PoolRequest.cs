using UnityEngine;

[System.Serializable]
public struct PoolSetting
{
    public string poolName;
    public GameObject target;
    public uint countInitial;
    public uint countAdditional;
}
[CreateAssetMenu(fileName = "PoolRequest", menuName = "PoolRequests/DefaultPoolRequest")]
public class PoolRequest : ScriptableObject
{
    public PoolSetting[] settings;
}
