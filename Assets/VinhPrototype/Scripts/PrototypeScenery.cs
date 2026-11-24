using UnityEngine;

public sealed class PrototypeScenery : MonoBehaviour
{
    private SpriteRenderer _sails;
    private readonly SpriteRenderer[] _clouds = new SpriteRenderer[5];
    private readonly SpriteRenderer[] _smoke = new SpriteRenderer[6];
    private readonly SpriteRenderer[] _water = new SpriteRenderer[216];
    private readonly SpriteRenderer[] _ripples = new SpriteRenderer[12];
    private Sprite _rippleSprite;
    private Texture2D _rippleTexture;
    private Vector2 _fountainSize;
    private readonly Transform[] _birds = new Transform[5];
    private Sprite _birdPixel;
    private Sprite _dayHorizon;
    private Sprite _nightHorizon;
    private float _flockStart;
    private float _nextFlock;
    private bool _flockFlying;

    public void Initialize(SpriteRenderer[] world, SpriteRenderer nightSky)
    {
        Sprite baseSprite = Resources.Load<Sprite>("Scenery/windmill_base");
        Sprite sailsSprite = Resources.Load<Sprite>("Scenery/windmill_sails");
        Sprite cloudSprite = Resources.Load<Sprite>("Scenery/cloud");
        Sprite smokeSprite = Resources.Load<Sprite>("Scenery/smoke");
        SpriteRenderer mill = null;
        SpriteRenderer horizon = null;
        SpriteRenderer workshop = null;
        SpriteRenderer fountain = null;
        foreach (SpriteRenderer item in world)
        {
            if (item && item.gameObject.name == "Cối xay gió")
                mill = item;
            if (item && item.gameObject.name == "Chân trời")
                horizon = item;
            if (item && item.gameObject.name == "Xưởng chế biến")
                workshop = item;
            if (item && item.gameObject.name == "Đài phun nước")
                fountain = item;
        }

        _dayHorizon = LoadHorizon("Scenery/horizon_clear_day", 256, 215);
        _nightHorizon = LoadHorizon("Scenery/horizon_clear_night", 256, 216);
        if (horizon && _dayHorizon)
        {
            horizon.sprite = _dayHorizon;
            JoinHorizonToGround(horizon.transform);
        }
        if (nightSky && _nightHorizon)
        {
            nightSky.sprite = _nightHorizon;
            JoinHorizonToGround(nightSky.transform);
        }

        if (mill && baseSprite && sailsSprite)
        {
            mill.sprite = baseSprite;
            _sails = CreateSprite("Cánh cối xay", mill.transform, sailsSprite, mill.sortingOrder + 1);
            _sails.transform.localPosition = new Vector3(.08f, .375f, 0);
        }
        if (cloudSprite)
        {
            for (int i = 0; i < _clouds.Length; i++)
            {
                _clouds[i] = CreateSprite("Mây bay " + (i + 1), transform, cloudSprite, 4);
                _clouds[i].transform.position = new Vector3(-7f + i * 3.5f, 4.25f - (i % 3) * .13f, 0);
                _clouds[i].transform.localScale = Vector3.one * (.55f + (i % 3) * .12f);
            }
        }
        if (workshop && smokeSprite)
        {
            for (int i = 0; i < _smoke.Length; i++)
                _smoke[i] = CreateSprite("Khói xưởng " + (i + 1), workshop.transform, smokeSprite, workshop.sortingOrder + 1);
        }
        _birdPixel = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(.5f, .5f), Texture2D.whiteTexture.width);
        if (fountain)
        {
            _fountainSize = fountain.sprite.bounds.size;
            _rippleTexture = new Texture2D(48, 24, TextureFormat.RGBA32, false);
            _rippleTexture.filterMode = FilterMode.Point;
            var pixels = new Color[48 * 24];
            for (int y = 0; y < 24; y++)
                for (int x = 0; x < 48; x++)
                {
                    float radius = new Vector2((x - 23.5f) / 22f, (y - 11.5f) / 10f).magnitude;
                    pixels[y * 48 + x] = radius > .84f && radius < 1f ? Color.white : Color.clear;
                }
            _rippleTexture.SetPixels(pixels);
            _rippleTexture.Apply();
            _rippleSprite = Sprite.Create(_rippleTexture, new Rect(0, 0, 48, 24), new Vector2(.5f, .5f), 48);
            for (int i = 0; i < _ripples.Length; i++)
                _ripples[i] = CreateSprite("Gợn nước " + (i + 1), fountain.transform, _rippleSprite, fountain.sortingOrder + 1);
            for (int i = 0; i < _water.Length; i++)
            {
                _water[i] = CreateSprite("Tia nước " + (i + 1), fountain.transform, _birdPixel, fountain.sortingOrder + 2);
            }
        }
        for (int i = 0; i < _birds.Length; i++)
        {
            var bird = new GameObject("Chim xa " + (i + 1));
            bird.transform.SetParent(transform, false);
            _birds[i] = bird.transform;
            for (int wing = 0; wing < 2; wing++)
            {
                var sr = CreateSprite("Cánh chim", bird.transform, _birdPixel, 5);
                sr.color = new Color(.17f, .23f, .28f, .7f);
                sr.transform.localPosition = new Vector3(wing == 0 ? -.045f : .045f, .015f, 0);
                sr.transform.localRotation = Quaternion.Euler(0, 0, wing == 0 ? -22f : 22f);
                sr.transform.localScale = new Vector3(.1f, .018f, 1f);
            }
            bird.SetActive(false);
        }
        _nextFlock = Time.time + 15f;
    }

    private static Sprite LoadHorizon(string path, int bottom, int height)
    {
        var texture = Resources.Load<Texture2D>(path);
        if (!texture)
            return null;
        // Hai ảnh nền có dải tranh nằm giữa khung trắng; chỉ lấy dải tranh.
        return Sprite.Create(texture, new Rect(0, bottom, texture.width, height), new Vector2(.5f, .5f), texture.width / 16.72f);
    }

    private static void JoinHorizonToGround(Transform horizon)
    {
        // Che đường nối mỏng giữa hai lớp nền mà vẫn phủ kín mép trên màn hình.
        var scale = horizon.localScale;
        scale.y *= 1.04f;
        horizon.localScale = scale;
        horizon.position += Vector3.down * .015f;
    }

    private static SpriteRenderer CreateSprite(string name, Transform parent, Sprite sprite, int order)
    {
        var renderer = new GameObject(name).AddComponent<SpriteRenderer>();
        renderer.transform.SetParent(parent, false);
        renderer.sprite = sprite;
        renderer.sortingOrder = order;
        return renderer;
    }

    public void Animate(Color shade, float daylight)
    {
        if (_sails)
        {
            _sails.transform.Rotate(0, 0, -22f * Time.deltaTime);
            _sails.color = shade;
        }
        for (int i = 0; i < _clouds.Length; i++)
        {
            if (!_clouds[i])
                continue;
            var cloud = _clouds[i];
            var position = cloud.transform.position;
            float direction = i % 2 == 0 ? 1f : -1f;
            position.x += direction * (.07f + i * .015f) * Time.deltaTime;
            if (position.x > 10f)
                position.x = -10f;
            else if (position.x < -10f)
                position.x = 10f;
            cloud.transform.position = position;
            // Mây mờ dần khi đi qua vị trí mặt trời vào ban ngày.
            float sunClearance = Mathf.InverseLerp(.8f, 2f, Mathf.Abs(position.x + 2.05f));
            float opacity = .55f * Mathf.Lerp(1f, sunClearance, daylight);
            cloud.color = new Color(shade.r, shade.g, shade.b, opacity);
        }
        for (int i = 0; i < _smoke.Length; i++)
        {
            if (!_smoke[i])
                continue;
            float progress = Mathf.Repeat(Time.time * .22f + (i % 3) / 3f, 1f);
            float chimneyX = i < 3 ? .37f : .92f;
            _smoke[i].transform.localPosition = new Vector3(chimneyX + .12f * progress, (i < 3 ? .82f : .64f) + .65f * progress, 0);
            _smoke[i].transform.localScale = Vector3.one * (.65f + .45f * progress);
            float opacity = .9f * Mathf.Sin(progress * Mathf.PI);
            _smoke[i].color = new Color(shade.r, shade.g, shade.b, opacity);
        }
        for (int i = 0; i < _water.Length; i++)
        {
            if (!_water[i])
                continue;
            int stream = i / 18;
            float angle = stream * Mathf.PI / 6f;
            float progress = Mathf.Repeat(Time.time * .8f + (i % 18) / 18f, 1f);
            float endX = Mathf.Cos(angle) * .23f;
            float endY = -.11f + Mathf.Sin(angle) * .045f;
            // Coordinates are fractions of the sprite: nozzle at (50%, 37.5%).
            float x = endX * progress;
            float y = Mathf.Lerp(.125f, endY, progress) + 1.2f * progress * (1f - progress);
            var drop = _water[i];
            drop.transform.localPosition = new Vector3(x * _fountainSize.x, y * _fountainSize.y, 0);
            Vector2 tangent = new Vector2(endX * _fountainSize.x, (endY - .125f + 1.2f * (1f - 2f * progress)) * _fountainSize.y);
            drop.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg - 90f);
            drop.transform.localScale = new Vector3(_fountainSize.x * .012f, _fountainSize.y * .035f, 1);
            drop.color = new Color(.72f * shade.r, .96f * shade.g, shade.b, .95f);
        }
        for (int i = 0; i < _ripples.Length; i++)
        {
            if (!_ripples[i]) continue;
            float angle = i * Mathf.PI / 6f;
            float progress = Mathf.Repeat(Time.time * .8f + i / 12f, 1f);
            _ripples[i].transform.localPosition = new Vector3(Mathf.Cos(angle) * .23f * _fountainSize.x, (-.11f + Mathf.Sin(angle) * .045f) * _fountainSize.y, 0);
            _ripples[i].transform.localScale = Vector3.one * _fountainSize.x * Mathf.Lerp(.035f, .15f, progress);
            _ripples[i].color = new Color(.7f * shade.r, .95f * shade.g, shade.b, .75f * (1f - progress));
        }
        if (!_flockFlying && Time.time >= _nextFlock)
        {
            _flockFlying = true;
            _flockStart = Time.time;
        }
        if (!_flockFlying)
            return;
        float elapsed = Time.time - _flockStart;
        if (elapsed > 18f)
        {
            foreach (var bird in _birds) bird.gameObject.SetActive(false);
            _flockFlying = false;
            _nextFlock = Time.time + 55f;
            return;
        }
        for (int i = 0; i < _birds.Length; i++)
        {
            _birds[i].gameObject.SetActive(true);
            _birds[i].position = new Vector3(-9f + elapsed + i * .23f, 4.22f - i * .1f + .025f * Mathf.Sin(elapsed * 3 + i), 0);
        }
    }

    private void OnDestroy()
    {
        if (_rippleSprite) Destroy(_rippleSprite);
        if (_rippleTexture) Destroy(_rippleTexture);
        if (_birdPixel)
            Destroy(_birdPixel);
        if (_dayHorizon)
            Destroy(_dayHorizon);
        if (_nightHorizon)
            Destroy(_nightHorizon);
    }
}
