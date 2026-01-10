using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public string itemName;
    [TextArea] public string itemDescription;
    public GameObject uiPanel; // Drag your 'InteractionMenu' here
    public TMPro.TextMeshProUGUI descriptionText; // Drag the Text object here
    public Transform interactionPoint;

    private ClickToMove playerMovement;
    
    // Track the currently open item so the button knows what to close
    private static InteractableItem currentlyOpen;

    void Start()
    {
        // Hide the UI when the game starts
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    public void ShowUI(ClickToMove player)
    {
        // Close any other open item first
        if (currentlyOpen != null && currentlyOpen != this)
    {
            currentlyOpen.CloseUI();
        }
        
        currentlyOpen = this;
        playerMovement = player;
        uiPanel.SetActive(true);
        descriptionText.text = $"<b>{itemName}</b>\n{itemDescription}";
    }

    public void CloseUI()
    {
        uiPanel.SetActive(false);
        currentlyOpen = null;
        
        // Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.ResumeMovement();
        }
    }

    // Static method - the button calls THIS, works for any item!
    public static void CloseCurrentUI()
    {
        if (currentlyOpen != null)
        {
            currentlyOpen.CloseUI();
        }
    }
}
