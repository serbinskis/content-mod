using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ContentMod
{
    public static class ContentHud
    {
        public static Dictionary<string, ContentModule<bool>> ContentModules = new Dictionary<string, ContentModule<bool>>();

        public static void Register(IContentModule contentModule)
        {
            if (contentModule.GetKey() == KeyCode.Mouse0) { return; }
            if (contentModule.GetGUIType() != ContentStatic.GUIType.TOGGLE) { return; }
            if (ContentModules.ContainsKey(contentModule.GetId())) { return; }
            ContentModules.Add(contentModule.GetId(), (ContentModule<bool>) contentModule);
        }

        public static void DisplayUI()
        {
            if (!ContentMisc.toggleHud.GetValue()) { return; }
            string active = string.Join(Environment.NewLine, ContentModules.Values.ToList().Where(mod => mod.GetValue() && mod.GetShowInHud()).Select(mod => mod.GetName()));
            GUI.Label(new Rect(5, 5, Screen.width, Screen.height), "Menu is on " + ContentMisc.toggleMenu.GetKey() + "\nPress " + ContentMisc.toggleHud.GetKey() + " to Remove Hud\n\n" + active);
        }
    }
}
