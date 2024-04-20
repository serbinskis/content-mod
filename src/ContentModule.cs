using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ContentMod.ContentStatic;

namespace ContentMod
{
    public interface IContentModule
    {
        string GetId();
        string GetName();
        GUIType GetGUIType();
        KeyCode GetKey();
        void SetKey(KeyCode key);
        void Execute();
        void Toggle();
        void Load();
        void Save();
        List<string> GetList();
        void SetOpened(bool opened);
        bool IsOpened();
        bool GetShowInHud();
        void SetSrollPosition(Vector2 srollPosition);
        Vector2 GetSrollPosition();

    }

    public class ContentStatic
    {
        public static string STATES_FILENAME = "states.json";
        public static string KEYBINDS_FILENAME = "keys.json";
        public static Dictionary<string, string> ModuleStates = new Dictionary<string, string>();
        public static Dictionary<string, KeyCode> ModuleKeys = new Dictionary<string, KeyCode>();
        public enum GUIType { TOGGLE, SLIDER, BUTTON, DROPDOWN }

        public static void Save()
        {
            File.WriteAllText(ContentStatic.STATES_FILENAME, JsonConvert.SerializeObject(ContentStatic.ModuleStates));
        }
    }

    public class ContentModule<T> : IContentModule
    {
        private string id;
        private string name;
        private GUIType guiType;
        private T value;
        private T minValue;
        private T maxValue;
        private KeyCode key;
        private Action action;
        private List<string> list;
        private bool opened = false;
        private Vector2 srollPosition;
        private bool showInHud = true;

        public ContentModule(string id, string name, T value, KeyCode key, GUIType guiType) : this(id, name, value, key, guiType, null) { }

        public ContentModule(string id, string name, T value, KeyCode key, GUIType guiType, Action action)
        {
            this.id = id;
            this.name = name;
            this.guiType = guiType;
            this.value = value;
            this.key = key;
            this.action = (action != null) ? action : () => Toggle();
        }

        public ContentModule(string id, string name, T value, T minValue, T maxValue, KeyCode key, Action action) : this(id, name, value, key, GUIType.SLIDER, action)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public string GetId() { return id; }

        public string GetName() { return name; }

        public GUIType GetGUIType() { return guiType; }

        public T GetValue() { return value; }

        public T GetMinValue() { return minValue; }

        public T GetMaxValue() { return maxValue; }

        public ContentModule<T> SetList(List<string> list)
        {
            this.list = list;
            return this;
        }

        public List<string> GetList()
        {
            return this.list;
        }

        public void SetOpened(bool opened)
        {
            this.opened = opened;
        }

        public bool IsOpened()
        {
            return this.opened;
        }

        public ContentModule<T> SetShowInHud(bool opened)
        {
            this.showInHud = opened;
            return this;
        }

        public bool GetShowInHud()
        {
            return this.showInHud;
        }

        public void SetSrollPosition(Vector2 srollPosition)
        {
            this.srollPosition = srollPosition;
        }

        public Vector2 GetSrollPosition()
        {
            return this.srollPosition;
        }

        public KeyCode GetKey() { return key; }

        public void SetValue(T value)
        {
            this.value = value;
            this.Save();
        }

        public void SetKey(KeyCode key)
        {
            this.key = key;
            this.Save();
        }

        public void Execute()
        {
            this.action();
            this.Save();
        }

        public void Toggle()
        {
            if (typeof(T) == typeof(bool))
            {
                this.value = (T) (object) (!((bool) (object) value));
            }
        }

        public void Load()
        {
            if (File.Exists(ContentStatic.STATES_FILENAME) && (ContentMisc.saveSettings.GetValue() || (ContentMisc.saveSettings.GetId().Equals(this.GetId()))))
            {
                Dictionary<string, string> moduleStates = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ContentStatic.STATES_FILENAME));
                if (typeof(T) == typeof(float)) { moduleStates[id] = moduleStates[id].Replace(",", "."); }
                if (moduleStates.ContainsKey(id)) { try { SetValue((T) Convert.ChangeType(moduleStates[id], typeof(T))); } catch { } }
            }

            if (File.Exists(ContentStatic.KEYBINDS_FILENAME))
            {
                Dictionary<string, KeyCode> moduleKeys = JsonConvert.DeserializeObject<Dictionary<string, KeyCode>>(File.ReadAllText(ContentStatic.KEYBINDS_FILENAME));
                if (moduleKeys.ContainsKey(id)) { try { SetKey(moduleKeys[id]); } catch { } }
            }

            ContentKeybinds.Register(this);
            ContentHud.Register(this);
        }

        public void Save()
        {
            ContentStatic.ModuleStates[id] = Convert.ToString(value);
            ContentStatic.ModuleKeys[id] = key;
        }
    }
}
