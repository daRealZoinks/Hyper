using Hyper.ScriptableObjects;
using Hyper.UI.Menus;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapMenuManager : MonoBehaviour
{
    public List<MapInfo> maps = new();

    public MapButton mapButtonPrefab;

    public UnityEvent onMapSelected;

    private GridLayoutGroup _mapIconGridLayoutGroup;
    private GameSettingsManager _gameSettingsManager;

    private void Awake()
    {
        _mapIconGridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        _gameSettingsManager = FindAnyObjectByType<GameSettingsManager>();

        foreach (var map in maps)
        {
            var button = Instantiate(mapButtonPrefab, _mapIconGridLayoutGroup.transform);
            button.Initialize(map);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                _gameSettingsManager.SetMap(map);
                onMapSelected.Invoke();
            });
        }
    }
}
