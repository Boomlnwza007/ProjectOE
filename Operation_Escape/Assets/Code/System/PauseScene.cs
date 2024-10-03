using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class PauseScene : MonoBehaviour
{
    public GameObject menuPause;
    public GameObject menuAfterDie;
    public static Transform spawnPoint;
    public bool onPauseMenu;
    public bool onmenuAfterDie;
    public bool retrying;

    private void Awake()
    {
        onPauseMenu = menuPause.activeSelf;
        onmenuAfterDie = menuAfterDie.activeSelf;
    }

    public void Retry()
    {
        if (retrying)
        {
            return;
        }
        Time.timeScale = 1;
        ControlScene.scene.ChangeScene(1);
        //StartCoroutine(wait());
        //EventSystem.current.SetSelectedGameObject(null);
    }

    public void NextScene(int index)
    {
        Time.timeScale = 1;
        ControlScene.scene.ChangeScene(index);
    }
    public void Exit()
    {
        Time.timeScale = 1;
        ControlScene.scene.ExitGame();
    }

    public void HidePause()
    {
        if (ControlScene.scene.LoadScene)
        {
            return;
        }

        onPauseMenu = !onPauseMenu;
        menuPause.SetActive(onPauseMenu);
        if (onPauseMenu)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void HideAfterDie()
    {
        if (ControlScene.scene.LoadScene)
        {
            return;
        }

        onmenuAfterDie = !onmenuAfterDie;
        menuAfterDie.SetActive(onmenuAfterDie);
        if (onmenuAfterDie)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    IEnumerator wait() 
    {
        ControlScene.scene.PlayAnimation();
        retrying = true;
        yield return new WaitForSeconds(1f);
        HideAfterDie();
        PlayerControl.control.Spawn(spawnPoint);
        yield return new WaitForSeconds(1.5f);
        retrying = false;
        ControlScene.scene.PlayAnimation();
    }
}
