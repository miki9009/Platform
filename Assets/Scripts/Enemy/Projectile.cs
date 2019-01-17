using UnityEngine;

public class Projectile : MonoBehaviour
{

    public Rigidbody rb;
    IShooter shooter;
    public GameObject explosion;

    private void OnCollisionEnter(Collision collision)
    {
        CancelInvoke();
        //Instantiate(explosion, transform.position, Quaternion.identity);
        StaticParticles.CreateExplosion(transform.position); 
        if (collision.gameObject.layer == Layers.Character)
        {
            collision.transform.GetComponent<CharacterMovement>().Hit();
        }
        ResetMe();
    }

    public void Shoot(IShooter shooter, Vector3 direction, float force, bool useGravity = true)
    {
        this.shooter = shooter;
        gameObject.SetActive(true);
        transform.rotation = Quaternion.LookRotation(direction);
        rb.velocity = Vector3.zero;
        rb.useGravity = useGravity;
        rb.AddForce(direction * force, ForceMode.VelocityChange);
        shooter.Trails.transform.position = transform.position;
        shooter.Trails.Play();
        Invoke("ResetMe", 5);
    }

    private void Update()
    {
        if(shooter != null && shooter.Trails!=null)
            shooter.Trails.transform.position = transform.position;
    }

    void ResetMe()
    {
        if(shooter!=null)
        {
            shooter.ResetProjectile(this);
            shooter.Trails.Stop();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}