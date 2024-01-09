using UnityEngine;

namespace IA.Utils
{
    [System.Serializable]
    public class HingeJointRotationCalculator
    {
        [SerializeField] private Transform transform;
        [SerializeField] private Transform parentTransform;
        [SerializeField] private HingeJoint hingeJoint;
        [SerializeField] private JointSpring jointSpring;

        [SerializeField] private Quaternion targetRotation;
        [SerializeField] private Quaternion rotationOffset;

        public HingeJointRotationCalculator(Transform _transform, Transform _parent, HingeJoint _hingeJoint, Quaternion _targetRot)
        {
            transform = _transform;
            parentTransform = _parent;
            hingeJoint = _hingeJoint;
            targetRotation = _targetRot;

            jointSpring = hingeJoint.spring;

            rotationOffset = targetRotation * Quaternion.Inverse(transform.localRotation);
        }

        public void CalculateTargetPosition()
        {
            float angle = CalculateAngleToDown(rotationOffset * -parentTransform.up);

            jointSpring.targetPosition = (angle);
            hingeJoint.spring = jointSpring;
        }

        public float CalculateAngleToDown(Vector3 direction)
        {
            // Calculate the target direction by negating the up direction of the parent transform
            Vector3 targetDirection = direction;

            // Calculate the angle between the downward direction (Vector3.down) and the target direction
            float angle = Vector3.Angle(Vector3.down, targetDirection);

            // Calculate the sign of the angle
            float sign = Mathf.Sign(Vector3.Dot(Vector3.right, targetDirection));

            // Scale the angle by the sign
            angle *= sign;

            // Ensure that the angle is always between 0 and 360 degrees
            angle = Mathf.Repeat(angle, 360f);

            return angle;
        }
    }
}
