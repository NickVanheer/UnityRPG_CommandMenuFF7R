using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TestRPGMenu : MonoBehaviour {

    public RPGMenu PrimaryMenu;
    public RPGMenu SecondaryMenu;

    public List<GameObject> SceneObjects;

    void Start()
    {
        RPGMenuData mainBattleMenu = new RPGMenuData("Main battle UI");
        RPGMenuData mainItemMenu = new RPGMenuData("Items");

        mainItemMenu.AddItem(new RPGMenuItemData("Potion", "Restore health"));
        mainItemMenu.AddItem(new RPGMenuItemData("Ether", "Restore some mana"));

        //Items menu
        RPGMenuItemData item1 = new RPGMenuItemData("Attack", "Attack with the current equipped weapon.", "Attack", 0, 0);
        RPGMenuItemData item2 = new RPGMenuItemData("Battle items", "Open a new section", mainItemMenu);
        RPGMenuItemData item3 = new RPGMenuItemData("Open a new window", "open it noaw", SecondaryMenu);

        mainBattleMenu.MenuItems.Add(item1);
        mainBattleMenu.MenuItems.Add(item2);
        mainBattleMenu.MenuItems.Add(item3);

        //Final
        PrimaryMenu.ClearContents();
        //SecondaryMenu.Hide();

        PrimaryMenu.RefreshContent(mainBattleMenu);
        //PrimaryMenu.SetAsActivePanel();
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
