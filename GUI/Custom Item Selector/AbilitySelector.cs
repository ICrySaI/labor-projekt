using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AbilitySelector : Control
{
    [Export(PropertyHint.NodeType, "Node3D")]
    private Inventory inventory;
    [Export]
    private int SelectionCount = 3;

    private VBoxContainer itemList;
    private Button confirmButton;
    private SelectableItem selected;

    private PackedScene selectableItemScene = GD.Load<PackedScene>("res://GUI/Custom Item Selector/SelectableItem.tscn");

    public void showSelector()
    {
        GetTree().Paused = true;
        Globals.ReleaseMouse();
        // empty the selection
        foreach (Node n in itemList.GetChildren()) n.Free();
        selected = null;
        // create new selection
        for(int i = 0; i < SelectionCount; i++)
        {
            IInventoryItem newItem = getRandomUnownedItem();
            if (newItem != null)
            {
                SelectableItem newSelectable = selectableItemScene.Instantiate<SelectableItem>();
                itemList.AddChild(newSelectable);
                newSelectable.Item = newItem;
                newSelectable.SelectableItemPressed += SelectableItemPressed;
            }
        }
        Visible = true;
    }

    // selects a random ability, if the ability is already in the inventory then it selects a random upgrade for that ability
    private IInventoryItem getRandomUnownedItem()
    {
        AbilityBase ability = Globals.AbilityRepository.PickRandom().Instantiate<AbilityBase>();
        // check if we already have this ability in the inventory or in the selection
        bool inInventory = inventory.abilities.Count(a => a.ability.ItemID == ability.ItemID) > 0;
        bool inSelection = itemList.GetChildren().Count(c => ((SelectableItem)c).Item.ItemID == ability.ItemID) > 0;
        if(!inInventory)
        {
            if (inSelection)
            {
                // if we don't have the ability, but it's already in the selection, free it and return null
                ability.Free();
                return null;
            }
            return ability;
        }
        // if we have the ability select a random upgrade and free the ability
        UpgradeBase upgrade = ability.upgradeRepository.PickRandom();
        ability.Free();
        return upgrade;
    }

    public void ConfirmButtonPressed()
    {
        if(selected != null)
        {
            // adds the ability or upgrade to the inventory
            if (selected.Item is AbilityBase)
            {
                inventory.addAbility((AbilityBase)selected.Item);
            }
            else
            {
                UpgradeBase upgrade = (UpgradeBase)selected.Item;
                AbilityBase ability = inventory.abilities.Single(a => a.ability.ItemID == upgrade.AbilityID).ability;
                ability.AddUpgrade(upgrade);
            }
            // unpauses the game and hides itself
            Globals.CaptureMouse();
            Visible = false;
            GetTree().Paused = false;
        }
    }

    public void SelectableItemPressed(SelectableItem item)
    {
        if (item != selected)
        {
            if(selected != null) selected.unselect();
            item.select();
            selected = item;
        }
    }

    public override void _Ready()
    {
        itemList = GetNode<VBoxContainer>("%ItemList");
        confirmButton = GetNode<Button>("%ConfirmButton");
        confirmButton.Pressed += ConfirmButtonPressed;
        base._Ready();
    }

}
