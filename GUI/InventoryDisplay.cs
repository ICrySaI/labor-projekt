using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryDisplay : Control
{
    [Export(PropertyHint.NodeType, "Node3D")]
    public Inventory inventory;

    private VBoxContainer container;

    private PackedScene itemDisplayScene = GD.Load<PackedScene>("res://GUI/ItemDisplay.tscn");

    public override void _Ready()
    {
        container = GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");

        base._Ready();
    }

    public void refresh()
    {
        List<IInventoryItem> itemList = inventory.GetItems();
        // remove the previous displays
        foreach (Node n in container.GetChildren())
        {
            n.QueueFree();
        }
        // create new ones
        foreach (IInventoryItem item in itemList)
        {
            ItemDisplay newDisplay = itemDisplayScene.Instantiate<ItemDisplay>();
            container.AddChild(newDisplay);
            newDisplay.Item = item;
        }
    }
}
