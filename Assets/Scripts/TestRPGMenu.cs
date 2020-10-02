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
        RPGMenuData mainSkillMenu = new RPGMenuData("Skills");

        mainSkillMenu.AddItem(new RPGMenuItemData("Omnislash", "A powerful slash"));
        mainSkillMenu.AddItem(new RPGMenuItemData("Spiral Cut", "Another powerful slash"));
        mainSkillMenu.AddItem(new RPGMenuItemData("Dive Cut", "You know the drill"));

        //Top level item menu
        RPGMenuItemData item1 = new RPGMenuItemData("Attack", "Attack with the current equipped weapon.", "Attack", 0, 0);
        RPGMenuItemData item2 = new RPGMenuItemData("Skills", "Open a new section", mainSkillMenu);
        RPGMenuItemData item3 = new RPGMenuItemData("Item", "Select an item to use", SecondaryMenu);

        mainBattleMenu.MenuItems.Add(item1);
        mainBattleMenu.MenuItems.Add(item2);
        mainBattleMenu.MenuItems.Add(item3);

        //Final
        PrimaryMenu.ClearContentsAndSections();
        PrimaryMenu.OpenNewSection(mainBattleMenu);
        //SecondaryMenu.Hide();
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
