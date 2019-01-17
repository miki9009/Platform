using Engine;
using UnityEngine;
using UnityEngine.UI;

public class WagonArrows : MonoBehaviour
{
    public Image leftArrow;
    public Image rightArrow;
    public Text distanceLabel;

    float maxCritical;
    Color wagonColor;


    CharacterWagon _wagon;
    public CharacterWagon Wagon
    {
        get
        {
            return _wagon;
        }
        set
        {
            if (value != null)
            {
                maxCritical = value.maxCritical;
                gameObject.SetActive(true);
            }

            _wagon = value;
        }
    }

    private void Awake()
    {
        wagonColor = new Color(1, 1, 1, 0);
        gameObject.SetActive(false);
        Character.CharacterCreated += Init;
        GameManager.LevelClear += Restart;
    }

    private void OnDestroy()
    {
        Character.CharacterCreated -= Init;
        GameManager.LevelClear -= Restart;
    }

    void Restart()
    {
        gameObject.SetActive(false);
    }

    void Init(Character character)
    {
        if(character.IsLocalPlayer && character.movement.GetType() == typeof(CharacterWagon))
        {
            Wagon = character.movement as CharacterWagon;
        }
    }

    int frame = 0;
    private void Update()
    {
        if(_wagon!=null)
        {
            if(_wagon.criticalDir < 0)
            {
                rightArrow.color = new Color(1, 1, 1, _wagon.critical / maxCritical);
                leftArrow.color = wagonColor;
            }
            else
            {
                leftArrow.color = new Color(1, 1, 1, _wagon.critical / maxCritical);
                rightArrow.color = wagonColor;
            }
            if(frame % 60 == 0)
            {
                frame = 0;
                distanceLabel.text = string.Format("{0:#.##}m", _wagon.distance);
            }

        }

    }

    //void OnGUI()
    //{
    //    if(_wagon!=null)
    //    Draw.TextColor(10, 350, 255, 0, 0, 1, "Critical: " + _wagon.critical);
    //}
}