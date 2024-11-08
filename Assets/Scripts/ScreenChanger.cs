using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para trabajar con escenas

public class SceneChanger : MonoBehaviour
{
    // Método que se llama cuando se hace clic en el botón
    public void ChangeScene(string sceneName)
    {
         SoundManager.instance.PlaySound("SFX_UI_Select");
        SceneManager.LoadScene(sceneName);  // Cambia la escena al nombre que se pasa como argumento
    }
}
