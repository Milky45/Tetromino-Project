
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerSetup : MonoBehaviour
{
    void Start()
    {
        gameObject.AddComponent<NetworkManager>();
        gameObject.AddComponent<UnityTransport>();
        NetworkManager.Singleton.NetworkConfig = new NetworkConfig()
        {
            PlayerPrefab = null,
            ConnectionApproval = false,
            EnableSceneManagement = true,
            NetworkTransport = gameObject.GetComponent<UnityTransport>()
        };
    }
}
