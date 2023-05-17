using System;
using UnityEngine;

namespace Oculus.Interaction
{
    /// <summary>
    /// A Transformer that translates the target, with optional parent-space constraints
    /// </summary>
    public class CustomTranslateTransformer : MonoBehaviour, ITransformer
    {
        [Serializable]
        public class OneGrabTranslateConstraints
        {
            public bool ConstraintsAreRelative;
            public FloatConstraint MinX;
            public FloatConstraint MaxX;
            public FloatConstraint MinY;
            public FloatConstraint MaxY;
            public FloatConstraint MinZ;
            public FloatConstraint MaxZ;
        }

        [SerializeField]
        private OneGrabTranslateConstraints _constraints =
            new OneGrabTranslateConstraints()
        {
            MinX = new FloatConstraint(),
            MaxX = new FloatConstraint(),
            MinY = new FloatConstraint(),
            MaxY = new FloatConstraint(),
            MinZ = new FloatConstraint(),
            MaxZ = new FloatConstraint()
        };

        public OneGrabTranslateConstraints Constraints
        {
            get
            {
                return _constraints;
            }

            set
            {
                _constraints = value;
                UpdateParentConstraints();
            }
        }

        private OneGrabTranslateConstraints _parentConstraints = null;
        private Vector3 _initialPosition;
        private Vector3 _grabOffsetInLocalSpace;

        private Int32 staticAxis;

        private Vector3 delta;
        private Vector3 deltaScaled;
        private IGrabbable _grabbable;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
            _initialPosition = _grabbable.Transform.localPosition;
            
            _parentConstraints = new OneGrabTranslateConstraints();

            _parentConstraints.MinX = new FloatConstraint();
            _parentConstraints.MinY = new FloatConstraint();
            _parentConstraints.MinZ = new FloatConstraint();
            _parentConstraints.MaxX = new FloatConstraint();
            _parentConstraints.MaxY = new FloatConstraint();
            _parentConstraints.MaxZ = new FloatConstraint();
        }

        private void UpdateParentConstraints()
        {

            _initialPosition = _grabbable.Transform.localPosition;

            if (staticAxis == 0){
                _parentConstraints.MinX.Constrain = true;
                _parentConstraints.MaxX.Constrain = true;
                _parentConstraints.MinX.Value = _initialPosition.x;
                _parentConstraints.MaxX.Value = _initialPosition.x;
            } else if (staticAxis == 1){
                _parentConstraints.MinY.Constrain = true;
                _parentConstraints.MaxY.Constrain = true;
                _parentConstraints.MinY.Value = _initialPosition.y;
                _parentConstraints.MaxY.Value = _initialPosition.y;
            } else {
                _parentConstraints.MinZ.Constrain = true;
                _parentConstraints.MaxZ.Constrain = true;
                _parentConstraints.MinZ.Value = _initialPosition.z;
                _parentConstraints.MaxZ.Value = _initialPosition.z;
            }
        }

        public void BeginTransform()
        {
            staticAxis = gameObject.GetComponent<InformationHolder>().getStaticAxis();
            UpdateParentConstraints();

            var grabPoint = _grabbable.GrabPoints[0];
            Transform targetTransform = _grabbable.Transform;
            _grabOffsetInLocalSpace = targetTransform.InverseTransformVector(
                    grabPoint.position - targetTransform.position);
                    
            delta = new Vector3(0.01f, 0.01f, 0.01f);
            deltaScaled = new Vector3(delta.x / targetTransform.lossyScale.x, delta.y / targetTransform.lossyScale.y, delta.z / targetTransform.lossyScale.z);
        }

        public void UpdateTransform()
        {
            var grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;
            var constrainedPosition = grabPoint.position -
                                      targetTransform.TransformVector(_grabOffsetInLocalSpace);

            if (grabPoint.Equals(new Pose(new Vector3(0.00f, 0.00f, 0.00f), new Quaternion(0.00f, 0.00f, 0.00f, 1.00f)))){
                return;
            }


            // the translation constraints occur in parent space
            if (targetTransform.parent != null)
            {
                constrainedPosition = targetTransform.parent.InverseTransformPoint(constrainedPosition);
            }

            if (_parentConstraints.MinX.Constrain)
            {
                constrainedPosition.x = Mathf.Max(constrainedPosition.x, _parentConstraints.MinX.Value);
            }
            if (_parentConstraints.MaxX.Constrain)
            {
                constrainedPosition.x = Mathf.Min(constrainedPosition.x, _parentConstraints.MaxX.Value);
            }
            if (_parentConstraints.MinY.Constrain)
            {
                constrainedPosition.y = Mathf.Max(constrainedPosition.y, _parentConstraints.MinY.Value);
            }
            if (_parentConstraints.MaxY.Constrain)
            {
                constrainedPosition.y = Mathf.Min(constrainedPosition.y, _parentConstraints.MaxY.Value);
            }
            if (_parentConstraints.MinZ.Constrain)
            {
                constrainedPosition.z = Mathf.Max(constrainedPosition.z, _parentConstraints.MinZ.Value);
            }
            if (_parentConstraints.MaxZ.Constrain)
            {
                constrainedPosition.z = Mathf.Min(constrainedPosition.z, _parentConstraints.MaxZ.Value);
            }

            // Convert the constrained position back to world space
            if (targetTransform.parent != null)
            {
                constrainedPosition = targetTransform.parent.TransformPoint(constrainedPosition);
            }

            Vector3 old_pos = targetTransform.position;

            targetTransform.position = constrainedPosition;
            
            // NOTE : there may be need to transform delta axis  
            // Collider[] colliders = Physics.OverlapBox(constrainedPosition, GetComponent<Collider>().bounds.size / 2f + deltaScaled, gameObject.transform.rotation);
            GameObject boundingBox = targetTransform.GetComponent<BoundingBoxHandler>().getBoundingBox();
            Collider[] colliders = Physics.OverlapBox(boundingBox.transform.position, boundingBox.transform.lossyScale / 2f + delta, boundingBox.transform.rotation);
            bool hasBase = false;
            foreach(Collider collider in colliders){
                if(collider.gameObject == gameObject){
                    // found itself
                } else if(collider.gameObject.transform.Equals(gameObject.transform.parent)){
                    // found parent
                    hasBase = true;
                } else if(collider.gameObject.transform.parent.Equals(gameObject.transform)){
                    //found child
                } else if(collider.gameObject.ToString().Equals("ControllerGrabLocation (UnityEngine.GameObject)")){
                    //found controller
                } else{
                    targetTransform.position = old_pos;
                    return;
                }
            }
            if (!hasBase){
                targetTransform.position = old_pos;
                return;
            }
        }

        public void EndTransform() { }

        #region Inject

        public void InjectOptionalConstraints(OneGrabTranslateConstraints constraints)
        {
            _constraints = constraints;
        }

        #endregion
    }
}
