using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    /// <summary>
    /// A Transformer that translates, rotates and scales the target on a plane.
    /// </summary>
    public class CustomTwoGrabPlaneTransformer : MonoBehaviour, ITransformer
    {
        [SerializeField, Optional]
        private Transform _planeTransform = null;

        private Vector3 _capturePosition;

        private Vector3 _initialLocalScale;
        private float _initialDistance;
        private float _initialScale = 1.0f;
        private float _activeScale = 1.0f;

        private Pose _previousGrabA;
        private Pose _previousGrabB;

        [Serializable]
        public class TwoGrabPlaneConstraints
        {
            public FloatConstraint MinScale;
            public FloatConstraint MaxScale;
            public FloatConstraint MinY;
            public FloatConstraint MaxY;
        }

        [SerializeField]
        private TwoGrabPlaneConstraints _constraints;

        public TwoGrabPlaneConstraints Constraints
        {
            get
            {
                return _constraints;
            }
            set
            {
                _constraints = value;
            }
        }

        private IGrabbable _grabbable;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
        }

        public void BeginTransform()
        {
            var grabA = _grabbable.GrabPoints[0];
            var grabB = _grabbable.GrabPoints[1];
            var targetTransform = _grabbable.Transform;

            // Use the centroid of our grabs as the capture plane point
            _capturePosition = targetTransform.position;

            Transform planeTransform = _planeTransform != null ? _planeTransform : targetTransform;
            Vector3 rotationAxis = planeTransform.up;

            // Project our positional offsets onto a plane with normal equal to the rotation axis
            Vector3 initialOffset = grabB.position - grabA.position;
            Vector3 initialVector = Vector3.ProjectOnPlane(initialOffset, rotationAxis);
            _initialDistance = initialVector.magnitude;

            _initialScale = _activeScale = targetTransform.localScale.x;
            _previousGrabA = grabA;
            _previousGrabB = grabB;
        }

        public void UpdateTransform()
        {
            var grabA = _grabbable.GrabPoints[0];
            var grabB = _grabbable.GrabPoints[1];
            var targetTransform = _grabbable.Transform;

            Transform planeTransform = _planeTransform != null ? _planeTransform : targetTransform;
            Vector3 rotationAxis = planeTransform.up;

            // Project our positional offsets onto a plane with normal equal to the rotation axis
            Vector3 initialOffset = _previousGrabB.position - _previousGrabA.position;
            Vector3 initialVector = Vector3.ProjectOnPlane(initialOffset, rotationAxis);

            Vector3 targetOffset = grabB.position - grabA.position;
            Vector3 targetVector = Vector3.ProjectOnPlane(targetOffset, rotationAxis);

            // Scale logic
            float activeDistance = targetVector.magnitude;
            if(Mathf.Abs(activeDistance) < 0.0001f) activeDistance = 0.0001f;

            float scalePercentage = activeDistance / _initialDistance;

            _activeScale = _initialScale * scalePercentage;

            // Scale constraints
            _activeScale = Mathf.Max(_initialScale / 3, _activeScale);
            _activeScale = Mathf.Min(_initialScale * 3, _activeScale);



            // Bring back the bbox alignment
            BoxCollider boxCollider = targetTransform.GetComponent<BoxCollider>();
            Vector3 boxcollider_size_scaled = Vector3.Scale(boxCollider.size, targetTransform.lossyScale);
            Vector3 boxcollider_center_scaled = Vector3.Scale(boxCollider.center, targetTransform.lossyScale);
            targetTransform.localPosition -= Vector3.Scale(boxcollider_size_scaled / 2, gameObject.GetComponent<InformationHolder>().getStaticAxisVector());
            targetTransform.localPosition += boxcollider_center_scaled;

            targetTransform.localScale = _activeScale * Vector3.one;

            // Repeat the bbox alignment

            boxcollider_size_scaled = Vector3.Scale(boxCollider.size, targetTransform.lossyScale);
            boxcollider_center_scaled = Vector3.Scale(boxCollider.center, targetTransform.lossyScale);
            targetTransform.localPosition -= boxcollider_center_scaled;
            targetTransform.localPosition += Vector3.Scale(boxcollider_size_scaled / 2, gameObject.GetComponent<InformationHolder>().getStaticAxisVector());




            // Vector3 currentSize = targetTransform.parent.InverseTransformDirection(targetTransform.GetComponent<BoxCollider>().bounds.size);
            // Vector3 currentSizeAbs = new Vector3(Mathf.Abs(currentSize.x), Mathf.Abs(currentSize.y), Mathf.Abs(currentSize.z));
            // Vector3 localSizeAbs = new Vector3(currentSizeAbs.x / targetTransform.parent.lossyScale.x, currentSizeAbs.y / targetTransform.parent.lossyScale.y, currentSizeAbs.z / targetTransform.parent.lossyScale.z);

            // switch(gameObject.GetComponent<InformationHolder>().getStaticAxis()){
            //     case 0:
            //         targetTransform.localPosition =  new Vector3(localSizeAbs.x / 2, targetTransform.localPosition.y, targetTransform.localPosition.z);
            //         break;
            //     case 1:
            //         targetTransform.localPosition =  new Vector3(targetTransform.localPosition.x, localSizeAbs.y / 2, targetTransform.localPosition.z);
            //         break;
            //     case 2:
            //         targetTransform.localPosition =  new Vector3(targetTransform.localPosition.x, targetTransform.localPosition.y, localSizeAbs.z / 2);
            //         break;
            // }

            // targetTransform.localPosition -= Vector3.Scale(targetTransform.localPosition, gameObject.GetComponent<InformationHolder>().getStaticAxisVector());



            _previousGrabA = grabA;
            _previousGrabB = grabB;
        }

        public void EndTransform() {}

        #region Inject

        public void InjectOptionalPlaneTransform(Transform planeTransform)
        {
            _planeTransform = planeTransform;
        }

        public void InjectOptionalConstraints(TwoGrabPlaneConstraints constraints)
        {
            _constraints = constraints;
        }

        #endregion
    }
}
