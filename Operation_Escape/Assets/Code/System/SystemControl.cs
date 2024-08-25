using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemControl : MonoBehaviour
{
    [SerializeField]private PlayerControl playerControl;
    //[SerializeField]private CinemachineControl cinemachineControl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    public void Cutscene(bool On)
    {
        playerControl.EnableInput(On);
        CinemachineControl.Instance.cancameraMove = On;
    }
}
