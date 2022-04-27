using UnityEngine;

public class BulletControl : MonoBehaviour
{
    //Script para la duración del proyectil al generarse

    public float speed;
    public float lifeDuration;
    float lifeTimer;

    void Start()
    {
        lifeTimer = lifeDuration;
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }
}
