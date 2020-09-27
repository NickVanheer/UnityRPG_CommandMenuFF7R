using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

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
    //public bool IsRoot = false;
    
    //Indexes
    private int selectedIndex = 0;

    //Subsection in this single menu
    Stack<RPGMenuWrapper> MenuSections;

    //Contains all separate RPG menus globally
    public static Stack<RPGMenu> GlobalMenuList;
    static short IDPool = -1;
    public short ID;

    public void Log(string text)
    {
        Debug.Log(ID + " : " + text);
    }

    public void AddMenuView(RPGMenuWrapper menu)
    {
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuWrapper>();

        MenuSections.Push(menu);
        Log("Added menu view: " + menu.MenuName);
    }

    private void Awake()
    {
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuWrapper>();

        if (GlobalMenuList == null)
            GlobalMenuList = new Stack<RPGMenu>();

        ID = ++IDPool;

        PickupUIElements(); //Picks up any UI elements already in the menu container
    }
    void Start () {
        //Menu data set in TestRPGMenu
        LoadMenuData();
    }

    //Fill in MenuItems from what's already on the UI in the editor
    void PickupUIElements()
    {
        foreach (Transform trans in MenuItemBackgroundHolder.transform)
        {
            RPGMenuItem item = trans.GetComponent<RPGMenuItem>();
            Assert.IsNotNull(item);
            item.MenuOfThisItem = this;
            MenuItems.Add(item);
        }

        Log("Added " + MenuItems.Count + " new menu items to the internal collection");
    }
	
	void Update () {
        RPGMenu current = GlobalMenuList.Peek();

        if (current != this)
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
            if(MenuSections.Count > 1)
            {
                MenuSections.Pop(); //pop the current one
                LoadMenuData();
            }
            else if(GlobalMenuList.Count > 1) //We are closing an additional dialog
            {
                RPGMenu menu = GlobalMenuList.Pop();
                Log("Closing menu window " + menu.gameObject.name);
                menu.gameObject.SetActive(false);

                RPGMenu currentM = GlobalMenuList.Peek();
            }
        }

        if (selectedIndex != currentIndex)
        {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, current.MenuItems.Count - 1);
            DrawUI();
        }
    }

    /// <summary>
    /// Destroys the current menu entries UI and builds new one based on the current section
    /// </summary>
    public void LoadMenuData()
    {
        for (int i = MenuItems.Count - 1; i >= 0; i--)
        {
            Destroy(MenuItems[i].gameObject);
        }

        MenuItems.Clear();

        var currentSection = MenuSections.Peek();

        foreach (var menuItem in currentSection.MenuItems)
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

        Log("Loading menu data of menu " + this.name + " with contents of section " + currentSection.MenuName);

        selectedIndex = 0;
        DrawUI();
    }

    public void DrawUI()
    {
        Assert.IsTrue(MenuItems.Count > 0, "No items in this menu list");

        for (int i = 0; i < MenuItems.Count; i++)
        {
            if (selectedIndex == i)
                MenuItems[i].ToggleSelected(true);
            else
                MenuItems[i].ToggleSelected(false);
        }

        var currentSection = MenuSections.Peek();
        MenuTitle.text = currentSection.MenuName;
        MenuHelp.text = MenuItems[selectedIndex].HelpText;

        //if (MenuSectionSequence.Count > 0 && MenuTitle != null)
        // MenuTitle.text = MenuSectionSequence.Peek().MenuName;
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
        GlobalMenuList.Push(menu);
        menu.gameObject.SetActive(true);
        Log("Opening new window: " + menu.gameObject.name);
    }
}
