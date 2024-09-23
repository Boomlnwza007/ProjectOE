using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlScene : MonoBehaviour
{
    public void ChangeScene(int nScene)
    {
        SceneManager.LoadScene(nScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
