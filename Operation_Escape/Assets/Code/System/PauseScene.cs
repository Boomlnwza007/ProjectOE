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
        PlayerControl.control.EnableInput(!onPauseMenu);
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
        PlayerControl.control.EnableInput(!onmenuAfterDie);

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
        ControlScene.scene.animator.speed = 10;
        retrying = true;
        ControlScene.scene.LoadScene = true;
        PlayerControl.control.EnableInput(false);
        CinemachineControl.Instance.cancameraMove = false;
        yield return new WaitForSeconds(1f);
        ControlScene.scene.animator.speed = 1;
        HideAfterDieCode();
        if (area != null)
        {
            foreach (var item in area.door)
            {
                item.UnlockDead();
            }

            area.ResetMon();
        }

        AreaEnermy[] areaAll = GameObject.FindObjectsOfType<AreaEnermy>();
        foreach (var item in areaAll)
        {
            item.ReItem();
        }

        if (spawnPoint != null)
        {
            Debug.Log(1);
            PlayerControl.control.Spawn(spawnPoint);
        }
        else
        {
            Debug.Log(2);
            PlayerControl.control.Spawn(firstSpawn);
            spawnPoint = firstSpawn;

        }
        yield return new WaitForSeconds(1f);
        retrying = false;
        ControlScene.scene.LoadScene = false;
        PlayerControl.control.EnableInput(true);
        CinemachineControl.Instance.cancameraMove = true;
        ControlScene.scene.PlayAnimation();
        PlayerControl.control.isdaed = false;
    }


}
