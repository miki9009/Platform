using UnityEngine;
using UnityEngine.UI;

public class SceneLevelElement : MonoBehaviour
{
    [LevelSelector]
    public string scene;

    public ScrollRectElement element;

    private void Start()
    {
        element.Selected += Element_Selected;
    }

    private void Element_Selected()
    {
        LevelButtonsCreator.PrepareButtons(MissionsConfig.Instance.GetMissions(scene));
    }
}