using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace ContentMod
{

    public static class ContentWindow
    {
        public enum Tab { PLAYER_TAB, ESP_TAB, WORLD_TAB, MISC_TAB, KEYBINDS_TAB }
        public static Tab tab = Tab.PLAYER_TAB;

        public static void DisplayUI(int windowID)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Player")) { tab = Tab.PLAYER_TAB; }
            if (GUILayout.Button("ESP")) { tab = Tab.ESP_TAB; }
            if (GUILayout.Button("World")) { tab = Tab.WORLD_TAB; }
            if (GUILayout.Button("Misc")) { tab = Tab.MISC_TAB; }
            if (GUILayout.Button("Keybinds")) { tab = Tab.KEYBINDS_TAB; }
            GUILayout.EndHorizontal();

            if (tab == Tab.PLAYER_TAB) { ContentPlayer.DisplayUI(); }
            if (tab == Tab.ESP_TAB) { ContentESP.DisplayUI(); }
            if (tab == Tab.WORLD_TAB) { ContentWorld.DisplayUI(); }
            if (tab == Tab.MISC_TAB) { ContentMisc.DisplayUI(); }
            if (tab == Tab.KEYBINDS_TAB) { ContentKeybinds.DisplayUI(); }

            GUI.DragWindow();
        }

        public static void DisplayUI(List<IContentModule> contentMods)
        {
            contentMods.ForEach(mod => {
                if (mod.GetGUIType() == ContentStatic.GUIType.TOGGLE)
                {
                    bool value = GUILayout.Toggle(((ContentModule<bool>) mod).GetValue(), mod.GetName());
                    ((ContentModule<bool>) mod).SetValue(value);
                }

                if (mod.GetGUIType() == ContentStatic.GUIType.SLIDER)
                {
                    ContentModule<float> fmod = (ContentModule<float>) mod;
                    GUILayout.Space(5);
                    GUILayout.Label(mod.GetName() + ": " + fmod.GetValue().ToString().Replace(",", "."));
                    float speed = GUILayout.HorizontalSlider(fmod.GetValue(), fmod.GetMinValue(), fmod.GetMaxValue());
                    fmod.SetValue(speed);
                    GUILayout.Space(5);
                }

                if (mod.GetGUIType() == ContentStatic.GUIType.BUTTON)
                {
                    if (GUILayout.Button(mod.GetName())) { mod.Execute(); }
                }

                if (mod.GetGUIType() == ContentStatic.GUIType.DROPDOWN)
                {
                    ContentModule<string> mod_dropdown = (ContentModule<string>) mod;
                    string name = mod_dropdown.GetValue().IsNullOrEmpty() ? mod.GetName() : mod.GetName() + ": " + mod_dropdown.GetValue();
                    if (GUILayout.Button(name)) { mod.SetOpened(!mod.IsOpened()); }

                    if (!mod.IsOpened()) { return; }
                    GUIDrawLine(2, 5, 2, new Color(0.3f, 0.3f, 0.3f));
                    mod.SetSrollPosition(GUILayout.BeginScrollView(mod.GetSrollPosition(), GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(100f)));

                    foreach (string itemName in mod_dropdown.GetList())
                    {
                        if (GUILayout.Button(itemName))
                        {
                            mod_dropdown.SetValue(itemName);
                            mod_dropdown.SetOpened(false);
                            ContentStatic.Save();
                        }
                    }

                    GUILayout.EndScrollView();
                    GUIDrawLine(5, 2, 2, new Color(0.3f, 0.3f, 0.3f));
                }
            });
        }

        public static void GUIDrawLine(int space_before, int space_after, int height, Color color) {
            GUILayout.Space(space_before);
            GUIStyle lineStyle = new GUIStyle();
            lineStyle.normal.background = ContentMod.MakeTexture(1, 1, color);
            GUILayout.Box("", lineStyle, GUILayout.Width(ContentMod.windowRect.width-50), GUILayout.Height(height));
            GUILayout.Space(space_after);
        }
    }
}