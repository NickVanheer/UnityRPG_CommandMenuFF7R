using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using System;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEditor;

[System.Serializable]
public class RPGMenuData
{
    public string MenuName;
    public List<RPGMenuItemData> MenuItems = new List<RPGMenuItemData>();

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

//Command menu: When selected menu items either perform an action, add a new section to the current menu or open up an entirely new menu
//TabMenu: Similar, but the menu items always stay visible
public enum RPGMenuType
{
    CommandMenu, TabMenu
}
public class RPGMenu : MonoBehaviour {

    //Prefabs
    public GameObject RPGMenuItemPrefab;
    public Sprite MenuItemSelectedSprite;

    public Color MenuItemBackgroundColor;
    public Color MenuItemSelectedColor;

    //Scene objects
    public Text MenuTitle;
    public Text MenuHelp;

    public bool ClearPanelContent = false;

    //Indexes

    //Data Subsection in this single menu
    public Stack<RPGMenuData> MenuSections;

    //Contains all separate RPG menus globally for navigation
    public static Stack<RPGMenu> GlobalMenuListNavigation;
    public short ID;
    private static short staticIDPool = -1;

    public string dbgGlobalStackCount = "0";
    public string dbgSectionCount = "0";
    public string dbgSelectedIndex = "0";
    public static string dbgCurrentInputMenu = "None";

    public static int MenuCountExisting = 0;

    public List<GameObject> WindowsOpenAtTheSameTime = new List<GameObject>();

    public RPGMenuType MenuType = RPGMenuType.CommandMenu;
    public GameObject HostWindowCommandMenuContent = null;
    public GameObject HostWindowTabControlContent = null;

    public bool ChangeTabsOnMove = false;
    public bool IsHorizontalKeyboardControl = false;

    public bool IsInitialized = false;

    private List<RPGMenuItem> menuItemsGO = new List<RPGMenuItem>();
    private int selectedIndex = -1;

    private bool isDownMotion = false;

    private void Awake()
    {
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuData>();

        if (GlobalMenuListNavigation == null)
            GlobalMenuListNavigation = new Stack<RPGMenu>();

        ID = ++staticIDPool;
        MenuCountExisting++;

        if (HostWindowCommandMenuContent == null)
            HostWindowCommandMenuContent = this.gameObject;

        if (ClearPanelContent)
            rawRemoveUIElements(); 
        else
            pickupUIElements(); //Picks up any UI elements already in the menu container, adds them to MenuItemsGO and creates a section
    }

    public void AddToGlobalStack(RPGMenu menu)
    {
        if(!GlobalMenuListNavigation.Contains(menu))
            GlobalMenuListNavigation.Push(menu);
    }

    //Enabled for the first time
    void Start()
    {
        //AddToGlobalStack(this);
        if (menuItemsGO.Count > 0)
        {
            selectedIndex = 0;
            updateSelected();
        }
    }

    public void OnEnable()
    {
        //AddToGlobalStack(this); //Messes up multiple windows
    }

    public void OnDisable()
    {
        
    }

    public void Update()
    {
        dbgGlobalStackCount = GlobalMenuListNavigation.Count.ToString();
        dbgSectionCount = MenuSections.Count.ToString();
        dbgSelectedIndex = selectedIndex.ToString();

        bool isActive = updateActiveVisual();

        if (!isActive)
            return;

        ScrollRect scrollRect = HostWindowCommandMenuContent.transform.parent.GetComponent<ScrollRect>();
        //Debug.Log(scrollRect.verticalNormalizedPosition);

        handleInput();
    }

