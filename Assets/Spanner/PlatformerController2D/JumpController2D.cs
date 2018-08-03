using UnityEngine;
using System;

namespace Spanner {
    [RequireComponent(typeof(PlatformerController2D))]
    public class JumpController2D : MonoBehaviour {

        #region Settings
        [Serializable]
        public class Settings {
            public float speed;
            public float jumpHeight;
            public float timeToJumpApex;
            public float accelerationTimeGround;
            public float accelerationTimeAirborne;
            public float jumpFallGravityMultiplier;
            public float shortHopGravityMultiplier;
        }
        public Settings settings;
        #endregion

        #region Input
        private Vector2 _inputVector;
        public Vector2 InputVector {
            get { return _inputVector; }
            set { _inputVector = value; }
        }
        #endregion

        #region Motion
        private Vector2 _velocity;
        public Vector2 Velocity {
            get { return _velocity; }
            set { _velocity = value; }
        }
        private float _velocityXSmoothing;
        #endregion

        #region Jumping
        private float _defaultGravity;
        private float _currentGravity;
        public float CurrentGravity {
            get { return _currentGravity; }
            set { _currentGravity = value; }
        }
        private bool _jumpButtonHeld;
        public bool JumpButtonHeld {
            get { return _jumpButtonHeld; }
            set { _jumpButtonHeld = value; }
        }
        private bool _isJumping;
        public bool IsJumping {
            get { return _isJumping; }
        }
        private float _jumpVelocity;
        private bool _jumpInput;
        #endregion

        #region Cardinality
        private int _facingDirection;
        public int FacingDirection {
            get { return _facingDirection; }
            set { _facingDirection = value; }
        }
        #endregion

        #region Other Controllers
        private PlatformerController2D _controller;
        #endregion

        private void Start() {
            _facingDirection = 1;
            _defaultGravity = ComputeDefaultGravity();
            _jumpVelocity = ComputeJumpVelocity();
            _controller = GetComponent<PlatformerController2D>();
        }

        private float ComputeDefaultGravity() {
            return -(2f * settings.jumpHeight) / Mathf.Pow(settings.timeToJumpApex, 2f);
        }

        private float ComputeJumpVelocity() {
            return Mathf.Abs(_defaultGravity) * settings.timeToJumpApex;
        }

        private void Update() {
            UpdateFacingDirection();
            UpdateJump();
            UpdateVelocity();
            ApplyMotion();
        }

        private void UpdateFacingDirection() {
            if (_inputVector.x != 0) {
                _facingDirection = (int) Mathf.Sign(_inputVector.x);
            }
        }

        private void UpdateJump() {
            HandleJumpCollision();
            HandleJumpGravity();
            if (_jumpInput) {
                HandleJumpInput();
            }
        }

        public void Jump() {
            _jumpInput = true;
        }

        private void HandleJumpCollision() {
            if (_isJumping && _controller.Collisions.grounded) {
                _isJumping = false;
            }

            if (!_isJumping && (_controller.Collisions.grounded || _controller.Collisions.topBlocked)) {
                _velocity = new Vector2(_velocity.x, 0f);
            }

            if (_isJumping && _controller.Collisions.topBlocked) {
                _velocity = new Vector2(_velocity.x, 0f);
            }
        }

        private void HandleJumpGravity() {
            _currentGravity = _defaultGravity;
            if (_isJumping && _velocity.y < 0f) {
                _currentGravity *= settings.jumpFallGravityMultiplier;
            } else if (_isJumping && _velocity.y > 0f && !_jumpButtonHeld) {
                _currentGravity *= settings.shortHopGravityMultiplier;
            }
        }

        private void HandleJumpInput() {
            _jumpInput = false;
            if (_controller.Collisions.grounded) {
                _isJumping = true;
                _velocity = new Vector2(_velocity.x, _jumpVelocity);
            }
        }

        private void UpdateVelocity() {
            float targetX = _inputVector.x * settings.speed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetX, ref _velocityXSmoothing, GetAccelerationTime());
            _velocity.y += _currentGravity * Time.deltaTime;
        }

        private float GetAccelerationTime() {
            return _controller.Collisions.grounded ? settings.accelerationTimeGround : settings.accelerationTimeAirborne;
        }

        private void ApplyMotion() {
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}
