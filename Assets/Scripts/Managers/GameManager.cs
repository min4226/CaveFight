using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public delegate void InitializeEvent();
public delegate void UpdateEvent(float deltaTime);
public delegate void DestroyEvent();

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance => _instance;

    UIManager _ui;
    public UIManager UI => _ui;

    InputManager _input;
    public InputManager Input => _input;

    DataManager _data;
    public DataManager Data => _data;

    CameraManager _camera;
    public CameraManager Camera => _camera;

    ObjectManager _objectM;
    public ObjectManager ObjectM => _objectM;

    SettingManager _setting;
    public SettingManager Setting => _setting;

    StageManager _stage;
    public StageManager Stage => _stage;

    [SerializeField] private Stage startStage;

    public Stage StageObject { get; private set; }

    [SerializeField] private MonsterSpawn monsterSpawn;
    public MonsterSpawn MonsterSpawnPoint { get; private set; }

    IEnumerator initializing;

    public static event InitializeEvent OnInitializeManager;
    public static event InitializeEvent OnInitializeController;
    public static event InitializeEvent OnInitializeCharacter;
    public static event InitializeEvent OnInitializeObject;

    public static event UpdateEvent OnUpdateManager;
    public static event UpdateEvent OnUpdateController;
    public static event UpdateEvent OnUpdateCharacter;
    public static event UpdateEvent OnUpdateObject;
    public static event UpdateEvent OnPhysicsCharacter;
    public static event UpdateEvent OnPhysicsObject;

    public static event DestroyEvent OnDestroyManager;
    public static event DestroyEvent OnDestroyController;
    public static event DestroyEvent OnDestroyCharacter;
    public static event DestroyEvent OnDestroyObject;

    [SerializeField] UIType startScreen;

    public static bool is2D = true;
    bool isLoading = true;
    bool isPlaying = true;
    void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        StageObject = startStage;
        MonsterSpawnPoint = monsterSpawn;
        initializing = InitializeManagers();
        StartCoroutine(initializing);

    }

    private void Update()
    {
        if (isLoading) return;
        InvokeInitializeEvent(ref OnInitializeManager);
        InvokeInitializeEvent(ref OnInitializeCharacter);
        InvokeInitializeEvent(ref OnInitializeController);
        InvokeInitializeEvent(ref OnInitializeObject);


        if (isPlaying)
        {
            float deltaTime = Time.deltaTime;

            OnUpdateManager?.Invoke(deltaTime);
            OnUpdateController?.Invoke(deltaTime);
            OnUpdateCharacter?.Invoke(deltaTime);
            OnUpdateObject?.Invoke(deltaTime);
        }

        InvokeDestroyEvent(ref OnDestroyObject);
        InvokeDestroyEvent(ref OnDestroyController);
        InvokeDestroyEvent(ref OnDestroyCharacter);
        InvokeDestroyEvent(ref OnDestroyManager);

    }

    private void FixedUpdate()
    {
        if (isLoading || !isPlaying) return;
        float deltaTime = Time.fixedDeltaTime;
        OnPhysicsCharacter?.Invoke(deltaTime);
        OnPhysicsObject?.Invoke(deltaTime);
    }
    IEnumerator InitializeManagers()
    {
        int totalLoadCount = 0;
        totalLoadCount += CreateManager(ref _ui).LoadCount;
        totalLoadCount += CreateManager(ref _data).LoadCount;
        totalLoadCount += CreateManager(ref _input).LoadCount;
        totalLoadCount += CreateManager(ref _camera).LoadCount;
        totalLoadCount += CreateManager(ref _objectM).LoadCount;
        totalLoadCount += CreateManager(ref _setting).LoadCount;
        totalLoadCount += CreateManager(ref _stage).LoadCount;



        yield return UI.Initialize(this);
        UIBase loadingUI = UIManager.ClaimOpenScreen(UIType.Loading);

        UI_LoadingScreen loadingScreen = loadingUI as UI_LoadingScreen;
        IProgress<int> loadingProgress = loadingUI as IProgress<int>;

        loadingProgress?.Set(0, totalLoadCount);

        yield return Data.Connect(this);
        loadingProgress?.AddCurrent(1);

        yield return ObjectM.Connect(this);
        loadingProgress?.AddCurrent(1);

        yield return UI.Connect(this);
        loadingProgress?.AddCurrent(1);

        yield return Input.Connect(this);
        loadingProgress?.AddCurrent(1);

        yield return Camera.Connect(this);
        loadingProgress?.AddCurrent(1);

        yield return Stage.Connect(this);
        loadingProgress?.AddCurrent(1);

        yield return Setting.Connect(this);
        loadingProgress?.AddCurrent(1);

        yield return null;

       

        loadingScreen?.SetComplete();
        isLoading = false;
        Pause();

        InputManager.OnAnyKey -= ShowTitleScreen; 
        InputManager.OnAnyKey += ShowTitleScreen;
        yield return new WaitUntil(() => isPlaying);
    }
        void DeleteManagers()
    {
        Input?.Disconnect();
        ObjectM?.Disconnect();
        UI?.Disconnect();
        Setting?.Disconnect();
        Stage?.Disconnect();
    }

    void OnDistroy()
    {
        if (initializing != null) StopCoroutine(initializing);
        DeleteManagers();
    }

    ManagerType CreateManager<ManagerType>(ref ManagerType targetVariable) where ManagerType : ManagerBase
    {
        if (targetVariable == null)
        {
            targetVariable = this.TryAddComponent<ManagerType>();

        }
        return targetVariable;
    }
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
    public static void Pause()
    {
        Instance.isPlaying = false;
    }

    public static void Unpause()
    {
        Instance.isPlaying = true;
    }

    /*public static GameObject CreateObject(GameObject prefab)
    {
        if (prefab == null) return null;
        GameObject result = Instantiate(prefab);
        RegistrationObject(result);
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
        Destroy(target);
    }
    public static void UnregistrationObject(GameObject target)
    {
        if (!target) return;
        foreach (var current in target.GetComponentsInChildren<IFunctionable>())
        { 
            current.UnregistrationFunctions();
        }
    }*/

    public void InvokeInitializeEvent(ref InitializeEvent OriginEvent)
    {
        if (OriginEvent != null)
        {
            InitializeEvent currentInitializeCharacter = OriginEvent;
            OriginEvent = null;
            currentInitializeCharacter.Invoke();
        }
    }

    public void InvokeDestroyEvent(ref DestroyEvent OriginEvent)
    {
        if (OriginEvent != null)
        {
            DestroyEvent CurrentEvent = OriginEvent;
            OriginEvent = null;
            CurrentEvent.Invoke();
        }
    }

    void ShowTitleScreen()
    {
        InputManager.OnAnyKey -= ShowTitleScreen;

        UIManager.ClaimOpenScreen(UIType.Title);
        Unpause();
    }
}
