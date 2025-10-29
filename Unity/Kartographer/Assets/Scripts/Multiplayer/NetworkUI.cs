using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField ipInputField;
    public Button hostButton;
    public Button joinButton;
    public GameObject joinScreen;
    [SerializeField] private AudioListener audioListener;

    [SerializeField] private TMP_Text ipDisplayText;

    void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(OnJoinClicked);

        string localIP = GetLocalIPAddress();
        Debug.Log($"[Network] Local IP Address: {localIP}");

        if (ipDisplayText != null)
            ipDisplayText.text = $"Host IP: {localIP}";
    }

    private void StartHost()
    {
        var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        transport.SetConnectionData(GetLocalIPAddress(), 7777); // host on port 7777
        NetworkManager.Singleton.StartHost();

        Debug.Log("Hosting on " + GetLocalIPAddress());

        // Show loading screen for host
        GameManager.Instance.LoadScene("OutPost"); // your loading screen handles fade/progress

        Destroy(joinScreen);
    }

    private void OnJoinClicked()
    {
        string ipAddress = ipInputField.text.Trim();
        if (string.IsNullOrEmpty(ipAddress))
        {
            Debug.LogWarning("No IP entered! Please enter the host's LAN IP address.");
            return;
        }

        var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        transport.SetConnectionData(ipAddress, 7777); // connect to host IP
        NetworkManager.Singleton.StartClient();

        Debug.Log("Trying to connect to " + ipAddress);
        // Optional: show temporary connecting/loading UI
        GameManager.Instance.LoadScene("OutPost"); 
    
        Destroy(joinScreen);
    }

    private string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                return ip.ToString();
        }
        return "127.0.0.1";
    }
}
