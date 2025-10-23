using Unity.Netcode;
using UnityEngine;

public class AdvancedNetworkRigidbody : NetworkBehaviour
{
    public bool teleportEnabled = true;
    public float teleportIfDistanceGreaterThan = 10f;

    private Rigidbody _rigidbody;

    private float _distance;
    private float _angle;

    private Vector3 _networkPosition = new();
    private Quaternion _networkRotation = new();

    private readonly NetworkVariable<PhysicsSnapshot> _physicsSnapshot = new(writePerm: NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        _physicsSnapshot.OnValueChanged += OnPhysicsSnapshotChanged;

    }

    public override void OnNetworkDespawn()
    {
        _physicsSnapshot.OnValueChanged -= OnPhysicsSnapshotChanged;
    }

    private void OnPhysicsSnapshotChanged(PhysicsSnapshot previousValue, PhysicsSnapshot newValue)
    {
        if (IsOwner) return;

        var localTime = NetworkManager.NetworkTimeSystem.LocalTime;
        var serverTime = NetworkManager.NetworkTimeSystem.ServerTime;
        var timeDifference = localTime - serverTime;

        Debug.Log($"Local Time: {localTime}, Server Time: {serverTime}, Difference: {timeDifference}");

        var predictedPosition = newValue.Position + newValue.LinearVelocity * (float)timeDifference;
        var predictedRotation = newValue.Rotation * Quaternion.Euler(newValue.AngularVelocity * Mathf.Rad2Deg * (float)timeDifference);

        if (teleportEnabled && Vector3.Distance(_rigidbody.position, predictedPosition) > teleportIfDistanceGreaterThan)
        {
            _rigidbody.position = predictedPosition;
            _rigidbody.rotation = predictedRotation;
        }
        else
        {
            _rigidbody.position = Vector3.Lerp(_rigidbody.position, predictedPosition, 0.1f);
            _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, predictedRotation, 0.1f);
        }

        _rigidbody.linearVelocity = newValue.LinearVelocity;
        _rigidbody.angularVelocity = newValue.AngularVelocity;
    }

    private void FixedUpdate()
    {
        if (!NetworkManager.IsListening) return;

        if (IsOwner)
        {
            _physicsSnapshot.Value = new PhysicsSnapshot
            {
                Position = _rigidbody.position,
                Rotation = _rigidbody.rotation,
                LinearVelocity = _rigidbody.linearVelocity,
                AngularVelocity = _rigidbody.angularVelocity
            };
        }
        else
        {
            _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, _networkPosition, _distance);
            _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, _networkRotation, _angle);
        }
    }

    private struct PhysicsSnapshot : INetworkSerializable
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref LinearVelocity);
            serializer.SerializeValue(ref AngularVelocity);
        }
    }
}
