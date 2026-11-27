using UnityEngine;

namespace SecondSpring.IntegratedPrototype
{
    public sealed class IntegratedPlot : MonoBehaviour
    {
        [SerializeField] private string _plotName;
        internal string PlotName => _plotName;
        internal FarmPlot Model { get; private set; }
        internal SpriteRenderer Soil { get; private set; }
        private SpriteRenderer _cropArt;
        private int _cropIndex;

        internal void Initialize()
        {
            Model = new FarmPlot(600);
            Soil = GetComponent<SpriteRenderer>();
            var child = new GameObject("Hình cây").transform;
            child.SetParent(transform, false);
            Vector3 size = transform.localScale;
            float unit = Mathf.Min(size.x, size.y);
            child.localScale = new Vector3(unit / size.x, unit / size.y, 1);
            _cropArt = child.gameObject.AddComponent<SpriteRenderer>();
            _cropArt.sortingLayerID = Soil.sortingLayerID;
            _cropArt.sortingOrder = Soil.sortingOrder + 1;
        }

        internal void Render(CropDefinition[] crops, IntegratedPlantArt art, Color shade)
        {
            Crop crop = Model.CurrentCrop;
            int soil = 0;
            int plant = -1;
            switch (Model.Status)
            {
                case FarmPlotStatus.Tilling: soil = 1; break;
                case FarmPlotStatus.ReadyToSow: soil = 2; break;
                case FarmPlotStatus.HavePlant:
                    soil = crop.WateringCount > 0 ? 3 : 2;
                    plant = crop.State == CropState.Stage1 ? 0 : (crop.State == CropState.Stage2 || crop.State == CropState.Regrowing) ? 1 : 2;
                    _cropIndex = System.Array.IndexOf(crops, crop.Definition);
                    break;
                case FarmPlotStatus.HaveResidue:
                    bool rotten = Model.ResidueType == ResidueType.RottenCrop;
                    soil = rotten ? 5 : 2;
                    plant = rotten ? 3 : 4;
                    break;
            }
            Soil.sprite = art.GetSprite(0, soil);
            Soil.color = shade;
            _cropArt.enabled = plant >= 0 && _cropIndex >= 0;
            if (_cropArt.enabled) _cropArt.sprite = art.GetSprite(_cropIndex + 1, plant);
            _cropArt.color = shade;
        }

        internal bool NeedsWater => Model.CurrentCrop is Crop crop &&
            crop.CanWater && crop.WateringCount == 0;

        internal string Status()
        {
            switch (Model.Status)
            {
                case FarmPlotStatus.Tilling:
                    return "Đang xới · còn " + GameClock.FormatDuration(Model.TillingTimeRemaining);
                case FarmPlotStatus.ReadyToSow:
                    return "Đất đã xới · chọn giống và gieo";
                case FarmPlotStatus.HaveResidue:
                    return Model.ResidueType == ResidueType.RottenCrop ? "Cây đã hỏng · xới để dọn" : "Còn tàn dư · xới để dọn";
                case FarmPlotStatus.HavePlant:
                    var crop = Model.CurrentCrop;
                    string stage = crop.State == CropState.Ripe ? "đã chín · thu hoạch trước khi thối sau " +
                        GameClock.FormatDuration(System.Math.Max(0, crop.Definition.RotThreshold - crop.RipeElapsedTime))
                        : NeedsWater ? "cần tưới" : "đang lớn · còn " + GameClock.FormatDuration(crop.GrowthTimeRemaining);
                    return crop.Definition.Name + " · " + stage;
                default: return "Đất trống · xới trước khi gieo";
            }
        }
    }
}
