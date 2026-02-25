using Godot;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

public partial class PlayerCharacter : RigidBody3D
{
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
    private float acceleration = 50;
    [Export(PropertyHint.Range, "1, 10")]
    private float jumpStrength = 5;
    [Export(PropertyHint.Range, "1, 10")]
    private float maxJumps = 2;
    [Export(PropertyHint.Range, "1, 10")]
    private float bulletJumpStrength = 5;
    [Export(PropertyHint.Range, "0, 1")]
    private float bulletJumpVerticalBoost = 0.5f;
    [Export(PropertyHint.Range, "1, 10")]
    private float maxBulletJumps = 1;
    [Export(PropertyHint.Range, "10, 500")]
    private float jumpDelay = 100;

    private Node3D cameraPivot;
    private float cameraRotationX = 0;
    private float cameraRotationY = 0;
    private int jumpCount = 0;
    private int bulletJumpCount = 0;
    private ulong lastJumpTime = 0;

    // gets called once when the node is ready (all children have been created), initialize stuff here
    public override void _Ready()
    {
        cameraPivot = GetNode<Node3D>("%CameraPivot");

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

        // print velocity for debug
        if(Time.GetTicksMsec() % 500 < 10) Debug.Print(LinearVelocity.ToString());

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
        // checks if we're allowed to jump
        if (@event.IsActionPressed("jump") && jumpCount < maxJumps && Time.GetTicksMsec() - lastJumpTime > jumpDelay)
        {
            // if we're falling jumping cancels the downwards velocity
            Vector3 velocity = LinearVelocity;
            if(velocity.Y < 0) velocity.Y = 0;
            LinearVelocity = velocity;

            // bullet jump if ctrl is pressed and we have enough left
            if (Input.IsActionPressed("crouch_slide") && bulletJumpCount < maxBulletJumps)
            {
                Vector3 bulletJumpVector = (-1 * cameraPivot.GlobalBasis.Z) + (Vector3.Up * bulletJumpVerticalBoost);
                ApplyCentralImpulse(bulletJumpVector * 10 * bulletJumpStrength);
                lastJumpTime = Time.GetTicksMsec();
                jumpCount++;
                bulletJumpCount++;
            }
            else // else normal jump
            {
                ApplyCentralImpulse(Vector3.Up * 10 * jumpStrength);
                lastJumpTime = Time.GetTicksMsec();
                jumpCount++;
            }
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

        base._UnhandledInput(@event);
    }


    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        // resets jumps if colliding with something and enough time has passed since the last jump
        if(state.GetContactCount() > 0 && Time.GetTicksMsec() - lastJumpTime > jumpDelay) jumpCount = 0; bulletJumpCount = 0;

        base._IntegrateForces(state);
    }
}