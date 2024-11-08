using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MonsterCreationManager : MonoBehaviour
{
    // Referencias públicas para el botón de submit y el TextMeshPro de hints
    [SerializeField] private TextMeshProUGUI hintText;  // Para mostrar el hint
    [SerializeField] private Button submitButton;       // Para el botón de submit

    // Variables para almacenar los sprites seleccionados
    private string selectedColor;
    private string selectedEyes;
    private string selectedMouth;
    private string sentenceToRemove = "I feel like nobody loves me";

    // Hints rotativos
    private string[] hints = {
        "Green is not a creative color", 
        "I like cat eyes", 
        "I love vampires"
    };

    private int currentHintIndex = 0;

    void Start()
    {
        // Mostrar el primer hint al inicio
        ShowNextHint();

        // Configurar el botón de submit para que ejecute el método OnSubmit al hacer clic
        submitButton.onClick.AddListener(OnSubmit);

        // Inicializar las selecciones de los componentes del monstruo
        selectedColor = "";
        selectedEyes = "";
        selectedMouth = "";
    }

    void ShowNextHint()
    {
        // Mostrar el hint actual y luego rotarlo
        hintText.text = hints[currentHintIndex];
        currentHintIndex = (currentHintIndex + 1) % hints.Length;
    }

    void OnSubmit()
    {
        // Verificar si el monstruo creado cumple con los hints
        if (CheckMonsterCreation())
        {
            ShowWin();
        }
        else
        {
            ShowLose();
        }
    }

    bool CheckMonsterCreation()
    {
        // Verificar las condiciones del monstruo según los hints
        if (hintText.text == "Green is not a creative color" && selectedColor != "green")
        {
            return false;
        }

        if (hintText.text == "I like cat eyes" && !(selectedEyes.Contains("eye_red") || selectedEyes.Contains("eye_yellow")))
        {
            return false;
        }

        if (hintText.text == "I love vampires" && !(selectedMouth.Contains("mouthF") || selectedMouth.Contains("mouthJ") || selectedMouth.Contains("mouth_closed_fangs")))
        {
            return false;
        }

        return true;
    }

    void ShowWin()
    {
        // Usa el GameManager para eliminar la oración asignada al minijuego
        GameManager.instance.RemoveSentenceForMinigame(sentenceToRemove);
        GameManager.instance.SetReturningToGameplay(true);

        // Cargar la escena de juego principal
        SceneManager.LoadScene("Gameplay");
    }

    void ShowLose()
    {
        // Aumenta el estrés al perder y vuelve a la escena de juego
        GameManager.instance.AddStress();
        GameManager.instance.SetReturningToGameplay(true);
        SceneManager.LoadScene("Gameplay");
    }

    // Métodos para registrar los sprites seleccionados
    public void SetColor(string color)
    {
        selectedColor = color;
    }

    public void SetEyes(string eyes)
    {
        selectedEyes = eyes;
    }

    public void SetMouth(string mouth)
    {
        selectedMouth = mouth;
    }
}
