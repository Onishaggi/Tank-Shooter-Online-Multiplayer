using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MirrorTanks
{
    public class NetworkingManager : NetworkManager
    {
        static NetworkingManager instance;

        string localPlayerName;
        
        List<NetworkingPlayer> playersList = new List<NetworkingPlayer>();
        public static NetworkingManager Instance => instance;
        public bool IsServer { get; private set; }
        public bool IsClient { get; private set; }
        public bool IsHost { get => IsServer && IsClient; }
        public string LocalPlayerName { get => localPlayerName; set => localPlayerName = value; }

        public NetworkingPlayer LocalPlayer => PlayersList.Find(x => x.isLocalPlayer);
        public NetworkingPlayer OtherPlayer => PlayersList.Find(x => !x.isLocalPlayer);

        public List<NetworkingPlayer> PlayersList { get => playersList; set => playersList = value; }

        public override void Awake()
        {

            base.Awake();
            if (instance == null)
            {
                instance = this;
            }
            DontDestroyOnLoad(gameObject);

        }
        public override void Start()
        {
            base.Start();
            //StartHost();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            IsServer = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            IsClient = true;
        }

        public void UpdatePlayerName(string pplayerName)
        {
            localPlayerName = pplayerName.ToLower();

        }

        public void AddPlayer(NetworkingPlayer player)
        {
            if (!PlayersList.Contains(player))
            {
                PlayersList.Add(player);
            }

        }
        public void RemovePlayer(NetworkingPlayer player)
        {
            if (PlayersList.Contains(player))
            {
                PlayersList.Remove(player);
            }
        }

        /////////////////
        public NetworkingPlayer GetPlayerByNetId(uint playerId)
        {

            return PlayersList.Find(x=>x.netId== playerId);
        }
    }
}
