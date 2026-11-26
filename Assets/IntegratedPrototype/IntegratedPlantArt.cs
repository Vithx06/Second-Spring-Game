using System;
using UnityEngine;

namespace SecondSpring.IntegratedPrototype
{
    internal sealed class IntegratedPlantArt : IDisposable
    {
        private readonly Texture2D _crops;
        private readonly Texture2D _items;
        private readonly Sprite[,] _sprites = new Sprite[9, 6];

        // Artwork frames use top-left pixel coordinates in PixelFarmAtlas.
        private static readonly Rect[,] Frames =
        {
            { new Rect(43,169,125,125), new Rect(207,170,124,124), new Rect(368,170,127,124), new Rect(529,170,127,124), new Rect(692,170,126,124), new Rect(855,170,126,124) },
            { new Rect(66,382,75,69), new Rect(213,350,104,103), new Rect(362,346,139,107), new Rect(531,350,124,103), new Rect(713,398,93,54), default },
            { new Rect(54,546,101,67), new Rect(208,495,118,119), new Rect(366,490,129,124), new Rect(534,502,117,112), new Rect(706,546,102,68), default },
            { new Rect(66,699,77,73), new Rect(205,656,115,118), new Rect(363,654,134,120), new Rect(530,654,127,120), new Rect(708,702,93,70), default },
            { new Rect(59,846,81,57), new Rect(200,810,128,96), new Rect(365,810,136,96), new Rect(528,818,131,88), new Rect(699,839,106,69), default },
            { new Rect(61,973,83,70), new Rect(204,937,121,108), new Rect(363,934,135,111), new Rect(526,942,130,103), new Rect(699,983,108,62), default },
            { new Rect(62,1110,83,74), new Rect(201,1073,127,113), new Rect(363,1069,135,117), new Rect(527,1076,128,109), new Rect(699,1122,108,64), default },
            { new Rect(63,1268,77,57), new Rect(214,1225,101,102), new Rect(367,1220,118,107), new Rect(537,1234,105,93), new Rect(720,1266,70,59), default },
            { new Rect(59,1416,85,50), new Rect(207,1364,113,105), new Rect(365,1358,122,111), new Rect(528,1371,116,98), new Rect(702,1407,102,61), default }
        };

        internal IntegratedPlantArt(Texture2D crops, Texture2D items)
        {
            _crops = crops;
            _items = items;
        }

        private Rect CropRect(int row, int column)
        {
            Rect frame = Frames[row, column];
            return new Rect(frame.x, _crops.height - frame.yMax, frame.width, frame.height);
        }

        internal Sprite GetSprite(int row, int column)
        {
            if (!_crops) return null;
            var sprite = _sprites[row, column];
            if (sprite) return sprite;
            var pivot = row == 0 ? new Vector2(.5f, .5f) : new Vector2(.5f, 0);
            return _sprites[row, column] = Sprite.Create(_crops, CropRect(row, column), pivot, 128f);
        }

        internal void DrawItem(Rect destination, int crop, bool seed)
        {
            if (_items)
            {
                int cell = crop + (seed ? 0 : 8);
                float side = Mathf.Min(destination.width, destination.height);
                var square = new Rect(destination.center.x - side / 2, destination.center.y - side / 2, side, side);
                DrawFrame(square, _items, new Rect((cell % 4) / 4f, (3 - cell / 4) / 4f, .25f, .25f));
            }
            else if (_crops)
            {
                Rect frame = CropRect(crop + 1, 2);
                DrawFrame(destination, _crops, new Rect(frame.x / _crops.width, frame.y / _crops.height,
                    frame.width / _crops.width, frame.height / _crops.height));
            }
        }

        private static void DrawFrame(Rect destination, Texture2D texture, Rect uv)
        {
            Color old = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTextureWithTexCoords(destination, texture, uv, true);
            GUI.color = old;
        }

        public void Dispose()
        {
            foreach (var sprite in _sprites)
                if (sprite) UnityEngine.Object.Destroy(sprite);
        }
    }
}
