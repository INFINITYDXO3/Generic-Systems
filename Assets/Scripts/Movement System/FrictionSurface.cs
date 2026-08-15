using UnityEngine;

public class FrictionSurface : MonoBehaviour
{
    [SerializeField] private FrictionType frictionType;
    [SerializeField] private float frictionValue;

    public FrictionType FrictionType {get => frictionType;}
    public float FrictionValue {get => frictionValue;}
}
