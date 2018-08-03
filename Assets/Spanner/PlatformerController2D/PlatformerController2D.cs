using System;
using UnityEngine;

namespace Spanner {
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformerController2D : MonoBehaviour {
        #region Settings
        [Serializable]
        public class Settings {
            public LayerMask collisionMask;
            public int horizontalRayCount;
            public int verticalRayCount;
            public float maxClimbAngle;
            public float maxDescendAngle;
        }
        public Settings settings;
        #endregion

        #region Constants
        public const float SKIN_WIDTH = 0.015f;
        #endregion

        #region Collision Modifiers
        private bool _forceCheckHorizontalCollisions;
        public bool ForceCheckHorizontalCollisions {
            set { _forceCheckHorizontalCollisions = value; }
        }
        private bool _forceCheckVerticalCollisions;
        public bool ForceCheckVerticalCollisions {
            set { _forceCheckVerticalCollisions = value; }
        }
        #endregion

        #region Raycast Properties
        public struct RaycastOrigins {
            public Vector2 topLeft;
            public Vector2 topRight;
            public Vector2 bottomLeft;
            public Vector2 bottomRight;
        }

        public struct RaycastSpacing {
            public float horizontalRaySpacing;
            public float verticalRaySpacing;
        }

        private RaycastOrigins _raycastOrigins;
        private RaycastSpacing _raycastSpacing;
        #endregion

        #region Raycast Calculations
        public void UpdateRaycastOrigins() {
            Bounds bounds = _collider.bounds;
            bounds.Expand(SKIN_WIDTH * -2f);

            _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        public void CalculateRaycastSpacing() {
            Bounds bounds = _collider.bounds;
            bounds.Expand(SKIN_WIDTH * -2f);
            float hSpacing = bounds.size.y / (settings.horizontalRayCount - 1);
            float vSpacing = bounds.size.x / (settings.verticalRayCount - 1);

            _raycastSpacing.horizontalRaySpacing = hSpacing;
            _raycastSpacing.verticalRaySpacing = vSpacing;
        }
        #endregion

        #region Collision Information
        public struct CollisionInfo {
            public bool topBlocked;
            public bool leftBlocked;
            public bool rightBlocked;
            public bool grounded;
            public bool climbingSlope;
            public bool descendingSlope;

            public float slopeAngle, slopeAngleOld;

            public Vector2 velocityOld;

            public void Reset() {
                topBlocked = leftBlocked = rightBlocked = grounded = false;
                climbingSlope = descendingSlope = false;
                slopeAngleOld = slopeAngle;
                slopeAngle = 0f;
            }
        }

        private CollisionInfo _collisionInfo;
        public CollisionInfo Collisions {
            get { return _collisionInfo; }
        }
        private BoxCollider2D _collider;
        #endregion

        public void Start() {
            _collider = GetComponent<BoxCollider2D>();
            _collisionInfo = new CollisionInfo();
            CalculateRaycastSpacing();
        }

        public void Move(Vector2 v) {
            UpdateRaycastOrigins();
            _collisionInfo.Reset();
            _collisionInfo.velocityOld = v;

            if (v.y < 0f) {
                DescendSlope(ref v);
            }

            if (v.x != 0f || _forceCheckHorizontalCollisions) {
                HandleHorizontalCollisions(ref v);
            }

            if (v.y != 0f || _forceCheckVerticalCollisions) {
                HandleVerticalCollisions(ref v);
            }

            transform.Translate(v);
        }

        private void HandleHorizontalCollisions(ref Vector2 velocity) {
            float direction = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH;

            for (int i = 0; i < settings.horizontalRayCount; i++) {
                if (_forceCheckHorizontalCollisions) {
                    // Start at the bottom of the collider
                    Vector2 rayOriginLeft = _raycastOrigins.bottomLeft;
                    Vector2 rayOriginRight = _raycastOrigins.bottomRight;
                    // Move up by the spacing amount depending on which ray we're casting
                    rayOriginLeft += Vector2.up * (_raycastSpacing.horizontalRaySpacing * i);
                    rayOriginRight += Vector2.up * (_raycastSpacing.horizontalRaySpacing * i);
                    // Calculate the results of the raycasts and handle the results
                    RaycastHit2D hitLeft = Physics2D.Raycast(rayOriginLeft, Vector2.left, 1, settings.collisionMask);
                    RaycastHit2D hitRight = Physics2D.Raycast(rayOriginRight, Vector2.right, 1, settings.collisionMask);
                    if (hitRight) {
                        HandleRayHit(hitRight, ref velocity, i, 1, rayOriginRight, 1);
                    }
                    if (hitLeft) {
                        HandleRayHit(hitLeft, ref velocity, i, -1, rayOriginLeft, 1);
                    }
                } else {
                    // If we're not forcing the collision checking, just check the direction the object is moving in
                    Vector2 rayOrigin = direction == -1 ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                    // Move up by the spacing amount depending on which ray we're casting
                    rayOrigin += Vector2.up * (_raycastSpacing.horizontalRaySpacing * i);
                    // Calculate the result of the raycast and handle the result
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, rayLength, settings.collisionMask);
                    if (hit) {
                        HandleRayHit(hit, ref velocity, i, direction, rayOrigin, rayLength);
                    }
                }
            }

            // Handle the climbing of slopes
            if (_collisionInfo.climbingSlope) {
                float directionX = Mathf.Sign(velocity.x);
                rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH;
                // The ray should be cast based on the object's y velocity
                Vector2 rayOrigin = (directionX < 0 ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) +
                    Vector2.up * velocity.y;
                // Calculate the result of the raycast and handle the result
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, settings.collisionMask);
                if (hit) {
                    // Compute the angle of the slope the object is ascending
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != _collisionInfo.slopeAngle) {
                        // If the angle doesn't match the stored slope angle, then update it and
                        // recalculate the velocity
                        velocity.x = hit.distance - SKIN_WIDTH * directionX;
                        _collisionInfo.slopeAngle = slopeAngle;
                    }
                }
            }
        }

        private void HandleRayHit(RaycastHit2D hit, ref Vector2 velocity, int i, float direction, Vector2 rayOrigin, float rayLength) {
            // Handle sloped surfaces
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (i == 0 && slopeAngle <= settings.maxClimbAngle) {
                if (_collisionInfo.descendingSlope) {
                    _collisionInfo.descendingSlope = false;
                    velocity = _collisionInfo.velocityOld;
                }

                float distanceToSlopeStart = 0f;
                if (slopeAngle != _collisionInfo.slopeAngleOld) {
                    distanceToSlopeStart = hit.distance - SKIN_WIDTH;
                    velocity.x -= distanceToSlopeStart * direction;
                }
                ClimbSlope(ref velocity, slopeAngle);
                velocity.x += distanceToSlopeStart * direction;
            }

            if (!_collisionInfo.climbingSlope || slopeAngle > settings.maxClimbAngle) {
                Debug.DrawRay(rayOrigin, Vector2.right * direction * rayLength, Color.red);
                velocity.x = (hit.distance - SKIN_WIDTH) * direction;
                rayLength = hit.distance;

                if (_collisionInfo.climbingSlope) {
                    velocity.y = Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                }

                _collisionInfo.leftBlocked = direction < 0f;
                _collisionInfo.rightBlocked = direction > 0f;
            }
        }

        private void HandleVerticalCollisions(ref Vector2 velocity) {
            float direction = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + SKIN_WIDTH;

            for (int i = 0; i < settings.verticalRayCount; i++) {
                Vector2 rayOrigin = direction == -1 ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (_raycastSpacing.verticalRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * direction, rayLength, settings.collisionMask);

                if (hit) {
                    Debug.DrawRay(rayOrigin, Vector2.up * direction * rayLength, Color.red);
                    velocity.y = (hit.distance - SKIN_WIDTH) * direction;
                    rayLength = hit.distance;

                    if (_collisionInfo.climbingSlope) {
                        velocity.x = velocity.y / Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }

                    _collisionInfo.topBlocked = direction > 0f;
                    _collisionInfo.grounded = direction < 0f;
                }
            }
        }

        private void ClimbSlope(ref Vector2 velocity, float slopeAngle) {
            float moveDistance = Mathf.Abs(velocity.x);
            float targetY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            if (velocity.y <= targetY) {
                velocity.y = targetY;
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                _collisionInfo.grounded = true;
                _collisionInfo.climbingSlope = true;
                _collisionInfo.slopeAngle = slopeAngle;
            }
        }

        private void DescendSlope(ref Vector2 velocity) {
            float direction = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = direction < 0f ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, settings.collisionMask);
            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0f && slopeAngle <= settings.maxDescendAngle) {
                    if (Mathf.Sign(hit.normal.x) == direction) {
                        if (hit.distance - SKIN_WIDTH <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
                            float distance = Mathf.Abs(velocity.x);
                            float targetY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * distance;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * distance * Mathf.Sign(velocity.x);
                            velocity.y -= targetY;

                            _collisionInfo.slopeAngle = slopeAngle;
                            _collisionInfo.descendingSlope = true;
                            _collisionInfo.grounded = true;
                        }
                    }
                }
            }
        }
    }
}
