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
    public bool IsAction;
    public string ActionToPerform;

    //Contents. Either: 
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
        IsAction = true;
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
        IsAction = false;
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
        IsAction = false;
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
        IsAction = false;
        ActionToPerform = "";
        ATBCost = 0;
        MPCost = 0;
    }

}

public class RPGMenuItem : MonoBehaviour {

    public RPGMenu ParentMenu;
    public RPGMenuItemData MenuItemData;

    Sprite backgroundSprite;
    Color backgroundColor;
    bool isCached = false;

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

        transform.GetChild(0).GetComponent<Text>().text = MenuItemData.Text; //Do in Menu Item class
    }
    public virtual void Invoke()
    {
        Assert.IsNotNull(ParentMenu, "RPG menu item does not have an overarching menu assigned");

        if(MenuItemData.IsAction)
        {
            if (CanInvoke())
            {
                //MenuItemData.ActionToPerform.Invoke();
                //TODO: Fire event to easily do this
                
            }
            return;
        }
        
        if (MenuItemData.MenuToOpen != null)
        {
            ParentMenu.OpenNewMenuWindow(MenuItemData.MenuToOpen); //TODO Either reload contents or show a new window based on this
        }
        else if(MenuItemData.DynamicMenuData != null)
        {
            //We don't need to open a new menu, we need to add a new subsection to the existing menu
            ParentMenu.OpenNewSection(MenuItemData.DynamicMenuData);
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
        }
        else
        {
            GetComponent<Image>().sprite = backgroundSprite;
            GetComponent<Image>().color = backgroundColor;
        }
    }

    public void ShowDisabled()
    {
        //GetComponent<Image>().sprite = backgroundSprite;
        GetComponent<Image>().color = Color.gray;
    }
}
