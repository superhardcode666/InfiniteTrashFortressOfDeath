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

using System;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.Items;
using Hardcode.ITFOD.Npc;
using UnityEngine;

public static class GameEvents
{
    #region static delegates

    public delegate void OnShowCombatScreen(bool showCombatScreen);
    public static OnShowCombatScreen toggleCombatScreenUI;

    public delegate void OnTriggerCombat(Actor enemy);
    public static OnTriggerCombat onCombatTriggered;

    public delegate void OnClearSpawnPosition(Vector3 position);
    public static OnClearSpawnPosition onClearSpawnPosition;

    public delegate void OnAddSpawnPosition(Vector3 position);
    public static OnAddSpawnPosition onAddSpawnPosition;

    public delegate void OnLootDrop(Vector3 position, Item lootDrop);
    public static OnLootDrop onLootDropped;

    public delegate void OnPlayerItemConsumed(int amount);
    public static OnPlayerItemConsumed onPlayerItemConsumed;

    public delegate void OnPlayerFeedback(string message);
    public static OnPlayerFeedback displayPlayerInfoMessage;

    public static Action<MusicModes> switchMusic;

    #endregion
}