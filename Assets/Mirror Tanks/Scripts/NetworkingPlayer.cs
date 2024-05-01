using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Unity.VisualScripting;


namespace MirrorTanks
{
    public class NetworkingPlayer : NetworkBehaviour
    {
        [Header("Movement")]
        [SerializeField] Transform cannonRotation;
        [SerializeField][SyncVar] float speed;
        [SerializeField][SyncVar] float rotationSpeed;
        [SerializeField] ReviveSphere _reviveSphere;
        Rigidbody rb;
        [SyncVar(hook = nameof(OnDeathOperations))] bool isDead;
        [Header("UI")]
        [SerializeField] Transform uiRoot;
        [SerializeField] TextMeshProUGUI text_pname;
        [SerializeField] Image img_hp;

        [Header("Shooting")]
        [SerializeField] Bullet bulletPrefab;
        [SerializeField] Transform bulletSpawningPos;
        [SerializeField] Transform cannonPivot;


        [Header("Stats")]
        [SyncVar(hook = nameof(OnRoleIDUpdated))] int roleDataID = -1;
        RoleData roleData;
        [SerializeField][SyncVar] float maxHealth;
        [SyncVar] int damage;
        //Reviving Vaars
        List<ReviveSphere> _playersReviving = new List<ReviveSphere>();


        //Properties
        Renderer _renderer;


        //Networked Variables
        [SyncVar(hook = nameof(OnNameUpdated))] string pName;
        [SyncVar(hook = nameof(OnHealthUpdated))] float hp;
        [SyncVar(hook = nameof(OnTeamIDUpdated))] int _teamID = -1;

        Transform camTransform;

        public int TeamID { get => _teamID; set => _teamID = value; }
        public Renderer MRenderer { get => _renderer; set => _renderer = value; }
        public bool IsDead { get => isDead; set => isDead = value; }
        public float Hp { get => hp; set => hp = value; }
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        public Transform CamTransform { get => camTransform; set => camTransform = value; }

        public int RoleDataID { get => roleDataID; set => roleDataID = value; }
        public RoleData RoleData { get => roleData; set => roleData = value; }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();


            CmdUpdatePlayerName(NetworkingManager.Instance.LocalPlayerName);

            CmdUpdatePlayerTeam(ServiceLocator.Instance.GetService<TeamsHandler>().LocalPlayerTeamID);

            CmdUpdateRoleDataID(ServiceLocator.Instance.GetService<RolesHandler>().AssignedRoleID);




        }
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            CamTransform = Camera.main.transform;

