using System.Collections;
using System.Collections.Generic;

public interface IUpgradeable
{
    List<IUpgradeOption> UpgradeOptions { get; }
    IUpgradeOption CurrentUpgrade { get; }

    bool ProgressUpgradeTree { get; }
    bool IsUnderUpgrade { get; }

    void OnUpgradeStarted(IUpgradeOption upgradeOption, out bool upgradeStarted);
    IEnumerator OnUpgradeRunning(IUpgradeOption upgradeOption);
    void OnUpgradeFinished(IUpgradeOption upgradeOption);
    void OnUpgradeCanceled();
}