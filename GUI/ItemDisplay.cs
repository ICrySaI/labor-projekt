using Godot;
using System;

public partial class ItemDisplay : Control
{
    public IInventoryItem Item {
        get {
            return _item;
        }
        set {
            icon.Texture = value.ItemIcon;
            name.Text = value.ItemName;
            description.Text = value.ItemDescription;
            _item = value;
    } }
    private IInventoryItem _item;

    private TextureRect icon;
    private Label name;
    private Label description;

    public override void _Ready()
    {
        icon = GetNode<TextureRect>("%Icon");
        name = GetNode<Label>("%Name");
        description = GetNode<Label>("%Description");

        base._Ready();
    }
}
