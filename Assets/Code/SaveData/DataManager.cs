using System.IO;
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
namespace Hardcode.ITFOD.SAVEDATA
{
    public class DataManager : MonoBehaviour
    {
        public void SetPath()
        {
            //saves to Asset Directory, for testing Purposes
            tempDataPath = Application.dataPath + Path.AltDirectorySeparatorChar + "DeathData.json";

            //use this for actual save data
            persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "DeathData.json";
        }

        public RunData LoadData()
        {
            var loadPath = persistentPath;

            try
            {
                using (var reader = new StreamReader(loadPath))
                {
                    var json = reader.ReadToEnd();
                    var data = JsonUtility.FromJson<RunData>(json);

                    Debug.Log("<color=green>DataManager:</color> RunData successfully loaded!");

                    if (data != null) return data;
                }
            }
            catch (IOException e)
            {
                Debug.Log("<color=green>DataManager:</color> no RunData found!");
            }

            return null;
        }

        public void KillExistingSaveData()
        {
            if (File.Exists(persistentPath))
            {
                Debug.Log("<color=green>DataManager:</color> previous RunData found!");
                File.Delete(persistentPath);
                Debug.Log("<color=green>DataManager:</color> and deleted!");
            }
            else
            {
                Debug.Log("<color=green>DataManager:</color> No previous RunData found!");
            }
        }

        public void SaveData(RunData data)
        {
            var savePath = persistentPath;
            var json = JsonUtility.ToJson(data);

            Debug.Log(json);

            using (var fs = new FileStream(savePath, FileMode.Create))
            {
                //FileMode.Create will make sure that if the file allready exists,
                //it is deleted and a new one create. If not, it is created normally.
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(json);
                }
            }

            Debug.Log("<color=green>DataManager:</color> RunData successfully saved!");
        }

        #region Field Declarations

        public string tempDataPath = "";
        public string persistentPath = "";

        #endregion
    }
}