using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    private void Start()
    {
        GameController.singleton.onGameStateChanged += GameController_OnGameStateChanged;
        Hide();
    }

    private void GameController_OnGameStateChanged(GameController.State state)
    {
        if (state == GameController.State.GameOver)
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
