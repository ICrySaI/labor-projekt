using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export(PropertyHint.NodeType, "Node3D")]
    private Inventory inventory;

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
            if(GetTree().Paused) resume();
            else pause();
        }

        base._Input(@event);
    }

    public void OnResumePressed()
    {
        resume();
    }

    public void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
