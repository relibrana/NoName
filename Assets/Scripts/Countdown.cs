using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    void Update()
    {
        if (GameManager.instance != null)
        {
            float remainingTime = GameManager.instance.timer - GameManager.instance.current_timer;
            timerText.text = ((int)(remainingTime + 1)).ToString(); // Muestra el tiempo restante
        }
    }
}
