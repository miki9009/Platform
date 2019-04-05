using UnityEngine;
using UnityEngine.UI;

public class CharacterProgressPanel : MonoBehaviour
{
    public Character Character { get; private set; }
    public Text characterLabelName;
    public Text placementLabel;
    public Text waypointLabel;
    public Text lapLabel;
    public RectTransform rectTransform;
    

    public void Initialize(Character character)
    {
        this.Character = character;
        characterLabelName.text = Character.IsBot ? "BOT" : "Player";
        Character.PlacementChanged += Character_PlacementChanged;
        Character.WaypointVisited += Character_WaypointVisited;
        Character.Dead += Character_Dead;
    }

    private void Character_Dead(Character character)
    {
        if (character == this.Character)
            Destroy(gameObject);
    }

    private void Character_WaypointVisited(int index)
    {
        Debug.Log("Waypoint visited BOT: " + Character.IsBot + " index: " + index);
        waypointLabel.text = index.ToString();
        lapLabel.text = Character.gameProgress.lap.ToString();
    }

    private void Character_PlacementChanged(int placement)
    {
        placementLabel.text = placement.ToString();
        CharacterProgressPanelManager.Instance.RefreshPositions();
    }

    private void OnDestroy()
    {
        if (!Character) return;
        Character.PlacementChanged -= Character_PlacementChanged;
        Character.WaypointVisited -= Character_WaypointVisited;
        Character.Dead -= Character_Dead;
    }
}