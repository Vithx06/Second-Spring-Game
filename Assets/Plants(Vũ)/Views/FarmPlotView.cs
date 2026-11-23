using UnityEngine;

public class FarmPlotView : MonoBehaviour
{
    private FarmPlot _plot;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _plot = new FarmPlot(3f);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        RefreshColor();
    }

    private void Update()
    {
        _plot.UpdateTilling(Time.deltaTime);
        RefreshColor();
    }

    private void OnMouseDown()
    {
        _plot.StartTilling();
        RefreshColor();
    }

    private void RefreshColor()
    {
        switch (_plot.Status)
        {
            case FarmPlotStatus.Untilled:
                _spriteRenderer.color = new Color32(110, 70, 35, 255);
                break;

            case FarmPlotStatus.Tilling:
                _spriteRenderer.color = Color.yellow;
                break;

            case FarmPlotStatus.ReadyToSow:
                _spriteRenderer.color = new Color32(180, 120, 60, 255);
                break;
        }
    }
}