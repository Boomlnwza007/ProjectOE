using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScene : MonoBehaviour
{
    public GameObject menuPause;
    public GameObject menuAfterDie;
    public static Transform spawnPoint;
    private bool onPauseMenu;
    private bool onmenuAfterDie;
    private void Awake()
    {
        onPauseMenu = menuPause.activeSelf;
        onmenuAfterDie = menuAfterDie.activeSelf;
    }

    public void Retry()
    {
        StartCoroutine(wait());
    }

    public void NextScene(int index)
    {
        ControlScene.scene.ChangeScene(index);
    }
    public void Exit()
    {
        ControlScene.scene.ExitGame();
    }

    public void HidePause()
    {
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
    }

    public void HideAfterDie()
    {
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
    }

    IEnumerator wait() 
    {
        ControlScene.scene.PlayAnimation();
        yield return new WaitForSeconds(1f);
        HideAfterDie();
        PlayerControl.control.Spawn(spawnPoint);
        yield return new WaitForSeconds(1.5f);
        ControlScene.scene.PlayAnimation();
    }
}
