using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestRPGMenu : MonoBehaviour {

    public RPGMenu SecondaryMenu;
    public List<GameObject> SceneObjects;

	void Start () {

        RPGMenuWrapper menuWrapper = new RPGMenuWrapper("Battle menu");
        menuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Attack", HelpText = "Execute a standard attack", InteractType = RPGMenuItemInteractType.MenuItemAction });
   
        RPGMenuWrapper subSkillMenuWrapper = new RPGMenuWrapper("MP Skills");
        subSkillMenuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Blaze", InteractType = RPGMenuItemInteractType.MenuItemAction });
        subSkillMenuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Booze", InteractType = RPGMenuItemInteractType.MenuItemAction });

        RPGMenuWrapper magicMenuWrapper = new RPGMenuWrapper("Magic");
        magicMenuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Fire", HelpText = "Use the power of fire", ActionToPerform = PrepareEvent(Fire), MPCost = 30, InteractType = RPGMenuItemInteractType.MenuItemAction });
        magicMenuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Thunder", ActionToPerform = PrepareEvent(Thunder), MPCost = 40, InteractType = RPGMenuItemInteractType.MenuItemAction });
        magicMenuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Blizzard", ActionToPerform = PrepareEvent(Blizzard), MPCost = 30, InteractType = RPGMenuItemInteractType.MenuItemAction });

        menuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Magic", HelpText = "Use powerful magic", InteractType = RPGMenuItemInteractType.MenuItemRenewContent, MenuToShowInstead = magicMenuWrapper });
        menuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Items", HelpText = "Use an item", InteractType = RPGMenuItemInteractType.MenuItemShowNewWindow, MenuToOpen = SecondaryMenu });

        GetComponent<RPGMenu>().ReloadMenuData(menuWrapper);

        GameManager.Instance.ChangeMP(0); //Trigger UI update to show correct MP
    }

    UnityEvent PrepareEvent(UnityAction action)
    {
        UnityEvent ev = new UnityEvent();
        ev.AddListener(action);
        return ev;
    }

    void Fire()
    {
        foreach (var obj in SceneObjects)
            obj.GetComponent<Renderer>().material.color = Color.red;
    }

    void Thunder()
    {
        foreach (var obj in SceneObjects)
            obj.GetComponent<Renderer>().material.color = Color.yellow;
    }
    
    void Blizzard()
    {
        foreach (var obj in SceneObjects)
            obj.GetComponent<Renderer>().material.color = Color.blue;
    }
}
