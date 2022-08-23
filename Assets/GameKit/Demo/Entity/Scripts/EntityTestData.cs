using System.Collections;
using System.Collections.Generic;
using UnityGameKit.Runtime;
using UnityEngine;

namespace UnityGameKit.Demo
{
    [System.Serializable]
    public class EntityTestData : EntityData
    {
        [SerializeField] private string m_testData;
        public EntityTestData(int entityId, int typeId) : base(entityId, typeId)
        {
            m_testData = "this is test data";
        }

        public string TestData
        {
            get
            {
                return m_testData;
            }
        }
    }
}
