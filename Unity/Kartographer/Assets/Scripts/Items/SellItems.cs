// using UnityEngine.InputSystem;
// using UnityEngine;
// using System;

// public class SellItems : MonoBehaviour
// {

//     private GameObject localPlayer;
//     private InteractPrompt interactPrompt;
//     private InputActionMap playerMap;
//     private PlayerInputManager inputManager;
//     private InputAction interactAction;

//     [HideInInspector]
//     public bool sellItems;
//     public int value;

//     void OnTriggerEnter(Collider other)
//     {
//         if (other.tag == "Player")
//         {
//             localPlayer = other.gameObject;
//             interactPrompt = localPlayer.GetComponent<InteractPrompt>();


//             inputManager = localPlayer.GetComponent<PlayerInputManager>();
//             playerMap = inputManager.controls.FindActionMap("Player", true);
//             interactAction = playerMap.FindAction("Interact");

//             interactPrompt.ToggleInteractPrompt("E", "sell items");
//         }
//     }

//     void OnTriggerExit(Collider other)
//     {
//         if (other.gameObject == localPlayer)
//         {
//             interactPrompt.ToggleInteractPrompt();
//             localPlayer = null;

//         }
//     }

//     void Update()
//     {
//         if (interactAction != null)
//         {
//             if (interactAction.WasPressedThisFrame())
//             {
//                 sellItems = true;
//             }
//         }
//         if (value == 0)
//         {
//             sellItems = false;
//         }
//     }
// }

using UnityEngine.InputSystem;
using UnityEngine;

public class SellItems : MonoBehaviour
{
    private GameObject localPlayer;
    private InteractPrompt interactPrompt;
    private PlayerInputManager inputManager;
    private InputAction interactAction;

    [HideInInspector] public bool sellItems;
    [HideInInspector] public PlayerWallet PlayerWalletRef;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            localPlayer = other.gameObject;
            interactPrompt = localPlayer.GetComponent<InteractPrompt>();
            PlayerWalletRef = localPlayer.GetComponent<PlayerWallet>();

            inputManager = localPlayer.GetComponent<PlayerInputManager>();
            interactAction = inputManager.controls.FindActionMap("Player", true).FindAction("Interact");

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
        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            sellItems = true;
        }
    }
}