            NetworkingManager.Instance.AddPlayer(this);




        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            Hp = MaxHealth;//instead of calling CmdUpdateHealth
        }


        // Update is called once per frame
        void Update()
        {
            if (isLocalPlayer && IsDead == false)
            {
                //Movement
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");



                Vector3 movVec = new Vector3(x, 0, z) * (speed * Time.deltaTime);
                rb.Move(rb.position + movVec, rb.rotation);



                bool right = Input.GetKey(KeyCode.RightArrow);
                bool left = Input.GetKey(KeyCode.LeftArrow);

                bool shoot = Input.GetKeyDown(KeyCode.Space);

                if ((right || left))
                {
                    if (!(right && left))
                    {
                        float rotationAngle = rotationSpeed * Time.deltaTime;
                        if (left)
                        {
                            rotationAngle *= -1;
                        }

                        cannonRotation.Rotate(transform.up, rotationAngle);
                    }



                }
                if (shoot)
                {
                    CmdShoot();
                }

            }

            uiRoot.LookAt(CamTransform);


        }

        [Command]
        void CmdUpdatePlayerName(string name)
        {
            pName = name;

        }
        [Command]
        void CmdUpdateHealth(float health)//dericated
        {
            Hp = health;
        }
        [Command]
        void CmdUpdatePlayerTeam(int teamID)
        {
            TeamID = teamID;

        }
        [Command]

        void CmdUpdateRoleDataID(int sroleDataID)
        {//we assign th data id and we call the getrole function and we give it our role id and it rerturns its rolde data
            //we probably wont need to sync the variable but it wont be updated in the ui unless we damage it so there is alot of room for optimization here


            RoleDataID = sroleDataID;

            ServerUpdatePlayerData(ServiceLocator.Instance.GetService<RolesHandler>().GetRole(RoleDataID, this));
        }


        public void ServerUpdatePlayerData(RoleData rd)
        {

            maxHealth = rd.MaxHealth;
            hp = maxHealth;
            rotationSpeed = rd.RotationSpeed;
            speed = rd.MovementSpeed;
            damage = rd.Damage;

        }

        [Server]
        public void ApplyDamage(int damage, uint ownerID)
        {
            Hp -= damage;
            Hp = Mathf.Max(Hp, 0);
            //check death condition
            if (Hp <= 0)
            {
                ServerRPCDie(ownerID);//beacause is dead beccame syncvar so it should only be called at the server
                //RpcDie(ownerID);
                if (IsDead == false)
                {
                    ServiceLocator.Instance.GetService<GameUI>().CmdRegisterKillInUI($"{NetworkingManager.Instance.GetPlayerByNetId(ownerID).pName} Killed {pName}");

                }
                IsDead = true;
            }
        }
        [Command]
        void CmdShoot()
        {
            Bullet bullet = Instantiate(bulletPrefab, bulletSpawningPos.position, cannonPivot.rotation);
            bullet.Init(netId);
            bullet.Damage = damage;
            RpcShoot(bullet.transform.position, bullet.transform.rotation);
        }
        [ClientRpc]
        void RpcShoot(Vector3 position, Quaternion rotation)
        {
            if (!NetworkingManager.Instance.IsHost)
            {
                Bullet bullet = Instantiate(bulletPrefab, bulletSpawningPos.position, cannonPivot.rotation);
                bullet.Init(netId);

            }

        }
        [Server]
        void ServerRPCDie(uint netID)
        {
            if (IsDead == false)
            {
                //IsDead=true;
                RpcDie(netID);
            }

        }

        [ClientRpc]

        void RpcDie(uint netiD)
        {

            _playersReviving.Clear();
            Debug.Log($"{NetworkingManager.Instance.GetPlayerByNetId(netiD).pName} Killed {pName} ");
            _reviveSphere.gameObject.SetActive(true);


        }
        void OnHealthUpdated(float oldVal, float newVal)
        {
            Hp = newVal;
            img_hp.fillAmount = Hp / MaxHealth;
        }
        void OnNameUpdated(string oldVal, string newVal)
        {
            pName = newVal;
            text_pname.text = pName;
        }
        void OnTeamIDUpdated(int oldVal, int newVal)
        {
            TeamID = newVal;
            MRenderer = GetComponent<Renderer>();

            ServiceLocator.Instance.GetService<TeamsHandler>().AssignPlayerToThereTeam(this, TeamID);//same as we did in the role we give this function the team id and it assigns the player to there team and its properties

        }
        void OnRoleIDUpdated(int oldVar, int newVar)
        {
            RoleDataID = newVar;
        }
        private void OnTriggerEnter(Collider other)
        {

            if (other.CompareTag("ReviveSphere") && other.gameObject.transform.parent.GetComponent<NetworkingPlayer>().TeamID == TeamID)
            {
                if (!_playersReviving.Find(x => x == other.gameObject.GetComponent<ReviveSphere>()))//we check here becuase if the player didnt exit the trigger and entered the trigger again probably wont need it
                                                                                                    //we use it for safty reasons
                    _playersReviving.Add(other.gameObject.GetComponent<ReviveSphere>());
            }

        }

        private void OnTriggerExit(Collider other)
        {

            if (other.CompareTag("ReviveSphere") && other.gameObject.transform.parent.GetComponent<NetworkingPlayer>().TeamID == TeamID)
            {
                _playersReviving.RemoveAll(x => x == other.gameObject.GetComponent<ReviveSphere>());//we remove all the instances just incase probably wont need it but its there for safty reasons
                                                                                                    //maybe we should have used dictionary instead!
            }

        }

        void OnDeathOperations(bool oldVal, bool newVal)
        {
            IsDead = newVal;
            if (NetworkingManager.Instance.IsServer)
            {
                if (IsDead)//so here if the player died while reviving the player the player being revived must reset
                {
                    foreach (var reviveSphere in _playersReviving)
                    {
                        // reviveSphere.removeAllyFromList(this);//if we gonna use normal list for better performance check the better bandwith version of this project

                        reviveSphere.ServerRemoveAllyFromList(this);//we remove all for safty 

                    }


                }
                else//so here if the player became alive while he is in range of a revive sphere he should add himself to the players revivng the player and start reviveing him
                {
                    foreach (var reviveSphere in _playersReviving)
                    {
                        //reviveSphere.addAllyToList(this);//if we gonna use normal list for better performance check the better bandwith version of this project
                        if (!reviveSphere.AllyPlayers.Find(x => x == this))//if its already there dont add again we could havve used dictionary
                            reviveSphere.ServerAddAllyFromList(this);

                    }


                }
            }

            if (IsDead)//when ever a player dies remove him from the list of currently active team players or the player gets revived add him back
            {
                switch (TeamID)
                {
                    case 0:
                        ServiceLocator.Instance.GetService<TeamsHandler>().BlueSideTeam.Remove(this); break;
                    case 1:
                        ServiceLocator.Instance.GetService<TeamsHandler>().RedSideTeam.Remove(this); break;
                }
            }
            else
            {
                switch (TeamID)
                {
                    case 0:
                        ServiceLocator.Instance.GetService<TeamsHandler>().BlueSideTeam.Add(this); break;
                    case 1:
                        ServiceLocator.Instance.GetService<TeamsHandler>().RedSideTeam.Add(this); break;
                }

            }
            if (NetworkingManager.Instance.IsServer)
                CheckForWinners();// we check for if all players of a team dies here we could used event but this works too



        }

        [Server]
        void CheckForWinners()
        {
            if (ServiceLocator.Instance.GetService<TeamsHandler>().BlueSideTeam.Count <= 0)
            {
                ServiceLocator.Instance.GetService<GameUI>().ShowWinner(1);
            }
            else if (ServiceLocator.Instance.GetService<TeamsHandler>().RedSideTeam.Count <= 0)
            {
                ServiceLocator.Instance.GetService<GameUI>().ShowWinner(0);
            }
        }



    }
}
