using UnityEngine;

public class RentStation : MonoBehaviour
{
    [Header("Rent Settings")]
    [SerializeField] private string interactionPrompt = "Press E to Pay Rent";
    [SerializeField] private string insufficientFundsPrompt = "Not Enough Money!";
    [SerializeField] private bool isRentDue = false;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public string GetInteractionPrompt()
    {
        if (gameManager == null)
        {
            return interactionPrompt;
        }

        if (!isRentDue)
        {
            return "Rent not due yet";
        }

        int currentPoints = gameManager.GetCurrentPoints();
        int rentCost = gameManager.GetCurrentRentCost();

        if (currentPoints < rentCost)
        {
            return $"{insufficientFundsPrompt} ({currentPoints}/{rentCost})";
        }

        return $"Press E to Pay Rent (${rentCost})";
    }

    public void Interact(PlayerController player)
    {
        if (gameManager == null || !isRentDue)
        {
            return;
        }

        int currentPoints = gameManager.GetCurrentPoints();
        int rentCost = gameManager.GetCurrentRentCost();

        if (currentPoints >= rentCost)
        {
            gameManager.PayRent();
            isRentDue = false;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCreateSandwichSound();
            }
        }
    }

    public void SetRentDue(bool isDue)
    {
        isRentDue = isDue;
    }

    public bool IsRentDue()
    {
        return isRentDue;
    }
}
