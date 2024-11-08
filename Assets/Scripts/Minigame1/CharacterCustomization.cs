using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections;

public class CharacterCustomization : MonoBehaviour
{
    public GameObject character;

    private SpriteRenderer leftEyeRenderer, rightEyeRenderer, bodyRenderer, rightLegRenderer, leftLegRenderer, mouthRenderer, rightArmRenderer, leftArmRenderer;

    public Button colorButton, bodyButton, armsButton, legsButton, eyesButton, mouthButton;
    public GameObject optionsPanel;
    public GameObject optionPrefab;

    private string selectedColor = "";
    private string resourcesPath = "Monster/"; // Ruta base donde se encuentran los recursos
    private char[] optionsLetters = { 'A', 'B', 'C', 'D', 'E' }; // Letras para agregar a la ruta

    private GameObject currentOptionsPanel; // Para almacenar el panel actual de opciones
    public TextMeshProUGUI warningText;

    void Start()
    {
        // Inicialización de los renderizadores de las partes del cuerpo
        leftEyeRenderer = character.transform.Find("left_eye").GetComponent<SpriteRenderer>();
        rightEyeRenderer = character.transform.Find("right_eye").GetComponent<SpriteRenderer>();
        bodyRenderer = character.transform.Find("body").GetComponent<SpriteRenderer>();
        rightLegRenderer = character.transform.Find("right_leg").GetComponent<SpriteRenderer>();
        leftLegRenderer = character.transform.Find("left_leg").GetComponent<SpriteRenderer>();
        mouthRenderer = character.transform.Find("mouth").GetComponent<SpriteRenderer>();
        rightArmRenderer = character.transform.Find("right_arm").GetComponent<SpriteRenderer>();
        leftArmRenderer = character.transform.Find("left_arm").GetComponent<SpriteRenderer>();

        // Asignación de los eventos de los botones
        colorButton.onClick.AddListener(() => ShowColorOptions());
        bodyButton.onClick.AddListener(() => ShowBodyOptions());
        armsButton.onClick.AddListener(() => ShowArmsOptions());
        legsButton.onClick.AddListener(() => ShowLegsOptions());
        eyesButton.onClick.AddListener(() => ShowEyesOptions());
        mouthButton.onClick.AddListener(() => ShowMouthOptions());

        // Inicializamos la opción de color
        ShowColorOptions();
    }

    void ShowColorOptions()
    {
        ClearPreviousOptions(); // Eliminar opciones previas antes de mostrar nuevas
        ShowOptions("Colors", SetColor); // Mostrar las opciones de color
    }

    void ShowBodyOptions()
    {
        if (string.IsNullOrEmpty(selectedColor))
        {
            Debug.LogWarning("Please select a color first!");
            ShowWarningText("Please select a color first!");  // Mostrar mensaje de advertencia
            return;
        }

        ClearPreviousOptions(); // Eliminar opciones previas antes de mostrar nuevas

        // Agregar las letras A, B, C, D, E a la ruta de los sprites de cuerpo
        foreach (char letter in optionsLetters)
        {
            string path = "body_" + selectedColor + letter;
            Debug.Log("Loading sprites from path: " + path); // Imprimir la ruta para depurar
            ShowOptions(path, SetBody);
        }
    }

