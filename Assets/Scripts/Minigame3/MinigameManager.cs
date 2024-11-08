using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    // Referencias a los sprites
    public SpriteRenderer option1SpriteRenderer;
    public SpriteRenderer option2SpriteRenderer;
    public SpriteRenderer fallingSpriteRenderer; // El sprite apagado que se debe activar
    public GameObject targetObject; // El GameObject donde el sprite debe encajar al final
    public string sentenceToRemove;

    // Lista de todas las formas de Tetris
    public List<Sprite> tetrisFigures;

    // La imagen correcta que debe aparecer
    public Sprite correctFigure;

    // Velocidad de caída
    public float fallSpeed = 2f;

    // Para saber cuál opción se ha seleccionado
    private SpriteRenderer selectedOption;

    // Control de selección para evitar seleccionar nuevamente
    private bool optionSelected = false;

    // Referencias a los GameObjects de Win y Lose dentro del Canvas
    public GameObject winObject;
    public GameObject loseObject;

    // Start es llamado cuando comienza la escena
    void Start()
    {
        LoadTetrisFigures();  // Asegúrate de cargar las figuras primero
        SetRandomOptions();   // Luego establecer las opciones con las figuras cargadas
        fallingSpriteRenderer.gameObject.SetActive(false);
        winObject.SetActive(false); // Asegúrate de que Win esté oculto al principio
        loseObject.SetActive(false); // Asegúrate de que Lose esté oculto al principio
    }

    // Función para cargar las figuras de Tetris desde la carpeta Resources/TetrisFigures
    void LoadTetrisFigures()
    {
        Sprite[] loadedFigures = Resources.LoadAll<Sprite>("TetrisFigures");

        if (loadedFigures.Length == 0)
        {
            Debug.LogError("No se encontraron figuras de Tetris en la carpeta Resources/TetrisFigures.");
        }
        else
        {
            tetrisFigures = new List<Sprite>(loadedFigures);
            Debug.Log("Figuras cargadas correctamente: " + tetrisFigures.Count);

            // Verifica que las figuras hayan sido cargadas correctamente
            foreach (var sprite in tetrisFigures)
            {
                Debug.Log("Figura cargada: " + sprite.name);
            }
        }
    }

    // Función para cambiar las opciones
    void SetRandomOptions()
    {
        if (tetrisFigures.Count == 0)
        {
            Debug.LogError("La lista de figuras de Tetris está vacía, no se pueden seleccionar opciones.");
            return;
        }

        // Primero, seleccionamos aleatoriamente si la figura correcta irá en Option1 o Option2
        int correctOptionIndex = Random.Range(0, 2); // 0 para Option1, 1 para Option2

        // Si la opción seleccionada es Option1, asignamos la figura correcta a Option1
        if (correctOptionIndex == 0)
        {
            option1SpriteRenderer.sprite = correctFigure;
            // Asignamos una figura aleatoria para Option2
            option2SpriteRenderer.sprite = GetRandomTetrisFigure();
        }
        else
        {
            // Asignamos la figura correcta a Option2
            option2SpriteRenderer.sprite = correctFigure;
            // Asignamos una figura aleatoria para Option1
            option1SpriteRenderer.sprite = GetRandomTetrisFigure();
        }

        // Habilitar los colliders para detectar clics en las opciones
        option1SpriteRenderer.gameObject.AddComponent<BoxCollider2D>();
        option2SpriteRenderer.gameObject.AddComponent<BoxCollider2D>();
    }

    // Función para obtener una figura aleatoria de Tetris (sin la figura correcta)
    Sprite GetRandomTetrisFigure()
    {
        // Filtramos para obtener una figura aleatoria que no sea la figura correcta
        List<Sprite> availableFigures = new List<Sprite>(tetrisFigures);
        availableFigures.Remove(correctFigure); // Quitamos la figura correcta de las opciones

        return availableFigures[Random.Range(0, availableFigures.Count)];
    }

    // Llamado cuando se hace clic en una opción
  // Llamado cuando se hace clic en una opción
