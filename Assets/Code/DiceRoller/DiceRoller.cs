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
namespace Hardcode.ITFOD
{
    public class DiceRoller
    {
        public int Roll(int dieAmount, int dieSides, int modifier = 0)
        {
            var result = 0;

            for (var i = 0; i < dieAmount; i++)
            {
                // +1 as we are using the int overload of Random.Range which is max exclusive
                var roll = Random.Range(dieAmount, dieSides + 1);
                result += roll;
            }

            result += modifier;
            return result;
        }
    }
}