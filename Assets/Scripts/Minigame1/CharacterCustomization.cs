using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomization : MonoBehaviour
{
    public GameObject character;  // Referencia al GameObject del personaje

    // Referencias a cada parte del personaje
    private SpriteRenderer leftEyeRenderer, rightEyeRenderer, bodyRenderer, rightLegRenderer, leftLegRenderer, mouthRenderer, rightArmRenderer, leftArmRenderer;

    public Button colorButton, bodyButton, armsButton, legsButton, eyesButton, mouthButton;  // Botones para cada parte
    public GameObject optionsPanel;  // Panel donde se mostrarán las opciones de sprites
    public GameObject optionPrefab;  // Prefab de cada opción de color o parte

    private string selectedColor = "";  // Color seleccionado actualmente
    private string resourcesPath = "Monster/";  // Ruta base para los recursos de los sprites

    void Start()
    {
        // Obtener los componentes SpriteRenderer de cada parte del personaje
        leftEyeRenderer = character.transform.Find("left_eye").GetComponent<SpriteRenderer>();
        rightEyeRenderer = character.transform.Find("right_eye").GetComponent<SpriteRenderer>();
        bodyRenderer = character.transform.Find("body").GetComponent<SpriteRenderer>();
        rightLegRenderer = character.transform.Find("right_leg").GetComponent<SpriteRenderer>();
        leftLegRenderer = character.transform.Find("left_leg").GetComponent<SpriteRenderer>();
        mouthRenderer = character.transform.Find("mouth").GetComponent<SpriteRenderer>();
        rightArmRenderer = character.transform.Find("right_arm").GetComponent<SpriteRenderer>();
        leftArmRenderer = character.transform.Find("left_arm").GetComponent<SpriteRenderer>();

        // Asegurarse de que el panel de opciones esté activo
        optionsPanel.SetActive(true);

        // Asignar funciones de clic a cada botón
        colorButton.onClick.AddListener(() => ShowColorOptions());  // Mostrar opciones de colores
        bodyButton.onClick.AddListener(() => ShowBodyOptions());
        armsButton.onClick.AddListener(() => ShowArmsOptions());
        legsButton.onClick.AddListener(() => ShowLegsOptions());
        eyesButton.onClick.AddListener(() => ShowEyesOptions());
        mouthButton.onClick.AddListener(() => ShowMouthOptions());
    }

    // Mostrar las opciones de color primero
    void ShowColorOptions()
    {
        Debug.Log("Showing color options");
        ShowOptions("Colors", SetColor);  // Mostrar opciones de colores
    }

    // Mostrar las opciones de personalización para cada parte
    void ShowBodyOptions()
    {
        if (string.IsNullOrEmpty(selectedColor))
        {
            Debug.LogWarning("Please select a color first!");
            return;
        }
        ShowOptions("body", SetBody);
    }

    void ShowArmsOptions()
    {
        if (string.IsNullOrEmpty(selectedColor))
        {
            Debug.LogWarning("Please select a color first!");
            return;
        }
        ShowOptions("arm", SetArms);
    }

    void ShowLegsOptions()
    {
        if (string.IsNullOrEmpty(selectedColor))
        {
            Debug.LogWarning("Please select a color first!");
            return;
        }
        ShowOptions("leg", SetLegs);
    }

    void ShowEyesOptions()
    {
        if (string.IsNullOrEmpty(selectedColor))
        {
            Debug.LogWarning("Please select a color first!");
            return;
        }
        ShowOptions("eye", SetEyes);
    }

    void ShowMouthOptions()
    {
        if (string.IsNullOrEmpty(selectedColor))
        {
            Debug.LogWarning("Please select a color first!");
            return;
        }
        ShowOptions("mouth", SetMouth);
    }

    // Función para mostrar las opciones de la categoría especificada
    void ShowOptions(string category, System.Action<Sprite> onSelect)
    {
        Debug.Log($"ShowOptions - Category: {category}");

        // Asegurarse de que el panel esté activo
        if (!optionsPanel.activeSelf) 
        {
            Debug.LogWarning("Options panel is not active!");
            optionsPanel.SetActive(true);  // Asegurarse de que esté activo
        }

        ClearOptions();  // Limpiar opciones anteriores
        string path = resourcesPath + category;

        if (category == "Colors")  // Si la categoría es "Colors", no se requiere color seleccionado
        {
            Debug.Log("Showing color options");
        }
        else if (!string.IsNullOrEmpty(selectedColor))
        {
            path += "/" + selectedColor;  // Agregar el color a la ruta para partes específicas
            Debug.Log($"Path with selected color: {path}");
        }
        else
        {
            Debug.LogWarning("Please select a color first!");
            return;  // No mostrar partes si no se ha seleccionado color
        }

        // Cargar los sprites
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        if (sprites.Length == 0)
        {
            Debug.LogWarning("No sprites found in path: " + path);
            return;
        }

        // Comprobar si los sprites se cargaron correctamente
        Debug.Log($"Loaded {sprites.Length} sprites from path: {path}");

        // Crear botones
        foreach (Sprite sprite in sprites)
        {
            Debug.Log($"Sprite Found: {sprite.name}");

            // Instanciar un botón para cada sprite
            GameObject option = Instantiate(optionPrefab, optionsPanel.transform);
            
            // Acceder al Image hijo dentro del prefab (asegurándonos de no afectar el sprite del botón)
            Image buttonImage = option.GetComponentInChildren<Image>();  // Busca el primer Image hijo

            if (buttonImage != null)
            {
                // Cambiar solo el sprite del Image dentro del prefab
                buttonImage.sprite = sprite;  // Asignar el sprite al Image del hijo
            }

            // Configurar el botón para que ejecute la función de selección
            option.GetComponent<Button>().onClick.AddListener(() => 
            {
                Debug.Log($"Option Clicked: {sprite.name}");
                onSelect(sprite);  // Llamar a la función correspondiente (SetColor, SetBody, etc.)
            });
        }
    }

    // Limpiar las opciones anteriores en el panel
    void ClearOptions()
    {
        Debug.Log("Clearing previous options");
        foreach (Transform child in optionsPanel.transform)
        {
            Destroy(child.gameObject);  // Destruir los objetos hijos del panel
        }
    }

    // Funciones para asignar la parte seleccionada del personaje
    void SetColor(Sprite colorSprite)
    {
        Debug.Log($"Color Selected: {colorSprite.name}");
        selectedColor = colorSprite.name;  // Guardar el color seleccionado
        // Después de elegir el color, puedes permitir al usuario elegir otras partes del personaje.
        ShowBodyOptions();  // Opción para elegir el cuerpo después del color
    }

    void SetBody(Sprite bodySprite)
    {
        Debug.Log($"Body Selected: {bodySprite.name}");
        bodyRenderer.sprite = bodySprite;  // Cambiar el sprite del cuerpo
    }

    void SetArms(Sprite armsSprite)
    {
        Debug.Log($"Arms Selected: {armsSprite.name}");
        leftArmRenderer.sprite = armsSprite;
        rightArmRenderer.sprite = armsSprite;
    }

    void SetLegs(Sprite legsSprite)
    {
        Debug.Log($"Legs Selected: {legsSprite.name}");
        leftLegRenderer.sprite = legsSprite;
        rightLegRenderer.sprite = legsSprite;
    }

    void SetEyes(Sprite eyesSprite)
    {
        Debug.Log($"Eyes Selected: {eyesSprite.name}");
        leftEyeRenderer.sprite = eyesSprite;
        rightEyeRenderer.sprite = eyesSprite;
    }

    void SetMouth(Sprite mouthSprite)
    {
        Debug.Log($"Mouth Selected: {mouthSprite.name}");
        mouthRenderer.sprite = mouthSprite;
    }
}
