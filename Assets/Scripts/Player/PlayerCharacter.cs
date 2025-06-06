using KinematicCharacterController;
using UnityEngine;

public enum CrouchInput
{
    None,
    Toggle
}

public enum Stance
{
    Stand,
    Crouch,
    Slide
}

public struct CharacterState
{
    public bool Grounded;
    public Stance Stance;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool JumpSustain;
    public CrouchInput Crouch;
}
public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform cameraTarget;
    [Space]
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float crouchSpeed = 7f;
    [SerializeField] private float walkResponse = 25f;
    [SerializeField] private float crouchResponse = 20f;
    [Space]
    [SerializeField] private float airSpeed = 10f;
    [SerializeField] private float airAcceleration = 35f;
    [Space]
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private int maxJumps = 2;

    [SerializeField] private float coyoteTime = 0.15f;
    [Range(0f, 1f)]
    [SerializeField] private float jumpSustainGravity = 0.4f;
    [SerializeField] private float gravity = -10f;
    [Space]
    [SerializeField] private float slideStartSpeed = 54f;    // was 40f (40 * 1.35 = 54)
    [SerializeField] private float slideEndSpeed = 34f;      // was 25f (25 * 1.35 = 33.75)
    [SerializeField] private float slideFriction = 0.8f;     // (optional: lower for longer slides, e.g. try 0.7f)
    [SerializeField] private float slideSteerAcceleration = 6.75f; // was 5f (5 * 1.35 = 6.75)
    [SerializeField] private float slideGravity = -121.5f;   // was -90f (-90 * 1.35 = -121.5)
    [Space]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchHeightResponse = 15;
    [Range(0f, 1f)]
    [SerializeField] private float standCameraTargetHeight = 0.9f;
    [Range(0f, 1f)]
    [SerializeField] private float crouchCameraTargetHeight = 0.7f;
    [SerializeField] private float maxAirSpeed = 25f;
    [Space]
    [SerializeField] private float dashForce = 40f;
    [SerializeField] private float dashCooldownBetween = 0.3f; // Minimum time between dashes
    [SerializeField] private float dashRechargeTime = 2f;      // Time to recharge one dash (when grounded)
    [SerializeField] private float dashFov = 90f;
    [SerializeField] private float dashFovDuration = 0.15f;
    [SerializeField] private float normalFov = 70f;
    [SerializeField] private int maxDashCharges = 2;
    private int dashCharges;
    private float dashRechargeTimer = 0f;
    private float dashCooldownTimer = 0f;

    [SerializeField] private float grapplePullSpeed = 60f;
    [SerializeField] private float grappleLowGravity = -1.5f;
    [SerializeField] private float grapplePropelForce = 8f;
    [SerializeField] private float maxGrappleDistance = 40f;

    private CharacterState _state;
    private CharacterState _laststate;

    private CharacterState _tempstate;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedSustainedJump;
    private bool _requestedCrouch;
    private bool _requestedCrouchInAir;
    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;
    private Collider[] _uncrouchOverlapResults;
    private float grappleCooldownTimer = 0f;
    private bool isGrappling = false;
    private Vector3 grapplePoint;
    private Vector3 grappleStartPosition;
    private Vector3 grappleDirection;
    private bool grappleArrived = false;
    private float lastDashTime = -10f;
    private bool isDashing = false;
    private Vector3 dashVelocity;
    private float dashDuration = 0.15f;
    private float dashTimer = 0f;
    private float dashAirtimeTimer = 0f;
    private const float dashAirtimeDuration = 0.4f;
    private int jumpsRemaining;
    private float dashFovTimer = 0f;

    // Add this field to store the original position before grappling:
    private Vector3 grappleOriginPosition;

    // Add at the top with other fields:
    [SerializeField] private LineRenderer grappleLine;
    [SerializeField] private Transform firePoint;
    

    public void Initialize()
    {
        _state.Stance = Stance.Stand;
        _uncrouchOverlapResults = new Collider[8];
        motor.CharacterController = this;
        dashCharges = maxDashCharges;
        dashRechargeTimer = 0f;
        dashCooldownTimer = 0f;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.Rotation;
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
        _requestedMovement = input.Rotation * _requestedMovement;
        var wasRequestingJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if(_requestedJump && !wasRequestingJump)      
           _timeSinceJumpRequest = 0f;       
        _requestedSustainedJump = input.JumpSustain;
        var wasRequestingCrouch = _requestedCrouch;
        _requestedCrouch = input.Crouch switch
        {
            CrouchInput.Toggle => !_requestedCrouch,
            CrouchInput.None => _requestedCrouch,
            _ => _requestedCrouch
        };
        if(_requestedCrouch && !wasRequestingCrouch)
        {
            _requestedCrouchInAir = !_state.Grounded;
        }
        else if(!_requestedCrouch && wasRequestingCrouch)
        {
            _requestedCrouchInAir = false;
        }
    }

    public void UpdateBody(float deltaTime)
    {
        // --- Dash recharge now works regardless of grounded state ---
        if (dashCharges < maxDashCharges)
        {
            dashRechargeTimer += deltaTime;
            if (dashRechargeTimer >= dashRechargeTime)
            {
                dashCharges++;
                dashRechargeTimer = 0f;
            }
        }
        else
        {
            dashRechargeTimer = 0f;
        }

        // Dash cooldown between consecutive dashes
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= deltaTime;

        // Add this at the top of the method:
        if (grappleCooldownTimer > 0f)
            grappleCooldownTimer -= deltaTime;

        var currentHeight = motor.Capsule.height;
        var normalizedHeight = currentHeight / standHeight;

        var cameraTargetHeight = currentHeight * 
        (
        _state.Stance is Stance.Stand
        
            ? standCameraTargetHeight
            : crouchCameraTargetHeight
        );
        var rootTargetScale = new Vector3(1f, normalizedHeight, 1f);
        
        cameraTarget.localPosition = Vector3.Lerp
        (
            a:cameraTarget.localPosition,
            b:new Vector3(0f, cameraTargetHeight, 0f),
            t:1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
        );
        root.localScale = Vector3.Lerp
        (
            a:root.localScale,
            b:rootTargetScale,
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
        );
    }


    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane(_requestedRotation * Vector3.forward, motor.CharacterUp);

        if (forward != Vector3.zero)
        {
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
        }
        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        _state.Acceleration = Vector3.zero;

        // --- Add this block at the very top of the method ---
        if (isGrappling)
        {
            // Low gravity while grappling
            var toGrapple = grapplePoint - transform.position;
            float distance = toGrapple.magnitude;
            Vector3 moveDir = toGrapple.normalized;

            // Pull toward grapple point
            currentVelocity = moveDir * grapplePullSpeed;
            currentVelocity += motor.CharacterUp * grappleLowGravity * deltaTime;

            // Arrived at grapple point
            if (distance < 2f)
            {
                isGrappling = false;
                grappleArrived = true;
                if (grappleLine != null)
                    grappleLine.enabled = false;

                // Only propel if the grapple point is NOT above the player
                // Calculate the difference between the grapple point Y and the player's Y at the moment the grapple started
                float verticalDifference = grapplePoint.y - grappleOriginPosition.y;
                if (verticalDifference < 1.0f) // You can tweak this threshold
                {
                    // Propel toward the original position where grapple started
                    Vector3 propelDir = (grappleOriginPosition - transform.position).normalized;
                    currentVelocity = propelDir * (grapplePropelForce * 2.5f); // Increased multiplier for more noticeable propel
                }
            }
            return;
        }

        _state.Acceleration = Vector3.zero;
        if (motor.GroundingStatus.IsStableOnGround)
        {
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;
            jumpsRemaining = maxJumps; // <-- Reset jumps when grounded

            var groundedMovement = motor.GetDirectionTangentToSurface
                (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
                ) * _requestedMovement.magnitude;

                {
                var moving = groundedMovement.sqrMagnitude > 0f;
                var crouching = _state.Stance is Stance.Crouch;
                var wasStanding = _laststate.Stance is Stance.Stand;
                var wasInAir = !_laststate.Grounded;
                if(moving && crouching && wasStanding || wasInAir)
                {
                    _state.Stance = Stance.Slide;

                    if(wasInAir)
                    {

                        float airSpeedForSlide = _laststate.Velocity.magnitude;
                        currentVelocity = Vector3.ProjectOnPlane
                        (
                            vector: _laststate.Velocity,
                            planeNormal: motor.GroundingStatus.GroundNormal
                        );

                        // Give a boost if sliding right after jumping
                        var slideSpeed = Mathf.Max(slideStartSpeed * 1.5f, airSpeedForSlide * 1.2f);
                        currentVelocity = motor.GetDirectionTangentToSurface
                        (
                            direction: currentVelocity,
                            surfaceNormal: motor.GroundingStatus.GroundNormal
                        ).normalized * slideSpeed;
                    }
                    else
                    {
                        var slideSpeed = Mathf.Max(slideStartSpeed, currentVelocity.magnitude);
                        currentVelocity = motor.GetDirectionTangentToSurface
                        (
                            direction: currentVelocity,
                            surfaceNormal: motor.GroundingStatus.GroundNormal
                        ) * slideSpeed;
                    }
                }

                }
            
            if (_state.Stance is Stance.Stand or Stance.Crouch)
            
            {                
                var speed = _state.Stance is Stance.Stand     
                    ? walkSpeed
                    : crouchSpeed;
                var response = _state.Stance is Stance.Stand
                    ? walkResponse
                    : crouchResponse;

                var targetVelocity = groundedMovement * speed;
               var moveVelocity = Vector3.Lerp
                (
                    a: currentVelocity,
                    b: targetVelocity,
                    t: 1f - Mathf.Exp(-response * deltaTime)
                );
                _state.Acceleration = moveVelocity - currentVelocity;
                currentVelocity = moveVelocity;
            }

            // Cpntinuar no slide
            else
            {
                currentVelocity -= currentVelocity * (slideFriction * deltaTime);

                {
                    var force = Vector3.ProjectOnPlane
                    (
                        vector: -motor.CharacterUp,
                        planeNormal: motor.GroundingStatus.GroundNormal
                    ) * slideGravity;
                    currentVelocity -= force * deltaTime;
                }

                {
                    var currentSpeed = currentVelocity.magnitude;
                    var targetVelocity = groundedMovement * currentVelocity.magnitude;
                    var steerVelocity = currentVelocity;
                    var steerForce = (targetVelocity - currentVelocity) * slideSteerAcceleration * deltaTime;

                    steerVelocity += steerForce;
                    steerVelocity = Vector3.ClampMagnitude(currentVelocity, currentSpeed);

                    _state.Acceleration = (steerVelocity - currentVelocity) / deltaTime;
                    currentVelocity = steerVelocity;
                }

                if(currentVelocity.magnitude < slideEndSpeed)
                    _state.Stance = Stance.Crouch;
            }
        
        }
        else
        {
            _timeSinceUngrounded += deltaTime;

            if (_requestedMovement.sqrMagnitude > 0f)
            {
                
                var planarMovement = Vector3.ProjectOnPlane
                (
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp
                ) * _requestedMovement.magnitude;
                

                var currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );

                var movementForce = planarMovement * airAcceleration * deltaTime;

                if (currentPlanarVelocity.magnitude < airSpeed)
                {
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                }
                else if (Vector3.Dot(currentPlanarVelocity, movementForce) < 0f)
                {

                    var constrainedMovementForce = Vector3.ProjectOnPlane
                    (
                        vector: movementForce,
                        planeNormal: currentPlanarVelocity.normalized
                    );

                    movementForce = constrainedMovementForce;
                }

                if (motor.GroundingStatus.FoundAnyGround)
                {
                    if(Vector3.Dot(movementForce, currentVelocity + movementForce)> 0f)
                    {
                        var obstructionNormal = Vector3.Cross
                        (
                            motor.CharacterUp,
                            Vector3.Cross
                            (
                                motor.CharacterUp,
                                motor.GroundingStatus.GroundNormal
                            )
                        ).normalized;    

                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);                   
                    }
                }

                currentVelocity += movementForce;
            }

            if (isDashing)
            {
                // No gravity while dashing in air
            }
            else
            {
                var effectiveGravity = gravity;
                var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                if (_requestedSustainedJump && verticalSpeed > 0f) 
                {
                    effectiveGravity *= jumpSustainGravity;
                }
                currentVelocity += motor.CharacterUp * effectiveGravity * deltaTime;
            }

            // --- CAP AIR SPEED HERE ---
            var up = motor.CharacterUp;
            var planarVel = Vector3.ProjectOnPlane(currentVelocity, up);
            var verticalVel = Vector3.Project(currentVelocity, up);

            if (planarVel.magnitude > maxAirSpeed)
            {
                planarVel = planarVel.normalized * maxAirSpeed;
                currentVelocity = planarVel + verticalVel;
            }
            // --- END CAP ---
        }

        if (_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            if ((grounded || canCoyoteJump || jumpsRemaining > 0) && jumpsRemaining > 0)
            {
                _requestedJump = false;
                _requestedCrouch = false;
                _requestedCrouchInAir = false;

                motor.ForceUnground(time: 0f);
                _ungroundedDueToJump = true;

                float jumpVelocity = jumpSpeed;

                // If jumping from slide, scale jump height and distance with slide speed
                if (_state.Stance == Stance.Slide)
                {
                    // Use the velocity before any jump modification
                    float slideSpeed = currentVelocity.magnitude;

                    // Scale vertical jump height with slide speed
                    float jumpHeightMultiplier = Mathf.Lerp(1f, 2.2f, Mathf.InverseLerp(slideStartSpeed, slideStartSpeed * 2f, slideSpeed));
                    jumpVelocity *= jumpHeightMultiplier;

                    // Scale horizontal boost with slide speed
                    Vector3 slideDirection = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp).normalized;
                    float forwardBoost = Mathf.Lerp(0.5f, 1.5f, Mathf.InverseLerp(slideStartSpeed, slideStartSpeed * 2f, slideSpeed)) * slideSpeed;
                    currentVelocity += slideDirection * forwardBoost;
                }

                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpVelocity);

                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);

                jumpsRemaining--;
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;
                var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                _requestedJump = canJumpLater;
            }
            
        }

        if (isDashing)
        {

            currentVelocity += dashVelocity;
            dashTimer += deltaTime;
            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                dashFovTimer = 0f; // End FOV effect immediately when dash ends
            }

            dashVelocity = Vector3.zero;
            return;
        }

        if (dashAirtimeTimer > 0f)
        {
            dashAirtimeTimer -= deltaTime;
        }

        if (dashFovTimer > 0f)
            dashFovTimer -= Time.deltaTime;
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        _tempstate = _state;
        // Agachar
        if (_requestedCrouch && _state.Stance == Stance.Stand)
        {
            _state.Stance = Stance.Crouch;
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
            );
        }
            
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        // Levantar
        if (!_requestedCrouch && _state.Stance is not Stance.Stand)
        {
            _state.Stance = Stance.Stand;
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: standHeight,
                yOffset: standHeight * 0.5f
            );

            var pos = motor.TransientPosition;
            var rot = motor.TransientRotation;
            var mask = motor.CollidableLayers;
            if (motor.CharacterOverlap(pos, rot, _uncrouchOverlapResults, mask, QueryTriggerInteraction.Ignore) > 0)
            {
                _requestedCrouch = true;
                motor.SetCapsuleDimensions
                (
                    radius: motor.Capsule.radius,
                    height: crouchHeight,
                    yOffset: crouchHeight * 0.5f
                );
            }
            else
            {
                _state.Stance = Stance.Stand;
            }

        }
        _state.Grounded = motor.GroundingStatus.IsStableOnGround;
        _state.Velocity = motor.Velocity;
        _laststate = _tempstate;
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        if(!motor.GroundingStatus.IsStableOnGround && _state.Stance is Stance.Slide)
        {
            _state.Stance = Stance.Crouch;
        }
        
    }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atVelocity, Quaternion atRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public Transform GetCameraTarget()
    {
        return cameraTarget;
    }
    
    public CharacterState GetState() => _state;
    public CharacterState GetLastState() => _laststate;

    public void TryDash()
    {
        // Only allow dash if not already dashing, have charges, not sliding, and cooldown passed
        if (isDashing || dashCharges <= 0 || _state.Stance == Stance.Slide || dashCooldownTimer > 0f)
            return;

        Vector3 dashDir = _requestedMovement;
        if (dashDir.sqrMagnitude < 0.01f)
            dashDir = transform.forward;

        dashDir = Vector3.ProjectOnPlane(dashDir, motor.CharacterUp).normalized;
        dashVelocity = dashDir * dashForce;
        isDashing = true;
        dashTimer = 0f;
        lastDashTime = Time.time;
        dashFovTimer = dashFovDuration;

        dashCharges--;
        dashCooldownTimer = dashCooldownBetween; // Start cooldown before next dash
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("PlayerCharacter collided with: " + collision.gameObject.name + " | Tag: " + collision.gameObject.tag);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("PlayerCharacter trigger entered by: " + other.name + " | Tag: " + other.tag);
    }

    public float GetTargetFov()
    {
        return dashFovTimer > 0f ? dashFov : normalFov;
    }

    // --- Add this method to start a grapple ---
    public void TryStartGrapple(Vector3 origin, Vector3 direction)
    {
        if (isGrappling) return;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxGrappleDistance))
        {
            if (hit.collider.CompareTag("Grapple"))
            {
                grapplePoint = hit.point;
                grappleStartPosition = transform.position;
                grappleOriginPosition = transform.position; // Store where grapple started
                grappleDirection = (grapplePoint - transform.position).normalized;
                isGrappling = true;
                grappleArrived = false;

                // Enable and set up the grapple line
                if (grappleLine != null)
                {
                    grappleLine.enabled = true;
                    grappleLine.positionCount = 2;
                    grappleLine.SetPosition(0, firePoint != null ? firePoint.position : transform.position);
                    grappleLine.SetPosition(1, grapplePoint);
                }
            }
        }
    }

    // --- Add a method to cancel grapple if needed ---
    public void CancelGrapple()
    {
        isGrappling = false;
        grappleArrived = false;
        if (grappleLine != null)
            grappleLine.enabled = false;
    }

    // In Update() or LateUpdate(), update the line if grappling:
    void LateUpdate()
    {
        if (isGrappling && grappleLine != null)
        {
            grappleLine.SetPosition(0, firePoint != null ? firePoint.position : transform.position);
            grappleLine.SetPosition(1, grapplePoint);
        }
        else if (grappleLine != null)
        {
            grappleLine.enabled = false;
        }
    }

    public int GetDashCharges()
    {
        return dashCharges;
    }
}
