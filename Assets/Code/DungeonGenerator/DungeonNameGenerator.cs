using Hardcode.ITFOD.Game;
using TMPro;
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

public class DungeonNameGenerator : MonoBehaviour
{
    #region Start Up

    private void Awake()
    {
        _dungeonNameDisplay = GetComponent<TextMeshProUGUI>();
        GameManager.getMapFloorName += GenerateMapFloorName;

        OnCreated();
    }

    private void OnCreated()
    {
        if (freshRun)
        {
            if (_dungeonNameDisplay != null) GenerateMapFloorName(1);
            freshRun = false;
        }
    }

    #endregion

    #region Field Declarations

    [SerializeField] private bool freshRun;
    public bool FreshRun => freshRun;

    private TextMeshProUGUI _dungeonNameDisplay;

    private readonly string[] _roomNameLocations =
    {
        "Corridor", "Chapel", "Throne", "Tunnel", "Sugar Walls", "Catacomb", "Tomb", "Shame-Corner", "Booty Hole",
        "Cellar",
        "Armpits", "Labyrinth", "Grotto", "Maze", "Trove", "Void", "Campus", "Temple", "Abbatoir", "The Situation",
        "Server Room", "Mukbang", "Depths", "Kingdom", "Basement", "Shitstorm"
    };

    private readonly string[] _roomNameAdjectives =
    {
        "disgusting ", "akward", "stank-ass", "revolting", "shocking", "funemployed",
        "entitled", "dirtpilled", "drunk-double-texting", "doomscrolling", "authoritarian", "thirstposting",
        "brocrastinating", "pearl-clutching", "resolutionary", "unsolicited", "hypernormal", "dank", "moist",
        "chronic", "parasocial", "coercive"
    };

    private readonly string[] _roomNameNouns =
    {
        "Ant Picnics", "Waluigism", "Fascist Overlords", "Catholic Roulette", "toxic Masculinity",
        "Bothsideism", "Copaganda", "Animesexuals", "ADHD", "Bronies", "Rule 46", "white Fratbros",
        "Copsuckers", "backpedaling after getting Caught", "Pimp Nails", "Eggplant Emojis", "Main Character Syndrome",
        "Clout Demons", "Dudes arguing online", "Mouthfeel", "Elder Goths", "Geriatrocity", "Shitcoins",
        "Debtpression", "Cringe", "Clown Coups", "smooth Brains", "Microcoughs", "Thoughts & Prayers"
    };

    public void GenerateMapFloorName(int floor)
    {
        var x = Random.Range(0, _roomNameLocations.Length - 1);
        var y = Random.Range(0, _roomNameAdjectives.Length - 1);
        var z = Random.Range(0, _roomNameNouns.Length - 1);

        _dungeonNameDisplay.SetText(
            $"<color=red>Floor {floor}:</color> {_roomNameLocations[x]} of {_roomNameAdjectives[y]} {_roomNameNouns[z]}");
    }

    #endregion
}