using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    // These methods will be hooked up to your buttons
    public void OnHostButtonClicked()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully");
        }
        else
        {
            Debug.LogError("Failed to start host");
        }
    }

    public void OnClientButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client trying to connect...");
    }
}
