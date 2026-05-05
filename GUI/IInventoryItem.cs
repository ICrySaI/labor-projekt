using Godot;
using System;

public partial interface IInventoryItem
{
    public Texture2D ItemIcon { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
}
