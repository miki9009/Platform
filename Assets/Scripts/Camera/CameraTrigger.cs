using Engine;
using System.Collections;
using UnityEngine;

public class CameraTrigger : LevelElement
{
    public Transform camDummy;

    public float speed = 1;
    Vector3 localPosition;
    public BoxCollider boxCollider;
    Coroutine routine;

    public GameCamera GameCam
    {
        get; private set;
    }

    private void Start()
    {
        camDummy.gameObject.SetActive(false);
        GameCam = Controller.Instance.gameCamera;
    }

    private void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponentInParent<Character>();
        if(character.IsLocalPlayer)
        {
            if(routine==null)
            {
                routine = StartCoroutine(SetCamera());
                Debug.Log("Run Coroutine CameraTrigger");
            }
        }
    }

    IEnumerator SetCamera()
    {
        float time = 0;
        if(GameCam == null)
        {
            Debug.LogError("Game camera is null");
            yield break;
        }
        while (time < 1)
        {
            time += Time.deltaTime * speed;
            GameCam.localPosition = Vector3.Lerp(GameCam.localPosition, new Vector3(-localPosition.x,localPosition.y, localPosition.z), time);
            yield return null;
        }
        routine = null;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data!=null)
        {
            if(data.ContainsKey("LocalPosition"))
                localPosition = (Float3)data["LocalPosition"];
            camDummy.localPosition = localPosition;
            if (data.ContainsKey("Speed"))
                speed = (float)data["Speed"];

            if (data.ContainsKey("BoxSize"))
                boxCollider.size = (Float3)data["BoxSize"];
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        data["Speed"] = speed;
        data["LocalPosition"] = (Float3)camDummy.localPosition;
        data["BoxSize"] = (Float3)boxCollider.size;
    }

#if UNITY_EDITOR
    [Header("Only in editor")]
    public Mesh mesh;
    public Material mat;
    Transform dummyPlayer;
    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        if(dummyPlayer == null)
        {
            dummyPlayer = new GameObject("DummyPlayer").transform;
            dummyPlayer.SetParent(transform);
            dummyPlayer.localPosition = Vector3.zero;
            dummyPlayer.gameObject.AddComponent<MeshRenderer>().sharedMaterial = mat;
            dummyPlayer.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            dummyPlayer.localRotation = Quaternion.Euler(-90, 0, 0);
            dummyPlayer.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        }
        camDummy.LookAt(transform);
    }
#endif
}
