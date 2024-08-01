using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject endLevelMenu;

    private GameObject[] menus;
    private GameManager _gameManager;

    void Awake()
    {
        Instance = this;

        menus = new GameObject[] { mainMenu, pauseMenu, gameOverMenu, endLevelMenu };
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void ShowMenu(GameObject menuToShow, bool show)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(menu == menuToShow && show);
        }
    }

    public void ShowHideGameOverMenu(bool show) => ShowMenu(gameOverMenu, show);
    public void ShowHideEndLevelMenu(bool show) => ShowMenu(endLevelMenu, show);
    public void ShowHideMainMenu(bool show)
    {
        ShowMenu(mainMenu, show);
        if (show)
        {
            _gameManager.IsPaused = false;
            _gameManager.ResetGame();
        }
    }
    public void ShowHidePauseMenu(bool show)
    {
        ShowMenu(pauseMenu, show);
        _gameManager.IsPaused = show;
    }
}