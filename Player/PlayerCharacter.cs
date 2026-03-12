using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

public partial class PlayerCharacter : RigidBody3D
{
    // export variables
    [ExportGroup("Camera")]
    [Export(PropertyHint.Range, "1, 10")]
    private float mouseSensitivityX = 5;
    [Export(PropertyHint.Range, "1, 10")]
    private float mouseSensitivityY = 5;
    [Export(PropertyHint.Range, "45, 90")]
    private float verticalLimiterTop = 85;
    [Export(PropertyHint.Range, "45, 90")]
    private float verticalLimiterBottom = 75;

    [ExportGroup("Movement")]
    [Export(PropertyHint.Range, "1, 20")]
    private float moveSpeed = 10;
    [Export(PropertyHint.Range, "10, 100")]
    private float acceleration = 100;
    [Export(PropertyHint.Range, "10, 100")]
    private float airAcceleration = 20;
    [Export(PropertyHint.Range, "1, 50")]
    private float jumpStrength = 15;
    [Export(PropertyHint.Range, "1, 10")]
    private float maxJumps = 2;
    [Export(PropertyHint.Range, "10, 500")]
    private float jumpDelayMS = 100;
    [Export(PropertyHint.Range, "1, 50")]
    private float bulletJumpStrength = 30;
    [Export(PropertyHint.Range, "0, 1")]
    private float bulletJumpVerticalSkew = 0.2f;
    [Export(PropertyHint.Range, "1, 10")]
    private float maxBulletJumps = 1;
    [Export(PropertyHint.Range, "10, 1000")]
    private float bulletJumpDurationMS = 300;
    [Export(PropertyHint.Range, "1, 50")]
    private float slideBoostStrength = 10;
    [Export(PropertyHint.Range, "10, 1000")]
    private float slideBoostDelayMS = 500;
    [Export(PropertyHint.Range, "1000, 10000")]
    private float glideDurationMS = 2000;
    [Export(PropertyHint.Range, "1, 50")]
    private float dashStrength = 20;
    [Export(PropertyHint.Range, "100, 2000")]
    private float dashCooldownMS = 1000;

    // camera variables
    private Node3D cameraPivot;
    private float cameraRotationX = 0;
    private float cameraRotationY = 0;
    // movement variables
    private int jumpCount = 0;
    private int bulletJumpCount = 0;
    private ulong lastJumpTime = 0;
    private ulong lastBulletJumpTime = 0;
    private ulong lastSlideTime = 0;
    private float baseGravityScale = 1;
    private bool isSliding = false;
    private ulong glideStartTime = 0; // start time of current glide
    private ulong glideTimer = 0; // duration of current glide (time since start)
    private ulong glideTotalTime = 0; // total glide time since last touching the ground
    private bool isGliding = false;
    private bool canDash = true;

    // player variables
    private CollisionShape3D playerCollision;
    private MeshInstance3D playerMesh;
    private ShapeCast3D groundDetector;
    private bool onGround = false;
    private float friction = 0;
    private Dictionary<string, float> gravityMultipliers = new Dictionary<string, float>();

