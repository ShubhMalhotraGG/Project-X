using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnerSystem : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        var nm = NetworkManager.Singleton;
        if (nm.IsServer)
        {
            nm.OnClientConnectedCallback += HandleClientConnected;

            // ALSO spawn the host’s player immediately:
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        GameObject go = Instantiate(playerPrefab);
        go.GetComponent<NetworkObject>()
          .SpawnAsPlayerObject(clientId);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
    }
}