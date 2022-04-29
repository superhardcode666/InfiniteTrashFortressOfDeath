using System;
using UnityEngine;

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

[Serializable]
public class DeathTimer : GenericSingletonClass<DeathTimer>
{
    public void UpdateRunTimer()
    {
        if (timerActive) currentTime = currentTime + Time.deltaTime;
        runTime = TimeSpan.FromSeconds(currentTime);
        currentTimeText = FormatTimer();
    }

    public string FormatTimer()
    {
        return runTime.ToString(@"mm\:ss\:ff");
    }

    public void StopTimer()
    {
        timerActive = false;
        GetFinalTime();

        Debug.Log($"Final Time set to: {GetFinalTime()}");

        Debug.Log("<color=red>-----------------Timer stopped!-----------------</color>");
    }

    public void PauseTimer()
    {
        timerActive = false;

        Debug.Log("<color=red>-----------------Timer paused!-----------------</color>");
    }

    public void StartTimer()
    {
        if (!timerStarted)
        {
            currentTime = 0;
            timerStarted = true;
            timerActive = true;

            Debug.Log("<color=magenta>-----------------Timer resumed!-----------------</color>");

            return;
        }

        timerActive = true;

        Debug.Log("<color=yellow>-----------------Timer started!-----------------</color>");
    }

    public string GetFinalTime()
    {
        finalRunTimeText = FormatTimer();
        return finalRunTimeText;
    }

    #region Field Declarations

    public bool timerActive;
    public bool timerStarted;
    public float currentTime;

    private string currentTimeText;
    private string finalRunTimeText = "NOT SET";

    private TimeSpan runTime;

    #endregion
}