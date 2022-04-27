using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class NarrativText1 : MonoBehaviour
{
    [Header ("--Text--")]
    public Text mision1;
    public Text mision2;
    public Text mision3;
    public Text mision4;
    public Text clue1;
    public Text clue2;
    public Text clue3;
    public Text endLevel;
    public Text objTakedText;

    [Header ("--Booleanos--")] //Booleanos que se van activando de forma progresiva a medida que se completa las misiones
    public bool misionEnable;
    public bool bmision1;
    public bool bmision2;
    public bool bmision3;
    public bool bmision4;
    public bool bendMision;
    public bool clue1Taked;
    public bool clue2Taked;
    int clueCount;              //Contador de pistas recogidas

    [Header ("--References--")]
    public GameObject _clue3;
    public GameObject textNarrative;

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
            textNarrative.gameObject.SetActive(true);
            Mision1();
        }
        else textNarrative.gameObject.SetActive(false);
    }

    void Mision1() //Función de la misión principal de la escena 1. Las misiones se activarán cuando la anterior se cumpla.
    {
        //La misión 1 empezará desde unity por defecto

        if(bmision2)
        {
            mision2.gameObject.SetActive(true);
            mision2.text = "The town has been attacked. Investigate the area and find where your family is" + "\n Clues " + clueCount + " / 2";
        }
        if(bmision3)
        {
            _clue3.gameObject.SetActive(true);
            mision3.gameObject.SetActive(true);
            mision3.text = "All the people have been killed. Follow the blood trail in the mountains to find out who the culprits are ...";
        }
        if(bmision4)
        {
            mision4.gameObject.SetActive(true);
            mision4.text = "You found the body of your executed wife ... Pick up the note they left ...";
        }
        if (bendMision) StartCoroutine(WaitForEnd());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Mision1")
        {
            Destroy(mision1.gameObject);
            Destroy(other.gameObject);
            bmision2 = true;
        }
        if(other.gameObject.tag == "Mision4" && clue1Taked && clue2Taked)
        {
            Destroy(mision3.gameObject);
            Destroy(other.gameObject);
            bmision3 = false;
            bmision4 = true;
        }

        if (other.gameObject.tag == "Clue1" || other.gameObject.tag == "Clue2" || other.gameObject.tag == "Clue3") objTakedText.gameObject.SetActive(true);
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Mision3" && clue1Taked && clue2Taked)
        {
            Destroy(mision2.gameObject);
            Destroy(other.gameObject);
            bmision2 = false;
            bmision3 = true;
        }

        if (other.gameObject.tag == "Clue1" && Input.GetKey(KeyCode.E))
        {
            StartCoroutine(WaitForClue1());
            clue1Taked = true;
            clueCount++;
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Clue2" && Input.GetKey(KeyCode.E))
        {
            StartCoroutine(WaitForClue2());
            clue2Taked = true;
            clueCount++;
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Clue3" && Input.GetKey(KeyCode.E))
        {
            Destroy(other.gameObject);
            objTakedText.gameObject.SetActive(false);
            StartCoroutine(WaitForClue3());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Clue1" || other.gameObject.tag == "Clue2" || other.gameObject.tag == "Clue3") objTakedText.gameObject.SetActive(false);
    }
    IEnumerator WaitForClue1() //Corrutina para imprimir en pantalla el texto de la pista 1
    {
        clue1.gameObject.SetActive(true);
        yield return new WaitForSeconds(10f);
        clue1.gameObject.SetActive(false);
    } 
    IEnumerator WaitForClue2() //Corrutina para imprimir en pantalla el texto de la pista 2
    {
        clue2.gameObject.SetActive(true);
        yield return new WaitForSeconds(10f);
        clue2.gameObject.SetActive(false);
        clueCount++;
    }
    IEnumerator WaitForClue3() //Corrutina para imprimir en pantalla el texto de la pista 3
    {
        clue3.gameObject.SetActive(true);
        yield return new WaitForSeconds(10f);
        clue3.gameObject.SetActive(false);
        bmision4 = false;
        bendMision = true;
    }
    IEnumerator WaitForEnd() //Corrutina para imprimir en pantalla el texto de fin de escena 1
    {
        endLevel.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(1);
    }

    /* EXPLICACIÓN MISIONES:
     * La misión principal empieza por la Misión1 (activa desde el unity). Esta se desactivará cuando el player llegue al punto por defecto colicionando con un trigger
     * donde la bmision2 pasará a ser true y se ejecutará. Cuando el Player recolecte las 2 pistas de la bmisión2 que se encuentran en el pueblo, se activará la bmisión3,
     * pasando a false la bmisión2.
     * Luego la última misión, bmisión4 se actvará al colicionar con el trigger correspondiente. Al ejecutarse la corrutina se pasará de escena.
     */
}
