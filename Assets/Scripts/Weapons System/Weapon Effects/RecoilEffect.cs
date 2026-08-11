using UnityEngine;

public class RecoilEffect : MonoBehaviour
{
    [Header("Recoil Speed settings")]
    [SerializeField] private float snappiness = 10f;
    [SerializeField] private float returnSpeed = 20f;

    [Header("Recoil affected objects")]
    [SerializeField] private Transform spineTransform;
    [SerializeField] private CameraSystem cameraSystem;
    
    //Recoil Effect private fields
    private Vector3 targetRotation;
    private Vector3 currentRotation;
    private Vector3 targetPosition;
    private Vector3 currentPosition;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = spineTransform.localPosition;
    }


    private void Update()
    {
        UpdateRecoilEffect();
    }


    public void ApplyRecoil(RecoilData recoilData)
    {
        targetRotation += new Vector3(Random.Range(-recoilData.RecoilX, recoilData.RecoilX), recoilData.RecoilY, Random.Range(-recoilData.RecoilZ, recoilData.RecoilZ));
        targetPosition -= new Vector3(0, initialPosition.y, recoilData.KickbackZ);                
    }

    private void UpdateRecoilEffect()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.deltaTime * snappiness);
        
        targetPosition = Vector3.Lerp(targetPosition, initialPosition, Time.deltaTime * returnSpeed);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * snappiness);

        cameraSystem.AdditionalRotation(currentRotation.x, currentRotation.y, currentRotation.z);
        currentPosition.y = spineTransform.localPosition.y;
        spineTransform.localPosition = currentPosition;
    }
}
