using MelonLoader;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HelperFunctions;

namespace ContentMod
{
    public static class ContentPlayer
    {
        public static ContentModule<bool> infiniteHeal = new ContentModule<bool>("infiniteHeal", "Infinite Heal", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> infiniteOxygen = new ContentModule<bool>("infiniteOxygen", "Infinite Oxygen", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> infiniteStamina = new ContentModule<bool>("infiniteStamina", "Infinite Stamina", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> infiniteJump = new ContentModule<bool>("infiniteJump", "Infinite Jump", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> infinitesShockStick = new ContentModule<bool>("infinitesShockStick", "Infinite Shock Stick", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> infiniteBattery = new ContentModule<bool>("infiniteBattery", "Infinite Battery", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> infiniteCameraTime = new ContentModule<bool>("infiniteCameraTime", "Infinite Camera Time", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> antiFall = new ContentModule<bool>("antiFall", "Anti Fall", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> antiRagdoll = new ContentModule<bool>("antiRagdoll", "Anti Ragdoll", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> preventDeath = new ContentModule<bool>("preventDeath", "Prevent Death", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> ignoreWebs = new ContentModule<bool>("ignoreWebs", "Ignore Webs", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<float> movementSpeed = new ContentModule<float>("movementSpeed", "Movement Speed", 2.3f, 2.3f, 10f, KeyCode.None, null);
        public static ContentModule<float> pushForce = new ContentModule<float>("pushForce", "Push Force", 0.25f, 0.25f, 100f, KeyCode.None, null);
        public static ContentModule<string> pushPlayer = new ContentModule<string>("pushPlayer", "Push Player", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => TakeDamageAndAddForce(0f, pushForce.GetValue(), 2.5f));
        public static ContentModule<string> killPlayer = new ContentModule<string>("killPlayer", "Kill Player", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => TakeDamageAndAddForce(9999999f, 0.25f, 2.5f));
        public static ContentModule<bool> revive = new ContentModule<bool>("revive", "Revive Yourself", false, KeyCode.None, ContentStatic.GUIType.BUTTON, () => revive.SetValue(true));
        public static ContentModule<string> addMoney = new ContentModule<string>("addMoney", "Add 1000 Money (Host Only)", "", KeyCode.None, ContentStatic.GUIType.BUTTON, () => SurfaceNetworkHandler.RoomStats.AddMoney(1000));
        public static List<IContentModule> contentMods = new List<IContentModule> { infiniteHeal, infiniteOxygen, infiniteStamina, infiniteJump, infinitesShockStick, infiniteBattery, infiniteCameraTime, antiFall, antiRagdoll, preventDeath, ignoreWebs, movementSpeed, pushForce, pushPlayer, killPlayer, revive, addMoney };
        private static Vector2 scrollPosition;

        public static void Load() {
            contentMods.ForEach(mod => mod.Load());
        }

        public static void OnUpdate()
        {
            if (infiniteHeal.GetValue())
            {
                try
                {
                    if (Player.localPlayer.data.dead) { Player.localPlayer.CallRevive(); }
                    Player.localPlayer.data.health = 100f;
                }
                catch { };
            }

            if (revive.GetValue())
            {
                try
                {
                    Player.localPlayer.CallRevive();
                    Player.localPlayer.data.health = 100f;
                }
                catch { };

                revive.SetValue(!revive.GetValue());
            }

            if (infiniteOxygen.GetValue()) { try { Player.localPlayer.data.remainingOxygen = 500f; } catch { }; }
            if (infiniteStamina.GetValue()) { try { Player.localPlayer.data.currentStamina = 100f; } catch { }; }
            if (infiniteBattery.GetValue()) { try { Battery.Update(); } catch { }; }
            if (infiniteCameraTime.GetValue()) { try { Battery.Update2(); } catch { }; }
            if (preventDeath.GetValue()) { try { Player.localPlayer.data.dead = false; } catch { }; }

            if (infiniteJump.GetValue())
            {
                try { Player.localPlayer.data.sinceGrounded = 0.4f; } catch { };
                try { Player.localPlayer.data.sinceJump = 0.7f; } catch { };
            }

            if (antiRagdoll.GetValue())
            {
                try { Player.localPlayer.refs.ragdoll.force = 0f; } catch { };
                try { Player.localPlayer.refs.ragdoll.torque = 0f; } catch { };
            }

            if (antiFall.GetValue())
            {
                try { Player.localPlayer.data.fallTime = 0f; } catch { };
            }

            if (ignoreWebs.GetValue())
            {
                foreach (Web web in GameObject.FindObjectsOfType<Web>())
                {
                    web.wholeBodyFactor = 0f;
                    web.distanceFactor = 0f;
                    web.drag = 0f;
                    web.force = 0f;
                }
            }

            if (movementSpeed.GetValue() != 2.3)
            {
                foreach (PlayerController playercon in GameObject.FindObjectsOfType<PlayerController>())
                {
                    playercon.sprintMultiplier = movementSpeed.GetValue();
                }
            }

            if (infinitesShockStick.GetValue())
            {
                foreach (ShockStickTrigger shockstick in GameObject.FindObjectsOfType<ShockStickTrigger>())
                {
                    foreach (Player player in Resources.FindObjectsOfTypeAll<Player>().ToList())
                    {
                        try {
                            if (!player.ai)
                            {
                                if (player.refs.view.Controller.ToString() == Player.localPlayer.refs.view.Controller.ToString())
                                {
                                    // Add originalplayer to the ignored players list if it's not already there
                                    if (!shockstick.ignoredPlayers.Contains(player))
                                    {
                                        shockstick.ignoredPlayers.Add(player);
                                        MelonLogger.Msg("Added " + player.refs.view.Controller.ToString() + " to the ignored list");
                                    }
                                }
                                else
                                {
                                    // Remove other players from the ignored players list
                                    if (shockstick.ignoredPlayers.Contains(player))
                                    {
                                        shockstick.ignoredPlayers.Remove(player);
                                        MelonLogger.Msg("Removed " + player.refs.view.Controller.ToString() + " from the ignored list");
                                    }
                                }
                            }
                        } catch {}
                    }
                }
            }
        }

        public static void DisplayUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
            ContentWindow.DisplayUI(contentMods);
            GUILayout.EndScrollView();
        }

        public static void TakeDamageAndAddForce(float damage, float force, float fall)
        {
            RaycastHit rayHit = HelperFunctions.LineCheck(Player.localPlayer.refs.cameraPos.position, Player.localPlayer.refs.cameraPos.position + Player.localPlayer.refs.cameraPos.forward * 100f, LayerType.All, 0.5f);
            if (rayHit.collider == null) { return; }
            Player player = rayHit.collider.transform.parent.GetComponentInParent<Player>();
            if ((player == null) || (player == Player.localPlayer)) { return; }

            try {
                player.refs.view.RPC("RPCA_TakeDamageAndAddForce", RpcTarget.All, damage, Player.localPlayer.refs.cameraPos.forward * force * (10f / (player.ai ? 4f : 1f)), fall);
                player.refs.view.RPC("RPC_MakeSound", RpcTarget.All, 0);
            } catch { }
        }
    }
}
