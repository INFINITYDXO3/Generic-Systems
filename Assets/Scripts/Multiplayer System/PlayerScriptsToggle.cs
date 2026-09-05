using UnityEngine;
using Mirror;

public class PlayerScriptsToggle : NetworkBehaviour
{
    [SerializeField] private MonoBehaviour[] scriptsToToggle;
    [SerializeField] private bool ToggleOnStart = true;

    private void Start()
    {
        if(ToggleOnStart) ToggleScripts();
    }

    public void ToggleScripts()
    {
        foreach (var script in scriptsToToggle)
        {
            script.enabled = isLocalPlayer;
        }
    }
}
