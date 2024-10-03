using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlScene : MonoBehaviour
{
    public Animator animator;
    public float timeTransition = 1f;

    public void ChangeScene(int nScene)
    {
        StartCoroutine(LoadLevel(nScene));
        //SceneManager.LoadScene(nScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadLevel(int levelIndex) 
    {
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(timeTransition);

        SceneManager.LoadScene(levelIndex);
    }
}
