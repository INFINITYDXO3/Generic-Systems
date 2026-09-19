using UnityEngine;

public class PlayerHandler : CharactersHandler
{
    public static PlayerHandler LocalPlayer;

    [SerializeField] protected CameraSystem cameraSystem;
    [SerializeField] private MeshRenderer bodyMeshRenderer;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(LocalPlayer == null && isLocalPlayer)
        {
            LocalPlayer = this;
        }

        if(TryGetComponent(out AudioListener audioListener))
        {
            audioListener.enabled = isLocalPlayer;
        }

        if(bodyMeshRenderer != null)
        {
            bodyMeshRenderer.enabled = !isLocalPlayer;
        }

        if(!isLocalPlayer)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            hand.gameObject.layer = LayerMask.NameToLayer("Enemy");
            Transform[] children = hand.GetComponentsInChildren<Transform>(true);
            foreach(Transform child in children)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
    }

    private void FixedUpdate()
    {
        SetAim(new (cameraSystem.CinemachineCamera.transform.position, cameraSystem.CinemachineCamera.transform.forward));
    }

    internal void ProcessLook(Vector2 lookInput)
    {
        if(cameraSystem == null) return;
        
        cameraSystem.RotateCamera(lookInput);
    }

    internal void ApplyCameraEffect(CameraEffectsType effectType, bool isOn)
    {
        if(cameraSystem == null) return;
        
        cameraSystem.ApplyCameraEffect(effectType, isOn);
    }

}
