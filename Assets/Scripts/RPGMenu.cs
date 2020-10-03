using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

[System.Serializable]
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

    public List<RPGMenuItem> MenuItemsGO;
    public bool ClearPanelContent = false;

    //Indexes
    private int selectedIndex = -1;

    //Data Subsection in this single menu
    public Stack<RPGMenuData> MenuSections;

    //Contains all separate RPG menus globally for navigation
    public static Stack<RPGMenu> GlobalMenuListNavigation;
    public short ID;
    private static short IDPool = -1;

    public string dbgGlobalStackCount = "0";
    public string dbgSectionCount = "0";
    public string dbgSelectedIndex = "0";

    public static int MenuCountExisting = 0;
    public static int MenuCountActive = 0;

    public List<GameObject> WindowsOpenAtTheSameTime = new List<GameObject>();

    private void Awake()
    {
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuData>();

        if (GlobalMenuListNavigation == null)
            GlobalMenuListNavigation = new Stack<RPGMenu>();

        ID = ++IDPool;
        MenuCountExisting++;

        MenuItemsGO = new List<RPGMenuItem>();

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
        if (MenuItemsGO.Count > 0)
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

        //if (MenuItemsGO.Count > 0)
            handleInput();
    }

    /*********** PUBLIC METHODS RELATED TO NAVIGATION *************/
    public void ClearContentsAndSections()
    {
        for (int i = MenuItemsGO.Count - 1; i >= 0; i--)
        {
            Destroy(MenuItemsGO[i].gameObject);
        }

        MenuItemsGO.Clear();
        MenuSections.Clear();
    }


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
        removeGameObjects();
        selectedIndex = 0;
        drawCurrentMenuSection();

        StartCoroutine(waitDrawSelected());
    }

    /*********** PUBLIC METHODS RELATED TO CONTENT AND DATA ************/
    public void AddMenuItem(RPGMenuItemData itemData)
    {
        GameObject gO = Instantiate<GameObject>(RPGMenuItemPrefab, MenuItemBackgroundHolder.transform, false);
        gO.name = itemData.Text;
        RPGMenuItem item = gO.GetComponent<RPGMenuItem>();
        item.ParentMenu = this;
        item.MenuItemData = itemData;
        //item.MenuToOpen = menuToOpen;

        this.MenuItemsGO.Add(item);
        item.transform.GetChild(0).GetComponent<Text>().text = itemData.Text; //Set the text, can be safer
    }

    public void AddMenuItemGOOnly()
    {
        GameObject gO = Instantiate<GameObject>(RPGMenuItemPrefab, MenuItemBackgroundHolder.transform, false);
        gO.name = "New item";
        RPGMenuItem item = gO.GetComponent<RPGMenuItem>();
        item.ParentMenu = this;
        item.MenuItemData = new RPGMenuItemData("New item", "Help text for this item");

        this.MenuItemsGO.Add(item);
        item.transform.GetChild(0).GetComponent<Text>().text = "New item"; //Set the text, can be safer
    }

    public void OpenNewSection(RPGMenuData data)
    {
        //Add to current stack MenuSections
        if (MenuSections == null)
            MenuSections = new Stack<RPGMenuData>();

        MenuSections.Push(data);

        //Clear current window content
        removeGameObjects();

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
        RPGMenuData currentMenu = new RPGMenuData(MenuTitle.text);
        foreach (Transform trans in MenuItemBackgroundHolder.transform)
        {
            RPGMenuItem item = trans.GetComponent<RPGMenuItem>();
            if(item != null)
            {
                item.ParentMenu = this;
                MenuItemsGO.Add(item);
                currentMenu.AddItem(item.MenuItemData);
            }
        }

        if(currentMenu.MenuItems.Count >= 0)
        {
            //Add to current stack MenuSections
            if (MenuSections == null)
                MenuSections = new Stack<RPGMenuData>();

            MenuSections.Push(currentMenu);
            Debug.Log("Picking up UI: " + this.gameObject.name + " and creating new section");
        }
    }

    IEnumerator waitDrawSelected()
    {
        yield return new WaitForEndOfFrame();
        updateSelected();
    }

    private void rawRemoveUIElements()
    {
        foreach (Transform trans in MenuItemBackgroundHolder.transform)
        {
            GameObject.Destroy(trans.gameObject);
        }
    }

    [ExecuteInEditMode]
    public void RawClearUIFromEditor()
    {
        for (int i = MenuItemBackgroundHolder.transform.childCount; i > 0; --i)
            DestroyImmediate(MenuItemBackgroundHolder.transform.GetChild(0).gameObject);

        /*
        var tempArray = new GameObject[parent.transform.childCount];

        for(int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = parent.transform.GetChild(i).gameObject;
        }

        foreach(var child in tempArray)
        {
            DestroyImmediate(child);
        }
         */
    }

    private void handleInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (MenuSections.Count > 1)
            {
                GoBackOneSectionInCurrentWindow();
            }
            else if (GlobalMenuListNavigation.Count > 0) //We are closing an additional dialog
            {
                CloseCurrentAndAdditionalOpenedWindows();
                //GoBackOneWindow();

            }
        }

        if (MenuItemsGO.Count == 0)
            return;

        int currentIndex = selectedIndex;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            selectedIndex++;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            selectedIndex--;
        if (Input.GetKeyDown(KeyCode.Return))
            MenuItemsGO[selectedIndex].Invoke();

        selectedIndex = Mathf.Clamp(selectedIndex, 0, MenuItemsGO.Count - 1);
        if (selectedIndex != currentIndex)
        {
            updateSelected();
        }
    }

    private void removeGameObjects()
    {
        for (int i = MenuItemsGO.Count - 1; i >= 0; i--)
        {
            Destroy(MenuItemsGO[i].gameObject);
        }

        MenuItemsGO.Clear();
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

    public void updateSelected()
    {
        Assert.IsTrue(MenuItemsGO.Count > 0, "No items in this menu list");

        for (int i = 0; i < MenuItemsGO.Count; i++)
        {
            if (selectedIndex == i)
                MenuItemsGO[i].ToggleSelected(true);
            else
                MenuItemsGO[i].ToggleSelected(false);
        }

        var currentSection = MenuSections.Peek();
        MenuHelp.text = currentSection.MenuItems[selectedIndex].HelpText;
    }

    public void Log(string text)
    {
        Debug.Log(ID + " : " + text);
    }
}
