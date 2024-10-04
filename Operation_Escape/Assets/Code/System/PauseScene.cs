using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class PauseScene : MonoBehaviour
{
    public GameObject menuPause;
    public GameObject menuAfterDie;
    public Transform firstSpawn;
    public static Transform spawnPoint;
    public static AreaEnermy area; 
    public bool onPauseMenu;
    public bool onmenuAfterDie;
    public bool retrying;

    private void Awake()
    {
        onPauseMenu = menuPause.activeSelf;
        onmenuAfterDie = menuAfterDie.activeSelf;
        spawnPoint = firstSpawn;
    }

    public void Retry()
    {
        if (retrying)
        {
            return;
        }

        Time.timeScale = 1;
        //ControlScene.scene.ChangeScene(1);
        StartCoroutine(wait());
        //EventSystem.current.SetSelectedGameObject(null);
    }

    public void NextScene(int index)
    {
        if (retrying)
        {
            return;
        }

        Time.timeScale = 1;
        ControlScene.scene.ChangeScene(index);
    }

    public void Exit()
    {
        if (retrying)
        {
            return;
        }
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
            //StartCoroutine(waitDie(0.2f));
        }
        else
        {
            Time.timeScale = 1;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void HideAfterDieCode()
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
        EventSystem.current.SetSelectedGameObject(null);
    }

    IEnumerator wait() 
    {
        ControlScene.scene.PlayAnimation();
        retrying = true;
        ControlScene.scene.LoadScene = true;
        PlayerControl.control.EnableInput(false);
        CinemachineControl.Instance.cancameraMove = false;
        yield return new WaitForSeconds(1.8f);
        HideAfterDieCode();
        if (area != null)
        {
            foreach (var item in area.door)
            {
                item.locked = false;
            }

            area.ResetMon();
        }        
        PlayerControl.control.Spawn(spawnPoint);
        yield return new WaitForSeconds(1f);
        retrying = false;
        ControlScene.scene.LoadScene = false;
        PlayerControl.control.EnableInput(true);
        CinemachineControl.Instance.cancameraMove = true;
        ControlScene.scene.PlayAnimation();
    }


}
