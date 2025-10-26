using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField]
    private Move_Player move_Player;
    public GameObject panel, draggable;
    private Dictionary<GameObject, bool> menus = new();
    private GameObject playerInv, mainMenu, controlsMenu, activeMenu;

    // Start is called before the first frame update
    void Start()
    {
        playerInv = panel.transform.GetChild(0).gameObject;
        mainMenu = panel.transform.GetChild(1).gameObject;
        controlsMenu = panel.transform.GetChild(2).gameObject;
        menus.Add(playerInv, false);
        menus.Add(mainMenu, false);
        menus.Add(controlsMenu, false);
        activeMenu = playerInv;
    }

    private void Update()
    {
        bool isE = Input.GetKeyUp(KeyCode.E);
        bool isEsc = Input.GetKeyUp(KeyCode.Escape);

        if (isE || isEsc)
        {
            activeMenu = !panel.activeSelf && isEsc ? mainMenu : activeMenu;
            ToggleFlow();
        }
    }

    public void AddMenu(GameObject menu, bool openPlayerInv)
    {
        menu.transform.SetParent(panel.transform, false);
        menu.transform.SetAsFirstSibling();
        menus.Add(menu, openPlayerInv);
    }

    public bool HasMenu(GameObject menu)
    {
        return menus.ContainsKey(menu);
    }

    public void ToggleMenus(GameObject menu)
    {
        activeMenu = !panel.activeSelf ? menu : playerInv;
        panel.SetActive(!panel.activeSelf);
        menu.SetActive(!menu.activeSelf);
        draggable.SetActive(!draggable.activeSelf);
        if (menus[menu])
        {
            playerInv.SetActive(!playerInv.activeSelf);
        }
    }

    private void ToggleFlow()
    {
        ToggleMenus(activeMenu);
        move_Player.TogglePauseMovement();
    }

    private void SwapMenus(GameObject currentMenu, GameObject nextMenu)
    {
        activeMenu = nextMenu;
        currentMenu.SetActive(false);
        nextMenu.SetActive(true);
    }

    public void PlayGame()
    {
        ToggleFlow();
    }

    public void ViewControls()
    {
        SwapMenus(activeMenu, controlsMenu);
    }
}
