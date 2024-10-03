using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ControlScene : MonoBehaviour
{
    public static ControlScene scene;
    public Animator animator;
    public float timeTransition = 1f;
    public bool LoadScene;

    private void Awake()
    {
        scene = this;
        StartCoroutine(LoadLevelAwake());
    }

    public void ChangeScene(int nScene)
    {
        StartCoroutine(LoadLevel(nScene));
        EventSystem.current.SetSelectedGameObject(null);

        //SceneManager.LoadScene(nScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        animator.SetTrigger("Start");
        LoadScene = true;
        yield return new WaitForSeconds(timeTransition);
        LoadScene = false;
        SceneManager.LoadScene(levelIndex);
    }

    public void PlayAnimation()
    {
        animator.SetTrigger("Start");
        Debug.Log("play");
    }

    IEnumerator LoadLevelAwake()
    { 
        LoadScene = true;
        yield return new WaitForSeconds(timeTransition);
        LoadScene = false;
    }
}
