using UnityEngine.UI;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Canvas worldCanvas;
    
    [SerializeField]
    private Image crosshair;
 
    private Vector2 position;

    public Vector2 Position {get => position;}

    private void Start()
    {
        UpdateCrosshair(new (Screen.width/2 , Screen.height/2));
    }


    private void UpdateCrosshair(Vector2 rectPosition)
    {
        crosshair.transform.position = rectPosition;
        position = crosshair.transform.position;
    }
}
