/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;

namespace Oculus.Interaction
{
    public class GrabInteractable : PointerInteractable<GrabInteractor, GrabInteractable>,
                                      IRigidbodyRef, ICollidersRef
    {
        [SerializeField, Interface(typeof(IPointableElement))]
        private MonoBehaviour _pointableElement;

        private Collider[] _colliders;
        public Collider[] Colliders => _colliders;

        [SerializeField]
        Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        [SerializeField, Optional]
        private Transform _grabSource;

        [SerializeField]
        private bool _useClosestPointAsGrabSource;

        [SerializeField]
        private float _releaseDistance = 0f;

        [SerializeField]
        private bool _resetGrabOnGrabsUpdated = true;

        [SerializeField, Optional]
        private PhysicsGrabbable _physicsGrabbable = null;

        private static CollisionInteractionRegistry<GrabInteractor, GrabInteractable> _grabRegistry = null;

        #region Properties
        public bool UseClosestPointAsGrabSource
        {
            get
            {
                return _useClosestPointAsGrabSource;
            }
            set
            {
                _useClosestPointAsGrabSource = value;
            }
        }
        public float ReleaseDistance
        {
            get
            {
                return _releaseDistance;
            }
            set
            {
                _releaseDistance = value;
            }
        }

        public bool ResetGrabOnGrabsUpdated
        {
            get
            {
                return _resetGrabOnGrabsUpdated;
            }
            set
            {
                _resetGrabOnGrabsUpdated = value;
            }
        }
        #endregion


        // protected bool _initialized = false;
        // protected override void OnEnable()
        // {
        //     print("STH ENTERED" + _initialized);
        //     if (_initialized){
        //         base.OnEnable();
        //     }
        // }

        // protected override void OnDisable()
        // {
        //     if (_initialized){
        //         base.OnDisable();
        //     }
        // }

        // public void Initialize(){
        //     _initialized = true;
        //     print("TRUENOW");
        //     OnEnable();
        // }

        protected override void Awake()
        {
            base.Awake();
            if (_grabRegistry == null)
            {
                _grabRegistry = new CollisionInteractionRegistry<GrabInteractor, GrabInteractable>();
                SetRegistry(_grabRegistry);
            }
            PointableElement = _pointableElement as IPointableElement;
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            this.AssertField(Rigidbody, nameof(Rigidbody));
            _colliders = Rigidbody.GetComponentsInChildren<Collider>();
            this.AssertCollectionField(_colliders, nameof(_colliders),
               $"The associated {AssertUtils.Nicify(nameof(Rigidbody))} must have at least one Collider.");
            this.AssertField(PointableElement, nameof(PointableElement));
            this.EndStart(ref _started);
        }

        public Pose GetGrabSourceForTarget(Pose target)
        {
            if (_grabSource == null && !_useClosestPointAsGrabSource)
            {
                return target;
            }

            if (_useClosestPointAsGrabSource)
            {
                return new Pose(
                    Collisions.ClosestPointToColliders(target.position, _colliders),
                    target.rotation);
            }

            return _grabSource.GetPose();
        }

        public void ApplyVelocities(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            if (_physicsGrabbable == null)
            {
                return;
            }
            _physicsGrabbable.ApplyVelocities(linearVelocity, angularVelocity);
        }

        #region Inject

        public void InjectAllGrabInteractable(Rigidbody rigidbody, IPointableElement pointableElement)
        {
            InjectRigidbody(rigidbody);
            InjectPointableElement(pointableElement);
        }

        public void InjectRigidbody(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void InjectPointableElement(IPointableElement pointableElement)
        {
            PointableElement = pointableElement;
            _pointableElement = pointableElement as MonoBehaviour;
        }

        public void InjectOptionalGrabSource(Transform grabSource)
        {
            _grabSource = grabSource;
        }

        public void InjectOptionalReleaseDistance(float releaseDistance)
        {
            _releaseDistance = releaseDistance;
        }

        public void InjectOptionalPhysicsGrabbable(PhysicsGrabbable physicsGrabbable)
        {
            _physicsGrabbable = physicsGrabbable;
        }
        #endregion
    }
}
