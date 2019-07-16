using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct RPGMenuWrapper
{
    public string MenuName;
    public List<RPGMenuItemWrapper> MenuItems;

    public RPGMenuWrapper(string name)
    {
        MenuName = name;
        MenuItems = new List<RPGMenuItemWrapper>();
    }
}

public class RPGMenu : MonoBehaviour {

    public string MenuName;
    public List<RPGMenuItem> MenuItems;

    //prefab
    public GameObject RPGMenuItemPrefab;
    public GameObject RPGMenuItemMagicPrefab;
    public Sprite MenuItemBackgroundSprite;
    public Sprite MenuItemSelectedSprite;

    public Color MenuItemBackgroundColor;
    public Color MenuItemSelectedColor;

    //Scene objects
    public GameObject MenuItemBackgroundHolder;
    public Text MenuTitle;
    public Text MenuHelp;
    public bool IsRoot = false;
    
    //Indexes
    private int selectedIndex = 0;
    private RPGMenu root;

    //Data-driven menus
    Stack<RPGMenuWrapper> MenuSectionSequence;
    static Stack<RPGMenu> MenuList;

    void Start () {

        if (MenuSectionSequence == null)
            MenuSectionSequence = new Stack<RPGMenuWrapper>();

        if (MenuList == null)
            MenuList = new Stack<RPGMenu>();

        if (IsRoot)
        {
            MenuList.Push(this);
            root = this;
        }

        if (MenuItems.Count > 0)
            DrawUI();
    }
	
	void Update () {

        //If we aren't on the top level of the menu.
        if (MenuList.Peek() != this)
            return;

        int currentIndex = selectedIndex;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            selectedIndex++;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            selectedIndex--;
        if (Input.GetKeyDown(KeyCode.Return))
            MenuItems[selectedIndex].Invoke();

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if(MenuSectionSequence.Count > 1)
            {
                MenuSectionSequence.Pop(); //pop the current one
               
                RPGMenuWrapper previous = MenuSectionSequence.Pop(); //pop the last one and retrieve it
                ReloadMenuData(previous);
            }
            else if(MenuList.Count > 1)
            {
                RPGMenu menu = MenuList.Pop();
                Debug.Log("Closing menu window " + menu.gameObject.name);
                menu.gameObject.SetActive(false);
            }
        }

