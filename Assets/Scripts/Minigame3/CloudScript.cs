using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    public float initialSpeed = 3f;
    public float variationSeed = 1f;

    private void Start()
    {
        // Asegurarse de que la velocidad inicial sea aleatoria
        initialSpeed = initialSpeed + Random.Range(-variationSeed, variationSeed);
    }

    private void Update()
    {
        // Movimiento de la nube a lo largo del eje X (izquierda)
        transform.Translate(Vector3.left * initialSpeed * Time.deltaTime);

        // Si la nube sale de la pantalla, reposicionar
        if (transform.position.x < -10f) // Ajusta este valor según tu escena
        {
            // Reposicionar la nube en el lado opuesto
            transform.position = new Vector3(10f, transform.position.y, transform.position.z);
        }
    }
}
