using Unity.Cinemachine;
using UnityEngine;

public class CharacterControllerCamView : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform camView;
    [SerializeField] private float yOffset;




    private void Start()
    {
        
    }

    private void Update()
    {
        UpdatePos();
    }

    private float GetCamViewDistance()
    {
        if (characterController != null && camView != null)
        {
            Vector3 worldCharacterCenter = characterController.transform.TransformPoint(characterController.center);
            return (camView.position - worldCharacterCenter).magnitude;
        }else return 0;
        
    }

    private void UpdatePos()
    {
        if (characterController != null && camView != null)
        {
            // cinemachineCamera.VerticalArmLength = GetCamViewDistance();
        }
    }

    private void OnDrawGizmos()
    {
        if (characterController != null && camView != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 clampedPos = characterController.transform.TransformPoint(characterController.center);
            clampedPos.y += GetCamViewDistance();

            Gizmos.DrawCube(clampedPos, Vector3.one * 0.3f);
        }
    }

}
