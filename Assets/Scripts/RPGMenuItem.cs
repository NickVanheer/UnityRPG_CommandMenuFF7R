using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;


public struct RPGMenuItemWrapper
{
    public string Text;
    public string HelpText;
    public UnityEvent ActionToPerform;
    public RPGMenu MenuToOpen;
    public RPGMenuWrapper MenuToShowInstead;
    public RPGMenuItemInteractType InteractType;

    public int ATBCost;
    public int MPCost;

}

public enum RPGMenuItemInteractType { MenuItemAction, MenuItemShowNewWindow, MenuItemRenewContent };

public class RPGMenuItem : MonoBehaviour {

    public RPGMenu MenuOfThisItem;
    public string Text;
    public string HelpText;
    public UnityEvent ActionToPerform;
    public RPGMenu MenuToOpen;
    public RPGMenuWrapper MenuToShowInstead;
    public RPGMenuItemInteractType InteractType;

    public virtual void Invoke()
    {
        Assert.IsNotNull(MenuOfThisItem, "RPG menu item does not have an overarching menu assigned");

        switch (InteractType)
        {
            case RPGMenuItemInteractType.MenuItemAction:
                if (CanInvoke())
                {
                    Debug.Log("Invoking command of menu item " + Text);
                    ActionToPerform.Invoke();
                }
                break;
            case RPGMenuItemInteractType.MenuItemShowNewWindow:
                if (MenuToOpen != null)
                    MenuOfThisItem.OpenNewMenuWindow(MenuToOpen);
                break;
            case RPGMenuItemInteractType.MenuItemRenewContent:
                MenuOfThisItem.ReloadMenuData(MenuToShowInstead);
                break;
            default:
                break;
        }
    }

    //Can be overriden to contain more checks
    public virtual bool CanInvoke()
    {
        if(ActionToPerform != null)
            return true;
        return false;
    }

    public virtual void ToggleSelected(bool isSelected)
    {
        if (isSelected)
        {
            GetComponent<Image>().sprite = MenuOfThisItem.MenuItemSelectedSprite;
            GetComponent<Image>().color = MenuOfThisItem.MenuItemSelectedColor;
        }
        else
        {
            GetComponent<Image>().sprite = MenuOfThisItem.MenuItemBackgroundSprite;
            GetComponent<Image>().color = MenuOfThisItem.MenuItemBackgroundColor;
        }
    }
}