    /*********** PUBLIC METHODS RELATED TO NAVIGATION *************/
    public void Hide()
    {
        var scaleComp = this.gameObject.GetComponent<ScaleOnClose>();
        
        if(scaleComp != null)
            scaleComp.Close();
        else
            this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void GoBackOneWindow()
    {
        if (GlobalMenuListNavigation.Count > 0)
            GlobalMenuListNavigation.Pop();

        Log("Closing menu window " + this.gameObject.name);
        Hide();
    }


    public void GoBackOneSectionInCurrentWindow()
    {
        MenuSections.Pop(); //pop the current one
        removeMenuItems();
        selectedIndex = 0;
        drawCurrentMenuSection();

        StartCoroutine(waitDrawSelected());
    }

    /*********** PUBLIC METHODS RELATED TO CONTENT AND DATA ************/
    public void AddMenuItem(RPGMenuItemData itemData)
    {
        GameObject gO = Instantiate<GameObject>(RPGMenuItemPrefab, HostWindowCommandMenuContent.transform, false);
        gO.name = itemData.Text;
        RPGMenuItem item = gO.GetComponent<RPGMenuItem>();
        item.ParentMenu = this;
        item.MenuItemData = itemData;
        //item.MenuToOpen = menuToOpen;

        this.menuItemsGO.Add(item);
        item.transform.GetChild(0).GetComponent<Text>().text = itemData.Text; //Set the text, can be safer
    }

    public void AddMenuItemGOOnly()
    {

        //GameObject gO = PrefabUtility.InstantiatePrefab(RPGMenuItemPrefab) as GameObject;
        //gO.transform.SetParent(HostWindowCommandMenuContent.transform, false);
        GameObject gO = Instantiate<GameObject>(RPGMenuItemPrefab, HostWindowCommandMenuContent.transform, false);
        gO.name = "New item";
        RPGMenuItem item = gO.GetComponent<RPGMenuItem>();
        item.ParentMenu = this;
        item.MenuItemData = new RPGMenuItemData("New item", "Help text for this item");

        this.menuItemsGO.Add(item);
        item.transform.GetChild(0).GetComponent<Text>().text = "New item"; //Set the text, can be safer
    }

    public void OpenNewSection(GameObject dynamicMenu)
    {
        if(MenuType == RPGMenuType.TabMenu)
        {
            //We simply show the menu
            if(this.HostWindowTabControlContent != null)
                this.HostWindowTabControlContent.SetActive(false);
            this.HostWindowTabControlContent = dynamicMenu;
            this.HostWindowTabControlContent.SetActive(true);
            return;
        }

        //Else: we are a regular RPG menu, clear the current menu and add this new section
        removeMenuItems();

        //Add to current stack MenuSections
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuData>();

        //The dynamicMenu parameter is a GameObject with RPGMenuSection component residing in the scene, which we need to copy over into the current menu
        RPGMenuData menuData = new RPGMenuData(dynamicMenu.name); //Todo use RPGMenuSection data

        for (int i = 0; i < dynamicMenu.transform.childCount; i++)
        {
            GameObject child = dynamicMenu.transform.GetChild(i).gameObject;
            RPGMenuItem item = child.GetComponent<RPGMenuItem>();

            if(item)
            {
                menuData.MenuItems.Add(item.MenuItemData);
            }
        }

        MenuSections.Push(menuData);

        //Add new menu items to current window
        drawCurrentMenuSection();
    }

    public void OpenGroupOfNewMenuWindows(List<GameObject> gameObjects)
    {
        AddToGlobalStack(this);
        //Only add the first one to the navigation list and link any open windows to this
        RPGMenu rpgComponent = gameObjects[0].GetComponent<RPGMenu>();
  
        foreach (var gameObject in gameObjects)
        {
            gameObject.SetActive(true);
            this.WindowsOpenAtTheSameTime.Add(gameObject);  
        }

        if (rpgComponent)
            AddToGlobalStack(rpgComponent);
        //if(rpgComponent)
        //GlobalMenuListNavigation.Push(rpgComponent);
    }

    public void CloseCurrentAndAdditionalOpenedWindows()
    {
        if (GlobalMenuListNavigation.Count > 1)
        {
            RPGMenu current = GlobalMenuListNavigation.Pop();
            RPGMenu previous = GlobalMenuListNavigation.Peek();
            Log("Closing menu window " + this.gameObject.name);
            Hide();

            if (previous.WindowsOpenAtTheSameTime.Count > 0)
            {
                foreach (var gO in previous.WindowsOpenAtTheSameTime)
                {
                    RPGMenu rpgComponent = gO.GetComponent<RPGMenu>();
                    if (rpgComponent)
                        rpgComponent.Hide();
                    else
                        gO.SetActive(false);
                }

                previous.WindowsOpenAtTheSameTime.Clear();
            }
        }
    }

    public void OpenNewMenuWindow(GameObject menuGO)
    {
        //Push the current menu also on the stack
        //GlobalMenuListNavigation.Push(this);

        RPGMenu rpgComponent = menuGO.GetComponent<RPGMenu>();
        if(rpgComponent)
            GlobalMenuListNavigation.Push(rpgComponent);
        
        menuGO.SetActive(true);

        Log("Opening new window: " + menuGO.name);
    }

    /********* Other **********/

    //Fill in MenuItems and the first section from what's already on the UI in the editor
    private void pickupUIElements()
    {
        string menuTitle = "Nameless menu";
        if (MenuTitle != null)
            menuTitle = MenuTitle.text;

        RPGMenuData currentMenu = new RPGMenuData(menuTitle);
        foreach (Transform trans in HostWindowCommandMenuContent.transform)
        {
            RPGMenuItem item = trans.GetComponent<RPGMenuItem>();
            if(item != null)
            {
                item.ParentMenu = this;
                menuItemsGO.Add(item);
                currentMenu.AddItem(item.MenuItemData);
            }
        }

        if(currentMenu.MenuItems.Count > 0)
        {
            //Add to current stack MenuSections
            if (MenuSections == null)
                MenuSections = new Stack<RPGMenuData>();

            MenuSections.Push(currentMenu);
            Debug.Log("Picking up UI: " + this.gameObject.name + " and creating new section");
        }
        else
        {
            Debug.LogError("Failed to pick up any menu items already in the menu (HostWindowCommandMenuContent). The data inside this menu might not have any RPGMenuItemData component attached?");
        }
    }

    IEnumerator waitDrawSelected()
    {
        yield return new WaitForEndOfFrame();
        updateSelected();
    }

    private void rawRemoveUIElements()
    {
        foreach (Transform trans in HostWindowCommandMenuContent.transform)
        {
            GameObject.Destroy(trans.gameObject);
        }
    }

    [ExecuteInEditMode]
    public void RawClearUIFromEditor()
    {
        for (int i = HostWindowCommandMenuContent.transform.childCount; i > 0; --i)
            DestroyImmediate(HostWindowCommandMenuContent.transform.GetChild(0).gameObject);
    }
    
    private void handleUpDownControlInput()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            selectedIndex++;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            selectedIndex--;
    }

