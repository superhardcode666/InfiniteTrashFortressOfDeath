using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFSW.QC.Extras
{
    public static class UtilCommands
    {
        private static readonly Pool<StringBuilder> _builderPool = new Pool<StringBuilder>();

        [Command("get-object-info", "Finds the specified GameObject and displays its transform and component data")]
        private static string ExtractObjectInfo(GameObject target)
        {
            var builder = _builderPool.GetObject();
            builder.Clear();

            builder.AppendLine($"Extracted info for object '{target.name}'");
            builder.AppendLine("Transform data:");
            builder.AppendLine($"   - position: {target.transform.position}");
            builder.AppendLine($"   - rotation: {target.transform.localRotation}");
            builder.AppendLine($"   - scale: {target.transform.localScale}");
            if (target.transform.childCount > 0) builder.AppendLine($"   - child count: {target.transform.childCount}");
            if (target.transform.parent) builder.AppendLine($"   - parent: {target.transform.parent.name}");

            var components = target.GetComponents<Component>().OrderBy(x => x.GetType().Name).ToArray();

            if (components.Length > 0)
            {
                builder.AppendLine("Component data:");
                for (var i = 0; i < components.Length; i++)
                {
                    var componentCount = 1;
                    var componentType = components[i].GetType();
                    builder.AppendLine($"   - {componentType.Name}");
                    while (i + 1 < components.Length && components[i + 1].GetType() == componentType)
                    {
                        componentCount++;
                        i++;
                    }

                    if (componentCount > 1) builder.Append($" ({componentCount})");
                }
            }

            if (target.transform.childCount > 0)
            {
                builder.AppendLine("Children:");

                var childCount = target.transform.childCount;
                for (var i = 0; i < childCount; i++) builder.AppendLine($"   - {target.transform.GetChild(i).name}");
            }

            var info = builder.ToString();
            _builderPool.Release(builder);
            return info;
        }

        [Command("get-scene-hierarchy", "Renders the GameObject hierarchy of the currently open scenes")]
        private static string GetSceneHierarchy()
        {
            var objects = new List<GameObject>();
            var buffer = _builderPool.GetObject();
            buffer.Clear();

            var sceneCount = SceneManager.sceneCountInBuildSettings;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneByBuildIndex(i);
                if (scene.isLoaded)
                {
                    objects.Clear();
                    scene.GetRootGameObjects(objects);

                    buffer.AppendLine(scene.name);
                    GetSceneHierarchy(objects.Select(x => x.transform).ToArray(), 0, buffer, new List<bool>());
                }
            }

            var result = buffer.ToString();
            _builderPool.Release(buffer);
            return result;
        }

        private static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            for (var i = 0; i < transform.childCount; i++) yield return transform.GetChild(i);
        }

        private static void GetSceneHierarchy(IList<Transform> roots, int depth, StringBuilder buffer,
            IList<bool> drawVertical)
        {
            const char terminalSymbol = '|';
            const char verticalSplitSymbol = '|';
            const char verticalSymbol = '|';
            const char horizontalSymbol = '-';
            const int indentation = 3;

            for (var i = 0; i < roots.Count; i++)
            {
                var root = roots[i];

                for (var j = 0; j < depth; j++)
                {
                    buffer.Append(drawVertical[j] ? verticalSymbol : ' ');
                    buffer.Append(' ', indentation - 1);
                }

                var terminal = i == roots.Count - 1;
                drawVertical.Add(!terminal);

                buffer.Append(terminal ? terminalSymbol : verticalSplitSymbol);
                buffer.Append(horizontalSymbol, indentation - 1);
                buffer.AppendLine(root.name);

                GetSceneHierarchy(root.GetChildren().ToList(), depth + 1, buffer, drawVertical);
                drawVertical.RemoveAt(drawVertical.Count - 1);
            }
        }

        [Command("add-component", "Adds a component of type T to the specified GameObject")]
        private static void AddComponent<T>(GameObject target) where T : Component
        {
            target.AddComponent<T>();
        }

        [Command("destroy-component", "Destroys the component of type T on the specified GameObject")]
        private static void DestroyComponent<T>(T target) where T : Component
        {
            Object.Destroy(target);
        }

        [Command("destroy", "Destroys a GameObject")]
        private static void DestroyGO(GameObject target)
        {
            Object.Destroy(target);
        }

        [Command("instantiate", "Instantiates a GameObject")]
        private static void InstantiateGO(
            [CommandParameterDescription("The original GameObject to instantiate a copy of.")]
            GameObject original,
            [CommandParameterDescription("The position of the instantiated GameObject.")]
            Vector3 position,
            [CommandParameterDescription("The rotation of the instantiated GameObject.")]
            Quaternion rotation)
        {
            Object.Instantiate(original, position, rotation);
        }

        [Command("instantiate", "Instantiates a GameObject")]
        private static void InstantiateGO(GameObject original, Vector3 position)
        {
            Object.Instantiate(original).transform.position = position;
        }

        [Command("instantiate", "Instantiates a GameObject")]
        private static void InstantiateGO(GameObject original)
        {
            Object.Instantiate(original);
        }

        [Command("teleport", "Teleports a GameObject")]
        private static void TeleportGO(GameObject target, Vector3 position)
        {
            target.transform.position = position;
        }

        [Command("teleport-relative", "Teleports a GameObject by a relative offset to its current position")]
        private static void TeleportRelativeGO(GameObject target, Vector3 offset)
        {
            target.transform.Translate(offset);
        }

        [Command("rotate", "Rotates a GameObject")]
        private static void RotateGO(GameObject target, Quaternion rotation)
        {
            target.transform.Rotate(rotation.eulerAngles);
        }

        [Command("set-active", "Activates/deactivates a GameObject")]
        private static void SetGOActive(GameObject target, bool active)
        {
            target.SetActive(active);
        }

        [Command("set-parent", "Sets the parent of the targert transform.")]
        private static void SetGOParent(Transform target, Transform parentTarget)
        {
            target.SetParent(parentTarget);
        }

        [Command("send-message", "Calls the method named 'methodName' on every MonoBehaviour in the target GameObject")]
        private static void SendGOMessage(GameObject target, string methodName)
        {
            target.SendMessage(methodName);
        }
    }
}