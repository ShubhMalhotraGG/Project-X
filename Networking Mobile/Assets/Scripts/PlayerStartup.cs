using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerStartup : NetworkBehaviour
{
    [SerializeField] private Canvas movementCanvas;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CinemachineBrain cinemachine;

    public override void OnNetworkSpawn()
    {
        bool isLocal = IsOwner;

        // UI canvas
        movementCanvas.enabled = isLocal;

        // Only disable the Camera component itself
        playerCamera.enabled = isLocal;
        if (cinemachine != null)
            cinemachine.enabled = isLocal;

        // Untag non-local so Camera.main and CinemachineBrain won't grab them
        playerCamera.tag = isLocal ? "MainCamera" : "Untagged";

        // Movement script
        GetComponent<PlayerController>().enabled = isLocal;

        base.OnNetworkSpawn();
    }

}
