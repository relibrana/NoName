using System.Collections;
using UnityEngine;

public class CloudPool : MonoBehaviour
{
    public int cloudsPerBatch = 2;
    public float spawnRate = 5.0f;
    private float elapsedTime = 0.0f;

    public PoolingManager cloudsPool;
    public float spawnAreaWidth = 10f; // Ancho del área donde las nubes aparecerán

    void Start()
    {
        // Iniciar la primera tanda de nubes
        StartCoroutine(SpawnCloudsBatch());
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Verificar si ha pasado el tiempo de espera para spawnear una nueva nube
        if (elapsedTime >= spawnRate)
        {
            elapsedTime = 0.0f; // Reiniciar el temporizador
        }
    }

    IEnumerator SpawnCloudsBatch()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            // Spawnear una tanda de nubes
            for (int i = 0; i < cloudsPerBatch; i++)
            {
                SpawnCloud(); // Pasamos a SpawnCloud sin parámetros
            }
        }
    }

    void SpawnCloud()
    {
        // Obtener la altura del CloudPool para la posición Y
        float spawnY = transform.position.y; // Usar la posición Y del CloudPool para todas las nubes

        // Calcular una posición aleatoria en el eje X dentro del área deseada
        float spawnX = Random.Range(-spawnAreaWidth / 2f, spawnAreaWidth / 2f);

        // Aquí la posición es completamente independiente de la cámara y basada en el CloudPool
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0); // Posición en el mundo

        // Usar el PoolingManager para obtener una nube
        GameObject cloud = cloudsPool.GetPooledObject(-1, spawnPosition, Vector3.zero, 20);

        // Asegurarse de que el script de movimiento esté asignado correctamente a la nube
        CloudScript cloudScript = cloud.GetComponent<CloudScript>();
        if (cloudScript != null)
        {
            // Se asigna una velocidad inicial aleatoria para el movimiento de la nube
            cloudScript.initialSpeed = cloudScript.initialSpeed + Random.Range(-cloudScript.variationSeed, cloudScript.variationSeed);
        }

        // Asegurarse de que la nube esté activa
        cloud.SetActive(true);
    }
}
