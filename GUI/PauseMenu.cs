using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export(PropertyHint.NodeType, "Node3D")]
    private Inventory inventory;

    [Export(PropertyHint.NodeType, "Control")]
    private AbilitySelector abilitySelector;

    [Export(PropertyHint.NodeType, "Control")]
    private Control gameOverScreen;

    [Signal]
    public delegate void QuitButtonPressedEventHandler();

    private InventoryDisplay inventoryDisplay;

    public void pause()
    {
        // pauses the scene tree
        GetTree().Paused = true;
        inventoryDisplay.refresh();
        Visible = true;
        Globals.ReleaseMouse();
    }

    public void resume()
    {
        // unpauses the scene tree
        GetTree().Paused = false;
        Visible = false;
        Globals.CaptureMouse();
    }

    public override void _Ready()
    {
        inventoryDisplay = GetNode<InventoryDisplay>("%InventoryDisplay");
        inventoryDisplay.inventory = inventory;
        inventoryDisplay.refresh();

        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            // cannot pause while selecting ability
            if (abilitySelector.Visible || gameOverScreen.Visible) return;

            if (GetTree().Paused) resume();
            else pause();
        }
    }

    public void OnResumePressed()
    {
        resume();
    }

    public void OnQuitPressed()
    {
        EmitSignalQuitButtonPressed();
    }
}
