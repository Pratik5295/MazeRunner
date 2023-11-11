using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //Add reference to virtual camera to this
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public void ShakeCamera(float intensity, float duration)
    {
        StartCoroutine(ShakeCameraCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCameraCoroutine(float intensity, float duration)
    {
        var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = intensity;
        noise.m_FrequencyGain = 10f; // Adjust the frequency as needed.

        yield return new WaitForSeconds(duration);

        noise.m_AmplitudeGain = 0f;
    }
}
