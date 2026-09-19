using UnityEngine;

public class AimController : MonoBehaviour
{
    private Aim aim;

    public void SetAim(Aim aim)
    {
        this.aim = aim;
    }

    public Aim GetAim()
    {
        return aim;
    }


}
