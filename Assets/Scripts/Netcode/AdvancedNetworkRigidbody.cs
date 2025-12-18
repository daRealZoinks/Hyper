using Unity.Netcode;
using UnityEngine;

namespace Hyper.Netcode
{
    public class AdvancedNetworkRigidbody : NetworkBehaviour
    {
        public float positionSmoothingFactor = 0.1f;
        public float rotationSmoothingFactor = 0.1f;

        public bool teleportEnabled = true;
        public float teleportIfDistanceGreaterThan = 10f;

        private Rigidbody _rigidbody;

        private Vector3 _networkPosition;
        private Quaternion _networkRotation;

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
            var timeDifference = (float)(localTime - serverTime);

            var predictedPosition = newValue.Position + newValue.LinearVelocity * timeDifference;
            var predictedRotation = newValue.Rotation * Quaternion.Euler(timeDifference * Mathf.Rad2Deg * newValue.AngularVelocity);

            if (teleportEnabled && Vector3.Distance(_rigidbody.position, predictedPosition) > teleportIfDistanceGreaterThan)
            {
                _rigidbody.position = predictedPosition;
                _rigidbody.rotation = predictedRotation;
            }
            else
            {
                _networkPosition = predictedPosition;
                _networkRotation = predictedRotation;
            }

            _rigidbody.linearVelocity = newValue.LinearVelocity;
            _rigidbody.angularVelocity = newValue.AngularVelocity;
        }

        private void FixedUpdate()
        {
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
                _rigidbody.position = Vector3.Lerp(_rigidbody.position, _networkPosition, positionSmoothingFactor);
                //_rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, _networkRotation, rotationSmoothingFactor);
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
}