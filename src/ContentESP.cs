using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ContentMod
{
    public static class ContentESP
    {
        public static ContentModule<bool> playerESP = new ContentModule<bool>("playerESP", "Player ESP", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> playerTracer = new ContentModule<bool>("playerTracer", "Player Tracer", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> mobESP = new ContentModule<bool>("mobESP", "Mob ESP", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> mobTracer = new ContentModule<bool>("mobTracer", "Mob Tracer", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> itemESP = new ContentModule<bool>("itemESP", "ItemESP", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static ContentModule<bool> divingBoxESP = new ContentModule<bool>("divingBoxESP", "Diving Box ESP", false, KeyCode.None, ContentStatic.GUIType.TOGGLE);
        public static List<IContentModule> contentMods = new List<IContentModule> { playerESP, playerTracer, mobESP, mobTracer, itemESP, divingBoxESP };
        private static Vector2 scrollPosition;

        public static void Load() {
            contentMods.ForEach(mod => mod.Load());
        }

        public static void OnUpdate() { }

        public static void OnGUI()
        {
            if (playerESP.GetValue())
            {
                foreach (Player player in Resources.FindObjectsOfTypeAll<Player>().ToList())
                {
                    if ((player == null) && (player.transform == null) || player.ai || player.IsLocal) { continue; }
                    Vector3? enemyBottom = null;

                    try
                    {
                        enemyBottom = player.HeadPosition();
                    }
                    catch (NullReferenceException)
                    {
                        continue;
                    }

                    if (enemyBottom == null) { return; }
                    Vector3 w2s = Camera.main.WorldToScreenPoint(enemyBottom.Value);
                    Vector3 enemyTop = enemyBottom.Value;
                    enemyTop.y += 2f;
                    Vector3 worldToScreenBottom = Camera.main.WorldToScreenPoint(enemyBottom.Value);
                    Vector3 worldToScreenTop = Camera.main.WorldToScreenPoint(enemyTop);

                    if (ESPUtils.IsOnScreen(w2s))
                    {
                        float height = Mathf.Abs(worldToScreenTop.y - worldToScreenBottom.y);
                        float x = w2s.x - height * 0.3f;
                        float y = Screen.height - worldToScreenTop.y;
                        Vector2 namePosition = new Vector2(w2s.x, Screen.height - w2s.y + 8f);
                        Vector2 hpPosition = new Vector2(x + (height / 2f) + 3f, y + 1f);
                        namePosition -= new Vector2(player.HeadPosition().x - player.HeadPosition().x, 0f);
                        hpPosition -= new Vector2(player.HeadPosition().x - player.HeadPosition().x, 0f);
                        float distance = Vector3.Distance(Camera.main.transform.position, player.HeadPosition());
                        int fontSize = Mathf.Clamp(Mathf.RoundToInt(12f / distance), 10, 20);

                        if (player.ai)
                        {
                            ESPUtils.DrawString(namePosition, player.name.Replace("(Clone)", ""), Color.red, true, fontSize, FontStyle.Bold);
                        }
                        else
                        {
                            ESPUtils.DrawString(namePosition, player.refs.view.Controller.ToString() + "\n" + "HP: " + player.data.health, Color.green, true, fontSize, FontStyle.Bold);
                            ESPUtils.DrawHealth(new Vector2(w2s.x, Screen.height - w2s.y + 22f), player.data.health, 100f, 0.5f, true);
                        }
                        if (playerTracer.GetValue()) { Render.DrawLine(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(w2s.x, Screen.height - w2s.y), Color.green, 2f); }
                    }
                }
            }

            if (mobESP.GetValue())
            {
                foreach (Bot enemy in GameObject.FindObjectsOfType<Bot>())
                {
                    if ((enemy == null) || (enemy.transform == null)) { continue; }
                    Vector3 w2s_headpos = Camera.main.WorldToScreenPoint(enemy.transform.position);
                    if (w2s_headpos.z <= 0f) { continue; }
                    Render.DrawColorString(new Vector2(w2s_headpos.x, Screen.height - w2s_headpos.y + 0.3f), enemy.name, Color.red, 12f);
                    if (mobTracer.GetValue()) { Render.DrawLine(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(w2s_headpos.x, Screen.height - w2s_headpos.y), Color.red, 2f); }
                }
            }

            if (divingBoxESP.GetValue())
            {
                foreach (UseDivingBellButton diving in GameObject.FindObjectsOfType<UseDivingBellButton>())
                {
                    if (diving == null) { continue; }
                    Vector3 playerHeadPos = diving.transform.position;
                    playerHeadPos.y += 0.2f;
                    Vector3 w2s_footpos = Camera.main.WorldToScreenPoint(diving.transform.position);
                    Vector3 w2s_headpos = Camera.main.WorldToScreenPoint(playerHeadPos);

                    if (w2s_footpos.z <= 0f) { continue; }
                    float height = w2s_headpos.y - w2s_footpos.y;
                    float widthOffset = 1f;
                    float width = height / widthOffset;

                    Render.DrawColorString(new Vector2(w2s_headpos.x, Screen.height - w2s_headpos.y + 1f), "Diving Bell", Color.blue, 12f);
                    Render.DrawBox(w2s_footpos.x - (width / 2), Screen.height - w2s_footpos.y - height, width, height, Color.blue, 2f);
                }
            }

            if (itemESP.GetValue())
            {
                foreach (ItemInstance itemInstance in GameObject.FindObjectsOfType<ItemInstance>())
                {
                    if (itemInstance == null) { continue; }
                    Item item = itemInstance.item;
                    Vector3 w2s_itempos = Camera.main.WorldToScreenPoint(itemInstance.transform.position);
                    if (w2s_itempos.z <= 0f) { continue; }
                    Render.DrawColorString(new Vector2(w2s_itempos.x, (float)Screen.height - w2s_itempos.y - 20f), item.name, Color.yellow, 12f);
                    Render.DrawBox(w2s_itempos.x - 10f, (float)Screen.height - w2s_itempos.y - 10f, 20f, 20f, Color.yellow, 2f);
                }
            }
        }

        public static void DisplayUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
            ContentWindow.DisplayUI(contentMods);
            GUILayout.EndScrollView();
        }
    }
}
