using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MissionManager : MonoBehaviour
{
    //  [Header("Повысить Лигу = 0")]
    //  [Header("Заработать N золота = 1")]
    //  [Header("Прокачать Speed N раз = 2")]
    //  [Header("Прокачать Reload N раз = 3")]
    //  [Header("Выиграть 5 раз = 4")]
    //  [Header("Купить один скин = 5")]
    [SerializeField] private InstructionMission[] missionPanel = new InstructionMission[6];
    [SerializeField] private UnityEngine.UI.Button CloseButton;

    [Header("Стереть все JSON")]
    [SerializeField] private bool resetJSON = false;

    private void Start()
    {
        CloseButton.onClick.AddListener(() => gameObject.SetActive(false));

        if (resetJSON) PlayerPrefs.DeleteKey("MissionInfo");

        for (int i = 0; i < missionPanel.Length; i++)
            missionPanel[i].go.SetActive(false);

        if (!PlayerPrefs.HasKey("MissionInfo"))
        {
            MissionController _missions = new MissionController();
            JsonUtility.ToJson(_missions);

            for (int i = 0; i < 3; i++)//выбираем первые три задания, которые станут активными
                _missions.missions[i].activeMission = 1;

            for (int i = 0; i < _missions.missions.Length; i++)
            {
                if (_missions.missions[i].activeMission == 1)
                    missionPanel[i].go.SetActive(true);

                _missions.missions[i].complite = 0;
                _missions.missions[i].haveCount = 0;
                missionPanel[i].haveCount = 0;
                _missions.missions[i].howManyNeed = missionPanel[i].howManyNeed;
                missionPanel[i].moneyCount.text = $"{0}/{missionPanel[i].howManyNeed}";
                _missions.missions[i].moneyReward = int.Parse(missionPanel[i].moneyReward.text);
            }

            string _st = JsonUtility.ToJson(_missions);
            PlayerPrefs.SetString("MissionInfo", _st);
        }
        else
        {
            MissionController _missions = JsonUtility.FromJson<MissionController>(PlayerPrefs.GetString("MissionInfo"));
            Debug.Log(JsonUtility.ToJson(_missions));

            for (int i = 0; i < missionPanel.Length; i++)
            {
                if (_missions.missions[i].activeMission == 1)
                    missionPanel[i].go.SetActive(true);

                missionPanel[i].haveCount = _missions.missions[i].haveCount;
                missionPanel[i].howManyNeed = _missions.missions[i].howManyNeed;
                missionPanel[i].moneyReward.text = _missions.missions[i].moneyReward.ToString();
                missionPanel[i].moneyCount.text = $"{missionPanel[i].haveCount}/{missionPanel[i].howManyNeed}";

                if (_missions.missions[i].complite == 1 && _missions.missions[i].activeMission == 1)
                {
                    int number1 = int.Parse(missionPanel[i].go.name.Substring(missionPanel[i].go.name.Length - 1, 1));

                    missionPanel[i].b_complite.onClick.AddListener(() =>
                    {
                      //  MainMenuController.Instance.SetALLCoin(_missions.missions[number1].moneyReward);
                        missionPanel[number1].haveCount = 0;
                        missionPanel[number1].howManyNeed = missionPanel[number1].howManyNeed * missionPanel[number1].multiple;
                        missionPanel[number1].moneyReward.text = (_missions.missions[number1].moneyReward * missionPanel[number1].multiple).ToString();
                        missionPanel[number1].moneyCount.text = $"{missionPanel[number1].haveCount}/{missionPanel[number1].howManyNeed}";

                        _missions.missions[number1].complite = 0;
                        _missions.missions[number1].haveCount = 0;
                        _missions.missions[number1].howManyNeed = missionPanel[number1].howManyNeed;
                        _missions.missions[number1].moneyReward = int.Parse(missionPanel[number1].moneyReward.text);
                        _missions.missions[number1].activeMission = 0;

                        missionPanel[number1].go.SetActive(false);

                        int number = UnityEngine.Random.Range(0, missionPanel.Length); ;
                        while (missionPanel[number].go.activeInHierarchy)
                            number = UnityEngine.Random.Range(0, missionPanel.Length);

                        _missions.missions[number].activeMission = 1;
                        missionPanel[number].go.SetActive(true);

                        missionPanel[number1].b_complite.onClick.RemoveAllListeners();

                        string newST = JsonUtility.ToJson(_missions);
                        PlayerPrefs.SetString("MissionInfo", newST);
                    });
                }
            }
        }
    }
}

[Serializable]
public class InstructionMission
{
    public int ID;
    public GameObject go;
    public Text moneyReward;
    public Text moneyCount;
    [HideInInspector] public int haveCount;
    public int howManyNeed;
    public Button b_complite;
    public int multiple = 2;
    [HideInInspector] public int complite;
}

[Serializable]
public class MissionController
{
    public MissionInfo[] missions = new MissionInfo[6];//набор всех имеющихся миссий
}

[Serializable]
public class MissionInfo
{
    public int activeMission;
    public int moneyReward;//сколько игрок получит
    public int haveCount;//сколько игрок выполнил необходимых действия для завершения
    public int howManyNeed;//сколько действий нужно для завершения
    public int complite;// 1 - завершено 0 - не завершено
}