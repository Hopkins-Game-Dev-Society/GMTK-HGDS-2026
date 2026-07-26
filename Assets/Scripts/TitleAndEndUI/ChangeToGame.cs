using BirthdayJobJam.UI;
using UnityEngine;

public class ChangeToGame : MonoBehaviour
{
    public void StartGame()
    {
        SceneTransitioner.LoadScene("Main Gameplay");
    }

    public void ToMainMenu()
    {
        SceneTransitioner.LoadScene("title");
    }
}