    // gets called once when the node is ready (all children have been created), initialize stuff here
    public override void _Ready()
    {
        cameraPivot = GetNode<Node3D>("%CameraPivot");
        playerCollision = GetNode<CollisionShape3D>("%PlayerCollision");
        playerMesh = GetNode<MeshInstance3D>("%PlayerMesh");
        groundDetector = GetNode<ShapeCast3D>("%GroundDetector");
        baseGravityScale = GravityScale;
        friction = PhysicsMaterialOverride.Friction;

        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        ulong currentTime = Time.GetTicksMsec();

        //---------- Apply movement forces ----------//

        Vector2 rawInput = Input.GetVector("move_left", "move_right", "move_forward", "move_backwards");
        Vector3 forward = cameraPivot.GlobalBasis.Z;
        Vector3 right = cameraPivot.GlobalBasis.X;

        Vector3 moveDirection = forward * rawInput.Y + right * rawInput.X;
        moveDirection.Y = 0; // makes sure that movement forces are always applied horizontally even if the camera is looking down
        moveDirection = moveDirection.Normalized();

        // calculate character's speed in the direction we are trying to move
        var velocityInMoveDirection = LinearVelocity.Dot(moveDirection); // dot is |a| * |b| * cos(angle between a and b), since moveDirection is normalized |b| is 1. |a| * cos(angle between a and b) is the magnitude of a's component that aligns with b (our velocity in the direction we are trying to move)
        // if we're below the max speed apply acceleration force
        if(velocityInMoveDirection < moveSpeed)
        {
            // accelerate if on ground and not sliding
            if(onGround && !isSliding) ApplyCentralForce(moveDirection * acceleration);
            // use air acceleration if not on ground
            if (!onGround) ApplyCentralForce(moveDirection * airAcceleration);
        }

        //---------- Checks for things that need to be checked every tick ----------//

        // checks if we're on the ground
        if(!onGround && groundDetector.IsColliding()) LandedOnGround();
        else if(onGround && !groundDetector.IsColliding()) LeftGround();

        //---------- Update things that need to be updated every tick ----------//

        // resets jumps
        if(onGround && currentTime - lastJumpTime > jumpDelayMS)
        {
            jumpCount = 0;
            bulletJumpCount = 0;
        }

        // updates glide timer and ends glide if we're over the max duration
        if(isGliding)
        {
            glideTimer = currentTime - glideStartTime;
            if(glideTotalTime + glideTimer > glideDurationMS) EndGlide();
        }

        base._PhysicsProcess(delta);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        // jump
        if (@event.IsActionPressed("jump") && jumpCount < maxJumps)
        {
            // bullet jump if ctrl is pressed
            if (Input.IsActionPressed("crouch_slide"))
            {
                BulletJump();
            }
            else // else normal jump
            {
                Jump();
            }
        }

        // slide
        if (@event.IsActionPressed("crouch_slide"))
        {
            StartSlide();
        }
        if (@event.IsActionReleased("crouch_slide"))
        {
            EndSlide();
        }

        // dash
        if (@event.IsActionPressed("dash"))
        {
            Dash();
        }

        base._UnhandledKeyInput(@event);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // capture mouse motion and move camera
        if(@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            // modify rotations based on mouse movement
            cameraRotationX -= mouseMotion.ScreenRelative.X * mouseSensitivityX * 0.001f;
            cameraRotationY -= mouseMotion.ScreenRelative.Y * mouseSensitivityY * 0.001f;
            cameraRotationY = Math.Clamp(cameraRotationY, Single.DegreesToRadians(-verticalLimiterTop), Single.DegreesToRadians(verticalLimiterBottom)); // clamps rotation so you can't turn the camera upside down

            // reset camera rotation
            Transform3D tran = cameraPivot.Transform;
            tran.Basis = Basis.Identity;
            cameraPivot.Transform = tran;

            // apply new rotation
            cameraPivot.RotateObjectLocal(Vector3.Up, cameraRotationX); // order is important, rotate around up axis first
            cameraPivot.RotateObjectLocal(Vector3.Right, cameraRotationY);
        }

        // glide
        if (@event.IsActionPressed("glide"))
        {
            StartGlide();
        }
        if (@event.IsActionReleased("glide"))
        {
            EndGlide();
        }

        base._UnhandledInput(@event);
    }

    public void LandedOnGround()
    {
        onGround = true;
        // touching the ground ends gliding and resets it's duration
        EndGlide();
        glideTotalTime = 0;
    }
    public void LeftGround()
    {
        onGround = false;
    }

    public void UpdateGravityScale()
    {
        float finalMultiplier = 1;
        foreach (KeyValuePair<string, float> m in gravityMultipliers)
        {
            finalMultiplier *= m.Value;
        }
        GravityScale = baseGravityScale * finalMultiplier;
    }

    private void Jump()
    {
        // checks if we're allowed to jump
        if (Time.GetTicksMsec() - lastJumpTime > jumpDelayMS)
        {
            // jumping while falling cancels vertical velocity
            Vector3 velocity = LinearVelocity;
            if(velocity.Y < 0) velocity.Y = 0;
            LinearVelocity = velocity;

            EndGlide();
            EndBulletJump();
            ApplyCentralImpulse(Vector3.Up * jumpStrength);
            lastJumpTime = Time.GetTicksMsec();
            jumpCount++;
        }
        
    }
    private void BulletJump()
    {
        // checks if we're allowed to bullet jump
        if (bulletJumpCount < maxBulletJumps && Time.GetTicksMsec() - lastBulletJumpTime > bulletJumpDurationMS)
        {
            // gets the direction we are trying to bullet jump
            Vector3 bulletJumpVector = ((-1 * cameraPivot.GlobalBasis.Z) + (Vector3.Up * bulletJumpVerticalSkew)).Normalized();

            // bullet jumping cancels momentum
            LinearVelocity = Vector3.Zero;

            // executes bullet jump
            EndSlide();
            EndGlide();
            gravityMultipliers.Add("bulletjump", 0);
            UpdateGravityScale();
            ApplyCentralImpulse(bulletJumpVector * bulletJumpStrength);

            ulong time = Time.GetTicksMsec();
            lastJumpTime = time;
            lastBulletJumpTime = time;
            jumpCount++;
            bulletJumpCount++;

            // starts a timer to remove the bullet jump gravity multiplier after the bullet jump duration elapses
            SceneTreeTimer t = GetTree().CreateTimer(bulletJumpDurationMS / 1000f, false, true, false);
            Callable c = Callable.From(EndBulletJump);
            t.Connect("timeout", c);

        }
    }
    private void EndBulletJump()
    {
        gravityMultipliers.Remove("bulletjump");
        UpdateGravityScale();
    }
    
