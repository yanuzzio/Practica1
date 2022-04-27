using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NarrativText2 : MonoBehaviour
{
    [Header ("--Text--")]
    public Text mision1;
    public Text mision2;
    public Text mision3;
    public Text mision4;
    public Text mision5;
    public Text victoryText;
    public Text endGameText;
    public Text objTakedText;

    [Header ("--Booleanos--")]    //Booleanos que se van activando de forma progresiva a medida que se completa las misiones
    public bool misionEnable;
    public bool bmision2;
    public bool bmision3;
    public bool bmision4;
    public bool bmision5;
    public bool bEndGame;
    public bool clue1Taked;
    public int deadCount;       //Contador de enemigos finales matados
    int clueCount;              //Contador de pistas recogidas

    [Header ("--References--")]
    public GameObject textNarrative2;
    public GameObject principalDoor;
    public GameObject lastEnemy;
    public GameObject walls;
    public GameObject keys;


    void Start()
    {
        misionEnable = true;
    }

    void Update()
    {
        //Condición para activar o desactivar el texto de la misión principal en pantalla
        if (Input.GetKeyDown(KeyCode.M)) misionEnable = !misionEnable;
        if (misionEnable)
        {
            textNarrative2.gameObject.SetActive(true);
            Mision2();
        }
        else textNarrative2.gameObject.SetActive(false);
    }

    void Mision2() //Función de la misión principal de la escena 2. Las misiones se activarán cuando la anterior se cumpla.
    {
        //La misión 1 empezará desde unity por defecto

        if (bmision2)
        {
            mision2.gameObject.SetActive(true);
            keys.gameObject.SetActive(true);
            mision2.text = "The front door is closed. Look for the keys in the area and try again" + "\n Keys " + clueCount + " / 3";
        }
        if(bmision3 && clueCount == 3)
        {
            bmision2 = false;
            mision2.gameObject.SetActive(false);
            mision3.gameObject.SetActive(true);
            principalDoor.gameObject.SetActive(false);
            mision3.text = "Search the main house and find your son. \n Tip: you have activated the alarms, be careful.";
        }
        if(clue1Taked)
        {
            bmision3 = false;
            mision3.gameObject.SetActive(false);
            mision4.gameObject.SetActive(true);
            mision4.text = "Leave the enemy base and save your son.";
        }
        if(bmision4)
        {
            mision5.gameObject.SetActive(true);
            mision5.text = "Win the last battle and escape over the bridge.";
        }
        if(deadCount == 6)
        {
            bmision5 = true;
            walls.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Mision1")
        {
            Destroy(other.gameObject);
            Destroy(mision1.gameObject);
            bmision2 = true;
        }

        if(other.gameObject.tag == "Mision3" && clue1Taked)
        {
            clue1Taked = false;  
            bmision4 = true;
            Destroy(other.gameObject);
            Destroy(mision4.gameObject);
            lastEnemy.gameObject.SetActive(true);
            principalDoor.gameObject.SetActive(true);
            walls.gameObject.SetActive(true);
        }
        if(other.gameObject.tag == "Mision4" && bmision5)
        {
            bmision4 = false;
            Destroy(other.gameObject);
            Destroy(mision5.gameObject);
            StartCoroutine(WaitForWin());
        }

        if (other.gameObject.tag == "Key" || other.gameObject.tag == "Clue1") objTakedText.gameObject.SetActive(true);

    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Key" && Input.GetKey(KeyCode.E))
        {
            bmision3 = true;
            clueCount++;
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
        }

        if(other.gameObject.tag == "Clue1" && Input.GetKey(KeyCode.E))
        {
            clue1Taked = true;
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Key" || other.gameObject.tag == "Clue1") objTakedText.gameObject.SetActive(false);
    }
    IEnumerator WaitForWin() //Corrutina que se ejecuta al ganar el juego
    {
        victoryText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        victoryText.gameObject.SetActive(false);
        endGameText.gameObject.SetActive(true);
    }

    /*EXPLICACIÓN MISIONES:
     * La Misión principal empieza por la misión1 (activa desde unity). La misión bmisión2 se ejecutará al colicionar con el trigger en el punto establecido.
     * Luego de juntar las 3 llaves, la bmisión3 pasará a true y la bmisión2 a false. Al cumplir el objetivo de clue1 (rescatar al hijo) se pasará la misión de huida.
     * Al colicionar con el trigger puesto en escena se activará la bmisión4 y cuando el contador de deadCount sea igual a 6 (todos los enemigos derrotados) se abrirán
     * las puertas para escarpar. Luego de esto se ganará al colicionar con el trigger de la bmisión5, lo que activará la corrutina de WaitForWin. 
     * El juego habrá finalizado y se volverá a empezar en la escena 1.
     */
}
