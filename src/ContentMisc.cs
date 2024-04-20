using Photon.Pun;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;
using Zorro.Core.CLI;
using Zorro.UI;

namespace ContentMod
{
    public static class ContentMisc
    {
        public static ContentModule<bool> saveSettings = new ContentModule<bool>("saveSettings", "Save Settings", true, KeyCode.Mouse0, ContentStatic.GUIType.TOGGLE).SetShowInHud(false);
        public static ContentModule<bool> toggleMenu = new ContentModule<bool>("toggleMenu", "Toggle Menu", false, KeyCode.RightControl, ContentStatic.GUIType.TOGGLE).SetShowInHud(false);
        public static ContentModule<bool> toggleHud = new ContentModule<bool>("toggleHud", "Toggle Hud", true, KeyCode.Delete, ContentStatic.GUIType.TOGGLE).SetShowInHud(false);
        public static ContentModule<bool> introScreen = new ContentModule<bool>("introScreen", "Disable Intro Screen", true, KeyCode.Delete, ContentStatic.GUIType.TOGGLE).SetShowInHud(false);
        public static ContentModule<bool> infiniteShop = new ContentModule<bool>("infiniteShop", "Infinite Shop", false, KeyCode.Delete, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<string> requestLobbyList = new ContentModule<string>("requestLobbyList", "Request Lobby List", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SteamMatchmaking.RequestLobbyList());
        public static ContentModule<string> joinRandomLobby = new ContentModule<string>("joinRandomLobby", "Join Random Lobby", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => JoinRandomLobby());
        public static ContentModule<bool> openConsole = new ContentModule<bool>("openConsole", "Open Console", false, KeyCode.None, ContentStatic.GUIType.BUTTON, () => OpenConsole(true));
        public static ContentModule<string> closeConsole = new ContentModule<string>("closeConsole", "Close Console", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => OpenConsole(false));
        public static List<IContentModule> contentMods = new List<IContentModule> { saveSettings, toggleMenu, toggleHud, introScreen, infiniteShop, requestLobbyList, joinRandomLobby, openConsole, closeConsole };
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

        public static async void JoinRandomLobby() {
            if (ContentMod.sceneIndex != 0)
            {
                RetrievableSingleton<ConnectionStateHandler>.Instance.Disconnect();
            }

            while ((ContentMod.sceneIndex != 0) && GetPageTitleText().IsNullOrEmpty()) { await Task.Delay(1); }
            while (!IsMainMenuReady()) { await Task.Delay(1); }
            await Task.Delay(100);
            MainMenuHandler.SteamLobbyHandler.JoinRandom();
        }

        public static void OpenConsole(bool open)
        {
            foreach (DebugUIHandler item in GameObject.FindObjectsOfType<DebugUIHandler>())
            {
                if (open) { item.Show(); } else { item.Hide(); }
            }
        }

        public static bool IsMainMenuReady() {
            try
            {
                ConnectionStateMachine stateMachine = RetrievableSingleton<ConnectionStateHandler>.Instance.StateMachine;
                if (stateMachine.CurrentState.GetType() != typeof(NoneState)) { return false; }
            } catch { return false; }

            return GetPageTitleText().IsNullOrEmpty();
        }

        public static string GetPageTitleText()
        {
            try
            {
                MainMenuUIHandler UIHandler = MainMenuHandler.Instance.UIHandler;
                if (!(UIHandler is UIPageHandler)) { return null; }
                UIPage CurrentPage = MainMenuHandler.Instance.UIHandler.currentPage;
                if (!(CurrentPage is MainMenuLoadingPage)) { return null; }
                string TitleText = ((MainMenuLoadingPage)CurrentPage).TitleText.text;
                return TitleText;
            }
            catch { return null; }
        }

        [HarmonyLib.HarmonyPatch(typeof(IntroScreenAnimator), "Start")]
        private static class IntroScreenAnimator_Start
        {
            private static bool Prefix(IntroScreenAnimator __instance)
            {
                if (!introScreen.GetValue()) { return true; }
                __instance.skipping = true;
                __instance.m_animator.enabled = false;
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ShopHandler), "OnAddToCartItemClicked")]
        private static class ShopHandler_OnAddToCartItemClicked
        {
            private static bool Prefix(ShopHandler __instance, byte itemID, Dictionary<byte, ShopItem> ___m_ItemsForSaleDictionary, PhotonView ___m_PhotonView)
            {
                if (!infiniteShop.GetValue()) { return true; }
                if (!___m_ItemsForSaleDictionary.ContainsKey(itemID)) { return true; }
                ___m_PhotonView.RPC("RPCA_AddItemToCart", 0, new object[] { itemID });
                __instance.addSFX.Play(__instance.transform.position, false, 1f, null);
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ShopHandler), "OnOrderCartClicked")]
        private static class ShopHandler_OnOrderCartClicked
        {
            private static bool Prefix(ShopHandler __instance, ShoppingCart ___m_ShoppingCart, PhotonView ___m_PhotonView)
            {
                if (!infiniteShop.GetValue()) { return true; }
                if (___m_ShoppingCart.IsEmpty) { return true; }
                __instance.purchaseSFX.Play(__instance.transform.position, false, 1f, null);
                byte[] array = Enumerable.ToArray<byte>(Enumerable.Select<ShopItem, byte>(___m_ShoppingCart.Cart, delegate (ShopItem item) { ShopItem shopItem = item; return shopItem.ItemID; }));
                ___m_PhotonView.RPC("RPCA_SpawnDrone", 0, new object[] { array });
                __instance.OnClearCartClicked();
                return false;
            }
        }
    }
}
