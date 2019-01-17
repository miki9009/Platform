using UnityEngine;
using UnityEngine.UI;

public class PowerUpDisplay : MonoBehaviour
{
    public Image image;
    public Text text;
    [HideInInspector]public int time;
    float curTime = 1;
    int seconds;
    int minutes;
    [HideInInspector] public PowerUp powerUp;
    [HideInInspector] public CollectionType type;
    private void Start()
    {
        Effects.ScalePulse(transform, 2, 1);
        GameManager.LevelChanged += (x) => time = 0;
    }

    public void ResetTo(PowerUp pwr)
    {
        time = pwr.time;
        powerUp = pwr;
        curTime = 1;
        ConvertTime();
        Debug.Log("Display Reset: Type " + type + " Seconds: " + seconds);
    }

    public void ConvertTime()
    {
        minutes = time / 60;
        seconds = time - minutes * 60;
        ChangeText();
    }

    public void ChangeText()
    {
        if (seconds < 1 && minutes > 0)
        {
            minutes--;
            seconds = 59;
        }
        else
        {
            seconds--;
        }
        text.text = string.Format("{0}:{1:00}", minutes, seconds);
        //Debug.Log(string.Format("{0}:{1:00}", minutes, seconds));
    }

    private void FixedUpdate()
    {
        if (curTime > 0)
        {
            curTime -= Time.fixedDeltaTime;
        }
        else
        {
            curTime = 1;
            time--;
            ChangeText();
            if (time <= 0)
            {
                powerUp.Disable();
                Destroy(gameObject);
            }
        }
    }
}
