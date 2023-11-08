using TMPro;
using UnityEngine;

public class ScoreService
{

    private TextMeshProUGUI _scoreText;
    private GameObject _gameOverPopup;

    public int Score { get; private set; }

    public ScoreService()
    {
        _scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        _gameOverPopup = GameObject.Find("GameOverPopup");
        if (_gameOverPopup is not null)
            _gameOverPopup.SetActive(false);
    }

    public void GameOverPopup()
    {
        if (_gameOverPopup is not null)
        {
            GameObject.Find("ResetButton")?.SetActive(false);
            _gameOverPopup.SetActive(true);
        }
    }

    public void AddScore(int number = 1)
    {
        Score += number;
        if (_scoreText is not null)
            _scoreText.text = $"Score : {Score}";
    }

    public void Reset()
    {
        AddScore(-Score);
    }
}
