using UnityEngine;

// Attach this script to your Canvas or UI Panel.
// Then have your Close button call CloseInteraction() on this object.
public class UIButtonHelper : MonoBehaviour
{
    public void CloseInteraction()
    {
        InteractableItem.CloseCurrentUI();
    }
}
