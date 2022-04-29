using System;
using System.IO;
using QFSW.QC.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFSW.QC.Editor
{
    public class QCInspectorBase : UnityEditor.Editor
    {
        private const string ROOT_PATH = "Source";
        protected string BannerName => "Banner.png";
        protected Texture2D Banner { get; private set; }

        protected T LoadAssetInSource<T>(string assetName, string root) where T : Object
        {
            var src = MonoScript.FromScriptableObject(this);
            var srcPath = AssetDatabase.GetAssetPath(src);
            var dirPath = Path.GetDirectoryName(srcPath);
            var pathParts = dirPath.Split(new[] {root}, StringSplitOptions.None);
            var rootPath = string.Join(root, pathParts.SkipLast()) + root;
            var files = Directory.GetFiles(rootPath, assetName, SearchOption.AllDirectories);

            if (files.Length > 0)
            {
                var bannerPath = files[0];
                return AssetDatabase.LoadAssetAtPath<T>(bannerPath);
            }

            return null;
        }

        protected virtual void OnEnable()
        {
            if (!Banner) Banner = LoadAssetInSource<Texture2D>(BannerName, ROOT_PATH);
        }

        public override void OnInspectorGUI()
        {
            EditorHelpers.DrawHeader(Banner);
            base.OnInspectorGUI();
        }
    }
}