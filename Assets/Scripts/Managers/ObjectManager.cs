using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectManager : ManagerBase
{
    
    readonly string[] globalPoolSettings =
    {
        "GlobalCharacterPool",
        "GlobalControllerPool",
        "GlobalEffectPool",
        "GlobalObjectPool",
        "GlobalUIPool",
    };

    
    List<PoolRequest> loadedPoolRequests = new();

    
    static Dictionary<string, ObjectPoolModule> poolDictionary = new();

    
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        RegistrationInHierarchy();
        RegistrationPool(globalPoolSettings);
        InitializePool();

        yield return null;
    }

    protected override void OnDisconnected()
    {

    }

    public static GameObject CreateObject(string wantName, Transform parent = null)
    {
        GameObject result = null;

        wantName = wantName.ToLower();

        if (poolDictionary.TryGetValue(wantName, out ObjectPoolModule pool))
        {
            result = pool.CreateObject(parent);
        }
        else if (DataManager.TryLoadDataFile(wantName, out GameObject prefab) && prefab)
        {
            result = Instantiate(prefab, parent);
        }

        if (!result)
        {
            Debug.LogError($"❌ Object를 찾을 수 없음 : {wantName}");
            return null;
        }

        RegistrationObject(result);

        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Transform parent = null)
    {
        if (prefab == null) return null;

        //                                      누가 주인인가
        GameObject result = Instantiate(prefab, parent); //만들고
        RegistrationObject(result); //등록함
        return result;
    }

    public static GameObject CreateObject(string wantName, Vector3 position)
    {
        GameObject result = CreateObject(wantName);
        if (result) result.transform.position = position;
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Vector3 position)
    {
        GameObject result = CreateObject(prefab);
        if (result) result.transform.position = position;
        return result;
    }

    public static GameObject CreateObject(string wantName, Vector3 position, Quaternion rotation)
    {
        GameObject result = CreateObject(wantName);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = CreateObject(prefab);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
        }
        return result;
    }

    public static GameObject CreateObject(string wantName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject result = CreateObject(wantName);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
            result.transform.localScale = scale;
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject result = CreateObject(prefab);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
            result.transform.localScale = scale;
        }
        return result;
    }

    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Space space = Space.Self)
    {
        GameObject result = CreateObject(wantName, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    break;
            }
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    break;
            }
        }
        return result;
    }

    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Quaternion rotation, Space space = Space.Self)
    {
        GameObject result = CreateObject(wantName, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation; //부모를 기준으로
                    break;
            }
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation; //부모를 기준으로
                    break;
            }
        }
        return result;
    }

    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, Space space = Space.Self)
    {
        GameObject result = CreateObject(wantName, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    result.transform.localScale = scale; //부모를 기준으로
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation;
                    result.transform.localScale = scale;
                    break;
            }
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    result.transform.localScale = scale; 
                    break;
                case Space.Self:
                    result.transform.localPosition = position; 
                    result.transform.localRotation = rotation;
                    result.transform.localScale = scale;
                    break;
            }
        }
        return result;
    }

    public static void RegistrationObject(GameObject target) 
    {
        if (target)
        {
            
            foreach (var current in target.GetComponentsInChildren<IFunctionable>())
            {
                current.RegistrationFunctions();
            }
        }
    }

    public static void DestroyObject(GameObject target)
    {
        if (!target) return;
        UnregistrationObject(target);
        if (target.TryGetComponent(out PooledObject pool)) 
        {
            pool.OnEnqueue(); 
        }
        else 
        {
            Destroy(target); 
        }
    }

    public static void UnregistrationObject(GameObject target)
    {
        if (!target) return;

        foreach (var current in target.GetComponentsInChildren<IFunctionable>())
        {
            current.UnregistrationFunctions();
        }
    }

    public void RegistrationInHierarchy()
    {
        foreach (MonoBehaviour current in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (current is IFunctionable currentFunctinable)
            { 
                currentFunctinable.RegistrationFunctions();
            }
        }
    }
    public void RegistrationPool(string poolName)
    {
        
        poolName = poolName.ToLower();

        
        PoolRequest currentRequest = DataManager.LoadDataFile<PoolRequest>(poolName);
        if (currentRequest == null) return;
        if (currentRequest.settings == null) return;

        loadedPoolRequests.Add(currentRequest);
        
        foreach (PoolSetting currentSetting in currentRequest.settings)
        {
            string currentName = currentSetting.poolName.ToLower();
            GameObject currentPrefab = currentSetting.target;
            
            if (currentPrefab == null) continue;
            
            if (poolDictionary.ContainsKey(currentName)) continue;
            
            poolDictionary.Add(currentName, new(currentSetting));
        }
    }

    
    public void RegistrationPool(params string[] poolNames)
    {
        foreach (string poolName in poolNames)
        {
            
            RegistrationPool(poolName);
        }
    }

    public void InitializePool()
    {
        foreach (ObjectPoolModule currentPool in poolDictionary.Values)
        {
            currentPool?.Initialize();
        }
    }
}
