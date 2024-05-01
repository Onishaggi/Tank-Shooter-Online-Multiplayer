using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    public class TeamsHandler : NetworkBehaviour
    {
        [SerializeField] private TeamData _redSideTeamData;
        [SerializeField] private TeamData _blueSideTeamData;


        private List<NetworkingPlayer> _redSideTeam= new List<NetworkingPlayer>();

        private List<NetworkingPlayer> _blueSideTeam= new List<NetworkingPlayer>();

        private int localPlayerTeamID;
        public List<NetworkingPlayer> RedSideTeam { get => _redSideTeam; }
        public List<NetworkingPlayer> BlueSideTeam { get => _blueSideTeam; }
        public TeamData RedSideTeamData { get => _redSideTeamData; set => _redSideTeamData = value; }
        public TeamData BlueSideTeamData { get => _blueSideTeamData; set => _blueSideTeamData = value; }
        public int LocalPlayerTeamID { get => localPlayerTeamID; set => localPlayerTeamID = value; }

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);
            DontDestroyOnLoad(gameObject);
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        ////s[ClientRpc]
        //public void RPCAddToRedSideTeam(NetworkingPlayer player)
        //{
        //    if (!NetworkingManager.Instance.IsHost)
        //    {
        //        player.gameObject.layer = LayerMask.NameToLayer(RedSideTeamData.FriendlyTeamLayer);
        //        player.MRenderer.material = RedSideTeamData.TeamMaterial;
        //        _redSideTeam.Add(player);
        //    }

        //}

        ////[ClientRpc]
        //public void RPCAddToBlueSideTeam(NetworkingPlayer player)
        //{
        //    if (!NetworkingManager.Instance.IsHost)
        //    {
        //        player.gameObject.layer = LayerMask.NameToLayer(BlueSideTeamData.FriendlyTeamLayer);
        //        player.MRenderer.material = BlueSideTeamData.TeamMaterial;

        //        _blueSideTeam.Add(player);
        //    }

        //}

        
        public void AddToRedSideTeam(NetworkingPlayer player)
        {
            //if (!NetworkingManager.Instance.IsHost)
            {
                player.gameObject.layer = LayerMask.NameToLayer(RedSideTeamData.FriendlyTeamLayer);
                player.MRenderer.material = RedSideTeamData.TeamMaterial;
                _redSideTeam.Add(player);
                //RPCAddToRedSideTeam(player);
            }

        }
      
        public void AddToBlueSideTeam(NetworkingPlayer player)
        {
            //if (!NetworkingManager.Instance.IsHost)
                player.gameObject.layer = LayerMask.NameToLayer(BlueSideTeamData.FriendlyTeamLayer);
            player.MRenderer.material = BlueSideTeamData.TeamMaterial;

            _blueSideTeam.Add(player);
            //RPCAddToBlueSideTeam(player);

        }
        public TeamData GetTeamDataByID(int id)
        {
            switch (id)
            {
                case 0: return _blueSideTeamData;
                case 1: return _redSideTeamData;
                default: return null;
            }
        }


        //TODO ASK the instructor
        public void AssignPlayerToThereTeam(NetworkingPlayer player,int teamID)
        {
            //if(isLocalPlayer)
            switch(teamID)
            {
                case 0: {  AddToBlueSideTeam(player); } break;
                case 1: {  AddToRedSideTeam(player); } break;
            }
        }

        public void UpdatePlayerTeamID(int teamID)
        {
            LocalPlayerTeamID = teamID;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
