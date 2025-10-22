using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField] private GameObject currentMenu;
    private Stack<GameObject> menuStack = new Stack<GameObject>();
    //private GameObject currentMenu;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Shows a new menu canvas and hides the current one.
    /// </summary>
    public void OpenMenu(GameObject newMenu)
    {
        if (currentMenu != null)
        {
            menuStack.Push(currentMenu);
            currentMenu.SetActive(false);
        }

        newMenu.SetActive(true);
        currentMenu = newMenu;
    }

    /// <summary>
    /// Goes back to the previous menu if any exist.
    /// </summary>
    public void GoBack()
    {
        if (menuStack.Count == 0)
            return;

        currentMenu.SetActive(false);
        currentMenu = menuStack.Pop();
        currentMenu.SetActive(true);
    }

    /// <summary>
    /// Clears all history (useful when returning to main menu).
    /// </summary>
    public void ResetMenus(GameObject mainMenu)
    {
        foreach (var menu in menuStack)
            menu.SetActive(false);

        menuStack.Clear();
        currentMenu = mainMenu;
        mainMenu.SetActive(true);
    }
}
