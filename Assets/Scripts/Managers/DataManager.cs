
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class DataManager : ManagerBase
{
    
   
    static Dictionary<System.Type, Dictionary<string, Object>> dataDictionary = new();
    event System.Action DisconnectEvent;
    //프로퍼티는 변수모양이지만 함수
    //				int GetLoadCount();
    public override int LoadCount
    {
        get
        {
            
            var task = Addressables.LoadResourceLocationsAsync("Global");
            var result = task.WaitForCompletion();
            int count = result.Count;
            task.Release();
            return count; //그래서 그 개수를 돌려줌!
        }
    }

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        Debug.Log("🔥 DataManager OnConnected 시작");

        UIBase loading = UIManager.ClaimGetUI(UIType.Loading);
        IProgress<int> progressUI = loading as IProgress<int>;
        IStatus<string> statusUI = loading as IStatus<string>;

        int loaded = 0;
        int total = LoadCount;

        Debug.Log($"🔥 Data LoadCount : {total}");

        string loadString = "Load Data";

        System.Action ProgressOnLoad = () =>
        {
            loaded++;

            Debug.Log($"🔥 Asset Loaded : {loaded}/{total}");

            progressUI?.AddCurrent(1);
            statusUI?.SetCurrentStatus($"{loadString} ({loaded}/{total})");
        };

        loadString = "Load Game Objects";

        Debug.Log("🔥 GameObject Load 시작");

        yield return LoadAllFromAssetBundle<GameObject>(
            "Global",
            ProgressOnLoad
        ).WaitForTask();

        Debug.Log("🔥 GameObject Load 완료");

        loadString = "Load Pool Requests";

        Debug.Log("🔥 PoolRequest Load 시작");

        yield return LoadAllFromAssetBundle<PoolRequest>(
            "Global",
            ProgressOnLoad
        ).WaitForTask();

        Debug.Log("🔥 PoolRequest Load 완료");

        yield return null;
    }
    protected override void OnDisconnected()
    {
        DisconnectEvent?.Invoke();
        DisconnectEvent = null;
    }

    
    bool TryGetFileFromResources<T>(string path, out T result) where T : Object
    {
        //Resources.LoadAll<T>(path);
        result = Resources.Load<T>(path);
        return result != null;
    }

    
    public static void SaveDataFile<T>(T target) where T : Object
    {
        if (target == null) return;
        Dictionary<string, Object> innerDictionary;

        //지금까지 이런 Object는 없었다. 처음보는 Type이다
        //innerDictionary가 존재하지 않을 것이기 때문에!
        if (!dataDictionary.TryGetValue(typeof(T), out innerDictionary))
        {
            //만들어야 한다!
            innerDictionary = new();
            //만들어서 해당 타입으로 등록해주기!
            dataDictionary.Add(typeof(T), innerDictionary);
        }

        //이 밑에부터는 무조건 innerDictionary가 있다!
        innerDictionary.TryAdd(target.name.ToLower(), target);
    }

    protected static T GetDataFromDictionary<T>(string fileName) where T : Object
    {
        //1.글자가 없을 때 fileName is null	   nullString
        //2.글자가 없을 때 fileName.length == 0 emptyString
        if (string.IsNullOrEmpty(fileName)) return null;

        fileName = fileName.ToLower();
        //룬 문자를 찾겠다 : 사전을 찾음 => 사전을 못 찾았어요 => 그런 거 없는데요?
        if (dataDictionary.TryGetValue(typeof(T), out Dictionary<string, Object> innerDictionary))
        {
            if (innerDictionary.TryGetValue(fileName, out Object result))
            {
                return result as T; //사전도 있었고 들어가보니까 파일도 있던데?
            }
        }

        //else는 안 적어야 위에 있는 두 겹의 if를 모두 처리 가능!
        return null;
    }

    public static T LoadDataFile<T>(string fileName) where T : Object
    {
        T result = GetDataFromDictionary<T>(fileName);
        if (!result) UIManager.ClaimErrorMessage(SystemMessage.FileNameNotFound(fileName));
        return result;
    }

    public static bool TryLoadDataFile<T>(string fileName, out T result) where T : Object
    {
        result = GetDataFromDictionary<T>(fileName);
        return result;
    }

    
    public async Task LoadAllFromAssetBundle<T>(string label, System.Action actionForEachLoad) where T : Object
    {
        //                                 V                (매개변수) => { 내용 }
        var finder = Addressables.LoadAssetsAsync<T>(label, (T loaded) =>
        {
            SaveDataFile(loaded); //로드 되었으니까 저장해 놓아야지 ㅎㅎ
            actionForEachLoad();  //할 일 있다고 하니까 해줘야지 ㅎㅎ
        });
        Task result = finder.Task;
        await result;
        DisconnectEvent += () => finder.Release();
    }

    public async void LoadFileFromAssetBundle<T>(string address) where T : Object
    {
        //기다리긴 하는데, "비동기"로 기다릴 거임!
        var finder = Addressables.LoadAssetAsync<T>(address);
        await finder.Task; //Start / Run에 해당하는 부분!
        SaveDataFile(finder.Result);
        finder.Release();
      
    }
}