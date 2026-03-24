using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;

namespace WmsLayer.Portal.Demo;

/// <summary>
/// Provides map data to an application
/// </summary>
public class MapViewModel : INotifyPropertyChanged
{
    public MapViewModel()
    {
        _map = new Map()
        {
            InitialViewpoint = new Viewpoint(
                new Envelope(-180, -85, 180, 85, SpatialReferences.Wgs84)
            ),
            Basemap = new Basemap(BasemapStyle.ArcGISStreets)
        };

        _ = LoadLayer();
    }

    private async Task LoadLayer()
    {
        try
        {
            // IMPORTANT: change to your Portal
            var portal = await ArcGISPortal.CreateAsync(new Uri(""));

            // IMPORTANT: change to your webmap item ID
            // The webmap must contain a WMS layer that has at least 2 styles
            // and the layer in the webmap must be configured to use a non-default style by setting the Custom Parameter STYLES to the name of the non-default style.
            var results = await portal.FindItemsAsync(
                new PortalQueryParameters($"id:XXXXXXXXXXXXXXXXXXXXXXXXXXXX")
            );

            var mapItem = results.Results.FirstOrDefault();

            if (mapItem is null || mapItem.Type != PortalItemType.WebMap)
            {
                throw new Exception("Item or Portal not configured correctly");
            }

            var map = new Map(mapItem);
            await map.LoadAsync();
            Map = map;
            MapTitle = mapItem.Title;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            MapTitle = e.Message;
        }
    }

    private string _mapTitle = "Loading map...";
    public string MapTitle
    {
        get { return _mapTitle; }
        set
        {
            _mapTitle = value;
            OnPropertyChanged();
        }
    }

    private Map _map;

    /// <summary>
    /// Gets or sets the map
    /// </summary>
    public Map Map
    {
        get => _map;
        set
        {
            _map = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}
