using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("--Text--")]
    public Text spaceBagText;
    public Text kitText;
    public Text lifeText;
    public Text ammoText;
    public Text bulletCountText;
    public Text ammoNullText;
    public Text healNullText;
    public Text objTakedText;
    public Text gameOverText;
    public Text lightEnableText;
    public Text exitGameText;
    
    [Header ("--Attributes--")]
    public int actualScene;
    public int actualLife;
    public int numBullet;
    public int numAmmo;
    public int numKit;
    public int maxSpaceBag;
    public float recoilForce;
    public bool canShoot;
    public bool gameEnable;
    int maxLife = 20;
    int spaceBag;
    int health;

    [Header("--References--")]
    public Transform cam;
    public GameObject posGun;
    public GameObject zoomIn;
    public GameObject zoomOut;
    public GameObject bulletPrefab;
    public GameObject ammoGO    ;
    public LayerMask ignoreLayer;
    public SwitchWeapon switchWeapon;
    RaycastHit hit;

    [Header ("--ZoomCamera--")]
    public int zoom = 20;
    public float smooth = 5f;
    public bool isZoomed;
    int normal = 70;
    int shootRange;
    Vector3 direction;

    void Start()
    {
        ammoGO = GameObject.FindGameObjectWithTag("Ammo");

        actualLife = maxLife;
        
        spaceBag = numAmmo + numKit;
        canShoot = true;
    }

    void Update()
    {
        //Booleano que cambia en InterfaceController
        if (gameEnable == false) return;

        GameOver();
        ReloadScene();

        CameraLight();

        CameraZoom();
        ShootGun();

        ReloadWeapon();
        Heal();

        if (Input.GetKeyDown(KeyCode.L)) SceneManager.LoadScene(0);
        if (Input.GetKeyDown(KeyCode.K)) Application.Quit();
    }

    void CameraLight() //Función para activar y desactivar la linterna
    {
        Light camLight = cam.GetComponent<Light>();
        if (Input.GetKeyDown(KeyCode.F))
        {
            camLight.enabled = !camLight.enabled;
        }

        //Text que se imprime en pantalla para avisar sobre el estado
        if (camLight.enabled) lightEnableText.gameObject.SetActive(true);
        else lightEnableText.gameObject.SetActive(false);
    }
    void CameraZoom() //Función para el zoom de la cámara
    {
        Camera camComp = cam.GetComponent<Camera>();

        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed;
        }
        //Al hacer zoom el alcance aumentará en 25 unidades y la precisión será exacta
        if (isZoomed)
        {
            camComp.fieldOfView = Mathf.Lerp(camComp.fieldOfView, zoom, Time.deltaTime * smooth);
            posGun.transform.localPosition = Vector3.Slerp(posGun.transform.localPosition, zoomIn.transform.localPosition, Time.deltaTime * 10f);
            direction = cam.forward;
            shootRange = 75;
        }
        //Al no hacer zoom se tendrá el alcance inicial y una presición Random de entre -0.01 y 0.01 en el eje X e Y
        else
        {
            camComp.fieldOfView = Mathf.Lerp(camComp.fieldOfView, normal, Time.deltaTime * smooth);
            posGun.transform.localPosition = Vector3.Slerp(posGun.transform.localPosition, zoomOut.transform.localPosition, Time.deltaTime * 5f);
            direction = cam.TransformDirection(new Vector3(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), 1));
            shootRange = 50;
        }
    }
    void ShootGun() //Función de disparo. Instacia el prefab de Bullet y utiliza Raycast para detección de colliders
    {
        posGun = GameObject.FindGameObjectWithTag("Weapon");

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            posGun.transform.position = posGun.transform.position - posGun.transform.forward * (recoilForce/50f);  //Retroceso físico al disparar

            GameObject bulletObj = Instantiate(bulletPrefab);
            bulletObj.transform.position = posGun.transform.position;

            if (Physics.Raycast(cam.position, direction, out hit, shootRange, ~ignoreLayer))
            {
                bulletObj.transform.LookAt(hit.point);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        hit.rigidbody.AddForce(cam.forward * 20f, ForceMode.Impulse);       //Retroceso fisico del enemigo al impacto de la bala.
                        hit.transform.GetComponent<EnemyController>().HitBulletGun();
                    }

                    if (hit.collider.gameObject.CompareTag("LastEnemy"))                     //Últimos enemigo de la escena 2.
                    {
                        hit.transform.GetComponent<EnemyController>().HitBulletGunLast();
                    }

                    if (hit.collider.gameObject.CompareTag("BlindSpot"))                // Impacto al punto débil del objeto.
                    {
                        hit.transform.GetComponent<EnemyController>().DestroyEnemy();
                    }
                }
            }
            Vector3 dir = cam.position + cam.forward * 10f;
            bulletObj.transform.LookAt(dir);

            BulletCount();
        }
    }
    void BulletCount() //Función para el recuento de balas usadas
    {
        numBullet--;
        bulletCountText.text = numBullet + " / 20";

        ammoGO = GameObject.FindGameObjectWithTag("Ammo");

        if (numBullet == 0)
        {
            ammoGO.gameObject.SetActive(false);
            canShoot = false;
        }
    }
    public void Heal() //Función para sumar life al usar el Input
    {
        if (Input.GetKeyDown(KeyCode.Q) && numKit > 0 && actualLife < maxLife)
        {
            health = Random.Range(2, 5);
            actualLife += health;
            lifeText.text = " Life " + actualLife;

            numKit--;
            kitText.text = "Kit: " + numKit;

            spaceBag--;
            spaceBagText.text = "Bag: " + spaceBag;

        }
        else if (Input.GetKeyDown(KeyCode.Q) && numKit <= 0) //Texto que se imprime en pantalla cuando la cantidad de kits es 0
        {
            StartCoroutine(WaitForHealNull());
        }
        else if (actualLife >= maxLife)     //Condición para que la vida actual no supere la vida máxima
        {
            actualLife = maxLife;
            lifeText.text = " Life " + actualLife;
        }
    }
    public void ReloadWeapon() //Función para recargar munición
    {
        if (Input.GetKeyDown(KeyCode.R) && numAmmo > 0)
        {
            numAmmo--;
            ammoText.text = "Ammo: " + numAmmo;

            numBullet = 20;
            bulletCountText.text = numBullet + " / 20";

            spaceBag--;
            spaceBagText.text = "Bag: " + spaceBag;

            ammoGO.gameObject.SetActive(true);

            canShoot = true;
        }
        else if (Input.GetKeyDown(KeyCode.R) && numAmmo <= 0) //Texto que se imprime en pantalla cuando la cantidad de cartuchos es 0
        {
            StartCoroutine(WaitForRealoadNull());
        }
    }
    void Bag() //Función para añadir elementos a la mochila
    {
        spaceBag++;
        spaceBagText.text = "Bag: " + spaceBag;
    }
    public void KitTaked() //Función para cuando se recoge un kit
    {
        numKit++;
        kitText.text = "Kit: " + numKit;
    }
    public void AmmoTaked() //Función para cuando se recoge un cartucho
    {
        numAmmo++;
        ammoText.text = "Ammo: " + numAmmo;
    }
    public void GameOver() //Función de derrota
    {
        if (actualLife <= 0)
        {
            StartCoroutine(WaitForDead());
            return;
        }
    }
    public void WeaponChangeWithZoom() //Función llamada en SwitchWeapon para salir del Zoom cuando se cambia de arma
    {
        if (isZoomed) isZoomed = false;
    }
    public void EnemyDamage() //Función de daño del enemigo llamada en EnemyController
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");

        actualLife -= enemy.GetComponent<EnemyController>().damageToPlayer;
        lifeText.text = " Life " + actualLife;
    }
    void ReloadScene() //Función para recargar el nivel
    {
        if (Input.GetKeyDown(KeyCode.J)) SceneManager.LoadScene(actualScene);
    }
    private void OnTriggerStay(Collider other)
    {
        //Condiciones para cuando se recogen items. Si la Bag está llena se reemplazará por el item opuesto
        if(other.gameObject.tag == "Cartridge" && Input.GetKey(KeyCode.E) && spaceBag < maxSpaceBag)
        {
            AmmoTaked();
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
            Bag();
        }
        else if ((other.gameObject.tag == "Cartridge" && Input.GetKey(KeyCode.E) && spaceBag == maxSpaceBag))
        {
            AmmoTaked();
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
            numKit--;
            kitText.text = "Kit: " + numKit;
        }

        if (other.gameObject.tag == "Kit" && Input.GetKey(KeyCode.E) && spaceBag < maxSpaceBag)
        {
            KitTaked();
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
            Bag();
        }
        else if ((other.gameObject.tag == "Kit" && Input.GetKey(KeyCode.E) && spaceBag == maxSpaceBag))
        {
            KitTaked();
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
            numAmmo--;
            ammoText.text = "Ammo: " + numAmmo;
        }

        //Recoger AK-47. Vuelve true al respectivo booleano y el arma se vuelve utilizable
        if(other.gameObject.layer == 10 && Input.GetKey(KeyCode.E))
        {
            Destroy(other.gameObject);
            switchWeapon.bWeaponAk = true;
            objTakedText.gameObject.SetActive(false);
        }

        //Recoger M4A4. Vuelve true al respectivo booleano y el arma se vuelve utilizable
        if (other.gameObject.layer == 9 && Input.GetKey(KeyCode.E))
        {
            Destroy(other.gameObject);
            switchWeapon.bWeaponM4 = true;
            objTakedText.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Activación del Spwan al entrar en el trigger del objeto
        if(other.gameObject.tag == "Spawn") other.gameObject.transform.GetChild(0).gameObject.SetActive(true);

        //Texto que se imprime en pantalla al entrar en el trigger del objeto
        if (other.gameObject.tag == "Cartridge" || other.gameObject.tag == "Kit") objTakedText.gameObject.SetActive(true);

        //Condición de derrota al tocar el agua
        if (other.gameObject.layer == 4) actualLife -= 100;

        //M4a4
        if (other.gameObject.layer == 9) objTakedText.gameObject.SetActive(true);

        //Ak-47
        if (other.gameObject.layer == 10) objTakedText.gameObject.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        ///Desactivación del texto en pantalla del respectivo objeto
        
        if (other.gameObject.tag == "Cartridge" || other.gameObject.tag == "Kit") objTakedText.gameObject.SetActive(false);

        if (other.gameObject.layer == 9) objTakedText.gameObject.SetActive(false);

        if (other.gameObject.layer == 10) objTakedText.gameObject.SetActive(false);
    }
    IEnumerator WaitForRealoadNull() //Corrutina que se ejecuta cuando la cantidad de cartuchos es 0
    {
        ammoNullText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        ammoNullText.gameObject.SetActive(false);
    }
    IEnumerator WaitForHealNull() //Corrutina que se ejecuta cuando la cantidad de kits es 0
    {
        healNullText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        healNullText.gameObject.SetActive(false);
    }
    IEnumerator WaitForDead() //Corrutina que se ejecuta cuando el Player pierde la vida
    {
        gameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(actualScene);
    }
}
