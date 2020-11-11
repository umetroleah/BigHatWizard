using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CinemachineShake : MonoBehaviour
{

    public static CinemachineShake Instance { get; private set; }

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private float timer;

    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float frequency, float amplitude, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannel = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannel.m_AmplitudeGain = amplitude;
        cinemachineBasicMultiChannel.m_FrequencyGain = frequency;
        timer = time;
    }


    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannel = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannel.m_AmplitudeGain = 0f;
                cinemachineBasicMultiChannel.m_FrequencyGain = 0f;

            }
        }
    }
}
