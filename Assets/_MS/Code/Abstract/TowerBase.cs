using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour
{
    public List<TowerUpgrade> upgrades;

    protected int currentUpgradeIndex = -1;

    public void ApplyUpgrade(int upgradeIndex)
    {
        if (upgradeIndex >= 0 && upgradeIndex < upgrades.Count)
        {
            currentUpgradeIndex = upgradeIndex;
            upgrades[currentUpgradeIndex].applyUpgrade(this);
        }
    }
}
