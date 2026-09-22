using UnityEngine;

public class UI_LoadingScreen : UI_ScreenBase,IProgress<int>, IStatus<string>
{
    
    public bool IsOpen => gameObject.activeSelf;

    public int Current  { get; protected set;}

    public int Max { get; protected set; }

    public float Progress => (float)Current / Max;

    public int AddCurrent(int value) => Set(Current + value, Max);


    public int AddMax(int value) => Set(Current, Max + value);
    

    public void Close() => gameObject.SetActive(false);

    public void Open() => gameObject.SetActive(true);

    public UnityEngine.UI.Slider progressBar;
    public TMPro.TextMeshProUGUI progressText;
    public TMPro.TextMeshProUGUI explainText;

    public GameObject layoutOnComplete;
    public GameObject layoutOnLoading;
    public string SetCurrentStatus(string newText)
    {
        explainText.SetText(newText);
        return newText;
    }

    public void SetComplete()
    { 
        layoutOnComplete.SetActive(true);
        layoutOnLoading.SetActive(false);
    }
    public int Set(int newCurrent)
    {
        Current = Mathf.Min(newCurrent, Max);
        progressBar.value = Progress;
        progressText.SetText($"{Progress * 100.0f : 0.00}%");
        return Current;
    } 
    

    public int Set(int newCurrent, int newMax)
    {
        layoutOnComplete.SetActive(false);
        layoutOnLoading.SetActive(true);
        Max = newMax;
        return Set(newCurrent);
    }

    public void Toggle() => gameObject.SetActive(!IsOpen);


    
}
