using UnityEngine;

public class FlechaMovimiento : MonoBehaviour
{
    public float alturaRebote = 0.1f;  // Altura máxima del rebote
    public float tiempoRebote = 0.5f;  // Tiempo total para un rebote completo (sube y baja)
    
    private Vector3 posicionInicial;   // Posición inicial de la flecha
    private float tiempoTranscurrido = 0f;  // Tiempo acumulado

    void Start()
    {
        // Guardar la posición inicial de la flecha
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Rebote con Mathf.PingPong
        // Mathf.PingPong devuelve un valor que oscila entre 0 y 1, multiplicamos por la distancia para que rebote en un rango mayor
        float desplazamientoY = Mathf.PingPong(Time.time * (1 / tiempoRebote), 1) * alturaRebote;

        // Aplicamos el rebote a la posición de la flecha
        transform.position = new Vector3(posicionInicial.x, posicionInicial.y + desplazamientoY, posicionInicial.z);
    }
}
