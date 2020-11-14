using System.Collections.Generic;

public interface IUpgradable
{
    List<IUpgradeOption> UpgradeOptions { get; }
    bool ProgressUpgradeTree { get; }

    void Upgrade(IUpgradeOption upgradeOption, int upgradeIndex);
}