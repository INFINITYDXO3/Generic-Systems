using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] private Transform hand;

    private Aim aim;

    public void SetAim(Aim aim)
    {
        this.aim = aim;

        if(hand != null)
        {
            Vector3 direction = aim.Direction;
            hand.rotation = Quaternion.LookRotation(direction);
        }
    }

    public Aim GetAim()
    {
        return aim;
    }


}
