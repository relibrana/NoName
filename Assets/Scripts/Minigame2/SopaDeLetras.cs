using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WordGroup
{
    public string hint; // La pista o frase para este grupo de palabras
    public List<string> words; // Lista de palabras en este grupo
}

public class SopaDeLetras : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;
    public Text hintText; // Mostrar la pista
    public List<WordGroup> wordGroups; // Lista de grupos de palabras
    public string correctWord; // Palabra correcta de la partida actual
    private WordGroup currentWordGroup; // Grupo de palabras actual
    private float timerDuration = 7f; // Duración máxima para seleccionar una palabra
    private float timer;
    private bool isWordSelected = false;
    private string selectedWord = "";

    void Start()
    {
        // Inicializamos el juego seleccionando un grupo de palabras al azar
        InitializeGame();
    }

    void InitializeGame()
    {
        if (wordGroups.Count > 0)
        {
            // Selecciona un grupo de palabras al azar
            currentWordGroup = wordGroups[Random.Range(0, wordGroups.Count)];
            
            // Elige una palabra correcta al azar de este grupo
            correctWord = currentWordGroup.words[Random.Range(0, currentWordGroup.words.Count)];
            
            // Muestra la pista en el UI
            hintText.text = "Pista: " + currentWordGroup.hint;

            // Inicia el temporizador
            timer = timerDuration;
        }
    }

    void Update()
    {
        if (!isWordSelected)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ShowLose();
            }
        }
    }

    public void ShowWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Usa el GameManager para eliminar la oración asignada al minijuego
        GameManager.instance.RemoveSentenceForMinigame(correctWord);

        // Cargar la escena de juego principal
        SceneManager.LoadScene("Gameplay");
    }

    public void ShowLose()
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

    // Método que se llama cuando el jugador selecciona una palabra
    public void SelectWord(string word)
    {
        if (isWordSelected) return;

        selectedWord = word;
        isWordSelected = true;

        // Verificar si la palabra es correcta
        if (selectedWord == correctWord)
        {
            ShowWin();
        }
        else
        {
            ShowLose();
        }
    }

    // Método que asigna los sprites según el estado de la letra
    public void SetWordSprites(List<Sprite> normalSprites, List<Sprite> selectedSprites, List<Sprite> wrongSprites)
    {
        // Aquí puedes implementar el cambio de sprites para las letras según su estado
    }
}

