using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    //public GameObject Walk; //0
    //public GameObject Shoot; //1
    //public GameObject Melee; //2
    //public GameObject Ulti; //3
    //public GameObject Heal; //4
    // public GameObject Reload; //5
    // public GameObject Dash; //6
    // public GameObject Weapon; //7
    public static Tutorial set; 
    public GameObject[] mode;
    public int curMode;
    public bool tutorial;
    public Coroutine curWait;
    //public KeyCode key;
    public string key;

    private void Awake()
    {
        set = this;
    }

    public void show(int index,float time)
    {
        if (tutorial)
        {
            return;
        }

        curMode = index;
        switch (curMode)
        {
            case 0:
                key = "Walk";
                break;
            case 1:
                key = "Fire1";
                break;
            case 2:
                key = "Fire2";
                break;
            case 3:
                key = "Ultimate";
                break;
            case 4:
                key = "Heal";
                break;
            case 5:
                key = "Reload";
                break;
            case 6:
                key = "Jump";
                break;
            case 7:
                key = "Wheel";
                break;
        }
        mode[curMode].SetActive(true);
        curWait = StartCoroutine(Wait(time));
        tutorial = true;
    }

    private void Update()
    {
        if (tutorial)
        {
            if (key == "Walk")
            {
                if (PlayerControl.control.playerMovement.rb.velocity != Vector2.zero)
                {
                    StopCoroutine(curWait);
                    curWait = StartCoroutine(Wait(0.5f));
                    tutorial = false;
                }
            }
            else if (Input.GetButton(key))
            {
                StopCoroutine(curWait);
                curWait = StartCoroutine(Wait(0.5f));
                tutorial = false;
            }
        }
    }

    IEnumerator Wait(float wait)
    {
        yield return new WaitForSeconds(wait);
        mode[curMode].SetActive(false);
    }
}
