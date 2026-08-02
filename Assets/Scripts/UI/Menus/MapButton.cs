using Hyper.ScriptableObjects;
using TMPro;
using UnityEngine;

public class MapButton : MonoBehaviour
{
    [HideInInspector]
    public MapInfo mapInfo;

    private TextMeshProUGUI _mapNameText;

    public void Initialize(MapInfo map)
    {
        mapInfo = map;
        _mapNameText = GetComponentInChildren<TextMeshProUGUI>();
        _mapNameText.text = mapInfo.mapName;
    }
}
