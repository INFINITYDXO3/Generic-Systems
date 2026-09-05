using Unity.Cinemachine;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering.Universal;
public class CameraSystem : NetworkBehaviour
{
    [SerializeField] private float xSensitivity = 30f;
    [SerializeField] private float ySensitivity = 30f;

    
    [Header("Cinemachine")]
    [SerializeField] private CameraEffects cameraEffects;
    [SerializeField] private CinemachineCamera cinemachineCam;
    [SerializeField] private CinemachineBrain cinemachineBrain;

    [Header("Weapons Camera"), SerializeField] private Camera weaponsCam;

    [Header("Clamp Settings")]

    [SerializeField, Tooltip("How far in degrees can you move the camera up")]
    private float TopClamp = 70.0f;

    [SerializeField, Tooltip("How far in degrees can you move the camera down")]
    private float BottomClamp = -30.0f;

    private float xRotation = 0f;
    private float additionalXRotation;
    private float additionalYRotation;
    private float additionalZRotation;
    
    public CameraEffects CameraEffects {get => cameraEffects;}
    public Camera Cam {get => cinemachineBrain.OutputCamera;}
    public CinemachineCamera CinemachineCamera {get => cinemachineCam;}

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!isLocalPlayer)return;
        cinemachineCam.Priority = 1;
        if(cinemachineBrain == null) cinemachineBrain = FindAnyObjectByType<CinemachineBrain>();
        if(cinemachineBrain != null) Cam.GetUniversalAdditionalCameraData().cameraStack.Add(weaponsCam);

    }

    public void RotateCamera(Vector2 lookVector)
    {
        float mouseX = lookVector.x;
        float mouseY = lookVector.y;

        xRotation -= mouseY * Time.deltaTime * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, BottomClamp, TopClamp);
        
        cinemachineCam.Follow.localRotation = Quaternion.Euler(xRotation + additionalXRotation, additionalYRotation, additionalZRotation);
        transform.Rotate(mouseX * Time.deltaTime * xSensitivity * Vector3.up);

    }

    public void AdditionalRotation(float x, float y, float z)
    {
        additionalXRotation = -y;
        additionalYRotation = x;
        additionalZRotation = z;
    }

    public void ApplyCameraEffect(CameraEffectsType effectType, bool isOn)
    {
        switch (effectType)
        {
            case CameraEffectsType.ShakeEffect:
                cameraEffects.ShakeEffect();
                break;
            case CameraEffectsType.SprintEffect:
                cameraEffects.SprintEffect(isOn);
                break;
        }
    }

}
