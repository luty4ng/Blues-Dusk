using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LubanConfig.DataTable;
using UnityGameKit.Runtime;

[System.Serializable]
public class CylinderEntityData : EntityData
{
    [SerializeField] private string m_cubeEntityData;
    private Item m_configData;
    public CylinderEntityData(int entityId, int typeId) : base(entityId, typeId)
    {
        m_configData = DataTable.instance.ItemTable.Get(typeId);
        m_cubeEntityData = "This Is Cylinder Entity Data";
    }

    public Item EntityConfigData
    {
        get
        {
            return m_configData;
        }
    }
}
