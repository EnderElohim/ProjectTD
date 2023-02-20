using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrArrowTower : TowerBase
{
    public scrArrowTower()
    {
        upgrades = new List<TowerUpgrade>()
        {
            new TowerUpgrade()
            {
                upgradeName = "Long-Range Sniper",
                upgradeDescription = "Increases attack range, but cannot attack nearby enemies",
                applyUpgrade = (tower) =>
                {
                    // Increase the attack range, but decrease the damage for nearby enemies
                    print("A");
                }
            },
            new TowerUpgrade()
            {
                upgradeName = "Area-of-Effect Attack",
                upgradeDescription = "Can hit multiple enemies at once",
                applyUpgrade = (tower) =>
                {
                    // Change the attack method to an area-of-effect attack
                     print("B");
                }
            },
            new TowerUpgrade()
            {
                upgradeName = "Buffing Aura",
                upgradeDescription = "Increases the power of nearby towers",
                applyUpgrade = (tower) =>
                {
                    // Add a buffing aura to the tower
                     print("C");
                }
            }
        };
    }
}
