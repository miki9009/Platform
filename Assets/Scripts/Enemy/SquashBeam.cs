using UnityEngine;

public class SquashBeam : MonoBehaviour
{
    public float maxY = 10;
    public Transform upper;
    public float speed;
    bool up;
    Vector3 startPos;
    Vector3 downPos = new Vector3(0, -1, 0);
    GameCamera cam;
    public ParticleSystem parts;
    public CollisionBroadcast collisionBroadcast;

    [Header("Shake options")]
    public float time;
    public float force;
    [Range(0,1)]
    public float amplitude;

    bool shakeActivated;

    private void Awake()
    {
        upper.localPosition = new Vector3(0, maxY, 0);
        startPos = upper.localPosition;
        collisionBroadcast.CollisionEntered += (Collision collision) =>
        {
            if (!up && collision.gameObject.layer == Layers.Character)
            {
                collision.gameObject.GetComponent<CharacterMovement>().Hit(null, 100, true);
            }
        };
    }

    private void Start()
    {

        cam = Controller.Instance.gameCamera.GetComponent<GameCamera>();
    }

    private void Update()
    {
        if(up)
        {
            upper.localPosition = Vector3.Lerp(upper.localPosition, startPos, Time.deltaTime * speed / 20);
            if(upper.localPosition.y > maxY-1)
            {
                up = false;
            }
        }
        else
        {
            upper.localPosition = Vector3.Lerp(upper.localPosition, downPos, Time.deltaTime * speed);
            
            if(upper.localPosition.y < 0)
            {
                up = true;
                if (shakeActivated)
                {
                    cam.Shake(time, force, amplitude);
                    parts.Play();
                }
            }
        }     

    }

    private void OnTriggerEnter(Collider other)
    {
        shakeActivated = true;
    }

    private void OnTriggerExit(Collider other)
    {
        shakeActivated = false;
    }
}