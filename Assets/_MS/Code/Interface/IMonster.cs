using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonster 
{
    public int maxHealth { get; }
    public int currentHealth { get; set; }
    public bool isFlying { get; set; }
    public void TakeDamage(int damage);
    public void Die();
}
