using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    [CreateAssetMenu(menuName = "Custom/Role Data")]
    public class RoleData : ScriptableObject
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private int _damage;

        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
        public int Damage { get => _damage; set => _damage = value; }
    }
}
