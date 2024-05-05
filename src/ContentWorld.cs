using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebSocketSharp;

namespace ContentMod
{
    public static class ContentWorld
    {
        public static List<string> monsterNames = new List<string> { "Player", "Harpooner", "Puffo", "Wallo", "Streamer", "Worm", "RobotButton", "FireMonster", "CamCreep", "BlackHoleBot", "Mime", "BarnacleBall", "BigSlap", "Bombs", "Dog", "Ear", "EyeGuy", "Flicker", "Ghost", "Jello", "Knifo", "Larva", "Mouthe", "Slurper", "Snatcho", "Spider", "Zombe", "Toolkit_Fan", "Toolkit_Hammer", "Toolkit_Iron", "Toolkit_Vaccuum", "Toolkit_Whisk", "Toolkit_Wisk", "Weeping", };
        public static List<string> itemNames = ItemDatabase.Instance.lastLoadedItems.Select(item => item.name).ToList();
        public static ContentModule<string> selectItem1 = new ContentModule<string>("selectItem1", "Select Item 1", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem1 = new ContentModule<string>("spawnItem1", "Spawn Item 1", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem1.GetValue()));
        public static ContentModule<string> giveItem1 = new ContentModule<string>("giveItem1", "Give Item 1", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem1.GetValue()));

        public static ContentModule<string> selectItem2 = new ContentModule<string>("selectItem2", "Select Item 2", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem2 = new ContentModule<string>("spawnItem2", "Spawn Item 2", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem2.GetValue()));
        public static ContentModule<string> giveItem2 = new ContentModule<string>("giveItem2", "Give Item 2", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem2.GetValue()));

        public static ContentModule<string> selectItem3 = new ContentModule<string>("selectItem3", "Select Item 3", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem3 = new ContentModule<string>("spawnItem3", "Spawn Item 3", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem3.GetValue()));
        public static ContentModule<string> giveItem3 = new ContentModule<string>("giveItem3", "Give Item 3", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem3.GetValue()));

        public static ContentModule<string> selectItem4 = new ContentModule<string>("selectItem4", "Select Item 4", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem4 = new ContentModule<string>("spawnItem4", "Spawn Item 4", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem4.GetValue()));
        public static ContentModule<string> giveItem4 = new ContentModule<string>("giveItem4", "Give Item 4", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem4.GetValue()));

        public static ContentModule<string> selectItem5 = new ContentModule<string>("selectItem5", "Select Item 5", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem5 = new ContentModule<string>("spawnItem5", "Spawn Item 5", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem5.GetValue()));
        public static ContentModule<string> giveItem5 = new ContentModule<string>("giveItem5", "Give Item 5", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem5.GetValue()));

        public static ContentModule<string> selectItem6 = new ContentModule<string>("selectItem6", "Select Item 6", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem6 = new ContentModule<string>("spawnItem6", "Spawn Item 6", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem6.GetValue()));
        public static ContentModule<string> giveItem6 = new ContentModule<string>("giveItem6", "Give Item 6", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem6.GetValue()));

        public static ContentModule<string> selectItem7 = new ContentModule<string>("selectItem7", "Select Item 7", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem7 = new ContentModule<string>("spawnItem7", "Spawn Item 7", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem7.GetValue()));
        public static ContentModule<string> giveItem7 = new ContentModule<string>("giveItem7", "Give Item 7", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem7.GetValue()));

        public static ContentModule<string> selectItem8 = new ContentModule<string>("selectItem8", "Select Item 8", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem8 = new ContentModule<string>("spawnItem8", "Spawn Item 8", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem8.GetValue()));
        public static ContentModule<string> giveItem8 = new ContentModule<string>("giveItem8", "Give Item 8", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem8.GetValue()));

        public static ContentModule<string> selectItem9 = new ContentModule<string>("selectItem9", "Select Item 9", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(itemNames);
        public static ContentModule<string> spawnItem9 = new ContentModule<string>("spawnItem9", "Spawn Item 9", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnItem(selectItem9.GetValue()));
        public static ContentModule<string> giveItem9 = new ContentModule<string>("giveItem9", "Give Item 9", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => GiveItem(selectItem9.GetValue()));

        public static ContentModule<string> selectMonster = new ContentModule<string>("selectMonster", "Select Monster", "", KeyCode.Mouse0, ContentStatic.GUIType.DROPDOWN).SetList(monsterNames);
        public static ContentModule<string> spawnMonster = new ContentModule<string>("spawnMonster", "Spawn Monster", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SpawnMonster(selectMonster.GetValue()));
        public static List<IContentModule> contentMods = new List<IContentModule> { selectMonster, spawnMonster, selectItem1, spawnItem1, giveItem1, selectItem2, spawnItem2, giveItem2, selectItem3, spawnItem3, giveItem3, selectItem4, spawnItem4, giveItem4, selectItem5, spawnItem5, giveItem5, selectItem6, spawnItem6, giveItem6, selectItem7, spawnItem7, giveItem7, selectItem8, spawnItem8, giveItem8, selectItem9, spawnItem9, giveItem9 };
        private static Vector2 scrollPosition;

        public static void Load() {
            contentMods.ForEach(mod => mod.Load());
        }

        public static void OnUpdate() { }

        public static void DisplayUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
            ContentWindow.DisplayUI(contentMods);
            GUILayout.EndScrollView();
        }

        public static void SpawnItem(string itemName)
        {
            if (itemName.IsNullOrEmpty()) { return; }
            var item = ItemDatabase.Instance.lastLoadedItems.FirstOrDefault(i => i.name == itemName);
            if (item == null) { return; }
            Vector3 debugItemSpawnPos = MainCamera.instance.GetDebugItemSpawnPos();
            Player.localPlayer.RequestCreatePickup(item, new ItemInstanceData(Guid.NewGuid()), debugItemSpawnPos, Quaternion.identity);
        }

        public static void GiveItem(string itemName)
        {
            if (itemName.IsNullOrEmpty()) { return; }
            var item = ItemDatabase.Instance.lastLoadedItems.FirstOrDefault(i => i.name == itemName);
            if (item == null) { return; }
            Player.localPlayer.TryGetInventory(out var inventory);
            inventory.TryAddItem(new ItemDescriptor(item, new ItemInstanceData(Guid.NewGuid())));
        }

        public static void SpawnMonster(string monsterName)
        {
            if (monsterName.IsNullOrEmpty()) { return; }
            MonsterSpawner.SpawnMonster(monsterName);
        }
    }
}