        if (selectedIndex != currentIndex)
        {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, MenuItems.Count - 1);
            DrawUI();
        }
    }

    public void DrawUI()
    {
        for (int i = 0; i < MenuItems.Count; i++)
        {
            if (selectedIndex == i)
                MenuItems[i].ToggleSelected(true);
            else
                MenuItems[i].ToggleSelected(false);
        }

        if(root != null)
        root.MenuHelp.text = MenuItems[selectedIndex].HelpText;

        if (MenuSectionSequence.Count > 0 && MenuTitle != null)
            MenuTitle.text = MenuSectionSequence.Peek().MenuName;
    }

    //The menu item should just perform an action, like an attack or item.
    public void AddMenuItemToAction(RPGMenuItemWrapper menuItemWrapper)
    {
        if(menuItemWrapper.MPCost > 0)
        {
            GameObject gO = Instantiate<GameObject>(RPGMenuItemMagicPrefab, MenuItemBackgroundHolder.transform, false);
            RPGMenuItemMagic magicMenuItem = gO.GetComponent<RPGMenuItemMagic>();
            magicMenuItem.MPCost = menuItemWrapper.MPCost;
            magicMenuItem.Text = menuItemWrapper.Text;
            magicMenuItem.MenuOfThisItem = this;
            magicMenuItem.ActionToPerform = menuItemWrapper.ActionToPerform;
            magicMenuItem.HelpText = menuItemWrapper.HelpText;
            magicMenuItem.InteractType = RPGMenuItemInteractType.MenuItemAction;
            this.MenuItems.Add(magicMenuItem);
            magicMenuItem.transform.GetChild(0).GetComponent<Text>().text = menuItemWrapper.Text;
            magicMenuItem.transform.GetChild(2).GetComponent<Text>().text = menuItemWrapper.MPCost.ToString();
        }
        else
        {
            GameObject gO = Instantiate<GameObject>(RPGMenuItemPrefab, MenuItemBackgroundHolder.transform, false);
            RPGMenuItem item = gO.GetComponent<RPGMenuItem>();
            item.Text = menuItemWrapper.Text;
            item.MenuOfThisItem = this;
            item.ActionToPerform = menuItemWrapper.ActionToPerform;
            item.HelpText = menuItemWrapper.HelpText;
            item.InteractType = RPGMenuItemInteractType.MenuItemAction;
            this.MenuItems.Add(item);

            item.transform.GetChild(0).GetComponent<Text>().text = menuItemWrapper.Text;
        }
    }

    //The menu item will make premade content appear in a different window.
    public void AddMenuItemLinkToNewWindow(string name, RPGMenu menuToOpen, string helpText = "")
    {
        GameObject gO = Instantiate<GameObject>(RPGMenuItemPrefab, MenuItemBackgroundHolder.transform, false);
        RPGMenuItem item = gO.GetComponent<RPGMenuItem>();
        item.Text = name;
        item.MenuOfThisItem = this;
        item.MenuToOpen = menuToOpen;
        item.HelpText = helpText;
        item.InteractType = RPGMenuItemInteractType.MenuItemShowNewWindow;

        this.MenuItems.Add(item);
        item.transform.GetChild(0).GetComponent<Text>().text = name;

    }

    //Content of the menu item should be renewed, in the same window.
    public void AddMenuItemLinkToNewContent(string name, RPGMenuWrapper menuToShowInstead, string helpText = "")
    {
        GameObject gO = Instantiate<GameObject>(RPGMenuItemPrefab, MenuItemBackgroundHolder.transform, false);

        RPGMenuItem item = gO.GetComponent<RPGMenuItem>();
        item.Text = name;
        item.MenuOfThisItem = this;
        item.MenuToShowInstead = menuToShowInstead;
        item.HelpText = helpText;
        item.InteractType = RPGMenuItemInteractType.MenuItemRenewContent;
        this.MenuItems.Add(item);

        item.transform.GetChild(0).GetComponent<Text>().text = name;
    }

    public void OpenNewMenuWindow(RPGMenu menu)
    {
        MenuList.Push(menu);
        menu.gameObject.SetActive(true);
        Debug.Log("Opening new menu window " + menu.gameObject.name);
    }

    public void ReloadMenuData(RPGMenuWrapper wrapper)
    {
        if (MenuSectionSequence == null)
            MenuSectionSequence = new Stack<RPGMenuWrapper>();

        MenuSectionSequence.Push(wrapper);

        for (int i = MenuItems.Count - 1; i >= 0; i--)
        {
            Destroy(MenuItems[i].gameObject);
        }

        MenuItems.Clear();

        foreach (var menuItem in wrapper.MenuItems)
        {
            switch (menuItem.InteractType)
            {
                case RPGMenuItemInteractType.MenuItemAction:
                    AddMenuItemToAction(menuItem);
                    break;
                case RPGMenuItemInteractType.MenuItemShowNewWindow:
                    AddMenuItemLinkToNewWindow(menuItem.Text, menuItem.MenuToOpen, menuItem.HelpText);
                    break;
                case RPGMenuItemInteractType.MenuItemRenewContent:
                    AddMenuItemLinkToNewContent(menuItem.Text, menuItem.MenuToShowInstead, menuItem.HelpText);
                    break;
                default:
                    break;
            }
        }

        Debug.Log("Reloading menu data of menu " + this.name + " with contents of section " + wrapper.MenuName);

        //refresh UI
        selectedIndex = 0;
        DrawUI();
    }
}
