using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class PlayerCharacter : RigidBody3D
{
    [Export(PropertyHint.Range, "1, 10")]
    private float mouseSensitivityX = 5;
    [Export(PropertyHint.Range, "1, 10")]
    private float mouseSensitivityY = 5;

    private Node3D cameraPivot;
    private float cameraRotationX = 0;
    private float cameraRotationY = 0;

    // gets called once when the node is ready (all children have been created), initialize stuff here
    public override void _Ready()
    {
        cameraPivot = GetNode<Node3D>("%CameraPivot");

        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        // capture mouse when game window is clicked, release with escape
        if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left) Input.MouseMode = Input.MouseModeEnum.Captured;
        if(@event.IsActionPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;


        base._Input(@event);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // capture mouse motion and move camera
        if(@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            // modify rotations based on mouse movement
            cameraRotationX -= mouseMotion.ScreenRelative.X * mouseSensitivityX * 0.001f;
            cameraRotationY -= mouseMotion.ScreenRelative.Y * mouseSensitivityY * 0.001f;
            cameraRotationY = Math.Clamp(cameraRotationY, Single.DegreesToRadians(-75), Single.DegreesToRadians(85)); // clamps rotation so you can't turn the camera upside down

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
}