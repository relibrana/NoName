using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SentenceManager : MonoBehaviour
{
    public static SentenceManager Instance { get; private set; }

    public List<string> sentences = new List<string>()
    {
        "Siento que nadie me ama",  
        "Con cada año que pasa, soy más tonta",  
        "Nadie quiere trabajar conmigo",  
        "Estoy perdida en la vida"  
    };

    public GameObject sentencePrefab;
    public Vector3 offset = new Vector3(50, 0, 0);
    public GameObject canvasObject;

    private Camera mainCamera;
    private List<GameObject> cloudObjects = new List<GameObject>();
    private Dictionary<string, GameObject> sentenceCloudMapping = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        AssignSentencesToClouds();  // Asignamos las oraciones a las nubes al inicio
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectSentenceClick();
        }
    }

    void DetectSentenceClick()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            TextMeshProUGUI textComponent = clickedObject.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                string clickedSentence = textComponent.text;
                if (sentenceCloudMapping.ContainsKey(clickedSentence))
                {
                    int index = sentences.IndexOf(clickedSentence);
                    if (index >= 0 && index < sentences.Count)
                    {
                        Debug.Log("Cargando escena de minijuego con índice: " + index);
                        LoadMinijuegoScene(index);
                    }
                }
                else
                {
                    Debug.LogWarning("Oración no encontrada en el mapeo de nubes: " + clickedSentence);
                }
            }
        }
    }

    void AssignSentencesToClouds()
    {
        List<GameObject> clouds = new List<GameObject>();
        FindCloudsInChildren(canvasObject.transform, clouds);

        Debug.Log("Nubes encontradas: " + clouds.Count);

        if (clouds.Count < sentences.Count)
        {
            Debug.LogWarning("No hay suficientes nubes para asignar todas las oraciones. Se asignarán solo las primeras " + clouds.Count + " oraciones.");
        }

        List<string> shuffledSentences = new List<string>(sentences);
        ShuffleList(shuffledSentences);

        for (int i = 0; i < shuffledSentences.Count && i < clouds.Count; i++)
        {
            GameObject cloud = clouds[i];
            string sentence = shuffledSentences[i];

            Debug.Log("Asignando oración: " + sentence + " a la nube: " + cloud.name);

            sentenceCloudMapping[sentence] = cloud;

            cloud.SetActive(true);
            cloudObjects.Add(cloud);

            GameObject sentenceObj = Instantiate(sentencePrefab, cloud.transform.parent);
            RectTransform cloudRectTransform = cloud.GetComponent<RectTransform>();
            RectTransform sentenceRectTransform = sentenceObj.GetComponent<RectTransform>();
            sentenceRectTransform.anchoredPosition = cloudRectTransform.anchoredPosition + (Vector2)offset;

            TextMeshProUGUI textComponent = sentenceObj.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = sentence;
            }

            BoxCollider2D collider = sentenceObj.AddComponent<BoxCollider2D>();
            collider.size = sentenceRectTransform.sizeDelta;
            collider.isTrigger = true;
        }
    }

    void FindCloudsInChildren(Transform parent, List<GameObject> clouds)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag("Cloud") && child.GetComponent<UnityEngine.UI.Image>() != null)
            {
                clouds.Add(child.gameObject);
            }

            if (child.childCount > 0)
            {
                FindCloudsInChildren(child, clouds);
            }
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void LoadMinijuegoScene(int index)
    {
        string sceneName = "Minijuego #" + (index + 1);
        SceneManager.LoadScene(sceneName);
        Debug.Log("Escena cargada: " + sceneName);
    }

    // Método que asegura que la oración se reasigne antes de eliminarla
    public void RemoveSentenceForMinigame(string sentenceToRemove)
    {
        Debug.Log("Iniciando el proceso de eliminación para: " + sentenceToRemove);

        if (sentenceCloudMapping.ContainsKey(sentenceToRemove))
        {
            // Primero reasignamos la oración a la nube
            GameObject cloud = sentenceCloudMapping[sentenceToRemove];

            if (cloud != null)
            {
                // Desactivamos la nube, para asegurarnos que no esté visible
                cloud.SetActive(false);

                // Destruir la oración asociada
                GameObject sentenceObj = cloud.transform.GetChild(0).gameObject;
                if (sentenceObj != null)
                {
                    Destroy(sentenceObj);
                }

                // Eliminar la oración de la lista y de las nubes
                sentences.Remove(sentenceToRemove);
                sentenceCloudMapping.Remove(sentenceToRemove);
                cloudObjects.Remove(cloud);

                Debug.Log("Oración eliminada correctamente: " + sentenceToRemove);
            }
        }
        else
        {
            Debug.LogWarning("No se encontró la nube correspondiente a la oración: " + sentenceToRemove);
        }
    }
}
