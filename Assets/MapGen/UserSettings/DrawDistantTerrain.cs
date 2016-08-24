using System;

namespace UserSettings
{
    public class DrawDistantTerrain : SliderBase
    {
        WorldMapMaker worldMap;

        protected override void InitValue()
        {
            slider.value = (int)GameSettings.Instance.rendering.distantTerrainDetail;
            valueLabel.text = GameSettings.Instance.rendering.distantTerrainDetail.ToString();
            worldMap = FindObjectOfType<WorldMapMaker>();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.distantTerrainDetail = (GameSettings.LandscapeDetail)value;
            valueLabel.text = GameSettings.Instance.rendering.distantTerrainDetail.ToString();
            var regions = FindObjectsOfType<RegionMaker>();
            worldMap.gameObject.SetActive(GameSettings.Instance.rendering.distantTerrainDetail != GameSettings.LandscapeDetail.Off);
        }
    }
}
