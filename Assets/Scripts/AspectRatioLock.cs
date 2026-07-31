using UnityEngine;
public class AspectRatioLock : MonoBehaviour
{
    [SerializeField]
    private float targetAspect = 1.777778f;

    void Update()
    {
        Camera.main.aspect = targetAspect;
    }

}
