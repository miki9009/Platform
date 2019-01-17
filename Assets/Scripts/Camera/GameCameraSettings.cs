using Engine;
using UnityEngine;

public class GameCameraSettings : LevelElement
{
    public float x = 0;
    public float y = 5;
    public float z = -9;
    public float forwardFactor = 1.5f;
    public float rotationSpeed = 5;
    public float upFactor = 3;
    public float speed = 10;

    public override void ElementStart()
    {
        base.ElementStart();
        Apply();
    }

    private void Apply()
    {
        var cam = Controller.Instance.gameCamera.GetComponent<GameCamera>();
        cam.SetCamera(GameCamera.CameraType.NonRotation);
        cam.localPosition.x = x;
        cam.localPosition.y = y;
        cam.localPosition.z = z;
        cam.forwardFactor = forwardFactor;
        cam.rotationSpeed = rotationSpeed;
        cam.upFactor = upFactor;
        cam.speed = speed;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data != null)
        {
            if(data.ContainsKey("X"))
            {
                x = (float)data["X"];
            }
            if (data.ContainsKey("Y"))
            {
                y = (float)data["Y"];
            }
            if (data.ContainsKey("Z"))
            {
                z = (float)data["Z"];
            }
            if (data.ContainsKey("ForwardFactor"))
            {
                forwardFactor = (float)data["ForwardFactor"];
            }
            if (data.ContainsKey("RotationSpeed"))
            {
                rotationSpeed = (float)data["RotationSpeed"];
            }
            if (data.ContainsKey("UpFactor"))
            {
                upFactor = (float)data["UpFactor"];
            }
            if (data.ContainsKey("Speed"))
            {
                speed = (float)data["Speed"];
            }
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        if(data != null)
        {
            data["X"] = x;
            data["Y"] = y;
            data["Z"] = z;
            data["ForwardFactor"] = forwardFactor;
            data["RotationSpeed"] = rotationSpeed;
            data["UpFactor"] = upFactor;
            data["Speed"] = speed;
        }
    }
}