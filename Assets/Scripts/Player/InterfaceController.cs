using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    public PlayerController playerController;
    public Text interfaceText;
    public Text interfaceControllText;
    public Text pausedText;
    bool bInterfaceGame;
    bool bPauseGame;

    void Update()
    {
        InterfaceGame();

        GamePause();
    }

    void InterfaceGame() //Función para mostrar interface en pantalla
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            interfaceText.gameObject.SetActive(false);
            bInterfaceGame = !bInterfaceGame;
        }

        if (bInterfaceGame) interfaceControllText.gameObject.SetActive(true);
        else interfaceControllText.gameObject.SetActive(false);
    }

    void GamePause() //Función para pausar el juego
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            bPauseGame = !bPauseGame;
        }

        if(bPauseGame)
        {
            Time.timeScale = 0;
            pausedText.gameObject.SetActive(true);
            playerController.gameEnable = false;  
        }
        else
        {
            Time.timeScale = 1;
            pausedText.gameObject.SetActive(false);
            playerController.gameEnable = true;
        }
    }
}
