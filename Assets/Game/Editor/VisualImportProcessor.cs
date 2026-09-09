using UnityEditor;
using UnityEngine;
using LightJam;

namespace LightJam.EditorTools
{
    /// <summary>
    /// 拖进 Art / Resources/Visuals 的图片自动导入为 Sprite，方便直接换图。
    /// </summary>
    public class VisualImportProcessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            string path = assetPath.Replace('\\', '/');
            if (!path.Contains("/Game/Resources/Visuals/") && !path.Contains("/Game/Art/"))
                return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.spritePixelsPerUnit = SpriteFactory.PixelsPerUnit;
            importer.npotScale = TextureImporterNPOTScale.None;

            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            SpriteAlignment alignment = SpriteAlignment.Center;
            if (IsBottomPivot(fileName))
                alignment = SpriteAlignment.BottomCenter;
            else if (fileName == "ground")
                alignment = SpriteAlignment.TopCenter;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMode = (int)SpriteImportMode.Single;
            settings.spriteAlignment = (int)alignment;
            settings.spritePixelsPerUnit = SpriteFactory.PixelsPerUnit;
            settings.alphaIsTransparency = true;
            importer.SetTextureSettings(settings);
        }

        static bool IsBottomPivot(string fileName)
        {
            return fileName == "player"
                   || fileName == "note"
                   || fileName == "lamp"
                   || fileName == "lamp_used"
                   || fileName == "door_closed"
                   || fileName == "door_open";
        }
    }

    [InitializeOnLoad]
    public static class GameVisualsInstaller
    {
        const string AssetPath = "Assets/Game/Resources/GameVisuals.asset";
        const string VisualsPath = "Assets/Game/Resources/Visuals";

        static GameVisualsInstaller()
        {
            EditorApplication.delayCall += EnsureDefaultAsset;
        }

        static void EnsureDefaultAsset()
        {
            if (Application.isPlaying)
                return;
            if (!AssetDatabase.IsValidFolder("Assets/Game/Resources"))
                return;

            var asset = AssetDatabase.LoadAssetAtPath<GameVisuals>(AssetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<GameVisuals>();
                AssetDatabase.CreateAsset(asset, AssetPath);
            }

            bool changed = AssignIfEmpty(asset, "backdrop", ref asset.backdrop)
                           | AssignIfEmpty(asset, "floor", ref asset.floor)
                           | AssignIfEmpty(asset, "ground", ref asset.ground)
                           | AssignIfEmpty(asset, "wall", ref asset.wall)
                           | AssignIfEmpty(asset, "player", ref asset.player)
                           | AssignIfEmpty(asset, "window", ref asset.window)
                           | AssignIfEmpty(asset, "painting", ref asset.painting)
                           | AssignIfEmpty(asset, "note", ref asset.note)
                           | AssignIfEmpty(asset, "lamp", ref asset.lamp)
                           | AssignIfEmpty(asset, "lamp_used", ref asset.lampUsed)
                           | AssignIfEmpty(asset, "door_closed", ref asset.doorClosed)
                           | AssignIfEmpty(asset, "door_open", ref asset.doorOpen);

            if (!changed)
                return;

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        static bool AssignIfEmpty(GameVisuals _, string id, ref Sprite slot)
        {
            if (slot != null)
                return false;

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{VisualsPath}/{id}.png");
            if (sprite == null)
                return false;

            slot = sprite;
            return true;
        }
    }

    public static class VisualFolderMenu
    {
        const string VisualsPath = "Assets/Game/Resources/Visuals";

        [MenuItem("LightJam/打开素材文件夹", false, 10)]
        public static void RevealVisualsFolder()
        {
            if (!AssetDatabase.IsValidFolder(VisualsPath))
            {
                Debug.LogWarning("还没有素材文件夹：" + VisualsPath);
                return;
            }

            var folder = AssetDatabase.LoadAssetAtPath<Object>(VisualsPath);
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
            EditorUtility.RevealInFinder(VisualsPath);
        }
    }
}