    private void handleLeftRightControlInput()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            selectedIndex++;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            selectedIndex--;
    }

    private void handleInput()
    {
        dbgCurrentInputMenu = this.gameObject.name;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (MenuSections.Count > 1)
            {
                GoBackOneSectionInCurrentWindow();
            }
            else if (GlobalMenuListNavigation.Count > 0) //We are closing an additional dialog
            {
                CloseCurrentAndAdditionalOpenedWindows();
            }
        }

        if (menuItemsGO.Count == 0)
            return;

        int currentIndex = selectedIndex;

        if (!IsHorizontalKeyboardControl)
            handleUpDownControlInput();
        else
            handleLeftRightControlInput();

        if (Input.GetKeyDown(KeyCode.Return))
            menuItemsGO[selectedIndex].Invoke();

        selectedIndex = Mathf.Clamp(selectedIndex, 0, menuItemsGO.Count - 1);
        if (selectedIndex != currentIndex)
        {
            if (selectedIndex > currentIndex)
                isDownMotion = true;
            else
                isDownMotion = false;

            updateSelected();

            if(MenuType == RPGMenuType.TabMenu && ChangeTabsOnMove)
            {
                RPGMenuItem item = GetMenuItem(selectedIndex);
                OpenNewSection(item.MenuItemData.DynamicMenuObject);
            }
        }
    }

    public int MenuItemCount { get { return menuItemsGO.Count; } }

    public bool IsActive { get { return GlobalMenuListNavigation.Count > 0 && GlobalMenuListNavigation.Peek() == this; } }

    public List<string> GetMenuItemNames()
    {
        if (menuItemsGO.Count == 0)
            return new List<string>();
        List<string> itemNames = new List<string>();

        menuItemsGO.ForEach(item => 
        {
            if (item == null)
                return;
            RPGMenuItem menuItem = item.GetComponent<RPGMenuItem>();
            itemNames.Add(menuItem.MenuItemData.Text);
        });

        return itemNames;
    }

    private void removeMenuItems()
    {
        for (int i = menuItemsGO.Count - 1; i >= 0; i--)
        {
            Destroy(menuItemsGO[i].gameObject);
        }

        menuItemsGO.Clear();
    }

    private bool updateActiveVisual()
    {
        if (GlobalMenuListNavigation.Count > 0 && GlobalMenuListNavigation.Peek() != this)
        {
            GetComponent<CanvasGroup>().alpha = 0.5f;
            return false;
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 1.0f;
            return true;
        }
    }

    /// <summary>
    /// Builds new menu items based on the current section. Don't forget to clear
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

    public RPGMenuItem GetMenuItem(int index)
    {
        if (menuItemsGO.Count > index)
        {
            if (menuItemsGO[index] != null && menuItemsGO[index].GetComponent<RPGMenuItem>() != null)
                return menuItemsGO[index].GetComponent<RPGMenuItem>();
        }

        Debug.LogError("Trying to access a menu item index that doesn't exist. Index: " + index);
        return null;
    }

    private void updateSelected()
    {
        Assert.IsTrue(menuItemsGO.Count > 0, "No items in this menu list");

        for (int i = 0; i < menuItemsGO.Count; i++)
        {
            if (selectedIndex == i)
                menuItemsGO[i].ToggleSelected(true);
            else
                menuItemsGO[i].ToggleSelected(false);
        }

        var currentSection = MenuSections.Peek();
        MenuHelp.text = currentSection.MenuItems[selectedIndex].HelpText;

        UpdateScrollbar();
    }

    public void UpdateScrollbar()
    {
        ScrollRect scrollRect = HostWindowCommandMenuContent.transform.parent.GetComponent<ScrollRect>();

        if (scrollRect != null)
        {
            float itemHeight = (menuItemsGO[0].transform as RectTransform).rect.height;
            float scrollpanelHeight = (scrollRect.transform as RectTransform).rect.height;
            float containerHeight = (HostWindowCommandMenuContent.transform as RectTransform).rect.height;

            float elementsVisible = (float)scrollpanelHeight / itemHeight;
            float heightPerVisibleElement = 1 / elementsVisible;

            float heightPerElementContainer =  itemHeight / containerHeight; //The height also including elements outside of the visible area of the scrollview
            float normalizedDistanceFromEdge = heightPerVisibleElement * (selectedIndex + 1);

            //We're moving up and we aren't at the start of our list currently
            if (!isDownMotion && scrollRect.verticalNormalizedPosition < 0.9f)
                normalizedDistanceFromEdge = (heightPerVisibleElement * (menuItemsGO.Count + 1)) - (heightPerVisibleElement * (selectedIndex + 1));

            //The selected elemeent is out of the bounds of what's being displayed (> 1), so scroll the scrollview
            if (normalizedDistanceFromEdge > 1f)
            {
                //elementsOverCount = overflow / heightPerVisibleElement;

                float currentOffset = heightPerElementContainer * selectedIndex;
                currentOffset += 0.1f;
                
                if (currentOffset <= 0.1f)
                    currentOffset = 0;

                if (currentOffset >= 0.8f)
                    currentOffset = 1;

                scrollRect.verticalNormalizedPosition = 1 - currentOffset;
            }
        }
    }

    public void Log(string text)
    {
        Debug.Log(ID + " : " + text);
    }
}
