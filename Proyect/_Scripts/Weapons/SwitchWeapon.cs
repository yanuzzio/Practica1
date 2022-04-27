using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwitchWeapon : MonoBehaviour
{
    public static SwitchWeapon instance;
    public PlayerController playerController;
    public Text weaponNullText;

    [Header ("--Bool SwichtEnable--")]
    public int selectWeapon = 0;
    public bool canChange;
    public bool bUMP45Enable;       //Booleanos para diferenciar que arma se está usando y calcular el daño para el enemigo en EnemyController (HitBulletGun y HitBulletGunLast)
    public bool bM4A4Enable;            
    public bool bAK47Enable;

    [Header ("--Bool WeaponEnable--")]  //Booleanos que se activan cuando conseguimos el arma por primera vez. Se resetea al pasar de escena.
    public bool bWeaponUMP;
    public bool bWeaponM4;
    public bool bWeaponAk;

    void Start()
    {
        instance = this;

        canChange = true;
        bUMP45Enable = true;

        SelectWeapon();
    }

    void Update()
    {
        int previousSelect = selectWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1) && canChange)
        {
            bUMP45Enable = true;
            bM4A4Enable = false;
            bAK47Enable = false;
            selectWeapon = 0;
            StartCoroutine(WaitForChange());

            playerController.WeaponChangeWithZoom();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2 && canChange && bWeaponM4)
        {
            bM4A4Enable = true;
            bUMP45Enable = false;
            bAK47Enable = false;
            selectWeapon = 1;
            StartCoroutine(WaitForChange());

            playerController.WeaponChangeWithZoom();
        }
        //Condición para cuando no hemos conseguido el arma
        else if ((Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2 && canChange && !bWeaponM4))
        {
            StartCoroutine(WaitForWeapon());
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3 && canChange && bWeaponAk)
        {
            bAK47Enable = true;
            bUMP45Enable = false;
            bM4A4Enable = false;
            selectWeapon = 2;
            StartCoroutine(WaitForChange());

            playerController.WeaponChangeWithZoom();
        }
        //Condición para cuando no hemos conseguido el arma
        else if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3 && canChange && !bWeaponAk)
        {
            StartCoroutine(WaitForWeapon());
        }

        if (previousSelect != selectWeapon) 
        {
            SelectWeapon();
        }
    }
    void SelectWeapon() //Función para el cambio de arma
    {
        int i = 0;

        foreach (Transform weapon in transform)
        {
            if (i == selectWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
    IEnumerator WaitForChange() //Corrutina para intervalo entre cambio de arma
    {
        canChange = false;
        yield return new WaitForSeconds(3f);
        canChange = true;
    }
    IEnumerator WaitForWeapon()
    {
        weaponNullText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        weaponNullText.gameObject.SetActive(false);
    }
}