    void ShowWarningText(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;  // Establecer el mensaje de advertencia
            warningText.gameObject.SetActive(true);  // Activar el GameObject que contiene el texto
            StartCoroutine(HideWarningTextAfterDelay(2f));  // Desactivar después de 2 segundos
        }
    }

    IEnumerator HideWarningTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);  // Desactivar el GameObject del texto después del retraso
        }
    }

    void ShowArmsOptions()
    {
        if (string.IsNullOrEmpty(selectedColor))
        {
            Debug.LogWarning("Please select a color first!");
            ShowWarningText("Please select a color first!");  // Mostrar mensaje de advertencia
            return;
        }

        ClearPreviousOptions(); // Eliminar opciones previas antes de mostrar nuevas

        // Agregar las letras A, B, C, D, E a la ruta de los sprites de brazos
        foreach (char letter in optionsLetters)
        {
            string path = "arm_" + selectedColor + letter;
            Debug.Log("Loading sprites from path: " + path); // Imprimir la ruta para depurar
            ShowOptions(path, SetArms);
        }
    }

    void ShowLegsOptions()
    {
        if (string.IsNullOrEmpty(selectedColor))
        {
            Debug.LogWarning("Please select a color first!");
            ShowWarningText("Please select a color first!");  // Mostrar mensaje de advertencia
            return;
        }

        ClearPreviousOptions(); // Eliminar opciones previas antes de mostrar nuevas

        // Agregar las letras A, B, C, D, E a la ruta de los sprites de piernas
        foreach (char letter in optionsLetters)
        {
            string path = "leg_" + selectedColor + letter;
            Debug.Log("Loading sprites from path: " + path); // Imprimir la ruta para depurar
            ShowOptions(path, SetLegs);
        }
    }

    void ShowEyesOptions()
    {
        ClearPreviousOptions(); // Eliminar opciones previas antes de mostrar nuevas
        // Cargar todos los sprites dentro de la carpeta Monster
        Sprite[] allSprites = Resources.LoadAll<Sprite>(resourcesPath);

        // Filtrar los sprites que contienen la palabra "eye" en su nombre
        var eyeSprites = allSprites.Where(sprite => sprite.name.Contains("eye")).ToArray();

        if (eyeSprites.Length == 0)
        {
            Debug.LogWarning("No sprites found containing 'eye'");
            return;
        }

        // Mostrar las opciones
        foreach (Sprite sprite in eyeSprites)
        {
            GameObject option = Instantiate(optionPrefab, optionsPanel.transform);
            Transform imageTransform = option.transform.Find("Image");

            if (imageTransform != null)
            {
                Image buttonImage = imageTransform.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = sprite;
                }
            }

            option.GetComponent<Button>().onClick.AddListener(() => SetEyes(sprite));
        }
    }

    void ShowMouthOptions()
    {
        ClearPreviousOptions(); // Eliminar opciones previas antes de mostrar nuevas
        // Cargar todos los sprites dentro de la carpeta Monster
        Sprite[] allSprites = Resources.LoadAll<Sprite>(resourcesPath);

        // Filtrar los sprites que contienen la palabra "mouth" en su nombre
        var mouthSprites = allSprites.Where(sprite => sprite.name.Contains("mouth")).ToArray();

        if (mouthSprites.Length == 0)
        {
            Debug.LogWarning("No sprites found containing 'mouth'");
            return;
        }

        // Mostrar las opciones
        foreach (Sprite sprite in mouthSprites)
        {
            GameObject option = Instantiate(optionPrefab, optionsPanel.transform);
            Transform imageTransform = option.transform.Find("Image");

            if (imageTransform != null)
            {
                Image buttonImage = imageTransform.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = sprite;
                }
            }

            option.GetComponent<Button>().onClick.AddListener(() => SetMouth(sprite));
        }
    }

    void ShowOptions(string category, System.Action<Sprite> onSelect)
    {
        string path = resourcesPath + category;

        // Aquí usamos Resources.LoadAll para cargar todos los sprites de la categoría
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        if (sprites.Length == 0)
        {
            Debug.LogWarning("No sprites found in path: " + path);
            return;
        }

        // Instanciamos las opciones
        foreach (Sprite sprite in sprites)
        {
            GameObject option = Instantiate(optionPrefab, optionsPanel.transform);
            Transform imageTransform = option.transform.Find("Image");

            if (imageTransform != null)
            {
                Image buttonImage = imageTransform.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = sprite;
                }
            }

            option.GetComponent<Button>().onClick.AddListener(() => onSelect(sprite));
        }
    }

    void ClearPreviousOptions()
    {
        // Eliminar todas las opciones anteriores
        foreach (Transform child in optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void SetColor(Sprite colorSprite)
    {
        selectedColor = colorSprite.name.Replace("_0", "");
        Debug.Log("Color Selected: " + selectedColor);
    }

    void SetBody(Sprite bodySprite)
    {
        bodyRenderer.sprite = bodySprite;
    }

    void SetArms(Sprite armsSprite)
    {
        leftArmRenderer.sprite = armsSprite;
        rightArmRenderer.sprite = armsSprite;
    }

    void SetLegs(Sprite legsSprite)
    {
        leftLegRenderer.sprite = legsSprite;
        rightLegRenderer.sprite = legsSprite;
    }

    void SetEyes(Sprite eyesSprite)
    {
        leftEyeRenderer.sprite = eyesSprite;
        rightEyeRenderer.sprite = eyesSprite;

        // Aseguramos que los ojos estén siempre en el Order in Layer 2
        leftEyeRenderer.sortingOrder = 2;
        rightEyeRenderer.sortingOrder = 2;
    }

    void SetMouth(Sprite mouthSprite)
    {
        mouthRenderer.sprite = mouthSprite;

        // Aseguramos que la boca esté siempre en el Order in Layer 2
        mouthRenderer.sortingOrder = 3;
    }
}