using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class RPGMenuItemData
{
    public string Text;
    public string HelpText;

    public MenuItemActionType ItemType;

    //Contents. Either: 
    public string ActionToPerform;
    public RPGMenu MenuToOpen; //If the menu already exists in the editor
    public RPGMenuData DynamicMenuData; //If the menu is new/dynamic

    public int ATBCost;
    public int MPCost;

    //It's an action
    public RPGMenuItemData(string text, string helpText, string actionString, int atb, int mp)
    {
        Text = text;
        HelpText = helpText;
        MenuToOpen = null;
        DynamicMenuData = null;
        ItemType = MenuItemActionType.PerformAction;
        ActionToPerform = actionString;
        ATBCost = atb;
        MPCost = mp;
    }

    //It's a new menu UI panel to open
    public RPGMenuItemData(string text, string helpText, RPGMenu menuToOpen)
    {
        Text = text;
        HelpText = helpText;
        MenuToOpen = menuToOpen;
        DynamicMenuData = null;
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
        MenuToOpen = null;
        DynamicMenuData = null;
        ItemType = MenuItemActionType.PerformAction;
        ActionToPerform = "";
        ATBCost = 0;
        MPCost = 0;
    }

    //New content
    public RPGMenuItemData(string text, string helpText, RPGMenuData newData)
    {
        Text = text;
        HelpText = helpText;
        MenuToOpen = null;
        DynamicMenuData = newData;
        ItemType = MenuItemActionType.NewMenuSection;
        ActionToPerform = "";
        ATBCost = 0;
        MPCost = 0;
    }

}

public enum MenuItemActionType { PerformAction, NewWindow, NewMenuSection }

public class RPGMenuItem : MonoBehaviour {

    public RPGMenu ParentMenu;
    public RPGMenuItemData MenuItemData;

    Sprite backgroundSprite;
    Color backgroundColor;
    bool isCached = false;

    MenuItemSelectedAnimation animation;

    public void Start()
    {
        if (MenuItemData == null)
        {
            Debug.Log("Empty");
        }

        if (!isCached)
        {
            backgroundSprite = GetComponent<Image>().sprite;
            backgroundColor = GetComponent<Image>().color;
            isCached = true;
        }

        animation = GetComponent<MenuItemSelectedAnimation>();

        //transform.GetChild(0).GetComponent<Text>().text = MenuItemData.Text; 
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
                ParentMenu.OpenNewMenuWindow(MenuItemData.MenuToOpen);
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

            if (animation)
                animation.Select();
        }
        else
        {
            GetComponent<Image>().sprite = backgroundSprite;
            GetComponent<Image>().color = backgroundColor;

            if (animation)
                animation.Unselect();
        }
    }

    public void ShowDisabled()
    {
        //GetComponent<Image>().sprite = backgroundSprite;
        GetComponent<Image>().color = Color.gray;
    }
}
