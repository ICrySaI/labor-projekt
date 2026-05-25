using Godot;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class SelectableItem : PanelContainer
{
    private Panel selectionMarker;
    private ItemDisplay itemDisplay;
    public IInventoryItem Item {
        get {
            return itemDisplay.Item;
        }
        set {
            itemDisplay.Item = value;
    } }

    [Signal]
    public delegate void SelectableItemPressedEventHandler(SelectableItem item);

    public override void _Ready()
    {
        selectionMarker = GetNode<Panel>("HBoxContainer/SelectionMarker");
        itemDisplay = GetNode<ItemDisplay>("HBoxContainer/ItemDisplay");
        base._Ready();
    }

    public void select()
    {
        selectionMarker.Visible = true;
    }

    public void unselect()
    {
        selectionMarker.Visible = false;
    }

    public override void _GuiInput(InputEvent @event)
    { 
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Left)
            {
                EmitSignalSelectableItemPressed(this);
            }
        }
    }

}
