using System;
using System.Collections.Generic;
using System.Linq;
using SecondSpring.Prototype;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SecondSpring.IntegratedPrototype
{
    public sealed partial class IntegratedPrototypeView : MonoBehaviour
    {
        [SerializeField] private IntegratedPrototypeArt _art;
        [SerializeField] private Camera _mapCamera;
        [SerializeField] private SpriteRenderer _house, _pond, _nightSky;
        [SerializeField] private SpriteRenderer[] _barns, _swimmers, _world;

        private enum Page { Farm, Pen, Warehouse, Market, House, Games, Fishing, Mole }
        private const float Width = 1280;
        private const float Height = 721;
        private static readonly Rect Toolbar = new Rect(20, 531, 1240, 170);
        private static readonly Color Ink = new Color32(53, 38, 27, 255);
        private static readonly Color Cream = new Color32(247, 224, 176, 255);
        private static readonly Color Paper = new Color32(235, 205, 151, 255);
        private static readonly Color Wood = new Color32(85, 54, 36, 255);
        private static readonly Color Leaf = new Color32(62, 91, 56, 255);
        private static readonly Color Line = new Color32(176, 133, 85, 255);
        private static readonly Color Muted = new Color32(113, 77, 51, 255);
        private static readonly string[] Tools = { "Xới đất", "Gieo", "Tưới", "Thu hoạch", "Tăng năng suất", "Chín nhanh" };
        private static readonly string[] FishArtNames = { "perch", "carp", "sturgeon" };

        private IntegratedPrototypeSession _session;
        private IntegratedPlantArt _plantArt;
        private IntegratedPlot[] _plots;
        private SpriteRenderer _warehouse, _market;
        private IntegratedScenery _scenery;
        private readonly SpriteRenderer[,] _animalSprites = new SpriteRenderer[4, 2];
        private readonly List<Sprite> _generatedSprites = new List<Sprite>();
        private readonly HashSet<IntegratedPlot> _worked = new HashSet<IntegratedPlot>();
        private AudioSource _music, _effects;
        private bool _clickSounds;
        private int _lastTrack = -1;
        private int _clockSpeed = 1;
        private Page _page;
        private IntegratedPlot _selected;
        private CropTool? _tool;
        private int _seed;
        private bool _working;
        private Vector2 _lastPointer;
        private Vector2 _origin;
        private float _scale;
        private string _fieldNotice;
        private string _panelNotice;
        private Font _interfaceFont;
        private GUIStyle _body, _title, _small, _smallLight, _button, _light;

        private void Awake()
        {
            if (!_art) _art = Resources.Load<IntegratedPrototypeArt>("IntegratedPrototype/Artwork");
            if (!_art || !ResolveSceneReferences())
            {
                Debug.LogError("Prototype tích hợp thiếu cấu hình hình ảnh hoặc các đối tượng bản đồ.", this);
                enabled = false;
                return;
            }

            _plantArt = new IntegratedPlantArt(_art.soilAndCropAtlas, _art.seedAndProduceAtlas);
            _plots = FindObjectsByType<IntegratedPlot>().Where(plot => plot.gameObject.scene == gameObject.scene)
                .OrderBy(plot => plot.PlotName, StringComparer.Ordinal).ToArray();
            foreach (var plot in _plots) plot.Initialize();
            _scenery = gameObject.AddComponent<IntegratedScenery>();
            _scenery.Initialize(_art, _world, _nightSky);
            SetupAnimals();
            if (!_mapCamera.GetComponent<AudioListener>()) _mapCamera.gameObject.AddComponent<AudioListener>();
            _music = gameObject.AddComponent<AudioSource>();
            _music.volume = _art.musicVolume;
            _effects = gameObject.AddComponent<AudioSource>();
            _effects.volume = _art.effectVolume;
            _clickSounds = _art.enableClickSounds;
            _session = new IntegratedPrototypeSession(_plots.Select(plot => plot.Model));
            UpdateMap();
        }

        private bool ResolveSceneReferences()
        {
            var roots = gameObject.scene.GetRootGameObjects();
            if (!_mapCamera)
                _mapCamera = roots.SelectMany(root => root.GetComponentsInChildren<Camera>()).FirstOrDefault(camera => camera.CompareTag("MainCamera"));

            if (_world == null || _world.Length == 0 || Array.Exists(_world, sprite => !sprite))
            {
                string[] mapRoots = { "Phong cảnh", "Nhà và khu nuôi", "Trang trí và khu vực của Vũ" };
                _world = roots.Where(root => Array.IndexOf(mapRoots, root.name) >= 0)
                    .SelectMany(root => root.GetComponentsInChildren<SpriteRenderer>())
                    .Where(sprite => sprite.transform.parent != null && sprite.transform.parent.parent == null)
                    .ToArray();
            }
            SpriteRenderer Find(string name) => Array.Find(_world, sprite => sprite.name == name);
            if (!_house) _house = Find("Nhà chính");
            if (!_pond) _pond = Find("Ao cá");
            if (!_nightSky) _nightSky = Find("Trời đêm");
            if (_barns == null || _barns.Length != 4 || Array.Exists(_barns, barn => !barn))
                _barns = IntegratedPrototypeSession.Pens.Take(4).Select(Find).ToArray();
            if (_pond && (_swimmers == null || _swimmers.Length == 0 || Array.Exists(_swimmers, sprite => !sprite)))
                _swimmers = _pond.GetComponentsInChildren<SpriteRenderer>().Where(sprite => sprite != _pond).ToArray();
            _warehouse = Find("Nhà kho");
            _market = Find("Chợ nông sản");
            return _mapCamera && _house && _pond && _nightSky && _warehouse && _market && Array.TrueForAll(_barns, barn => barn);
        }

        private void SetupAnimals()
        {
            for (int pen = 0; pen < _barns.Length; pen++)
            {
                var barn = _barns[pen];
                var penTexture = _art.penTextures[pen];
                if (penTexture) barn.sprite = MakeSprite(penTexture, barn.sprite.bounds.size.x);
                Vector2 size = barn.sprite.bounds.size;
                for (int stage = 0; stage < 2; stage++)
                {
                    var texture = stage == 0 ? _art.adultTextures[pen] : _art.youngTextures[pen];
                    if (!texture) continue;
                    var sprite = new GameObject(stage == 0 ? "Trưởng thành" : "Con non").AddComponent<SpriteRenderer>();
                    sprite.transform.SetParent(barn.transform, false);
                    sprite.transform.localPosition = new Vector3(size.x * (stage == 0 ? -.25f : .06f), -size.y * .12f, 0);
                    sprite.sortingOrder = barn.sortingOrder + stage + 1;
                    sprite.sprite = MakeSprite(texture, size.x * (stage == 0 ? .2f : .13f));
                    _animalSprites[pen, stage] = sprite;
                }
            }
        }

        private Sprite MakeSprite(Texture2D texture, float worldWidth)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f), texture.width / worldWidth);
            _generatedSprites.Add(sprite);
            return sprite;
        }

        private void Update()
        {
            if (_session == null) return;
            _session.Tick(Time.deltaTime, _clockSpeed);
            _mapCamera.orthographicSize = Mathf.Max(4.705f, 8.36f / _mapCamera.aspect);
            UpdateMap();
            if (!_music.isPlaying && !AudioListener.pause && _art.musicTracks.Length > 0)
            {
                int next = Random.Range(0, _art.musicTracks.Length);
                if (next == _lastTrack && _art.musicTracks.Length > 1) next = (next + 1) % _art.musicTracks.Length;
                _lastTrack = next;
                _music.clip = _art.musicTracks[next];
                _music.Play();
            }
        }

        private void UpdateMap()
        {
            float hour = (float)(_session.Clock.MinuteOfDay / 60);
            float daylight = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(5, 7, hour)) *
                (1 - Mathf.SmoothStep(0, 1, Mathf.InverseLerp(17, 19, hour)));
            Color shade = Color.Lerp(new Color(.61f, .67f, .81f), Color.white, daylight);
            shade = Color.Lerp(shade, new Color(1, .72f, .51f), .55f * 4 * daylight * (1 - daylight));
            foreach (var layer in _world) layer.color = shade;
            _nightSky.color = new Color(1, 1, 1, 1 - daylight);
            _scenery.Animate(shade, daylight);
            for (int pen = 0; pen < 4; pen++)
                for (int stage = 0; stage < 2; stage++)
                {
                    var sprite = _animalSprites[pen, stage];
                    if (!sprite) continue;
                    bool adult = stage == 0;
                    sprite.enabled = _session.Livestock.Pens[pen].Animals.Exists(animal => animal.Adult == adult);
                    sprite.color = shade;
                }
            int fish = _session.Livestock.Pens[4].Animals.Count;
            for (int i = 0; i < _swimmers.Length; i++)
            {
                var sprite = _swimmers[i];
                sprite.enabled = i < fish;
                if (!sprite.enabled) continue;
                float phase = Time.time * (.7f + .09f * i) + i * 2;
                sprite.transform.localPosition = new Vector3(.10f + .30f * Mathf.Cos(phase), .18f + .10f * Mathf.Sin(phase + i), 0);
                sprite.flipX = Mathf.Sin(phase) > 0;
                sprite.color = new Color(shade.r, shade.g, shade.b, .55f);
            }
            foreach (var plot in _plots) plot.Render(_session.Crops, _plantArt, shade);
        }

        private void OnDestroy()
        {
            foreach (var sprite in _generatedSprites) Destroy(sprite);
            _plantArt?.Dispose();
            if (_interfaceFont) Destroy(_interfaceFont);
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) StopWorking();
        }

        private void OnGUI()
        {
            if (_session == null) return;
            Styles();
            var oldMatrix = GUI.matrix;
            _scale = Mathf.Min(Screen.width / Width, Screen.height / Height);
            _origin = new Vector2((Screen.width - Width * _scale) / 2, (Screen.height - Height * _scale) / 2);
            GUI.matrix = Matrix4x4.TRS(_origin, Quaternion.identity, new Vector3(_scale, _scale, 1));
            try
            {
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                {
                    if (_page != Page.Farm) Open(Page.Farm);
                    else if (_tool.HasValue) { _tool = null; StopWorking(); }
                    else _selected = null;
                    Event.current.Use();
                }
                if (_page == Page.Farm) DrawMap();
                else DrawPanel();
                DrawHud();
                if (_page == Page.Farm && !_working) DrawInvitation();
            }
            finally { GUI.matrix = oldMatrix; }
        }

        private void DrawMap()
        {
            bool toolbar = _selected != null && !_working;
            Vector2 pointer = Event.current.mousePosition;
            bool blocked = pointer.y < 77 || (toolbar && Toolbar.Contains(pointer)) ||
                (_session.Invitations.Invitation.HasValue && InvitationArea.Contains(pointer));
            foreach (var plot in _plots)
            {
                Rect rect = MapRect(plot.Soil);
                if (plot == _selected || (!blocked && rect.Contains(pointer))) Outline(rect, Cream, 2);
                if (plot.NeedsWater)
                {
                    float y = Mathf.Sin(Time.unscaledTime * 3 + plot.transform.position.x) * 2;
                    Picture(new Rect(rect.center.x - 8, rect.y - 19 + y, 16, 19), _art.waterDrop);
                }
            }
            if (_working || !blocked) HandleMapInput(pointer);
            if (_selected != null && !_working && _page == Page.Farm) DrawToolbar();
        }

        private void HandleMapInput(Vector2 pointer)
        {
            var e = Event.current;
            if (_working)
            {
                if (e.type == EventType.MouseDrag && e.button == 0)
                {
                    Sweep(pointer);
                    e.Use();
                }
                else if (e.type == EventType.MouseUp && e.button == 0)
                {
                    if (_tool == CropTool.Yield || _tool == CropTool.Growth)
                    {
                        var target = PlotAt(pointer);
                        if (target) Work(target);
                    }
                    else Sweep(pointer);
                    StopWorking();
                    e.Use();
                }
                return;
            }

            if (e.type != EventType.MouseDown || e.button != 0) return;
            var plot = PlotAt(pointer);
            if (plot)
            {
                if (_selected != plot && !_tool.HasValue) _fieldNotice = "";
                _selected = plot;
                if (_tool.HasValue)
                {
                    _working = true;
                    _lastPointer = pointer;
                    if (_tool != CropTool.Yield && _tool != CropTool.Growth) Work(plot);
                }
                e.Use();
                return;
            }
            if (MapRect(_house).Contains(pointer)) Open(Page.House);
            else if (MapRect(_market).Contains(pointer)) OpenMarket(0);
            else if (MapRect(_warehouse).Contains(pointer)) Open(Page.Warehouse);
            else
            {
                for (int pen = 0; pen < 5; pen++)
                {
                    var rect = MapRect(pen == 4 ? _pond : _barns[pen]);
                    if (!rect.Contains(pointer) && !(NeedsFeed(pen) && FeedBadge(rect).Contains(pointer))) continue;
                    OpenPen(pen);
                    break;
                }
            }
            if (_page != Page.Farm) e.Use();
        }

        private IntegratedPlot PlotAt(Vector2 point) => Array.Find(_plots, plot => MapRect(plot.Soil).Contains(point));

        private void Sweep(Vector2 pointer)
        {
            if (_tool == CropTool.Yield || _tool == CropTool.Growth) return;
            int steps = Mathf.CeilToInt(Vector2.Distance(_lastPointer, pointer) / 6);
            for (int i = 1; i <= steps; i++)
            {
                var plot = PlotAt(Vector2.Lerp(_lastPointer, pointer, (float)i / steps));
                if (plot) Work(plot);
            }
            _lastPointer = pointer;
        }

        private void Work(IntegratedPlot plot)
        {
            if (!_worked.Add(plot)) return;
            _selected = plot;
            _fieldNotice = _session.Work(plot.Model, _tool.Value, _seed)
                ? $"Đã {Tools[(int)_tool.Value].ToLowerInvariant()} tại {plot.PlotName.ToLowerInvariant()}."
                : "Chưa thực hiện được. Kiểm tra trạng thái đất, vật phẩm và chỗ trong kho.";
        }

        private void StopWorking()
        {
            _working = false;
            _worked.Clear();
        }

        private Rect MapRect(SpriteRenderer renderer)
        {
            if (!renderer || !renderer.enabled) return Rect.zero;
            Vector3 low = _mapCamera.WorldToScreenPoint(renderer.bounds.min);
            Vector3 high = _mapCamera.WorldToScreenPoint(renderer.bounds.max);
            return new Rect((low.x - _origin.x) / _scale, (Screen.height - high.y - _origin.y) / _scale,
                (high.x - low.x) / _scale, (high.y - low.y) / _scale);
        }

        private bool NeedsFeed(int pen)
        {
            var model = _session.Livestock.Pens[pen];
            return model.Animals.Count > 0 && model.FedDay != _session.Clock.Day && _session.Clock.Hour >= Farm.StartHour(pen);
        }

        private static Rect FeedBadge(Rect pen) => new Rect(pen.xMax - 27, pen.y - 19, 33, 30);

        private void DrawToolbar()
        {
            Board(Toolbar);
            Label(new Rect(37, 542, 1100, 29), _selected.PlotName + "  ·  " + _selected.Status(), _body);
            if (Button(new Rect(1194, 542, 46, 28), "×"))
            {
                _selected = null;
                _tool = null;
                return;
            }
            for (int i = 0; i < _session.Crops.Length; i++)
            {
                var crop = _session.Crops[i];
                int stock = _session.Inventory.GetQuantity(crop.SeedItemId);
                var rect = new Rect(37 + i * 76, 578, 69, 78);
                bool selected = _tool == CropTool.Seed && _seed == i && stock > 0;
                if (Button(rect, "", selected, stock > 0)) { SetTool(CropTool.Seed); _seed = i; }
                _plantArt.DrawItem(new Rect(rect.x + 12, rect.y + 3, 44, 44), i, true);
                Label(new Rect(rect.x + 4, rect.y + 45, 64, 31), crop.Name + " ×" + stock, selected ? _smallLight : _small);
            }
            for (int i = 0; i < Tools.Length; i++)
            {
                var tool = (CropTool)i;
                var rect = new Rect(659 + i * 97, 578, 90, 78);
                bool selected = _tool == tool;
                if (Button(rect, "", selected)) SetTool(tool);
                Picture(new Rect(rect.x + 26, rect.y + 7, 38, 35), _art.toolIcons[i]);
                Label(new Rect(rect.x + 5, rect.y + 45, 80, 29), Tools[i], selected ? _smallLight : _small);
            }
            string help = _tool.HasValue ? "Bấm ô đất hoặc giữ chuột trái và rê qua các ô. Phân bón áp dụng cho một ô khi nhả chuột."
                : "Chọn hạt giống hoặc công cụ. Bấm Esc để bỏ chọn.";
            Label(new Rect(37, 668, 1195, 24), string.IsNullOrEmpty(_fieldNotice) ? help : _fieldNotice, _small);
        }

        private void SetTool(CropTool tool)
        {
            _tool = tool;
            _fieldNotice = "";
        }

        private void DrawHud()
        {
            Board(new Rect(20, 13, 252, 51));
            Picture(new Rect(34, 21, 30, 32), _art.calendar);
            var time = _session.Clock;
            Label(new Rect(72, 20, 185, 35), $"Ngày {time.Day:00}    ·    {time.Hour:00}:{time.Minute:00}", _body);

            Board(new Rect(995, 13, 265, 51));
            Picture(new Rect(1007, 20, 34, 34), _art.coin);
            Label(new Rect(1050, 20, 198, 35), $"{_session.Wallet.Balance:N0} xu", _body);
            bool playing = _page == Page.Fishing || _page == Page.Mole;
            if (Button(new Rect(289, 17, 152, 43), "Nhà chính", _page == Page.House, !playing)) Open(Page.House);
            if (Button(new Rect(449, 17, 128, 43), "Kho", _page == Page.Warehouse, !playing)) Open(Page.Warehouse);
            if (Button(new Rect(585, 17, 128, 43), "Chợ", _page == Page.Market, !playing) && _page != Page.Market) OpenMarket(0);
            if (Button(new Rect(721, 17, 256, 43), "Trò chơi nhỏ", _page == Page.Games, !playing)) Open(Page.Games);

            if (_page != Page.Farm || _working) return;
            for (int pen = 0; pen < 5; pen++)
            {
                if (!NeedsFeed(pen)) continue;
                Rect rect = FeedBadge(MapRect(pen == 4 ? _pond : _barns[pen]));
                if (_selected != null && Toolbar.Overlaps(rect)) continue;
                Board(rect);
                Picture(rect, _art.feedTextures[_session.Livestock.FeedType(pen)]);
            }
        }

        private void Open(Page page)
        {
            if ((_page == Page.Mole || _page == Page.Fishing) && _page != page) _session.CancelGame();
            _page = page;
            _panelNotice = "";
            _scroll = Vector2.zero;
            StopWorking();
        }

        private void Styles()
        {
            if (_body != null) return;
            _interfaceFont = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "DejaVu Sans", "Liberation Sans" }, 19);
            _body = new GUIStyle(GUI.skin.label)
            {
                font = _interfaceFont ? _interfaceFont : GUI.skin.font,
                fontSize = 19,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };
            _body.normal.textColor = Ink;
            _title = new GUIStyle(_body) { fontSize = 30, fontStyle = FontStyle.Bold };
            _small = new GUIStyle(_body) { fontSize = 14 };
            _small.normal.textColor = Muted;
            _smallLight = new GUIStyle(_small);
            _smallLight.normal.textColor = Cream;
            _button = new GUIStyle(_body) { alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Bold };
            _light = new GUIStyle(_button);
            _light.normal.textColor = Cream;
        }

        private bool Button(Rect rect, string text, bool primary = false, bool enabled = true)
        {
            bool old = GUI.enabled;
            GUI.enabled = old && enabled;
            bool pressed = GUI.Button(rect, GUIContent.none, GUIStyle.none);
            GUI.enabled = old;
            bool hover = enabled && rect.Contains(Event.current.mousePosition);
            Color inner = !enabled ? new Color32(211, 189, 147, 255) : primary ? Leaf : hover ? Cream : Paper;
            Fill(rect, Wood);
            Fill(new Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4), inner);
            Label(rect, text, primary && enabled ? _light : _button);
            if (pressed && _clickSounds && _art.clickSound) _effects.PlayOneShot(_art.clickSound);
            return pressed;
        }

        private static void Board(Rect rect)
        {
            Fill(new Rect(rect.x + 3, rect.y + 4, rect.width, rect.height), new Color(.1f, .07f, .04f, .55f));
            Fill(rect, Wood);
            Fill(new Rect(rect.x + 4, rect.y + 4, rect.width - 8, rect.height - 8), Paper);
            Outline(new Rect(rect.x + 8, rect.y + 8, rect.width - 16, rect.height - 16), Line, 1);
        }
        private static void Outline(Rect r, Color color, float thickness)
        {
            Fill(new Rect(r.x, r.y, r.width, thickness), color);
            Fill(new Rect(r.x, r.yMax - thickness, r.width, thickness), color);
            Fill(new Rect(r.x, r.y, thickness, r.height), color);
            Fill(new Rect(r.xMax - thickness, r.y, thickness, r.height), color);
        }
        private static void Fill(Rect rect, Color color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = old;
        }
        private static void Label(Rect rect, string text, GUIStyle style) => GUI.Label(rect, text, style);
        private static void Picture(Rect rect, Texture2D texture)
        {
            if (texture) GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
        }
    }
}
