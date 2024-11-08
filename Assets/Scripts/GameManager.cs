using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<string> sentences = new List<string>();
    public GameObject sentencePrefab;
    public Vector3 offset = new Vector3(50, 0, 0);
    public GameObject canvasObject;
    public GameObject losePanel;

    private Camera mainCamera;
    private List<GameObject> cloudObjects = new List<GameObject>();
    private Dictionary<string, GameObject> sentenceCloudMapping = new Dictionary<string, GameObject>();

    private int stressLevel = 0;
    private float stressIncreaseInterval = 5f;

    public float timer = 30.0f;
    public float current_timer = 0.0f;
    private bool isReturningToGameplay = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Asegurarse de que solo haya una cámara principal
        if (Camera.main != null)
        {
            mainCamera = Camera.main;
            Camera.main.gameObject.SetActive(true); // Asegura que la cámara principal esté activa
        }
    }

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);

        InvokeRepeating("IncreaseStress", stressIncreaseInterval, stressIncreaseInterval);

        // Asegurarse de que la cámara principal esté asignada
        if (mainCamera == null && Camera.main != null)
        {
            mainCamera = Camera.main;
            Camera.main.gameObject.SetActive(true); // Asegura que la cámara principal esté activa
        }

        if (canvasObject == null)
            canvasObject = GameObject.Find("Pensamientos").transform.GetChild(2).gameObject;

        AssignSentencesToRandomClouds();  // Asigna las oraciones de forma aleatoria al inicio
    }

    private void Update()
    {
        UpdateTimer();

        if (Input.GetMouseButtonDown(0))
        {
            DetectSentenceClick();
        }
    }

    private void UpdateTimer()
    {
        if (current_timer < timer)
        {
            current_timer += Time.deltaTime;
            float remainingTime = timer - current_timer;

            CheckGameOverByTime(remainingTime);
        }
    }

    public void AddStress()
    {
        IncreaseStress();
    }

    private void IncreaseStress()
    {
        stressLevel++;
        Debug.Log("Se aumentó el estrés: " + stressLevel);

        if (stressLevel >= 6)
        {
            ShowLose();
            CancelInvoke("IncreaseStress");
        }
    }

    public void SetReturningToGameplay(bool value)
    {
        isReturningToGameplay = value;  // Establece el flag cuando sea necesario
    }

    private void ShowLose()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }

    public void CheckGameOverByTime(float remainingTime)
    {
        if (remainingTime <= 0)
        {
            ShowLose();
            CancelInvoke("IncreaseStress");
        }
    }

   private void DetectSentenceClick()
{
    if (mainCamera != null)
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            TextMeshProUGUI textComponent = clickedObject.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                string clickedSentence = textComponent.text.Trim();  // Elimina espacios en blanco innecesarios
                Debug.Log("Cargando minijuego usando la oración: " + clickedSentence);
                LoadMinijuegoScene(clickedSentence);
            }
        }
    }
}

private void LoadMinijuegoScene(string sentence)
{
    string sceneName = "";

    switch (sentence)
    {
        case "I feel like nobody loves me":
            sceneName = "Minijuego #1";
            break;
        case "Every year I get dumber":
            sceneName = "Minijuego #2";
            break;
        case "Nobody wants to work with me":
            sceneName = "Minijuego #3";
            break;
        case "I'm lost in life":
            sceneName = "Minijuego #4";
            break;
        default:
            Debug.LogWarning("Oración no corresponde a ningún minijuego: " + sentence);
            return;  // Salir si la oración no tiene un minijuego asignado
    }

    Debug.Log("Escena cargada: " + sceneName);
    SceneManager.LoadScene(sceneName);
}


    private void AssignSentencesToRandomClouds()
    {
        if (canvasObject == null)
            canvasObject = GameObject.Find("Pensamientos").transform.GetChild(2).gameObject;

        cloudObjects.Clear();
        sentenceCloudMapping.Clear();

        // Encontrar las nubes y mezclarlas aleatoriamente
        List<GameObject> clouds = new List<GameObject>();
        FindCloudsInChildren(canvasObject.transform, clouds);
        ShuffleList(clouds);

        // Asignar las oraciones a las nubes de forma aleatoria
        for (int i = 0; i < sentences.Count && i < clouds.Count; i++)
        {
            GameObject cloud = clouds[i];
            string sentence = sentences[i];

            if (!sentenceCloudMapping.ContainsKey(sentence))  // Verifica si la oración ya está mapeada
            {
                sentenceCloudMapping[sentence] = cloud;
                cloud.SetActive(true);
                cloudObjects.Add(cloud);

                GameObject sentenceObj = Instantiate(sentencePrefab, cloud.transform.parent);
                RectTransform cloudRectTransform = cloud.GetComponent<RectTransform>();
                RectTransform sentenceRectTransform = sentenceObj.GetComponent<RectTransform>();

                Vector3 adjustedOffset = offset + new Vector3(i * 10, 0, 0);
                sentenceRectTransform.anchoredPosition = cloudRectTransform.anchoredPosition + (Vector2)adjustedOffset;

                TextMeshProUGUI textComponent = sentenceObj.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = sentence;
                }

                BoxCollider2D collider = sentenceObj.AddComponent<BoxCollider2D>();
                collider.size = sentenceRectTransform.sizeDelta;
                collider.isTrigger = true;
            }
            else
            {
                Debug.LogWarning("La oración ya está mapeada: " + sentence);
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void FindCloudsInChildren(Transform parent, List<GameObject> clouds)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag("Cloud") && !clouds.Contains(child.gameObject))
            {
                clouds.Add(child.gameObject);
            }

            if (child.childCount > 0)
            {
                FindCloudsInChildren(child, clouds);
            }
        }
    }

 
    public void RemoveSentenceForMinigame(string sentence)
    {
        string trimmedSentence = sentence.Trim();
        int index = sentences.FindIndex(s => s.Trim() == trimmedSentence);

        if (index >= 0)
        {
            Debug.Log("Oración eliminada de la lista: " + trimmedSentence);
            sentences.RemoveAt(index);
        }
        else
        {
            Debug.LogWarning("La oración no existe en la lista para eliminar: " + trimmedSentence);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay" && isReturningToGameplay)
        {
            // Asegurarse de que solo haya una cámara principal
            if (mainCamera == null && Camera.main != null)
            {
                mainCamera = Camera.main;
                Camera.main.gameObject.SetActive(true); // Asegura que la cámara principal esté activa
            }

            // Asegurarse de que el canvasObject sea encontrado nuevamente
            if (canvasObject == null)
                canvasObject = GameObject.Find("Pensamientos").transform.GetChild(2).gameObject;

            // Llamar a la función para reasignar las oraciones en las nubes
            AssignSentencesToRandomClouds();
        }
    }

    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
