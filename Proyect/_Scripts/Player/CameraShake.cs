using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //Scrip para el shake de la cámara usado cuando el enemigo coliciona con el Player
    [Header ("--CameraShake")]
    public float duration = 1.5f;
    public float magnitude = 1.5f;

    public IEnumerator WaitForShake() //Corrutina que se ejecuta mientras elapse sea menor a duration. Al finalizar vuelve a su posición original
    {
        Vector3 originalPos = transform.localPosition;

        float elapse = 0f;

        while (elapse < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, originalPos.y, originalPos.z);

            elapse += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
