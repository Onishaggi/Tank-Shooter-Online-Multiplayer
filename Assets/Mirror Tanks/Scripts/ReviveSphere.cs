using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorTanks
{
    public class ReviveSphere : NetworkBehaviour
    {
        [SerializeField] TextMeshProUGUI _counterText;
        [SerializeField] ReviveData _reviveData;
        [SerializeField] Transform uiRoot;
       
        [SerializeField] Image _decayImage;

        NetworkingPlayer _ownerNP;
        GameObject _ownerGameObject;
        TeamData _ownerTeamData;
        public readonly SyncList<NetworkingPlayer> _allyPlayers= new SyncList<NetworkingPlayer>();// we wont need sync lists but i did it just incase and for learning too
                                                                                                  // for better bandwith we should use normal lists
       
        
        [SyncVar(hook =nameof(UpdateRevivingTime))] float _timeRemaningToRevive;
        [SyncVar(hook =nameof(UpdateDecayingTime))] float _decayTimer;
        [SyncVar] bool Decayed;

        public SyncList<NetworkingPlayer> AllyPlayers { get => _allyPlayers; }

        // Start is called before the first frame update
        private void Awake()
        {
            _ownerGameObject = transform.parent.gameObject;
            _ownerNP=_ownerGameObject.GetComponent<NetworkingPlayer>();
        }
        private void OnEnable()
        {
            
        }
        void Start()
        {
            _ownerTeamData=ServiceLocator.Instance.GetService<TeamsHandler>().GetTeamDataByID(_ownerNP.TeamID);
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            _timeRemaningToRevive = _reviveData.ReviveRequiredTime;
            _decayTimer = _reviveData.TimeToDie;

        }
        // Update is called once per frame
        void Update()
        {
            if (NetworkingManager.Instance.IsServer)
            {
                if (Decayed == false)
                {
                    DecayingOperation();

                    RevivingOperation();
                }
            }
            uiRoot.LookAt(_ownerNP.CamTransform);

        }

        //[Server] without the server its not gonna get ccalled on the server so the sync wont work but if i remove the server its working why?
        //maybe beacuse the check in the update that its called in the server anyway
        [Server]
        void RevivingOperation()
        {
            if (AllyPlayers.Count > 0)
            {
                if (_timeRemaningToRevive > 0)
                {
                    _timeRemaningToRevive -= Time.deltaTime;

                }
            }
            else
            {
                _timeRemaningToRevive = _reviveData.ReviveRequiredTime;
            }
            
        }

        [Server]
        void DecayingOperation()
        {
            if(AllyPlayers.Count <= 0) 
            {
                if(_decayTimer > 0)
                {
                    _decayTimer -= Time.deltaTime;
                }
            }
            else
            {
                _decayTimer = _reviveData.TimeToDie;
            }
        }
        public void addAllyToList(NetworkingPlayer player)//if we gonna use normal lists
        {
            if (!AllyPlayers.Find(x => x == player))
                AllyPlayers.Add(player);
            
        }
        [Server]
        public void ServerAddAllyFromList(NetworkingPlayer player)
        {
            if(!AllyPlayers.Find(x=>x==player))
                AllyPlayers.Add(player);
            
        }
       
        

        private void OnTriggerEnter(Collider other)
        {
            if(NetworkingManager.Instance.IsServer)
            if (other.CompareTag("Player"))
            {
                if(other.gameObject!=_ownerGameObject&& other.gameObject.layer == LayerMask.NameToLayer(_ownerTeamData.FriendlyTeamLayer)&&other.gameObject.GetComponent<NetworkingPlayer>().IsDead==false)
                {
                        //addAllyToList(other.gameObject.GetComponent<NetworkingPlayer>());
                        
                        ServerAddAllyFromList(other.gameObject.GetComponent<NetworkingPlayer>());
                   

                }
            }
        }
        
        public void removeAllyFromList(NetworkingPlayer player)// if we gonna use normal lists
        {
            AllyPlayers.RemoveAll(x=>x==player);
        }

        [Server]
        public void ServerRemoveAllyFromList(NetworkingPlayer player)
        {
            AllyPlayers.RemoveAll(x=>x==player);
            
        }
        [Server]
        public void ServerClearList() {  AllyPlayers.Clear(); }    
        
        

        private void OnTriggerExit(Collider other)
        {
            if (NetworkingManager.Instance.IsServer)
            if (other.CompareTag("Player"))
            {
                if (other.gameObject != _ownerGameObject && other.gameObject.layer == LayerMask.NameToLayer(_ownerTeamData.FriendlyTeamLayer))
                {
                    //removeAllyFromList(other.gameObject.GetComponent<NetworkingPlayer>());
                    ServerRemoveAllyFromList(other.gameObject.GetComponent<NetworkingPlayer>());
                }
            }

        }


        void UpdateRevivingTime(float oldVal,float newVal)
        {
            _timeRemaningToRevive += 1;
            _timeRemaningToRevive = newVal;
            

            if (_timeRemaningToRevive <= 0)
            {
                if (NetworkingManager.Instance.IsServer)//if i dont use this it wont cause a problem idk why
                    Revive();// why not checking is allowed
                             // as this is only called on the server but Decay isn't
            }
            float minutes = Mathf.FloorToInt(_timeRemaningToRevive / 60);
            float seconds = Mathf.FloorToInt(_timeRemaningToRevive % 60);
            _counterText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        void UpdateDecayingTime(float oldVal,float newVal)
        {
            _decayTimer += 1;
            _decayTimer = newVal;
            if (_decayTimer <= 0)
            {
                if (NetworkingManager.Instance.IsServer)//why do i use check here but in Revive function i dont.
                    Decay();                               //the error is its called when server is not active
                                                              //but this is triggered on by the changes in the updatewhich has a check to only be called in the server?
                                                                 //[1]MaybeBeacuse the hook function is called in clients too that a reason too
                                                                    //but again why revive allows it and decay doesnt
                                                                        //maybe[1] is the logical and correct answer and revive() is the abnormal behaviour and i must do the check to it anyway
                _counterText.text = "Died";
            }
            else
            {
                float minutes = Mathf.FloorToInt(_decayTimer / 60);
                float seconds = Mathf.FloorToInt(_decayTimer % 60);
                _counterText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                _decayImage.fillAmount=_decayTimer/_reviveData.TimeToDie;
            }
        }


        //NOICE
        [Server]
        void Revive()
        {
            _ownerNP.Hp = _ownerNP.MaxHealth;
            _ownerNP.IsDead = false;
            _timeRemaningToRevive = _reviveData.ReviveRequiredTime;
            AllyPlayers.Clear();
            RPCRevive();
            transform.gameObject.SetActive(false);
        }
        [ClientRpc]
        void RPCRevive()
        {
            if (!NetworkingManager.Instance.IsHost)
            {
                //AllyPlayers.Clear();
                transform.gameObject.SetActive(false);
            }
        }
        [Server]
        void Decay()
        {
            Decayed = true;
            
        }


    }
}
