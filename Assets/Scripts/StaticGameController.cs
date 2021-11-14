//ѕомните, чем больше if в работе, тем дольше будет обрабатыватьс€ код. 
//ѕример: завершени€ кадра с массивом из 1 000 000 if-ов обрабатываетс€ примерно 6-10 сек. на хорошем мобильном устройстве, и 0,5 сек на компьютере.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class StaticGameController : MonoBehaviour
{
    private static StaticGameController instance;
    public static StaticGameController Instance => instance;

    [HideInInspector] public bool gameIsPlayed = false;

    [Header("Timer")]
    [SerializeField] private bool useTimer = false;
    [SerializeField] private Text TimeText;
    private int minuts = 0;
    private float second = 0;

    [Header("TimerGoDown (≈сли useTimer = true)")]
    [SerializeField] private bool useTimerDown = false;
    [SerializeField] private int startMinutCount = 5;

    [Header("TimerGoUp (≈сли useTimer = true)")]
    [SerializeField] private bool useTimerUp = false;
    [SerializeField] private int maximumMinutCount = 5;

    [Header("UI Menu Elements")]
    [SerializeField] private GameObject startLevelPanel;
    [SerializeField] private GameObject[] GamePlayedPanels = new GameObject[1];
    [SerializeField] private TextMeshProUGUI PlayerPointText;
    [SerializeField] private TextMeshProUGUI WarriorPointText;
    [SerializeField] private GameObject winGamePanel;
    [SerializeField] private GameObject winBestShooter;
    [SerializeField] private GameObject winGoodWork;

    [SerializeField] private GameObject loseGamePanel;

    [Header("Win/Lose Points и множитель очков")]
    private int levelNumber = 0;
    [SerializeField] private Text winPointText;
    [SerializeField] private Text losePointText;
    [SerializeField] private int pointMultiplay = 10;

    [Header("Level UI Panels")]
    [SerializeField] private TextMeshProUGUI startLevelText;
    [SerializeField] private TextMeshProUGUI winLevelText;
    [SerializeField] private TextMeshProUGUI loseLevelText;

    [Header("«апускаем при старте")]
    [SerializeField] private Animator[] gameStartAnim = new Animator[0];
    [SerializeField] private ParticleSystem[] gameStartParticle = new ParticleSystem[0];

    [Header("«апускаем при завершении")]
    [SerializeField] private Animator[] gameEndAnim = new Animator[0];
    [SerializeField] private ParticleSystem[] gameEndParticle = new ParticleSystem[0];

    [Header("ID следующего уровн€")]
    [SerializeField] private int nextLevelID = 0;

    [SerializeField] private UnityEvent gameStarted;
    [SerializeField] private UnityEvent gameEnded;
    [SerializeField] private UnityEvent gameReset;

    private int pointValue = 0;

    [Header("–ежим разработчика")]
    [SerializeField] private bool resetPlayerPrefs = false;

    private void Awake()
    {
        instance = this;

        pointValue = 0;

        if (resetPlayerPrefs)
            PlayerPrefs.SetInt("Level", 1);

        if (PlayerPrefs.HasKey("Level"))
        {
            levelNumber = PlayerPrefs.GetInt("Level");
            startLevelText.text = "Level " + levelNumber.ToString();
            winLevelText.text = "Level " + levelNumber.ToString();
            loseLevelText.text = "Level " + levelNumber.ToString();
        }
        else
        {
            levelNumber = 1;
            startLevelText.text = "Level " + levelNumber.ToString();
            winLevelText.text = "Level " + levelNumber.ToString();
            loseLevelText.text = "Level " + levelNumber.ToString();
        }

        //отключаем лишние панели и включаем только стартовую
        startLevelPanel.SetActive(true);
        for (int i = 0; i < GamePlayedPanels.Length; i++)
            GamePlayedPanels[i].SetActive(false);

        winGamePanel.SetActive(false);
        loseGamePanel.SetActive(false);
    }

    public void NextLevel()//обращаемс€ из вне (кнопка UI на панели победы)
    {
        if (PlayerPrefs.HasKey("AllPoints"))
            PlayerPrefs.SetInt("AllPoints", PlayerPrefs.GetInt("AllPoints") + pointValue);
        else
            PlayerPrefs.SetInt("AllPoints", pointValue);

        if (PlayerPrefs.HasKey("Level"))
            PlayerPrefs.SetInt("Level", levelNumber + 1);
        else
            PlayerPrefs.SetInt("Level", levelNumber);

        SceneManager.LoadScene(nextLevelID);
        //при необходимости запускаем из этой функции корутину (если например надо, что бы перед запуском нового уровн€ доигрывалась анимаци€)
    }

    public void ResetLevel()//€ чаще использую полную перезагрузку сцены в случае если игрок проиграл и хочет начать заново.
    {
        if (PlayerPrefs.HasKey("AllPoints"))
            PlayerPrefs.SetInt("AllPoints", PlayerPrefs.GetInt("AllPoints") + pointValue);
        else
            PlayerPrefs.SetInt("AllPoints", pointValue);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//может не работать, не помню как точно
    }

    public void ActivateGameUI()
    {
        for (int i = 0; i < GamePlayedPanels.Length; i++)
            GamePlayedPanels[i].SetActive(true);
    }

    public void GameStarted()
    {
        startLevelPanel.SetActive(false);

        if (useTimer)
        {
            if (useTimerDown) minuts = startMinutCount;
            if (useTimerUp) minuts = 0;
            second = 0;
            TimeText.text = $"{minuts}:00";
        }

        pointValue = 0;

        {
            gameIsPlayed = true;
            gameStarted.Invoke();

            if (gameStartAnim.Length >= 1)
                for (int i = 0; i < gameStartAnim.Length; i++)
                    gameStartAnim[i].SetTrigger("Start");

            if (gameStartParticle.Length >= 1)
                for (int i = 0; i < gameStartAnim.Length; i++)
                    gameStartParticle[i].Play();
        }
    }

    public void GameEnded()
    {
        gameIsPlayed = false;

        gameEnded.Invoke();

        if (gameEndAnim.Length >= 1)
            for (int i = 0; i < gameEndAnim.Length; i++)
                gameEndAnim[i].SetTrigger("Start");

        bool win = false;
        int playerPoint = int.Parse(PlayerPointText.text);
        int warriorPoint = int.Parse(WarriorPointText.text);

        pointValue = warriorPoint * pointMultiplay;

        if (warriorPoint >= playerPoint)
            win = true;
        else
            win = false;

        if (win)
        {
            if (gameEndParticle.Length >= 1)
                for (int i = 0; i < gameEndParticle.Length; i++)
                    gameEndParticle[i].Play();

            for (int i = 0; i < GamePlayedPanels.Length; i++)
                GamePlayedPanels[i].SetActive(false);
            winGamePanel.SetActive(true);
            if (int.Parse(WarriorPointText.text) >= 35)
            {
                winBestShooter.SetActive(true);
                winGoodWork.SetActive(false);
            }
            else
            {
                winBestShooter.SetActive(false);
                winGoodWork.SetActive(true);
            }

            winPointText.text = pointValue.ToString();
        }
        else
        {
            for (int i = 0; i < GamePlayedPanels.Length; i++)
                GamePlayedPanels[i].SetActive(false);
            loseGamePanel.SetActive(true);

            losePointText.text = pointValue.ToString();
        }
    }

    public CastelSetting SetRefCastel(CastelController castel, bool win)
    {
        CastelSetting setting = new CastelSetting();
        if (win)
        {
            CSPlayerController.Instance.FirstTargetPos(castel.gameObject.transform.position);
            setting.Text = WarriorPointText;
            setting.anim = WarriorPointText.GetComponent<Animator>();
        }
        else
        {
            setting.Text = PlayerPointText;
            setting.anim = PlayerPointText.GetComponent<Animator>();
        }

        return setting;
    }
}

[Serializable]
public class CastelSetting
{
    public TMPro.TextMeshProUGUI Text;
    public Animator anim;
}