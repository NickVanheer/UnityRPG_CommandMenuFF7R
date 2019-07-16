using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGMenuItemMagic : RPGMenuItem
{
    public int MPCost;
    public override bool CanInvoke()
    {
        if (ActionToPerform != null && GameManager.Instance.CurrentMP >= MPCost)
            return true;
        return false;
    }

    public override void Invoke()
    {
        base.Invoke();
        GameManager.Instance.ChangeMP(-MPCost);
        //Make magic happen
    }
}