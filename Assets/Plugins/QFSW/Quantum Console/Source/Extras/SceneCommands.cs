using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace QFSW.QC.Extras
{
    public static class SceneCommands
    {
        private static async Task PollUntilAsync(int pollInterval, Func<bool> predicate)
        {
            while (!predicate()) await Task.Delay(pollInterval);
        }

        [Command("load-scene", "loads a scene by name into the game")]
        private static async Task LoadScene(string sceneName,
            [CommandParameterDescription(
                "'Single' mode replaces the current scene with the new scene, whereas 'Additive' merges them")]
            LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadMode);
            await PollUntilAsync(16, () => asyncOperation.isDone);
        }

        [Command("load-scene-index", "loads a scene by index into the game")]
        private static async Task LoadScene(int sceneIndex,
            [CommandParameterDescription(
                "'Single' mode replaces the current scene with the new scene, whereas 'Additive' merges them")]
            LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex, loadMode);
            await PollUntilAsync(16, () => asyncOperation.isDone);
        }

        [Command("unload-scene", "unloads a scene by name")]
        private static async Task UnloadScene(string sceneName)
        {
            var asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            await PollUntilAsync(16, () => asyncOperation.isDone);
        }

        [Command("unload-scene-index", "unloads a scene by index")]
        private static async Task UnloadScene(int sceneIndex)
        {
            var asyncOperation = SceneManager.UnloadSceneAsync(sceneIndex);
            await PollUntilAsync(16, () => asyncOperation.isDone);
        }

        private static IEnumerable<Scene> GetScenesInBuild()
        {
            var sceneCount = SceneManager.sceneCountInBuildSettings;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneByBuildIndex(i);
                yield return scene;
            }
        }

        [Command("all-scenes", "gets the name and index of every scene included in the build")]
        private static Dictionary<int, string> GetAllScenes()
        {
            var sceneData = new Dictionary<int, string>();
            var sceneCount = SceneManager.sceneCountInBuildSettings;
            for (var i = 0; i < sceneCount; i++)
            {
                var sceneIndex = i;
                var scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);

                sceneData.Add(sceneIndex, sceneName);
            }

            return sceneData;
        }

        [Command("loaded-scenes", "gets the name and index of every scene currently loaded")]
        private static Dictionary<int, string> GetLoadedScenes()
        {
            var loadedScenes = GetScenesInBuild().Where(x => x.isLoaded);
            var sceneData = loadedScenes.ToDictionary(x => x.buildIndex, x => x.name);
            return sceneData;
        }

        [Command("active-scene", "gets the name of the active primary scene")]
        private static string GetCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
            return scene.name;
        }

        [Command("set-active-scene", "sets the active scene to the scene with name 'sceneName'")]
        private static void SetActiveScene(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
                throw new ArgumentException($"Scene {sceneName} must be loaded before it can be set active");

            SceneManager.SetActiveScene(scene);
        }
    }
}