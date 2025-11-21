

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.Netcode.Components;

public class GarageDoor : NetworkBehaviour
{
    [Header("UI & Input")]
    [SerializeField] private Image percentageSlider;
    [SerializeField] private InputActionReference interact;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Image fadeImage;

    [Header("Audio")]
    [SerializeField] private AudioClip chargeUp;
    [SerializeField] private AudioClip teleportStartSound;
    [SerializeField] private AudioSource radioAudio;
    [SerializeField] private AudioClip radioStatic;
    [SerializeField] private AudioClip[] keyboardSounds;

    [Header("Gameplay")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject dayNightCycleGO;
    [SerializeField] private GarageButton garageButton;

    private GameObject localPlayer;
    private InteractPrompt interactPrompt;
    private AudioSource audioSource;
    private bool canPlaySound = true;
    private bool playerInTrigger = false;
    private float holdTime = 0f;
    private float requiredHoldTime = 3f;

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
        "[BOOT] Initializing control systems...",
        "[I] GolfKart_Desert edition loaded",
        "[OK] Physics tuning: !TractionBoost=1.2! | !DriftControl=0.85!",
        "[OK] Fuel system calibrated (max !15L!)",
        "",
        "[BOOT] Initializing control systems...",
        "![I] Opens Scanner Gun!",
        "![M] Opens Map!",
        "![Q] Drop Item!",
        "",
        "!>>> FIND 4 BUILDINGS TO LEAVE <<<!",
        ">>> ENTERING DESERT ZONE <<<"
    };

    void Start()
    {
        percentageSlider.gameObject.SetActive(false);
        dayNightCycleGO.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        localPlayer = other.gameObject;
        playerInTrigger = true;

        interactPrompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        interactPrompt?.ToggleInteractPrompt("E", "begin", true);

        ResetProgress();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = false;

        interactPrompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        interactPrompt?.ToggleInteractPrompt();

        localPlayer = null;
        ResetProgress();
    }

    private void Update()
    {
        if (!playerInTrigger) return;
        if (!localPlayer.GetComponent<PlayerObj>().isAlive) return;

        if (interact.action.IsPressed())
        {
            garageButton.PressButton();

            if (canPlaySound)
            {
                audioSource = SoundManager.Instance.PlayLoopingSound(chargeUp, transform.position, "SFX", 0.15f);
                canPlaySound = false;
            }

            percentageSlider.gameObject.SetActive(true);
            holdTime += Time.deltaTime;
            percentageSlider.fillAmount = Mathf.Clamp01(holdTime / requiredHoldTime);

            if (holdTime >= requiredHoldTime)
            {
                interactPrompt?.ToggleInteractPrompt();
                ResetProgress();

                if (IsServer)
                {
                    //StartCoroutine(ClientTeleportSequence()); // local UI for button presser
                    StartCoroutine(ServerTeleportEveryoneSequence());
                }
                else
                {
                    //StartCoroutine(ClientTeleportSequence()); // local UI for button presser
                    TriggerTeleportServerRpc();
                }
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
        if (audioSource != null) SoundManager.Instance.StopSound(audioSource);
        holdTime = 0f;
        if (percentageSlider != null)
        {
            percentageSlider.fillAmount = 0f;
            percentageSlider.gameObject.SetActive(false);
        }
    }

    // ----- Server teleport for all players -----
    [ServerRpc(RequireOwnership = false)]
    private void TriggerTeleportServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;
        StartCoroutine(ServerTeleportEveryoneSequence());
    }

    private IEnumerator ServerTeleportEveryoneSequence()
    {
        StartUISequenceClientRpc();
        yield return new WaitForSeconds(1);
        foreach (var clientPair in NetworkManager.Singleton.ConnectedClients)
        {
            ulong clientId = clientPair.Key;
            var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerObj>();
            client.inOutpost = false;
            SpawnPlayerAtRandom(clientId);
        }
        
        yield return null;
    }

    private void SpawnPlayerAtRandom(ulong clientId)
    {
        if (!IsServer || spawnPoints.Count == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (playerObject == null) return;

        CharacterController cc = playerObject.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        TeleportPlayerClientRpc(point.position, point.rotation, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { clientId } }
        });

        if (cc != null) cc.enabled = true;
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams rpcParams = default)
    {
        var player = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (player == null) return;

        var netTransform = player.GetComponent<NetworkTransform>();
        if (netTransform != null) netTransform.Teleport(position, rotation, Vector3.one);
        else player.transform.SetPositionAndRotation(position, rotation);
    }

    [ClientRpc]
    private void StartUISequenceClientRpc()
    {
        StartCoroutine(ClientTeleportSequence());
    }
    
    // ----- Client UI / Effects -----
    private IEnumerator ClientTeleportSequence()
    {
        percentageSlider.gameObject.SetActive(false);
        playerInTrigger = false;
        radioAudio.enabled = false;
        SoundManager.Instance.PlaySound2D(teleportStartSound, "SFX", 0.2f);
        loadingText.enabled = true;
        //yield return new WaitForSeconds(0.5f);

        SoundManager.Instance.PlayMusic(radioStatic, "SFX", 0.1f, 1);
        yield return FadeRoutine(true, 1f);
        yield return TypeLines();
        yield return new WaitForSeconds(2f);

        loadingText.enabled = false;
        SoundManager.Instance.StopMusic(1.25f);
        dayNightCycleGO.SetActive(true);
        yield return FadeRoutine(false, 1f);


    }

    private IEnumerator FadeRoutine(bool fadeIn, float duration)
    {
        float time = 0f;
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
        int soundFrequency = 7;

        foreach (string line in lines)
        {
            foreach (char c in line)
            {
                if (c == '!')
                {
                    if (!inColorTag) { loadingText.text += "<color=#960019>"; inColorTag = true; }
                    else { loadingText.text += "</color>"; inColorTag = false; }
                }
                else
                {
                    loadingText.text += c;
                    charCount++;
                    if (charCount % soundFrequency == 0)
                        SoundManager.Instance.PlaySound2D(keyboardSounds[rand], "SFX", 0.07f, true);

                    yield return new WaitForSeconds(1f / 85f); // typeSpeed
                }
            }
            loadingText.text += "\n";
            yield return new WaitForSeconds(0.05f); // lineDelay
        }

        if (inColorTag) loadingText.text += "</color>";
    }
}
