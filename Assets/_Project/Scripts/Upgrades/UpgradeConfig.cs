using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Upgrades
{
    [CreateAssetMenu(menuName = "Upgrades/UpgradeConfig")]
    public class UpgradeConfig : ScriptableObject
    {
        public UpgradeDefiniton[] definitons;
        
        public UpgradeDefiniton Get(UpgradeID id)
        {
            foreach (var def in definitons)
            {
                if (def.id == id) return def;
            }
            throw new System.Exception($"Upgrade {id} not found in config");
        }
    }
}