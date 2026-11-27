using UnityEngine;

// View của một ô chỉ đổi hình/màu theo Model; không tự xới, gieo hay trừ kho.
public sealed class PlantPlotView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _soil;
    [SerializeField] private SpriteRenderer _crop;
    private PlantSystemView _art;
    private CropType _lastCrop;
    public void Initialize(PlantSystemView art) { _art=art; }
    public void Render(FarmPlot model,bool selected)
    {
        string soil=model.Status==FarmPlotStatus.Untilled?"Untilled":model.Status==FarmPlotStatus.Tilling?"Tilling":
            model.ResidueType==ResidueType.RottenCrop?"Rotten":model.Status==FarmPlotStatus.HaveResidue?"Residue":
            model.CurrentCrop!=null && model.CurrentCrop.WateringCount>0?"Watered":"Ready";
        _soil.sprite=_art.FindSprite("Soil_"+soil);
        _soil.color=selected?new Color(1,.94f,.70f):Color.white;
        var plant=model.CurrentCrop;
        if(plant!=null)
        {
            _lastCrop=plant.Definition.Type;
            // Atlas dùng hình Stage2 cho lúc tái sinh; Model vẫn giữ trạng thái Regrowing riêng.
            var state=plant.State==CropState.Regrowing?CropState.Stage2:plant.State;
            _crop.sprite=_art.FindSprite(plant.Definition.Type+"_"+state);
        }
        else if(model.Status==FarmPlotStatus.HaveResidue)
            _crop.sprite=_art.FindSprite(_lastCrop+"_"+(model.ResidueType==ResidueType.RottenCrop?"Dead":"Harvested"));
        _crop.enabled=plant!=null || model.Status==FarmPlotStatus.HaveResidue;
    }
}
