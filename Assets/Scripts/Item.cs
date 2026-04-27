using UnityEngine;

public class Item
{
    public int itemID;
    public string name;
    public string description;


    public Item(int itemID, string name, string description)
    {
        this.itemID = itemID;
        this.name = name;
        this.description = description;
    }
}
