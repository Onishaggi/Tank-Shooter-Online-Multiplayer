using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MirrorTanks
{
    public class GameUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_TextMeshProUGUI;
        [SerializeField] TextMeshProUGUI _winnerText;


        private Stack<string> _killsUI=new Stack<string>();

        public Stack<string> KillsUI { get => _killsUI; set => _killsUI = value; }

        [ClientRpc]
        public void CmdRegisterKillInUI(string killInfo)
        {
            KillsUI.Push(killInfo+"\n");
            OnKillsUIUpdate();

        }
        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);
        }
        // Start is called before the first frame update
        void Start()
        {
            

        }

        // Update is called once per frame
        void Update()
        {
        
        }
        string getkillsText()
        {
            string temp="";
            foreach(var kill in KillsUI)
            {
                temp += kill;
            }
            return temp;
        }
        IEnumerator RemoveOldKillFromUI()
        {
            yield return new WaitForSeconds(5);
            if(KillsUI.Count > 0)
            {
                KillsUI.Pop();
            }
            m_TextMeshProUGUI.text= getkillsText();
        }
        void OnKillsUIUpdate()
        {
            m_TextMeshProUGUI.text = getkillsText();
            StartCoroutine(RemoveOldKillFromUI());

        }

        [ClientRpc]
        public void ShowWinner(int teamiD)
        {
            switch (teamiD)
            {
                case 0: { _winnerText.text = "Blue Team Wins"; _winnerText.color = Color.blue; } break; 
                case 1: { _winnerText.text = "Red Team Wins"; _winnerText.color = Color.red; } break; 
            }
            _winnerText.transform.parent.gameObject.SetActive(true);

        }
    }
}
