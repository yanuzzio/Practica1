using UnityEngine;

public class ItemsRotation : MonoBehaviour
{
    //Rotación de los objectos

    public float speedRotationY;

    void Update()
    {
        transform.Rotate(0, speedRotationY, 0);
    }
}
