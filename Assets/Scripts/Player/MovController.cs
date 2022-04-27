using UnityEngine;
using UnityEngine.UI;

public class MovController : MonoBehaviour
{

    [Header("--Camera--")]
    public Transform cam;
    public float hSpeed;
    public float vSpeed;
    float vMouse;
    float hMouse;
    float yLimit = 0.0f;

    [Header ("--Player--")]
    public CharacterController cc;
    public Image crossHair;
    public float velMov;
    public float velInitial;
    public float velRun;
    public float minDistanceEnemy;
    float x;
    float z;
    Vector3 move;
    float distance;

    [Header("--Jump--")]
    public float jumpForce;
    public float gravity = -15f;
    public bool isGrounded;
    float jumpValue;
    Vector3 velocity;

    void Start()
    {
        velInitial = velMov;

        Cursor.lockState = CursorLockMode.Locked;

        jumpValue = Mathf.Sqrt(jumpForce * -2 * gravity);
    }

    void Update()
    {
        //Condición para cuando el Player tenga vida 0 o menos no se ejecuten más acciones.
        if (GetComponent<PlayerController>().actualLife <=0) return;

        if (isGrounded && velocity.y < 0) velocity.y = gravity;

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        EnemyDetected();

        MovementCamera();
        SensibilityChange();
        MovementPlayer();
        Run();
        Jump();
    }

    void MovementPlayer() //Movimiento del Player en XZ
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        move = (transform.right * x + transform.forward * z).normalized;
        cc.Move(move * velMov * Time.deltaTime);
    }
    void MovementCamera() //Rotación de la cámara
    {
        hMouse = Input.GetAxis("Mouse X") * hSpeed * Time.deltaTime;
        vMouse += Input.GetAxis("Mouse Y") * vSpeed * Time.deltaTime;

        yLimit = -vMouse;
        yLimit = Mathf.Clamp(yLimit, -80f, 80f);

        transform.Rotate(0f, hMouse, 0f);
        cam.localRotation = Quaternion.Euler(yLimit, 0f, 0f);
    }
    void EnemyDetected() //Función para la detección del enemigo a cierta distancia del jugador
    {
        crossHair = crossHair.GetComponent<Image>();
        Transform closestEnemy = null;
        EnemyController[] allEnemies = GameObject.FindObjectsOfType<EnemyController>();

        foreach (EnemyController go in allEnemies)
        {
            distance = Vector3.Distance(go.transform.position, transform.position);

            if (distance < minDistanceEnemy)
            {
                closestEnemy = go.transform;
                crossHair.color = Color.red;
            }
            else if (distance >= minDistanceEnemy && closestEnemy == null)
            {
                crossHair.color = Color.white;
            }
        }
    }
    void SensibilityChange()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4) && hSpeed > 10) hSpeed -= 10f;
        else if (Input.GetKeyDown(KeyCode.Keypad6)) hSpeed += 10f;

        if (Input.GetKeyDown(KeyCode.Keypad2) && vSpeed > 10) vSpeed -= 10f;
        else if (Input.GetKeyDown(KeyCode.Keypad8)) vSpeed += 10f;

        if(Input.GetKeyDown(KeyCode.Keypad5))
        {
            hSpeed = 120;
            vSpeed = 100;
        }
    }
    void Run() //Función de Run
    {
        //Condición de según el arma activada será la velRun
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            if (SwitchWeapon.instance.bM4A4Enable)
            {
                velMov = velRun * 0.8f ; 
            }
            else if (SwitchWeapon.instance.bAK47Enable)
            {
                velMov = velRun * 0.6f;
            }
            else velMov = velRun;
        }
        else velMov = velInitial;
    }
    void Jump() //Función de saltar
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            velocity.y = jumpValue;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit) //Colisión del Player con el tag Ground
    {
        //Comparamos el tag. Si no estamos tocando el suelo pasará a true cuando entremos en contacto con él
        if(hit.collider.CompareTag("Ground"))
        {
            if (isGrounded == false) isGrounded = true; 
        }
    }
}
