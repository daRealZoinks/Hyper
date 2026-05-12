using Hyper.UI.Selector;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMenu : MonoBehaviour
{
    public ResolutionSelector resolutionSelector;
    public DisplayModeSelector windowModeSelector;
    public Toggle vSyncToggle;

    private void Start()
    {
        SetupResolutionSelector();
        SetupWindowModeSelector();
        SetupVSyncToggle();
    }

    private void SetupResolutionSelector()
    {
        resolutionSelector.options.AddRange(Screen.resolutions);

        resolutionSelector.SetValue(Screen.currentResolution);

        resolutionSelector.OnSelectionChanged += resolution =>
        {
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        };
    }

    private void SetupWindowModeSelector()
    {
        FullScreenMode[] windowModes =
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            FullScreenMode.ExclusiveFullScreen,
#endif
            FullScreenMode.FullScreenWindow,
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            FullScreenMode.MaximizedWindow,
#endif
            FullScreenMode.Windowed,
        };

        windowModeSelector.options.AddRange(windowModes);

        windowModeSelector.SetValue(Screen.fullScreenMode);

        windowModeSelector.OnSelectionChanged += mode =>
        {
            Screen.fullScreenMode = mode;
        };
    }

    private void SetupVSyncToggle()
    {
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

        vSyncToggle.onValueChanged.AddListener(isOn =>
        {
            QualitySettings.vSyncCount = isOn ? 1 : 0;
        });
    }
}
