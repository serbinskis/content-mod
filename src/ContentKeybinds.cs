using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace ContentMod
{
    public class KeyBindsUpdater : MonoBehaviour
    {
        void Update()
        {
            ContentKeybinds.OnUpdate();
        }
    }

    public static class ContentKeybinds
    {
        public static Dictionary<string, IContentModule> ContentModules = new Dictionary<string, IContentModule>();
        private static string moduleBeingSet;
        private static bool moduleWasSet;
        public static Vector2 scrollPosition;

        public static void HandleInput()
        {
            if (Event.current.isKey && (moduleBeingSet != null))
            {
                IContentModule contentModule = ContentModules.Values.FirstOrDefault(e => e.GetId().Equals(moduleBeingSet));
                contentModule.SetKey((Event.current.keyCode != KeyCode.Escape) ? Event.current.keyCode : KeyCode.None);
                File.WriteAllText(ContentStatic.KEYBINDS_FILENAME, JsonConvert.SerializeObject(ContentStatic.ModuleKeys));
                moduleBeingSet = null;
                moduleWasSet = true;
            }
        }

        public static void Register(IContentModule contentModule) {
            if (contentModule.GetKey() == KeyCode.Mouse0) { return; }
            if (ContentModules.ContainsKey(contentModule.GetId())) { return; }
            ContentModules.Add(contentModule.GetId(), contentModule);
        }

        public static void OnUpdate()
        {
            if (moduleWasSet) { moduleWasSet = false; return; }

            foreach (var mod in ContentModules.Values)
            {
                if (Input.GetKeyDown(mod.GetKey())) { mod.Execute(); }
            }
        }
        
        public static void DisplayUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var mod in ContentModules.Values)
            {
                if (mod.GetGUIType() == ContentStatic.GUIType.SLIDER) { continue; }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(mod.GetKey().ToString(), GUILayout.Width(130))) { moduleBeingSet = mod.GetId(); }
                GUILayout.Label("  " + mod.GetName());
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            HandleInput();
        }
    }
}