using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineControl : MonoBehaviour
{
    public static CinemachineControl Instance { get; private set; }
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineFramingTransposer transposer;
    private float shakeTimer;

    public Transform player;
    public float maxMouseDistance = 10f;
    public float maxCameraOffset = 5f;
    public float timeSmoothCamera = 10f;

    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cinemachineVirtualCamera.Follow = player;
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (cinemachineBasicMultiChannelPerlin != null)
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            shakeTimer = time;
        }
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

    private void LateUpdate()
    {
        if (player == null || transposer == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 playerPos = player.position;
        Vector3 direction = (mousePos - playerPos).normalized;

        float distance = Vector3.Distance(mousePos, playerPos);
        float cameraOffset = Mathf.Clamp(distance / maxMouseDistance * maxCameraOffset, 0, maxCameraOffset);


        Vector3 offset = direction * cameraOffset;
        transposer.m_TrackedObjectOffset = Vector3.Lerp(transposer.m_TrackedObjectOffset, offset, Time.deltaTime * timeSmoothCamera);
    }
}
