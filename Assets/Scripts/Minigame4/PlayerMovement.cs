using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameObject winPanel; // Panel que muestra "You Win"
    public GameObject losePanel; // Panel que muestra "Perdiste"
    private Vector2 difference;
    private bool canDrag = false;

    // Variable con la oración a eliminar
    private string sentenceToRemove = "I'm lost in life";

    private Collider2D playerCollider;

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
        if (losePanel != null)
            losePanel.SetActive(false);

        // Obtén el collider del jugador
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }

        if (Input.GetMouseButton(0) && canDrag)
        {
            OnMouseDrag();
        }
    }

    private void OnMouseDown()
    {
        // Revisamos si el click está sobre el jugador usando el collider
        if (playerCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            canDrag = true;
            difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
            Debug.Log("Jugador tocado!");
        }
        else
        {
            canDrag = false;
        }
    }

    private void OnMouseDrag()
    {
        Vector2 newPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;

        // Mueve al jugador a la nueva posición
        transform.position = newPosition;
    }

    // Usamos OnTriggerEnter2D para detectar la colisión con las paredes
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger detectado con: " + other.name); // Mensaje para depurar

        // Si el jugador colisiona con un objeto que tiene el tag "Wall", mostrar "Perdiste"
        if (other.CompareTag("Wall"))
        {
            ShowLose();
        }

        // Detectar si colisiona con el objetivo ("Goal")
        if (other.CompareTag("Goal"))
        {
            ShowWin();
        }
    }

    private void ShowLose()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
        // Aumenta el estrés al perder y vuelve a la escena de juego
        GameManager.instance.AddStress();
        GameManager.instance.SetReturningToGameplay(true);
        SceneManager.LoadScene("Gameplay");
    }

    private void ShowWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Usa el GameManager para eliminar la oración asignada al minijuego
        GameManager.instance.RemoveSentenceForMinigame(sentenceToRemove);
        GameManager.instance.SetReturningToGameplay(true);

        // Cargar la escena de juego principal
        SceneManager.LoadScene("Gameplay");
    }
}
