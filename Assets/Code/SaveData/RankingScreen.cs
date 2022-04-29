using System;
using System.Collections;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.SAVEDATA;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/*      __  _____    ____  ____ 
       / / / /   |  / __ \/ __ \
      / /_/ / /| | / /_/ / / / /
     / __  / ___ |/ _, _/ /_/ / 
    /_/_/_/_/__|_/_/_|_/_____/_ 
      / ____/ __ \/ __ \/ ____/ 
     / /   / / / / / / / __/    
    / /___/ /_/ / /_/ / /___    
    \____/\____/_____/_____/ 
        アンフェタミンを燃料
*/

public class RankingScreen : MonoBehaviour
{
    private void Awake()
    {
        dataManager = GetComponent<DataManager>();

        if (dataManager == null) Debug.Log("Data Manager not found...");
    }

    private void Start()
    {
        TryLoadingRunData();

        if (runData != null)
            StartCounters();
        else
            Debug.Log("No RunData available for Ranking.");
    }

    private void TryLoadingRunData()
    {
        dataManager.SetPath();
        runData = dataManager.LoadData();
    }

    public void OnRestartRun()
    {
        SceneManager.LoadScene(0);
    }

    public void OnQuitGame()
    {
        Application.Quit();
    }

    private void StartCounters()
    {
        if (runData != null)
        {
            time.SetText(runData.finalTime);
            slainBy.SetText(runData.slainBy);

            StartCoroutine(ReverseCountdown(0, runData.level, level));
            StartCoroutine(ReverseCountdown(0, runData.damageTaken, damageIn));
            StartCoroutine(ReverseCountdown(0, runData.damageDealt, damageOut));
            StartCoroutine(ReverseCountdown(0, runData.kills, kills));
            StartCoroutine(ReverseCountdown(0, runData.escapes, escapes));
            StartCoroutine(ReverseCountdown(0, runData.floor, floor));
            StartCoroutine(ReverseCountdown(0, runData.steps, steps));
            StartCoroutine(ReverseCountdown(0, runData.items, items));

            //SetQuote("if you take into account the <color=red>inevitability of death</color>, winning becomes a shallow ideal, no?");
        }
        else
        {
            SetQuote("<color=red>NO RUN DATA AVAILABLE!</color>");
        }
    }

    private void SetQuote(string text)
    {
        quote.SetText(text);
    }

    private IEnumerator ReverseCountdown(int startValue, int targetValue, TextMeshProUGUI textObject)
    {
        textObject.SetText("0");
        while (startValue < targetValue)
        {
            yield return null;

            startValue += 1;
            textObject.SetText(startValue.ToString());
        }
    }
    
    public static event Action<string> playAudio;

    #region Field Declarations

    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI damageIn;
    [SerializeField] private TextMeshProUGUI damageOut;
    [SerializeField] private TextMeshProUGUI kills;
    [SerializeField] private TextMeshProUGUI escapes;
    [SerializeField] private TextMeshProUGUI floor;
    [SerializeField] private TextMeshProUGUI steps;
    [SerializeField] private TextMeshProUGUI items;

    [SerializeField] private RunData runData;
    private DataManager dataManager;

    [SerializeField] private TextMeshProUGUI quote;
    [SerializeField] private TextMeshProUGUI slainBy;

    #endregion
}