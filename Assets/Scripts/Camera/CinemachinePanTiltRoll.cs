using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Hyper.Camera
{
    public class CinemachinePanTiltRoll : CinemachineComponentBase, IInputAxisOwner, IInputAxisResetSource, CinemachineFreeLookModifier.IModifierValueSource
    {
        public enum ReferenceFrames
        {
            ParentObject,
            World,
            TrackingTarget,
            LookAtTarget
        }

        public ReferenceFrames ReferenceFrame = ReferenceFrames.ParentObject;

        public enum RecenterTargetModes
        {
            AxisCenter,
            TrackingTargetForward,
            LookAtTargetForward
        }

        public RecenterTargetModes RecenterTarget = RecenterTargetModes.AxisCenter;

        public InputAxis PanAxis = DefaultPan;
        public InputAxis TiltAxis = DefaultTilt;
        public InputAxis RollAxis = DefaultRoll;

        private Quaternion _previousCameraRotation;

        private Action _resetHandler;

        private void OnValidate()
        {
            PanAxis.Validate();
            TiltAxis.Range.x = Mathf.Clamp(TiltAxis.Range.x, -90, 90);
            TiltAxis.Range.y = Mathf.Clamp(TiltAxis.Range.y, -90, 90);
            TiltAxis.Validate();
            RollAxis.Range.x = Mathf.Clamp(RollAxis.Range.x, -90, 90);
            RollAxis.Range.y = Mathf.Clamp(RollAxis.Range.y, -90, 90);
            RollAxis.Validate();
        }

        private void Reset()
        {
            PanAxis = DefaultPan;
            TiltAxis = DefaultTilt;
            RollAxis = DefaultRoll;
            ReferenceFrame = ReferenceFrames.ParentObject;
            RecenterTarget = RecenterTargetModes.AxisCenter;
        }

        private static InputAxis DefaultPan => new() { Value = 0, Range = new Vector2(-180, 180), Wrap = true, Center = 0, Recentering = InputAxis.RecenteringSettings.Default };
        private static InputAxis DefaultTilt => new() { Value = 0, Range = new Vector2(-90, 90), Wrap = false, Center = 0, Recentering = InputAxis.RecenteringSettings.Default };
        private static InputAxis DefaultRoll => new() { Value = 0, Range = new Vector2(-5, 5), Wrap = false, Center = 0, Recentering = InputAxis.RecenteringSettings.Default };

        public void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new IInputAxisOwner.AxisDescriptor { DrivenAxis = () => ref PanAxis, Name = "Look X (Pan)", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new IInputAxisOwner.AxisDescriptor { DrivenAxis = () => ref TiltAxis, Name = "Look Y (Tilt)", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
        }

        public void RegisterResetHandler(Action handler)
        {
            _resetHandler += handler;
        }

        public void UnregisterResetHandler(Action handler)
        {
            _resetHandler -= handler;
        }

        public float NormalizedModifierValue
        {
            get
            {
                var r = TiltAxis.Range.y - TiltAxis.Range.x;
                return (TiltAxis.Value - TiltAxis.Range.x) / (r > 0.001f ? r : 1) * 2 - 1;
            }
        }

        public bool HasResetHandler => _resetHandler != null;

        public override bool IsValid => enabled;

        public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid)
            {
                return;
            }

            if (deltaTime < 0 || !VirtualCamera.PreviousStateIsValid || !CinemachineCore.IsLive(VirtualCamera))
            {
                _resetHandler?.Invoke();
            }

            var referenceFrame = GetReferenceFrame(curState.ReferenceUp);

            var rotation = referenceFrame * Quaternion.Euler(TiltAxis.Value, PanAxis.Value, RollAxis.Value);
            curState.RawOrientation = rotation;

            if (VirtualCamera.PreviousStateIsValid)
            {
                curState.RotationDampingBypass *= UnityVectorExtensions.SafeFromToRotation(_previousCameraRotation * Vector3.forward, rotation * Vector3.forward, curState.ReferenceUp);
            }

            _previousCameraRotation = rotation;

            var gotInputX = PanAxis.TrackValueChange();
            var gotInputY = TiltAxis.TrackValueChange();
            var gotInputZ = RollAxis.TrackValueChange();

            if (PanAxis.Recentering.Time == TiltAxis.Recentering.Time && PanAxis.Recentering.Time == RollAxis.Recentering.Time)
            {
                gotInputX = gotInputX | gotInputY | gotInputZ;
                gotInputY = gotInputX;
                gotInputZ = gotInputX;

            }

            if (Application.isPlaying)
            {
                var recenterTarget = GetRecenterTarget();

                PanAxis.UpdateRecentering(deltaTime, gotInputX, recenterTarget.x);
                TiltAxis.UpdateRecentering(deltaTime, gotInputY, recenterTarget.y);
                RollAxis.UpdateRecentering(deltaTime, gotInputZ, recenterTarget.z);
            }
        }

        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            SetAxesForRotation(rot);
        }

        public override bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            _resetHandler?.Invoke();

            if (fromCam != null
                && (VirtualCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != 0
                && !CinemachineCore.IsLiveInBlend(VirtualCamera))
            {
                SetAxesForRotation(fromCam.State.RawOrientation);
                return true;
            }

            return false;
        }

        private void SetAxesForRotation(Quaternion targetRot)
        {
            _resetHandler?.Invoke();

            var up = VcamState.ReferenceUp;
            var forward = GetReferenceFrame(up) * Vector3.forward;

            PanAxis.Value = 0;
            var targetForward = targetRot * Vector3.forward;
            var a = forward.ProjectOntoPlane(up);
            var b = targetForward.ProjectOntoPlane(up);
            if (!a.AlmostZero() && !b.AlmostZero())
            {
                PanAxis.Value = Vector3.SignedAngle(a, b, up);
            }

            TiltAxis.Value = 0;
            forward = Quaternion.AngleAxis(PanAxis.Value, up) * forward;
            var right = Vector3.Cross(up, forward);
            if (!right.AlmostZero())
            {
                TiltAxis.Value = Vector3.SignedAngle(forward, targetForward, right);
            }

            RollAxis.Value = 0;
            forward = Quaternion.AngleAxis(TiltAxis.Value, right) * forward;
            var currentUp = Vector3.Cross(forward, right);
            var targetUp = targetRot * Vector3.up;
            if (!forward.AlmostZero() && !targetUp.AlmostZero())
            {
                RollAxis.Value = Vector3.SignedAngle(currentUp, targetUp, forward);
            }
        }

        private Vector3 GetRecenterTarget()
        {
            var t = RecenterTarget switch
            {
                RecenterTargetModes.TrackingTargetForward => VirtualCamera.Follow,
                RecenterTargetModes.LookAtTargetForward => VirtualCamera.LookAt,
                _ => null,
            };

            if (t != null)
            {
                var forward = t.forward;
                var parent = VirtualCamera.transform.parent;
                if (parent)
                {
                    forward = parent.rotation * forward;
                }
                var v = Quaternion.FromToRotation(Vector3.forward, forward).eulerAngles;

                return new Vector3()
                {
                    x = RecenterTarget == RecenterTargetModes.TrackingTargetForward ? NormalizeAngle(v.y) : PanAxis.Center,
                    y = RecenterTarget == RecenterTargetModes.LookAtTargetForward ? NormalizeAngle(v.x) : TiltAxis.Center,
                    z = RollAxis.Center
                };
            }

            return new Vector3(PanAxis.Center, TiltAxis.Center, RollAxis.Center);

            static float NormalizeAngle(float angle) => (angle + 180) % 360 - 180;
        }

        private Quaternion GetReferenceFrame(Vector3 up)
        {
            var target = ReferenceFrame switch
            {
                ReferenceFrames.TrackingTarget => FollowTarget,
                ReferenceFrames.LookAtTarget => LookAtTarget,
                ReferenceFrames.ParentObject => VirtualCamera.transform.parent,
                ReferenceFrames.World or _ => null,
            };

            return target ? target.rotation : Quaternion.FromToRotation(Vector3.up, up);
        }
    }
}
