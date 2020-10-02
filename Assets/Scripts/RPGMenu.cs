using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class RPGMenuData
{
    public string MenuName;
    public List<RPGMenuItemData> MenuItems;

    public RPGMenuData(string name)
    {
        MenuName = name;
        MenuItems = new List<RPGMenuItemData>();
    }

    public void AddItem(RPGMenuItemData data)
    {
        MenuItems.Add(data);
    }
}


public class RPGMenu : MonoBehaviour {

    //prefab
    public GameObject RPGMenuItemPrefab;
    public GameObject RPGMenuItemMagicPrefab;
    public Sprite MenuItemSelectedSprite;

    public Color MenuItemBackgroundColor;
    public Color MenuItemSelectedColor;

    //Scene objects
    public GameObject MenuItemBackgroundHolder;
    public Text MenuTitle;
    public Text MenuHelp;

    public List<RPGMenuItem> MenuItemsGameObjects;

    //Indexes
    private int selectedIndex = -1;

    //Data Subsection in this single menu
    Stack<RPGMenuData> MenuSections;

    //Contains all separate RPG menus globally for navigation
    public static Stack<RPGMenu> GlobalMenuListNavigation;
    public short ID;
    private static short IDPool = -1;

    public string DEBUGMenusOnTheStack = "0";
    private void Awake()
    {
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuData>();

        if (GlobalMenuListNavigation == null)
            GlobalMenuListNavigation = new Stack<RPGMenu>();

        ID = ++IDPool;

        MenuItemsGameObjects = new List<RPGMenuItem>();
        PickupUIElements(); //Picks up any UI elements already in the menu container
    }

    //Enabled for the first time
    void Start()
    {
        if (MenuItemsGameObjects.Count > 0)
        {
            selectedIndex = 0;
            UpdateSelected();
        }
    }

    public void RefreshContent(RPGMenuData data)
    {
        //Add to current stack MenuSections
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuData>();

        MenuSections.Push(data);

        //Clear current window content
        ClearContents();

        //Add new menu items to current window
        drawCurrentMenuSection();
    }

    public void GoBackOneWindow()
    {
        if (GlobalMenuListNavigation.Count > 1)
            GlobalMenuListNavigation.Pop();

        Log("Closing menu window " + this.gameObject.name);
        Hide();
    }

    public void GoBackOneSectionInCurrentWindow()
    {
        MenuSections.Pop(); //pop the current one
        ClearContents();
        selectedIndex = 0;
        drawCurrentMenuSection();
        
    }

    public void ClearContents()
    {
        for (int i = MenuItemsGameObjects.Count - 1; i >= 0; i--)
        {
            Destroy(MenuItemsGameObjects[i].gameObject);
        }

        MenuItemsGameObjects.Clear();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    //Todo: Make more scalable and safe, save all UI?
    //Fill in MenuItems and the first section from what's already on the UI in the editor
    void PickupUIElements()
    {
        RPGMenuData currentMenu = new RPGMenuData(MenuTitle.text);
        foreach (Transform trans in MenuItemBackgroundHolder.transform)
        {
            RPGMenuItem item = trans.GetComponent<RPGMenuItem>();
            Assert.IsNotNull(item);
            item.ParentMenu = this;
            MenuItemsGameObjects.Add(item);
            currentMenu.AddItem(item.MenuItemData);
        }

        //Add to current stack MenuSections
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuData>();

        MenuSections.Push(currentMenu);
        var currentSection = MenuSections.Peek();

        Debug.Log("Picking up UI: " + MenuSections.Count);
    }
	
    void HandleInput()
    {
        int currentIndex = selectedIndex;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            selectedIndex++;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            selectedIndex--;
        if (Input.GetKeyDown(KeyCode.Return))
            MenuItemsGameObjects[selectedIndex].Invoke();

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (MenuSections.Count > 1)
            {
                GoBackOneSectionInCurrentWindow();
            }
            else if (GlobalMenuListNavigation.Count > 1) //We are closing an additional dialog
            {
                GoBackOneWindow();

            }
        }

        if (selectedIndex != currentIndex)
        {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, MenuItemsGameObjects.Count - 1);
            UpdateSelected();
        }
    }

	void Update () {

        DEBUGMenusOnTheStack = GlobalMenuListNavigation.Count.ToString();

        if (GlobalMenuListNavigation.Count > 1 && GlobalMenuListNavigation.Peek() != this)
            return;

        HandleInput();
    }

    /// <summary>
    /// Destroys the current menu entries UI and builds new one based on the current section. Don't forget to clear
    /// </summary>
    private void drawCurrentMenuSection()
    {
        var currentSection = MenuSections.Peek();

        foreach (var menuItem in currentSection.MenuItems)
        {
            AddMenuItem(menuItem);
        }

        Log("Loading menu data of menu " + this.name + " with contents of section " + currentSection.MenuName);

        selectedIndex = 0;

        MenuTitle.text = currentSection.MenuName;
        MenuHelp.text = currentSection.MenuItems[0].HelpText;

    }
    public void AddMenuItem(RPGMenuItemData itemData)
    {
        GameObject gO = Instantiate<GameObject>(RPGMenuItemPrefab, MenuItemBackgroundHolder.transform, false);
        RPGMenuItem item = gO.GetComponent<RPGMenuItem>();
        item.ParentMenu = this;
        item.MenuItemData = itemData;
        //item.MenuToOpen = menuToOpen;

        this.MenuItemsGameObjects.Add(item);
        item.transform.GetChild(0).GetComponent<Text>().text = itemData.Text; //Do in Menu Item class
        //magicMenuItem.transform.GetChild(2).GetComponent<Text>().text = menuItemWrapper.MPCost.ToString();
    }

    public void UpdateSelected()
    {
        Assert.IsTrue(MenuItemsGameObjects.Count > 0, "No items in this menu list");

        for (int i = 0; i < MenuItemsGameObjects.Count; i++)
        {
            if (selectedIndex == i)
                MenuItemsGameObjects[i].ToggleSelected(true);
            else
                MenuItemsGameObjects[i].ToggleSelected(false);
        }

        var currentSection = MenuSections.Peek();
        MenuHelp.text = currentSection.MenuItems[selectedIndex].HelpText;
    }

    public void OpenNewMenuWindow(RPGMenu menu)
    {
        //Push the current menu also on the stack
        GlobalMenuListNavigation.Push(this);

        GlobalMenuListNavigation.Push(menu);
        menu.gameObject.SetActive(true);
        Log("Opening new window: " + menu.gameObject.name);
    }

    public void Log(string text)
    {
        Debug.Log(ID + " : " + text);
    }
}
