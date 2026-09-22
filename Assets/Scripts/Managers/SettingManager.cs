using System.Collections;
using UnityEngine;

public class SettingManager : ManagerBase
{
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        Application.targetFrameRate = 60;

        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        yield return null;
    }

    protected override void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    
}
