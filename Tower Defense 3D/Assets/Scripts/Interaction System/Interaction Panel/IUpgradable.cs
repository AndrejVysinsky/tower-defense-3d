using System.Collections.Generic;

public interface IUpgradable
{
    List<IUpgradeOption> UpgradeOptions { get; }
    IUpgradeOption CurrentUpgrade { get; }

    bool ProgressUpgradeTree { get; }

    void Upgrade(int upgradeIndex);
}