    private void StartSlide()
    {
        // return if already sliding
        if(isSliding) return;
        
        // start sliding
        playerCollision.Shape = new SphereShape3D();
        playerCollision.Translate(Vector3.Down * 0.5f);
        playerMesh.Mesh = new SphereMesh();
        playerMesh.Translate(Vector3.Down * 0.5f);

        EndBulletJump();
        isSliding = true;
        friction = PhysicsMaterialOverride.Friction;
        PhysicsMaterialOverride.Friction = 0.3f;

        // apply a boost if we're starting a slide on the ground and enough time has passed since the last one
        if(onGround && Time.GetTicksMsec() - lastSlideTime > slideBoostDelayMS)
        {
            // get the direction to apply boost
            Vector3 slideBoostVector = -1 * cameraPivot.GlobalBasis.Z;
            slideBoostVector.Y = 0;
            slideBoostVector = slideBoostVector.Normalized();
            // apply boost
            ApplyCentralImpulse(slideBoostVector * slideBoostStrength);
            lastSlideTime = Time.GetTicksMsec();
        }
    }
    private void EndSlide()
    {
        // return if not sliding
        if(isSliding == false) return;

        // end sliding
        playerCollision.Shape = new CapsuleShape3D();
        playerCollision.Translate(Vector3.Up * 0.5f);
        playerMesh.Mesh = new CapsuleMesh();
        playerMesh.Translate(Vector3.Up * 0.5f);
        isSliding = false;
        PhysicsMaterialOverride.Friction = friction;
    }

    private void StartGlide()
    {
        if(isGliding) return; // if we're gliding no need to start
        if(glideTotalTime > glideDurationMS) return; // can't glide if we're out of duration
        if (onGround) return; // can only glide in the air


        Vector3 velocity = LinearVelocity;
        if (glideTotalTime == 0) velocity.Y = 0; // if this is the first time we're starting the glide it cancels vertical movement
        else velocity.Y = Math.Min(velocity.Y, 0); // else starting a glide cancels upward movement
        LinearVelocity = velocity;

        glideStartTime = Time.GetTicksMsec();
        gravityMultipliers.Add("glide", 0.1f);
        UpdateGravityScale();
        EndBulletJump();
        isGliding = true;
    }
    private void EndGlide()
    {
        if(!isGliding) return; // if we're not gliding no need to end
        gravityMultipliers.Remove("glide");
        UpdateGravityScale();
        glideTotalTime += glideTimer;
        isGliding = false;
    }

    private void Dash()
    {
        if(!canDash) return; // returns if we can't dash

        // get the direction we are trying to dash in (similar to movement in _PhysicsProcess)
        Vector2 rawInput = Input.GetVector("move_left", "move_right", "move_forward", "move_backwards");
        Vector3 forward = cameraPivot.GlobalBasis.Z;
        Vector3 right = cameraPivot.GlobalBasis.X;
        Vector3 dashDirection;
        // if no direction is pressed, dash forward
        if(rawInput.IsZeroApprox()) dashDirection = -1 * forward;
        else dashDirection = forward * rawInput.Y + right * rawInput.X;
        // made direction horizontal and normalized
        dashDirection.Y = 0;
        dashDirection = dashDirection.Normalized();

        // dashing dampens momentum
        Vector3 velocity = LinearVelocity;
        velocity = velocity / 2;
        LinearVelocity = velocity;

        // applies impulse in dash direction
        ApplyCentralImpulse(dashDirection * dashStrength);

        // starts dash cooldown
        canDash = false;
        SceneTreeTimer t = GetTree().CreateTimer(dashCooldownMS / 1000f, false, true, false);
        Callable c = Callable.From(ResetDash);
        t.Connect("timeout", c);
    }
    private void ResetDash()
    {
        canDash = true;
    }
}