using UnityEngine;

public class PlayerHandler : CharactersHandler
{

    public override void OnStartClient()
    {
        base.OnStartClient();
    
        if(TryGetComponent(out AudioListener audioListener))
        {
            audioListener.enabled = isLocalPlayer;
        }
    }

    private void Update()
    {
        SetAim(new (cameraSystem.CinemachineCamera.transform.position, cameraSystem.CinemachineCamera.transform.forward));
    }

}
