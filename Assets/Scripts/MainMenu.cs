using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainMenu : MonoBehaviour
{
    public Text quitText;


    public void QuitGame()
    {
        quitText.gameObject.SetActive(true);
        Application.Quit();
    }

    public void PlayOnePlayer()
    {
        SceneManager.LoadScene("1Player");
    }


    public void PlayTwoPlayer()
    {
        SceneManager.LoadScene("2Player");
    }


    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
