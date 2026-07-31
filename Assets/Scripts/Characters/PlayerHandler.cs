using System;
using UnityEngine;

public class PlayerHandler : CharactersHandler
{
    private void Update()
    {
        SetAim(new (cameraSystem.Cam.transform.position, cameraSystem.Cam.transform.forward));
    }
}
