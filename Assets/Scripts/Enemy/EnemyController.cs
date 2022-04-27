using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("--References--")]
    public NarrativText2 narrativText2;
    public CameraShake cameraShake;
    public GameObject mainCamera;
    public GameObject playerTarget;
    public GameObject player;
    public Rigidbody rb;
    private PlayerController playerController;

    [Header("--Characteristic--")]
    public int damageToPlayer;
    public float damageUMP;
    public float damageM4;
    public float damageAK;
    public float life;
    public float moveSpeedXZ;
    public float moveSpeedY;
    public float minDistance;
    public float rotationSpeed;
    float distance;
    bool hitBullet;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerTarget = GameObject.FindWithTag("Player");
        mainCamera = GameObject.FindWithTag("MainCamera");
        playerController = playerTarget.GetComponent<PlayerController>();

        InvokeRepeating("ChangeDirection", 7, 5);          //Invocación NPC. Cambia de dirección en el segundo 7 y vuelve a repetir cada 3 s
    }

    void Update()
    {
        //Condición para cuando el Player tenga vida 0 o menos no se ejecuten más acciones.
        if (playerController.actualLife <= 0)   
        {
              Destroy(gameObject);
              return;
        }

        Movement();
    }

    void Movement() //Movimiento de los NPC
    {
        distance = Vector3.Distance(playerTarget.transform.position, transform.position);

        if(!mainCamera.GetComponent<Light>().enabled)
        {
            //A una distancia determinada el GameObject persigue al Player cuando la linterna está apagada
            if (distance < minDistance)
            {
                transform.LookAt(playerTarget.transform);
                rb.velocity = (transform.forward * moveSpeedXZ * 1.7f) + (transform.up * moveSpeedY);
            }
            else rb.velocity = transform.forward * moveSpeedXZ + (transform.up * moveSpeedY);
        }
        else 
        {
            //Aumentará la distancia mínima en un 30% si la linterna está prendida
            if (distance < (minDistance * 1.3f))
            {
                transform.LookAt(playerTarget.transform);
                rb.velocity = (transform.forward * moveSpeedXZ * 1.4f) + (transform.up * moveSpeedY);
            }
            else rb.velocity = transform.forward * moveSpeedXZ + (transform.up * moveSpeedY) * Time.deltaTime;
        }

        //Al atacar al enemigo con las balas, este se dirigue al player con 50% más de velocidad
        if (hitBullet)
        {
            transform.LookAt(playerTarget.transform);
            rb.velocity = transform.forward * moveSpeedXZ * (moveSpeedXZ * 0.5f);
        }
    }
    private void ChangeDirection() // NPC cambia de dirección en InvokeRepeating de Start
    {
        transform.Rotate(0, Random.Range(0, 360), 0);
    }
    private void OnCollisionEnter(Collision collision) // Colisión entre Player y CámaraShake
    {
        if (collision.gameObject.tag == "Player")
        {
            playerController.EnemyDamage();
            rb.AddForce(-transform.forward * 200f, ForceMode.Impulse);  //Retroceso del enemigo al impactar con el jugador

            StartCoroutine(mainCamera.GetComponent<CameraShake>().WaitForShake());
        }

        //Condición para cuando choque con los objetos (árboles, casas, etc)
        if(collision.gameObject.tag == "Ground" || collision.gameObject.layer == 12)
        {
            transform.Rotate(0, Random.Range(0, 360), 0);
        }
    }
    public void HitBulletGun() //Daño al enemigo según el arma para NPC generales
    {
        if (SwitchWeapon.instance.bUMP45Enable) life = life - damageUMP;
        if (SwitchWeapon.instance.bM4A4Enable) life = life - damageM4;
        if (SwitchWeapon.instance.bAK47Enable) life = life - damageAK;

        hitBullet = true;

        if (life <= 0)
        {
            DestroyEnemy();
        }
    }
    public void HitBulletGunLast() //Daño al enemigo según el arma para NPC finales
    {
        if (SwitchWeapon.instance.bUMP45Enable) life = life - damageUMP;
        if (SwitchWeapon.instance.bM4A4Enable) life = life - damageM4;
        if (SwitchWeapon.instance.bAK47Enable) life = life - damageAK;

        hitBullet = true;

        if (life <= 0)
        {
            narrativText2.deadCount++;
            DestroyEnemy();
        }
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
