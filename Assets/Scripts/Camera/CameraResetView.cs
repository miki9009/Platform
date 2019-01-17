using UnityEngine;

public class CameraResetView : MonoBehaviour
{
    public void ResetView()
    {
        Controller.Instance?.gameCamera?.ResetView();
    }
}