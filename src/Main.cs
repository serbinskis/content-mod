using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ContentMod
{
    public static class BuildInfo
    {
        public const string Name = "Content Mod";
        public const string Description = "A mod to cheat in Content Warning";
        public const string Author = "WobbyChip/ DXXNS / SnickersIZ / Akira";
        public const string Company = null;
        public const string Version = "0.0.10";
        public const string DownloadLink = null;
    }

    public class ContentMod : MelonMod
    {
        public static HarmonyLib.Harmony ContentHarmony = new HarmonyLib.Harmony("com.wobbychip.contentmod");
        public static Rect windowRect = new Rect(20, 20, 400, 400);
        public static int sceneIndex = -1;

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            sceneIndex = buildIndex;
            MelonLogger.Msg($"Scene initialized: {buildIndex} - {sceneName}");
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            ContentMisc.Load();
            ContentPlayer.Load();
            ContentESP.Load();
            ContentWorld.Load();
            ContentHarmony.PatchAll();
        }

        public override void OnUpdate()
        {
            ContentKeybinds.OnUpdate();
            ContentMisc.OnUpdate();
            ContentPlayer.OnUpdate();
            ContentESP.OnUpdate();
            ContentWorld.OnUpdate();
        }

        public override void OnGUI()
        {
            var customStyle = new GUIStyle(GUI.skin.window);
            customStyle.normal.background = MakeTexture(1, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f));
            customStyle.focused.background = MakeTexture(1, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f));
            customStyle.onNormal.background = MakeTexture(1, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f));
            customStyle.hover.background = MakeTexture(1, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f));
            customStyle.normal.textColor = Color.white;
            customStyle.focused.textColor = Color.white;
            customStyle.onNormal.textColor = Color.white;
            customStyle.hover.textColor = Color.white;

            ContentHud.DisplayUI();
            ContentESP.OnGUI();
            if (!ContentMisc.toggleMenu.GetValue()) { return; }
            windowRect = GUI.Window(0, windowRect, ContentWindow.DisplayUI, "Content Warning", customStyle);
        }

        public override void OnApplicationQuit()
        {
            ContentStatic.Save();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            ContentHarmony.UnpatchAll();
            ContentStatic.Save();
        }

        public static Texture2D MakeTexture(int width, int height, Color color)
        {
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) { pix[i] = color; }
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}