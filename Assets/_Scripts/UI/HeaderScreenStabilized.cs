using UnityEngine;
using UnityEngine.UIElements;

public class HeaderView : BaseView<HeaderModel, HeaderController>
{
    private VisualElement Header;
    private Label LatitudeInput;
    private Label LongitudeInput;

    private void OnEnable() => ContextManager.OnLocationChange += HandleUserLocationChange;
    private void OnDisable() => ContextManager.OnLocationChange -= HandleUserLocationChange;

    public override void Awake()
    {

        Header = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("header");
        LatitudeInput = Header.Q<Label>("latitudeInput");
        LongitudeInput = Header.Q<Label>("longitudeInput");

        base.Awake();
    }

    private void HandleUserLocationChange(GeographicCoordinates userLocation)
    {
        Controller.UpdateModel(userLocation);
        UpdateViewFromModel();
    }

    public override void UpdateViewFromModel()
    {
        LatitudeInput.text = Model.latitude;
        LongitudeInput.text = Model.longitude;
    }
}

public class HeaderModel : BaseModel
{
    public string latitude;
    public string longitude;
}


public class HeaderController : BaseController<HeaderModel>
{
    public void UpdateModel(GeographicCoordinates userLocation)
    {
        Model.latitude = Helpers.Deg2Str(userLocation.Latitude);
        Model.longitude = Helpers.Deg2Str(userLocation.Longitude);
    }
}
