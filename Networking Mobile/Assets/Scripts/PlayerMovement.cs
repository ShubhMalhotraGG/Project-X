using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float turnSpeed = 720f;

    [Header("Joystick Input")]
    public Joystick joystick;

    // Cache input to avoid sending every frame unnecessarily
    private Vector3 _lastSentInput;
    private Rigidbody _rb;
    private Animator _anim;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        if (joystick == null)
            Debug.LogError("Assign Joystick in Inspector");
    }

    public override void OnNetworkSpawn()
    {
        // Only keep the input-gathering and ServerRPC code on the owning client
        enabled = IsOwner;
        base.OnNetworkSpawn();
    }

    private void FixedUpdate()
    {
        // Gather input each physics frame
        Vector2 ji = new Vector2(joystick.Horizontal, joystick.Vertical);
        Vector3 dir = new Vector3(ji.x, 0, ji.y);
        bool moving = dir.sqrMagnitude > 0.01f;

        // Animate locally for responsiveness
        _anim.SetFloat("Speeed", moving ? 1f : 0f);

        // Only send to server if it actually changed
        if (moving && dir != _lastSentInput || !moving && _lastSentInput != Vector3.zero)
        {
            _lastSentInput = moving ? dir : Vector3.zero;
            SubmitMovementServerRpc(_lastSentInput);
        }

        // Handle jump input
        if (IsOwner && joystick.Vertical == 0 && Input.GetButtonDown("Jump"))  // or your own jump button
        {
            SubmitJumpServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void SubmitMovementServerRpc(Vector3 moveDir, ServerRpcParams rpcParams = default)
    {
        // This runs on the server�s copy of the player
        Vector3 vel = moveDir.normalized * moveSpeed;
        _rb.linearVelocity = new Vector3(vel.x, _rb.linearVelocity.y, vel.z);

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            _rb.MoveRotation(Quaternion.RotateTowards(
                _rb.rotation,
                targetRot,
                turnSpeed * Time.fixedDeltaTime));
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void SubmitJumpServerRpc(ServerRpcParams rpcParams = default)
    {
        // Ensure grounded on server before applying jump
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
