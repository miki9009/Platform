using Engine.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SettingsWindow : UIWindow
{
    public Text resolutionLabel;
    public Text showFpsLabel;
    public Text graphicsLabel;
    public Text lowFrameRateLabel;
    int index;
    Vector2 currentRes = new Vector2(1920, 1080);
    bool lowFrameRate;
    public string[] graphicsLevel;
    int selectedGraphics = 1;

    static SettingsWindow instance;
    bool buttonsMovement;
    bool showFps;

    private void Awake()
    {
        instance = this;
        BeginShow += AssignSettings;
        AssignSettings();
    }

    private void AssignSettings()
    {
        showFps = DataManager.Settings.showFps;
        selectedGraphics = DataManager.Settings.graphicsLevel;
        buttonsMovement = DataManager.Settings.buttonMovement;
        currentRes =  DataManager.Settings.resolution;
        lowFrameRate = DataManager.Settings.lowFrameRate;
        GameQualitySettings.ChangeQuality(selectedGraphics);

        graphicsLabel.text = graphicsLevel[selectedGraphics];
        SetLowFrameRateLoad(DataManager.Settings.lowFrameRate);
        SetFpsShowString(showFps);
        SetFPS();
        Screen.SetResolution((int)currentRes.x, (int)currentRes.y, true);
        SetResolutionString(currentRes.x, currentRes.y);
        Debug.Log("Settings Loaded");
    }


    public static void SetQuality()
    {
        instance.selectedGraphics = DataManager.Settings.graphicsLevel;
    }

    public override void OnHidden()
    {
        base.OnHidden();
        SetFPS();
        AssignSettings();
        Debug.Log("Target frame rate: " + (DataManager.Settings.lowFrameRate ? 30 : 60));

    }

    void SetFPS()
    {
        if (DataManager.Settings.lowFrameRate)
        {
            Application.targetFrameRate = 30;
            //QualitySettings.vSyncCount = 2;
        }
        else
        {
            Application.targetFrameRate = 60;
            //QualitySettings.vSyncCount = 1;
        }
    }

    public void Save()
    {
        Screen.SetResolution((int)currentRes.x, (int)currentRes.y,true);

        DataManager.Settings.resolution = currentRes;
        DataManager.Settings.buttonMovement = buttonsMovement;
        DataManager.Settings.showFps = showFps;
        DataManager.Settings.graphicsLevel = selectedGraphics;
        DataManager.Settings.lowFrameRate = lowFrameRate;
        DataManager.SaveData();
    }

    public void ChangeResolution()
    {
        index++;
        if (index >= GameQualitySettings.CachedResolutions.Length)
        {
            index = 0;
        }
        currentRes = GameQualitySettings.CachedResolutions[index];
        SetResolutionString(currentRes.x, currentRes.y);
    }

    void SetResolutionString(float width, float height)
    {
        resolutionLabel.text = string.Format("{0}x{1}", width, height);
    }

    void SetFpsShowString(bool showFps)
    {
        showFpsLabel.text = showFps ? "ON" : "OFF";
    }

    public void ChangeMovementType()
    {
        buttonsMovement = !buttonsMovement;
    }

    public void ChangeShowFps()
    {
        showFps = !showFps;
        SetFpsShowString(showFps);
    }

    public void SetGraphicsLevel()
    {
        if(selectedGraphics < graphicsLevel.Length-1)
        {
            selectedGraphics++;
        }
        else
        {
            selectedGraphics = 0;
        }
        graphicsLabel.text = graphicsLevel[selectedGraphics];
    }

    public void SetLowFrameRate()
    {
        lowFrameRate = !lowFrameRate;
        lowFrameRateLabel.text = lowFrameRate ? "ON" : "OFF";
    }

    public void SetLowFrameRateLoad(bool val)
    {
        lowFrameRate = val;
        lowFrameRateLabel.text = lowFrameRate ? "ON" : "OFF";
    }



}