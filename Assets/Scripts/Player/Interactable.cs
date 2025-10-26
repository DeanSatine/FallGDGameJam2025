using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string interactionPrompt = "Press E to make sandwich";
    [SerializeField] private int sandwichesGiven = 1;

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public void Interact(PlayerController player)
    {
        player.AddSandwiches(sandwichesGiven);
        player.CreateSandwichInHand();
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCreateSandwichSound();
        }
    }
}
