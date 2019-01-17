using UnityEngine;
using UnityEngine.UI;

public class RequiredUI : MonoBehaviour
{
    public Text requiredLabel;
    public Text currentPossesionLabel;
    public Image image;
    [System.NonSerialized]
    public Transform target;
    public event System.Action Enabled;

    private void OnEnable()
    {
        target = Controller.Instance.gameCamera.transform;
        if (Enabled != null)
        {
            Enabled();
        }
    }

    private void Update()
    {
        if(target!=null)
            transform.LookAt(target);
    }

    public void Assign(int required, int possesed, Sprite sprite)
    {
        currentPossesionLabel.color = possesed >= required ? Color.white : Color.red;
        requiredLabel.text = "/" + required;
        currentPossesionLabel.text = possesed.ToString();
        image.sprite = sprite;
    }

    
}