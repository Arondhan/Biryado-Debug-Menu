using System.Linq;
using System.Text.RegularExpressions;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

[BepInPlugin("local.byriado.debugmenu", "Byriado Debug Menu", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    static readonly Color Bg = new Color(0.08f, 0.09f, 0.12f, 0.97f);
    static readonly Color Card = new Color(0.15f, 0.17f, 0.22f, 1f);
    static readonly Color Accent = new Color(0.95f, 0.65f, 0.15f, 1f);

    bool open, cursorWasVisible;
    Rect win = new Rect(20, 20, 540, 680);
    Vector2 scroll;
    string filter = "", amount = "100";
    int tab;
    ArtefactData[] all;
    GUIStyle sWin, sCard, sBtn, sTab, sTabOn, sTitle, sDesc, sField, sName;

    static Texture2D Tex(Color c)
    {
        var t = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
        t.SetPixel(0, 0, c);
        t.Apply();
        return t;
    }

    static GUIStyle Box(Color c, int pad = 8)
    {
        var s = new GUIStyle { padding = new RectOffset(pad, pad, pad, pad) };
        s.normal.background = Tex(c);
        s.normal.textColor = Color.white;
        return s;
    }

    void InitStyles()
    {
        if (sWin != null) return;
        sWin = Box(Bg, 12);
        sWin.padding.top = 30;
        sWin.normal.textColor = Accent;
        sWin.fontStyle = FontStyle.Bold;
        sWin.fontSize = 14;
        sWin.alignment = TextAnchor.UpperCenter;
        sCard = Box(Card);
        sBtn = Box(new Color(0.25f, 0.28f, 0.36f), 6);
        sBtn.alignment = TextAnchor.MiddleCenter;
        sBtn.hover.background = Tex(Accent);
        sBtn.hover.textColor = Color.black;
        sBtn.active.background = Tex(Color.white);
        sBtn.active.textColor = Color.black;
        sTab = new GUIStyle(sBtn) { fontSize = 14, fontStyle = FontStyle.Bold };
        sTabOn = new GUIStyle(sTab);
        sTabOn.normal.background = Tex(Accent);
        sTabOn.normal.textColor = Color.black;
        sTitle = new GUIStyle { fontSize = 13, fontStyle = FontStyle.Bold };
        sTitle.normal.textColor = Accent;
        sDesc = new GUIStyle { wordWrap = true, fontSize = 12 };
        sDesc.normal.textColor = new Color(0.75f, 0.78f, 0.85f);
        sName = new GUIStyle { fontSize = 14, fontStyle = FontStyle.Bold };
        sField = Box(new Color(0.05f, 0.06f, 0.08f), 5);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            open = !open;
            if (open) cursorWasVisible = Cursor.visible; else Cursor.visible = cursorWasVisible;
        }
        // The game draws its own UI cursor under our IMGUI window, so show the system one while open.
        if (open) Cursor.visible = true;
    }

    void OnGUI()
    {
        if (!open) return;
        InitStyles();
        win = GUILayout.Window(7331, win, Draw, "BIRYADO DEBUG  ·  F1", sWin);
    }

    bool Btn(string t, params GUILayoutOption[] o)
    {
        return GUILayout.Button(t, sBtn, o);
    }

    static Color Rarity(ERarety r)
    {
        switch (r)
        {
            case ERarety.Common: return new Color(0.8f, 0.8f, 0.8f);
            case ERarety.Rare: return new Color(0.35f, 0.65f, 1f);
            case ERarety.Epic: return new Color(0.75f, 0.45f, 1f);
            case ERarety.Legendary: return new Color(1f, 0.7f, 0.15f);
            default: return Color.gray;
        }
    }

    static string Clean(string s)
    {
        return Regex.Replace(s ?? "", "<[^>]+>", "");
    }

    void Draw(int id)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Run", tab == 0 ? sTabOn : sTab)) tab = 0;
        if (GUILayout.Button("Artefacts", tab == 1 ? sTabOn : sTab)) tab = 1;
        GUILayout.EndHorizontal();
        GUILayout.Space(6);

        if (tab == 0) DrawRun(); else DrawArtefacts();
        GUI.DragWindow(new Rect(0, 0, 10000, 28));
    }

    void DrawRun()
    {
        var g = GlobalVariables.Instance;

        GUILayout.BeginVertical(sCard);
        GUILayout.Label($"Round {g.round}   ·   Money {g.money}   ·   Shots {g.totalNumberShoot}   ·   Slots {g.maxVisibleArtefactCount}", sTitle);
        GUILayout.EndVertical();
        GUILayout.Space(6);

        GUILayout.BeginVertical(sCard);
        GUILayout.Label("Money", sTitle);
        GUILayout.BeginHorizontal();
        amount = GUILayout.TextField(amount, sField, GUILayout.Width(90));
        int n = int.TryParse(amount, out var v) ? v : 0;
        if (Btn("+")) g.AddMoney(n);
        if (Btn("−")) g.AddMoney(-n);
        if (Btn("Set")) g.SetMoney(n);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(6);

        GUILayout.BeginVertical(sCard);
        GUILayout.Label("Round / shots", sTitle);
        GUILayout.BeginHorizontal();
        if (Btn("Round -1")) g.AddRound(-1);
        if (Btn("Round +1")) g.AddRound(1);
        if (Btn("Shots +1")) g.AddTotalNumberShoot(1);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(6);

        GUILayout.BeginVertical(sCard);
        GUILayout.Label("Artefact slots", sTitle);
        GUILayout.BeginHorizontal();
        if (Btn("−1")) g.SetMaxVisibleArtefactCount(g.maxVisibleArtefactCount - 1);
        if (Btn("+1")) g.SetMaxVisibleArtefactCount(g.maxVisibleArtefactCount + 1);
        if (Btn("+5")) g.SetMaxVisibleArtefactCount(g.maxVisibleArtefactCount + 5);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(6);

        GUILayout.BeginVertical(sCard);
        GUILayout.Label("Save / scene", sTitle);
        GUILayout.BeginHorizontal();
        if (Btn("Save run")) RunSaveManager.Instance?.SaveCurrentState();
        if (Btn("Load run")) RunSaveManager.Instance?.LoadSavedState();
        if (Btn("Restart scene")) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GUILayout.EndHorizontal();
        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Scenes:", sDesc, GUILayout.Width(50));
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            int idx = i;
            if (Btn(idx.ToString(), GUILayout.Width(36))) SceneManager.LoadScene(idx);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(6);

        GUILayout.BeginVertical(sCard);
        GUILayout.Label($"Game speed: {Time.timeScale:0.0}x", sTitle);
        Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0f, 5f);
        if (Btn("Reset to 1x")) Time.timeScale = 1f;
        GUILayout.EndVertical();
    }

    void DrawArtefacts()
    {
        var am = FindObjectOfType<ArtefactManager>();
        if (!am) GUILayout.Label("Open a run scene: ArtefactManager not found", sDesc);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Search", sTitle, GUILayout.Width(50));
        filter = GUILayout.TextField(filter, sField);
        GUILayout.EndHorizontal();
        GUILayout.Space(4);

        if (all == null) all = Resources.LoadAll<ArtefactData>("").OrderBy(a => a.rarety).ThenBy(a => a.artefactName).ToArray();
        string f = filter.ToLower();
        scroll = GUILayout.BeginScrollView(scroll, false, true);
        foreach (var a in all.Where(a => f == "" || a.artefactName.ToLower().Contains(f) || Clean(a.description).ToLower().Contains(f)))
        {
            bool has = am && am.HasArtefactInInventory(a);
            GUILayout.BeginHorizontal(sCard);

            var r = GUILayoutUtility.GetRect(52, 52, GUILayout.Width(52), GUILayout.Height(52));
            DrawSprite(r, a.sprite);

            GUILayout.BeginVertical();
            var nameStyle = new GUIStyle(sName);
            nameStyle.normal.textColor = Rarity(a.rarety);
            GUILayout.Label($"{a.artefactName}  ·  {a.rarety}{(has ? "  ✔" : "")}", nameStyle);
            GUILayout.Label(Clean(a.description), sDesc);
            GUILayout.EndVertical();

            GUI.enabled = am && !has;
            if (GUILayout.Button("Add", sBtn, GUILayout.Width(80), GUILayout.Height(52)))
                if (!am.AddArtefact(a)) am.AddArtefact(a, true); // ponytail: slots full -> add hidden
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(3);
        }
        GUILayout.EndScrollView();
    }

    static void DrawSprite(Rect r, Sprite s)
    {
        GUI.DrawTexture(r, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 0, new Color(0.05f, 0.06f, 0.08f), 0, 0);
        if (!s || !s.texture) return;
        var t = s.texture;
        var tr = s.textureRect;
        GUI.DrawTextureWithTexCoords(new Rect(r.x + 3, r.y + 3, r.width - 6, r.height - 6), t,
            new Rect(tr.x / t.width, tr.y / t.height, tr.width / t.width, tr.height / t.height));
    }
}
