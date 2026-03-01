using Godot;
using System;
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
    private float verticalLimiterTop = 90;
    [Export(PropertyHint.Range, "45, 90")]
    private float verticalLimiterBottom = 75;

    [ExportGroup("Movement")]
    [Export(PropertyHint.Range, "1, 20")]
    private float moveSpeed = 10;
    [Export(PropertyHint.Range, "10, 100")]
    private float acceleration = 100;
    [Export(PropertyHint.Range, "1, 50")]
    private float jumpStrength = 15;
    [Export(PropertyHint.Range, "1, 10")]
    private float maxJumps = 2;
    [Export(PropertyHint.Range, "10, 500")]
    private float jumpDelay = 100;
    [Export(PropertyHint.Range, "1, 50")]
    private float bulletJumpStrength = 20;
    [Export(PropertyHint.Range, "1, 100")]
    private float bulletJumpSpeed = 50;
    [Export(PropertyHint.Range, "0, 1")]
    private float bulletJumpVerticalSkew = 0.5f;
    [Export(PropertyHint.Range, "1, 10")]
    private float maxBulletJumps = 1;
    [Export(PropertyHint.Range, "10, 1000")]
    private float bulletJumpDuration = 500;

    // camera variables
    private Node3D cameraPivot;
    private float cameraRotationX = 0;
    private float cameraRotationY = 0;
    // movement variables
    private int jumpCount = 0;
    private int bulletJumpCount = 0;
    private ulong lastJumpTime = 0;
    private ulong lastBulletJumpTime = 0;
    private float gravityScale = 0;

    // gets called once when the node is ready (all children have been created), initialize stuff here
    public override void _Ready()
    {
        cameraPivot = GetNode<Node3D>("%CameraPivot");
        gravityScale = GravityScale;

        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        //---------- Apply movement forces ----------
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
            ApplyCentralForce(moveDirection * acceleration);
        }

        // resets gravity if we're out of bullet jump
        if(Time.GetTicksMsec() - lastBulletJumpTime > bulletJumpDuration) GravityScale = gravityScale;

        base._PhysicsProcess(delta);
    }


    public override void _Input(InputEvent @event)
    {
        // capture mouse when game window is clicked, release with escape
        if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left) Input.MouseMode = Input.MouseModeEnum.Captured;
        if(@event.IsActionPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;


        base._Input(@event);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        // jump
        if (@event.IsActionPressed("jump") && jumpCount < maxJumps)
        {
            // bullet jump if ctrl is pressed and we're allowed
            if (Input.IsActionPressed("crouch_slide") && bulletJumpCount < maxBulletJumps && Time.GetTicksMsec() - lastBulletJumpTime > bulletJumpDuration)
            {
                // gets the direction we are trying to bullet jump
                Vector3 bulletJumpVector = ((-1 * cameraPivot.GlobalBasis.Z) + (Vector3.Up * bulletJumpVerticalSkew)).Normalized();

                // bullet jumping upwards while falling cancels vertical velocity
                Vector3 velocity = LinearVelocity;
                if(velocity.Y < 0 && bulletJumpVector.Y > 0) velocity.Y = 0;
                LinearVelocity = velocity;

                // gets our velocity in the direction we are trying to jump
                var velocityInJumpDirection = LinearVelocity.Dot(bulletJumpVector); // explanation in movement script (_PhysicsProcess)
                // the faster we are the weaker the push is (to prevent infinite speed gain)
                float pushMultiplier = Math.Clamp((bulletJumpSpeed - velocityInJumpDirection) / bulletJumpSpeed, 0, 1);

                // executes bullet jump
                gravityScale = GravityScale;
                GravityScale = 0;
                ApplyCentralImpulse(bulletJumpVector * bulletJumpStrength * pushMultiplier);
                ulong time = Time.GetTicksMsec();
                lastJumpTime = time;
                lastBulletJumpTime = time;
                jumpCount++;
                bulletJumpCount++;
            }
            else if(Time.GetTicksMsec() - lastJumpTime > jumpDelay) // else normal jump if we're allowed
            {
                // jumping while falling cancels vertical velocity
                Vector3 velocity = LinearVelocity;
                if(velocity.Y < 0) velocity.Y = 0;
                LinearVelocity = velocity;

                ApplyCentralImpulse(Vector3.Up * jumpStrength);
                lastJumpTime = Time.GetTicksMsec();
                jumpCount++;
            }
        }

        // roll

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

        base._UnhandledInput(@event);
    }


    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        // resets jumps if colliding with something and enough time has passed since the last jump
        if(state.GetContactCount() > 0 && Time.GetTicksMsec() - lastJumpTime > jumpDelay) jumpCount = 0; bulletJumpCount = 0;

        base._IntegrateForces(state);
    }
}