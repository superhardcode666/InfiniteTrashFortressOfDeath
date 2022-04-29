using QFSW.QC.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace QFSW.QC.Demo
{
    public class RobotSpawner : MonoBehaviour
    {
        [SerializeField] private Robot robotPrefab;
        [SerializeField] private Text text;
        [SerializeField] private QuantumTheme theme;

        public int SpawnCount { [Command("demo.spawn-count")] get; private set; }

        private void Start()
        {
            UpdateText();
            SpawnRobot(3);
        }

        private void UpdateText()
        {
            if (!theme)
                text.text = $"{SpawnCount} robots spawned";
            else
                text.text = $"{SpawnCount.ToString().ColorText(theme.DefaultReturnValueColor)} robots spawned";
        }

        [Command("demo.spawn-robot", MonoTargetType.Single)]
        private void SpawnRobot(int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                SpawnCount++;
                var position = transform.position;
                position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                Instantiate(robotPrefab, position, Quaternion.identity).name = $"Robot {SpawnCount}";
            }

            UpdateText();
        }
    }
}