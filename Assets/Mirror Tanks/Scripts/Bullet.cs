using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    public class Bullet : MonoBehaviour
    {
        
        [SerializeField] int damage;
        [SerializeField] float speed;
        uint OwnerId;
        Rigidbody rb;

        public int Damage { get => damage; set => damage = value; }

        // Start is called before the first frame update
        void Start()
        {
            rb= GetComponent<Rigidbody>();
            rb.velocity = transform.forward*speed;

            Destroy(gameObject, 5);
        

        }
        public void Init(uint netId) {
            
            OwnerId = netId;
        }
        private void OnTriggerEnter(Collider other)
        {
            //todo aask
            TeamData playerTeamData = ServiceLocator.Instance.GetService<TeamsHandler>().GetTeamDataByID(NetworkingManager.Instance.GetPlayerByNetId(OwnerId).TeamID);
            if (NetworkingManager.Instance.IsServer)
            {
                if (other.CompareTag("Player"))
                {

                    if (other.gameObject.layer == LayerMask.NameToLayer(playerTeamData.EnemyTeamLayer))
                    {
                        other.GetComponent<NetworkingPlayer>().ApplyDamage(Damage, OwnerId);
                    }
                }
            }
            //Debug.Log(OwnerId);
            if (other.CompareTag("Player"))
            {
                if (other.gameObject.layer != LayerMask.NameToLayer(playerTeamData.FriendlyTeamLayer))
                    Destroy(gameObject);

            }
            else if ((!other.CompareTag("Gun"))&&(!other.CompareTag("ReviveSphere")))
            {
                Destroy(gameObject);
            }
            
            //Debug.Log(other.name);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
