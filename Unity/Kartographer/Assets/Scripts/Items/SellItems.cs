// This script displays a prompt for selling items at the recycler and toggles a bool "sellItems" used in recycler.cs
// sellItems = true when a player interacts with the recycler after depositing scrap
using UnityEngine.InputSystem;
using UnityEngine;

public class SellItems : MonoBehaviour
{
    private GameObject localPlayer;
    private InteractPrompt interactPrompt;
    [SerializeField] private InputActionReference interact;

    [HideInInspector] public bool sellItems;
    [HideInInspector] public PlayerWallet PlayerWalletRef;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            localPlayer = other.gameObject;
            interactPrompt = localPlayer.GetComponent<InteractPrompt>();
            PlayerWalletRef = localPlayer.GetComponent<PlayerWallet>();

            interactPrompt.ToggleInteractPrompt("E", "Sell Items");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == localPlayer)
        {
            interactPrompt.ToggleInteractPrompt();
            localPlayer = null;
            PlayerWalletRef = null;
        }
    }

    void Update()
    {
        if (interact.action.WasPressedThisFrame())
        {
            sellItems = true;
        }
    }
}
