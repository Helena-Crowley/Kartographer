using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Netcode.Components;
using System.Collections;
using TMPro;

public class GarageDoor : NetworkBehaviour
{
    [SerializeField] private Image percentageSlider;
    [SerializeField] private InputActionReference interact;
    [SerializeField] private AudioClip chargeUp;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private AudioClip teleportStartSound;
    [SerializeField] private Image fadeImage;

    private GameObject localPlayer;
    private bool playerInTrigger = false;
    private float holdTime = 0f;
    private float requiredHoldTime = 3f; // seconds to complete
    private AudioSource audioSource;
    private bool canPlaySound = true;
    private InteractPrompt interactPrompt;

    //Loading stuff
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private float typeSpeed;
    [SerializeField] private float lineDelay;
    [SerializeField] private AudioClip[] keyboardSounds;
    [SerializeField] private GarageButton garageButton;

    void Start() => percentageSlider.gameObject.SetActive(false);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        localPlayer = other.gameObject;
        playerInTrigger = true;

        interactPrompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        if (interactPrompt != null)
            interactPrompt.ToggleInteractPrompt("E", "begin", true);

        ResetProgress();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = false;

        interactPrompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        if (interactPrompt != null)
            interactPrompt.ToggleInteractPrompt();

        localPlayer = null;
        ResetProgress();
    }

    private void Update()
    {
        if (!playerInTrigger) return;

        if (interact.action.IsPressed())
        {

            garageButton.PressButton();

            if (canPlaySound)
            {
                audioSource = SoundManager.Instance.PlayLoopingSound(chargeUp, transform.position, "SFX", .15f);
                canPlaySound = false;
            }
            percentageSlider.gameObject.SetActive(true);
            holdTime += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTime / requiredHoldTime);
            percentageSlider.fillAmount = progress;

            if (holdTime >= requiredHoldTime)
            {
                canPlaySound = false;

                if (NetworkManager.Singleton.IsServer)
                {
                    ulong clientId = localPlayer.GetComponent<NetworkObject>().OwnerClientId;
                    StartCoroutine(TeleportSequence(clientId));
                    playerInTrigger = false;
                }
                else
                {
                    MovePlayerServerRpc();
                }

                interactPrompt.ToggleInteractPrompt();
                ResetProgress();
            }
        }
        else if (interact.action.WasReleasedThisFrame())
        {
            garageButton.UnpressButton();
            ResetProgress();
        }
    }

    private void ResetProgress()
    {
        canPlaySound = true;
        SoundManager.Instance.StopSound(audioSource);
        holdTime = 0f;
        if (percentageSlider != null)
            percentageSlider.fillAmount = 0f;
        percentageSlider.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void MovePlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        StartCoroutine(TeleportSequence(clientId));
    }

    private void SpawnPlayerAtRandom(ulong clientId)
    {
        if (!IsServer) return;
        if (spawnPoints == null || spawnPoints.Count == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        playerObject.gameObject.GetComponent<CharacterController>().enabled = false;

        if (playerObject != null)
        {
            Debug.Log("[SpawnPlayerAtRandom()]moving player to " + point.position);
            // For owner authority: tell the client to move themselves
            TeleportPlayerClientRpc(point.position, point.rotation,
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });
        }

        playerObject.gameObject.GetComponent<CharacterController>().enabled = true;
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams rpcParams = default)
    {
        Debug.Log("[SpawnPlayerAtRandom()]moving player to " + position);
        // This runs on the target client who owns the player
        if (NetworkManager.Singleton.LocalClient.PlayerObject != null)
        {
            var player = NetworkManager.Singleton.LocalClient.PlayerObject;
            var netTransform = player.GetComponent<NetworkTransform>();

            if (netTransform != null)
                netTransform.Teleport(position, rotation, Vector3.one);
            else
                player.transform.SetPositionAndRotation(position, rotation);
        }
    }


    private IEnumerator TeleportSequence(ulong clientId)
    {
        // Play pre-teleport sound
        SoundManager.Instance.PlaySound2D(teleportStartSound, "SFX", 0.2f);
        loadingText.enabled = true;
        yield return new WaitForSeconds(0.5f);
        // Fade to black
        yield return FadeRoutine(true, 1f); // your fade method (fadeIn = true)
        yield return TypeLines();

        // Move the player (server-side)
        if (IsServer)
            SpawnPlayerAtRandom(clientId);
        else
            MovePlayerServerRpc();

        // Wait a bit, then fade back in
        yield return new WaitForSeconds(0.75f);
        loadingText.enabled = false;
        yield return FadeRoutine(false, 1f);
    }

    private IEnumerator FadeRoutine(bool fadeIn, float duration)
    {
        float time = 0;
        Color start = fadeImage.color;
        Color end = fadeImage.color;
        end.a = fadeIn ? 1f : 0f;

        while (time < duration)
        {
            fadeImage.color = Color.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = end;
    }

    private IEnumerator TypeLines()
    {
        loadingText.text = "";
        int rand = Random.Range(0, keyboardSounds.Length);
        bool inColorTag = false;
        int charCount = 0;
        int soundFrequency = 7; //every n letters

        foreach (string line in lines)
        {
            foreach (char c in line)
            {
                if (c == '!')
                {
                    if (!inColorTag)
                    {
                        loadingText.text += "<color=#960019>";
                        inColorTag = true;
                    }
                    else
                    {
                        loadingText.text += "</color>";
                        inColorTag = false;
                    }
                }
                else
                {
                    loadingText.text += c;
                    charCount++;

                    if (charCount % soundFrequency == 0)
                    {
                        SoundManager.Instance.PlaySound2D(keyboardSounds[rand], "SFX", 0.07f, true);
                    }

                    yield return new WaitForSeconds(1f / typeSpeed);
                }
            }

            loadingText.text += "\n";
            yield return new WaitForSeconds(lineDelay);
        }

        if (inColorTag)
            loadingText.text += "</color>";
    }




    //Loading Script

    private string[] lines = new string[]
    {
        "// === Desert World Initialization Sequence ===",
        "// Project: !GolfKart Odyssey!",
        "// Environment: Arid Dunes [!Sector 7!]",
        "",
        "[BOOT] Starting terrain stream...",
        "[OK] Loading sand dune topology mesh (LOD 0-3)",
        "[OK] Applying heat haze shader variant (!DesertDay_01!)",
        "",
        "[BOOT] Initializing vehicle systems...",
        "[OK] GolfKart_Desert edition loaded",
        "[OK] Physics tuning: !TractionBoost=1.2! | !DriftControl=0.85!",
        "[OK] Fuel system calibrated (max !15L!)",
        "",
        "[FINALIZE] Establishing world link...",
        "[OK] World handshake successful",
        "![FAILED] Player is NOT ready.!",
        "",
        ">>> ENTERING DESERT ZONE <<<"
    };

}

