using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;

    [Header("Shake Effect Values")]
    [SerializeField] private float amplitude = 0.4f;
    [SerializeField] private float frequency = 0.4f;

    [Header("Sprint Effect Values")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 80f;

    
    


    

    private CinemachineBasicMultiChannelPerlin perlin;
    
    private Coroutine undoPerlin;


    private void Start()
    {
        virtualCamera.TryGetComponent(out perlin);
        perlin = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    


    //Shake Effect
    public void ShakeEffect()
    {
        perlin.AmplitudeGain = amplitude;
        perlin.FrequencyGain = frequency;

        undoPerlin ??= StartCoroutine(UndoPerlinEffect());
    }

    private IEnumerator UndoPerlinEffect()
    {
        while(perlin.AmplitudeGain > 0 || perlin.FrequencyGain > 0)
        {
            perlin.AmplitudeGain = Mathf.Clamp(perlin.AmplitudeGain - Time.deltaTime, 0, perlin.AmplitudeGain);
            perlin.FrequencyGain = Mathf.Clamp(perlin.FrequencyGain - Time.deltaTime, 0, perlin.FrequencyGain);
            yield return null;
        }

        undoPerlin = null;
    }
    
    //Sprint Effect
    public void SprintEffect(bool isSprinting)
    {
        float targetValue = (isSprinting)? sprintFOV : normalFOV;
        virtualCamera.Lens.FieldOfView = Mathf.Lerp(virtualCamera.Lens.FieldOfView, targetValue, Time.deltaTime * 2);
    }

    

}