void OnOptionClicked(SpriteRenderer option)
{
    if (optionSelected) return; // Si ya se seleccionó una opción, no hacer nada

    selectedOption = option;

    // Marcar que se seleccionó una opción
    optionSelected = true;

    // Desactivar los colliders para evitar que el jugador seleccione de nuevo
    option1SpriteRenderer.gameObject.GetComponent<Collider2D>().enabled = false;
    option2SpriteRenderer.gameObject.GetComponent<Collider2D>().enabled = false;

    // Verificar si la opción seleccionada es la correcta
    if (selectedOption.sprite == correctFigure)
    {
        fallingSpriteRenderer.gameObject.SetActive(true);
        fallingSpriteRenderer.sprite = selectedOption.sprite;
        StartCoroutine(StartFalling());
        ShowWin(); // Si es correcta, ganar de inmediato
    }
    else
    {
        // Si es incorrecta, iniciar la animación de caída
        fallingSpriteRenderer.gameObject.SetActive(true);
        fallingSpriteRenderer.sprite = selectedOption.sprite;
        StartCoroutine(StartFalling());
        ShowLose(); // Si es incorrecta, mostrar el Lose inmediatamente
    }
}


    // Coroutine para hacer que el sprite caiga y se encaje
    IEnumerator StartFalling()
    {
        // Obtener la posición inicial del sprite de caída
        Vector3 startPosition = fallingSpriteRenderer.transform.position;
        Vector3 targetPosition = targetObject.transform.position;

        // Movimiento de caída (animación de caída)
        while (fallingSpriteRenderer.transform.position.y > targetPosition.y)
        {
            fallingSpriteRenderer.transform.position = Vector3.MoveTowards(fallingSpriteRenderer.transform.position, targetPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }

        // Al llegar a la posición del target, podemos hacer algo adicional (por ejemplo, encajar el sprite)
        fallingSpriteRenderer.transform.position = targetPosition;

        // Si es la pieza correcta, activar win
        if (selectedOption.sprite == correctFigure)
        {
            ShowWin(); // Muestra el Win
        }
        else // Si es incorrecta, activar lose
        {
            ShowLose(); // Muestra el Lose
        }
    }

    // Mostrar el objeto win con un pequeño delay para que se vea la animación de caída
private void ShowWin()
{
    // Mostrar mensaje de depuración
    Debug.Log("¡Ganaste!");

    winObject.SetActive(true); // Activa el objeto Win

    // Espera 2 segundos antes de cargar la escena de juego principal para visualizar la animación de "win"
    StartCoroutine(DelayedLoadGameplayScene(true));
}

// Mostrar el objeto lose sin delay adicional
private void ShowLose()
{
    Debug.Log("¡Perdiste!");
    loseObject.SetActive(true); // Activa el objeto Lose
    StartCoroutine(DelayedLoadGameplayScene(false)); // Sin delay adicional
}

// Coroutine para cargar la escena después de un delay
IEnumerator DelayedLoadGameplayScene(bool isWin)
{
    if (isWin)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Cargando escena después de ganar...");
        GameManager.instance.RemoveSentenceForMinigame(sentenceToRemove);
        GameManager.instance.SetReturningToGameplay(true);
    }
    else
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Cargando escena después de perder...");
        GameManager.instance.AddStress();
        GameManager.instance.SetReturningToGameplay(true);
    }

    // Cargar la escena de juego principal
    SceneManager.LoadScene("Gameplay");
}
    // Función de actualización para detectar los clics en las opciones
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detecta clic izquierdo
        {
            // Hacer clic sobre Option1
            if (IsMouseOver(option1SpriteRenderer))
            {
                OnOptionClicked(option1SpriteRenderer);
            }
            // Hacer clic sobre Option2
            else if (IsMouseOver(option2SpriteRenderer))
            {
                OnOptionClicked(option2SpriteRenderer);
            }
        }
    }

    // Función para comprobar si el ratón está sobre un objeto
    bool IsMouseOver(SpriteRenderer option)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return option.bounds.Contains(mousePosition);
    }
}
