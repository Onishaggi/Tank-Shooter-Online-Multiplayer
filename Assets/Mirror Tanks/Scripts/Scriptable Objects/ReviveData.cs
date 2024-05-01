using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    [CreateAssetMenu(menuName = "Custom/Revive Sphere Data")]
    public class ReviveData : ScriptableObject
    {
        [SerializeField] private int _reviveRequiredTime;
        [SerializeField] private int _timeToDie;


        public int ReviveRequiredTime { get => _reviveRequiredTime; }
        public int TimeToDie { get => _timeToDie; }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
