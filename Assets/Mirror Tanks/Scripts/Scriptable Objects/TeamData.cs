using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    [CreateAssetMenu(menuName = "Custom/Team Data")]
    public class TeamData : ScriptableObject
    {
        [SerializeField] private string _teamName;
        
        [SerializeField] private Material _teamMaterial;

        [SerializeField] private string _friendlyTeamLayer;
        [SerializeField] private string _enemyTeamLayer;

        public string FriendlyTeamLayer { get => _friendlyTeamLayer; }
        public string EnemyTeamLayer { get => _enemyTeamLayer;  }

        public string TeamName { get => _teamName; }
        public Material TeamMaterial { get => _teamMaterial;  }
    }
}
