using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject[] spawnEnemys;
    public int numEnemys;
    public int maxEnemys;
    public int _enemydead;
    int index;

    void Start()
    {
        InvokeRepeating("EnemyGenerate", 0, 3f);    //Se crean enemigos en el instante 0 y cada 3 segundos.
    }

    public void EnemyGenerate() //Spawn de enemigos desde un array público
    {
        if (numEnemys < maxEnemys)
        {
            float x = Random.Range(-10f, 10);
            float y = 0f;
            float z = Random.Range(-10f, 10);
            Vector3 pos = new Vector3(x, y, z);

            index = Random.Range(0, spawnEnemys.Length); 
            Instantiate(spawnEnemys[index], transform.position + pos, Quaternion.identity);
            numEnemys++;
        }
    }
}
