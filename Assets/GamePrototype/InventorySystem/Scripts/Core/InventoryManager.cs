using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameKit;
using GameKit.DataStructure;

public class InventoryManager : SingletonBase<InventoryManager>
{
    private Dictionary<string, IInventory> inventories;
    private IInventory currentCachedInventory;
    public InventoryManager()
    {
        inventories = new Dictionary<string, IInventory>();
        currentCachedInventory = null;
    }

    public T GetFromInventory<T>(string inventoryName, string stockName) where T : class
    {
        if (HasInventory(inventoryName))
        {
            return inventories[inventoryName].GetStock(stockName).Data as T;
        }
        return null;
    }

    public bool AddToInventory<T>(string inventoryName, int id, string stockName, T data) where T : class
    {
        if (HasInventory(inventoryName))
        {
            IStock stock = inventories[inventoryName].CreateStock<T>(id, stockName, data);
            inventories[inventoryName].AddStock(stock);
            return true;
        }
        return false;
    }
    public IInventory GetInventory(string inventoryName)
    {
        if (HasInventory(inventoryName))
            return inventories[inventoryName];
        Utility.Debugger.LogFail("Can not get inventory, no inventory named {0}.", inventoryName);
        return null;
    }

    public bool CreateInventory(string inventoryName, int size, IInventoryHelper helper)
    {
        if (HasInventory(inventoryName))
        {
            Utility.Debugger.LogFail("Can not create inventory, inventory named {0} has exist.", inventoryName);
            return false;
        }
        Inventory inventory = new Inventory(inventoryName, size, Inventory.GetInventorySerialId());
        inventory.SetHelper(helper);
        inventories.Add(inventoryName, inventory);
        return true;
    }

    public bool RemoveInventory(string inventoryName)
    {
        if (!HasInventory(inventoryName))
        {
            Utility.Debugger.LogFail("Can not remove inventory, no inventory named {0}", inventoryName);
            return false;
        }
        inventories.Remove(inventoryName);
        return true;
    }

    public bool HasInventory(string inventoryName)
    {
        if (!inventories.ContainsKey(inventoryName))
            return false;
        return true;
    }
}
