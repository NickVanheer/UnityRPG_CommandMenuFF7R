using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class RPGMenuItemData
{
    public string Text;
    public string HelpText;

    public int ATBCost;
    public int MPCost;

    public MenuItemActionType ItemType = MenuItemActionType.PerformAction;

    //Contents. Either: 
    public string ActionToPerform;
    //public RPGMenu MenuToOpen; 
    public List<GameObject> WindowsToOpen; //If the menu already exists in the editor

    //[Header("Dynamic data for sections")]
    public GameObject DynamicMenuData; 

    public RPGMenuItemData()
    {
        Text = "Empty";
        HelpText = "Empty";
        WindowsToOpen = new List<GameObject>();
        ItemType = MenuItemActionType.PerformAction;
        ActionToPerform = "";
        ATBCost = 0;
        MPCost = 0;
    }

    //It's an action
    public RPGMenuItemData(string text, string helpText, string actionString, int atb, int mp)
    {
        Text = text;
        HelpText = helpText;
        WindowsToOpen = new List<GameObject>();
        ItemType = MenuItemActionType.PerformAction;
        ActionToPerform = actionString;
        ATBCost = atb;
        MPCost = mp;
    }

    //It's a new menu UI panel to open
    public RPGMenuItemData(string text, string helpText, GameObject menuToOpen)
    {
        Text = text;
        HelpText = helpText;
        WindowsToOpen = new List<GameObject>();
        WindowsToOpen.Add(menuToOpen);
        ItemType = MenuItemActionType.NewWindow;
        ActionToPerform = "";
        ATBCost = 0;
        MPCost = 0;
    }

    //Blank
    public RPGMenuItemData(string text, string helpText)
    {
        Text = text;
        HelpText = helpText;
        WindowsToOpen = new List<GameObject>();
        ItemType = MenuItemActionType.PerformAction;
        ActionToPerform = "";
        ATBCost = 0;
        MPCost = 0;
    }
}

[System.Serializable]
public enum MenuItemActionType { PerformAction, NewWindow, NewMenuSection }

public class RPGMenuItem : MonoBehaviour {

    public RPGMenu ParentMenu;
    public RPGMenuItemData MenuItemData;

    Sprite backgroundSprite;
    Color backgroundColor;
    bool isCached = false;

    MenuItemSelectedAnimation selectedAnimation;

    public void Start()
    {
        if (GetComponent<Image>() == null)
            return;

        if (!isCached)
        {
            backgroundSprite = GetComponent<Image>().sprite;
            backgroundColor = GetComponent<Image>().color;
            isCached = true;
        }

        selectedAnimation = GetComponent<MenuItemSelectedAnimation>();
    }
    public virtual void Invoke()
    {
        Assert.IsNotNull(ParentMenu, "RPG menu item does not have an overarching menu assigned");

        switch (MenuItemData.ItemType)
        {
            case MenuItemActionType.PerformAction:
                if (CanInvoke())
                {
                    //MenuItemData.ActionToPerform.Invoke();
                    //TODO: Fire event to easily do this

                }
                break;
            case MenuItemActionType.NewWindow:
                ParentMenu.OpenGroupOfNewMenuWindows(MenuItemData.WindowsToOpen);
                //MenuItemData.WindowsToOpen.ForEach((window) => { ParentMenu.OpenNewMenuWindow(window); }); //old
                break;
            case MenuItemActionType.NewMenuSection:
                ParentMenu.OpenNewSection(MenuItemData.DynamicMenuData);
                break;
            default:
                break;
        }
    }

    //Can be overriden to contain more checks
    public virtual bool CanInvoke()
    {
        if(MenuItemData.ActionToPerform != null)
            return true;
        return false;
    }

    public virtual void ToggleSelected(bool isSelected)
    {
        if(!isCached)
        {
            backgroundSprite = GetComponent<Image>().sprite;
            backgroundColor = GetComponent<Image>().color;
            isCached = true;
        }

        if (isSelected)
        {
            GetComponent<Image>().sprite = ParentMenu.MenuItemSelectedSprite;
            GetComponent<Image>().color = Color.white;

            if (selectedAnimation)
                selectedAnimation.Select();
        }
        else
        {
            GetComponent<Image>().sprite = backgroundSprite;
            GetComponent<Image>().color = backgroundColor;

            if (selectedAnimation)
                selectedAnimation.Unselect();
        }
    }

    public void ShowDisabled()
    {
        //GetComponent<Image>().sprite = backgroundSprite;
        GetComponent<Image>().color = Color.gray;
    }
}
