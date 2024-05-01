using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    public class RolesHandler:MonoBehaviour
    {
        [SerializeField] RoleData tankData;
        [SerializeField] RoleData dpsData;
        int assignedRoleID;
        public RoleData TankData { get => tankData; set => tankData = value; }
        public RoleData DpsData { get => dpsData; set => dpsData = value; }
        public int AssignedRoleID { get => assignedRoleID; set => assignedRoleID = value; }

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);
            DontDestroyOnLoad(gameObject);
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }
        public void AssginTankRole(NetworkingPlayer player)
        {

        }
        public void AssginDpsRole(NetworkingPlayer player)
        {

        }
        public RoleData GetRole(int roleID,NetworkingPlayer player)
        {
            //if(IsServer)
            switch (roleID)
            {
                case 0: { player.RoleData = TankData;  return TankData; }
                case 1: { player.RoleData = DpsData; return DpsData; }
                 default: return null;
            }
            
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
