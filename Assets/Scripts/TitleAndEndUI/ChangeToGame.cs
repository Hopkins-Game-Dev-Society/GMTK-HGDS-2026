using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToGame : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main Gameplay");
    }

    
}
