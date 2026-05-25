using Godot;
public partial interface IInventoryItem
{
    public string ItemID { get; }
    public Texture2D ItemIcon { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
}
