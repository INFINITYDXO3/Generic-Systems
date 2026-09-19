using UnityEngine;

public abstract class PlayerUIElement : MonoBehaviour
{
    public ElementType ElementType {get; private set;}

    /// <summary>
    /// Don't forget to set the element type in awake
    /// </summary>
    protected abstract void Awake();

    public abstract void SetValue<T>(T value);
    protected abstract void UpdateUI();

    protected void SetElementType(ElementType elementType)
    {
        ElementType = elementType;
    }
}
