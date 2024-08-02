using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineControl : MonoBehaviour
{
    public static CinemachineControl Instance { get; private set; }
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private float shakeTimer;
    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        cinemachineVirtualCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (cinemachineBasicMultiChannelPerlin != null)
        {
            Debug.Log("!null");
        }
        else
        {
            Debug.Log("null");
        }
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer<=0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }
}
