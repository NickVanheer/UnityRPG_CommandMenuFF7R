using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TestRPGMenu : MonoBehaviour {

    public RPGMenu PrimaryMenu;
    public RPGMenu SecondaryMenu;
    public List<GameObject> SceneObjects;

	void Awake () {

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

        //Loads the primary menu
        //PrimaryMenu.GetComponent<RPGMenu>().LoadMenuData(menuWrapper);
        PrimaryMenu.GetComponent<RPGMenu>().AddMenuView(menuWrapper);
        PrimaryMenu.gameObject.SetActive(true);
        RPGMenu.GlobalMenuList.Push(PrimaryMenu.GetComponent<RPGMenu>());


        //
        RPGMenuWrapper itemMenuWrapper = new RPGMenuWrapper("Items");
        itemMenuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Potion", HelpText = "Use a potion", InteractType = RPGMenuItemInteractType.MenuItemAction });
        itemMenuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Phoenix Down", HelpText = "Revive a party member", InteractType = RPGMenuItemInteractType.MenuItemAction });
        itemMenuWrapper.MenuItems.Add(new RPGMenuItemWrapper() { Text = "Ether", HelpText = "Restore MP", InteractType = RPGMenuItemInteractType.MenuItemAction });
        SecondaryMenu.GetComponent<RPGMenu>().AddMenuView(itemMenuWrapper);
        SecondaryMenu.gameObject.SetActive(false);

        //GameManager.Instance.ChangeMP(0); //Trigger UI update to show correct MP
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
