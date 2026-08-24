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
            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation.eulerAngles = new(rotation.eulerAngles.x, hand.eulerAngles.y, hand.eulerAngles.z);
            
            hand.rotation = rotation;

        }
    }

    public Aim GetAim()
    {
        return aim;
    }


}
