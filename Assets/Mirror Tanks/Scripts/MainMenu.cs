using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorTanks
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] TMP_InputField if_PlayerNmae;
        [SerializeField] TMP_Dropdown dropdown;
        [SerializeField] TMP_Dropdown roleDropdown;
        private TeamsHandler teamsHandler;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);
        }
        void Start()
        {
            teamsHandler = ServiceLocator.Instance.GetService<TeamsHandler>();//ignore it
        }
        public void OnStartServerClicked()
        {
            NetworkingManager.Instance.StartServer();
        }
        public void OnStartHostClicked()
        {
            if (!string.IsNullOrEmpty(if_PlayerNmae.text))
            {
                NetworkingManager.Instance.UpdatePlayerName(if_PlayerNmae.text);
                ServiceLocator.Instance.GetService<TeamsHandler>().UpdatePlayerTeamID(dropdown.value);
                ServiceLocator.Instance.GetService<RolesHandler>().AssignedRoleID=roleDropdown.value;
                NetworkingManager.Instance.StartHost();
                
            }


        }
        public void OnStartClientClicked()
        {
            if (!string.IsNullOrEmpty(if_PlayerNmae.text))
            {
                NetworkingManager.Instance.UpdatePlayerName(if_PlayerNmae.text);
                ServiceLocator.Instance.GetService<TeamsHandler>().UpdatePlayerTeamID(dropdown.value);
                ServiceLocator.Instance.GetService<RolesHandler>().AssignedRoleID = roleDropdown.value;
                NetworkingManager.Instance.StartClient();
            }
        }
        // Start is called before the first frame update
        

        // Update is called once per frame
        void Update()
        {

        }
    }
}
