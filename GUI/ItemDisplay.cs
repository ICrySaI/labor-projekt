using Godot;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class ItemDisplay : Control
{
    public IInventoryItem Item {
        get {
            return _item;
        }
        set {
            _item = value;
            icon.Texture = _item.ItemIcon;
            name.Text = _item.ItemName;
            description.Text = _item.ItemDescription;
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
