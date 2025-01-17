using UnityEngine;
using UnityEngine.UIElements;

public class FooterView : BaseView<FooterModel, FooterController>
{
    private VisualElement Footer;
    private Label CelestialBodyNameInput;
    private Label CelestialBodyAzimuthAltitudeInput;


    private void OnEnable()
    {
        TouchManager.OnObjectHit += HandleObjectHit;
        TouchManager.OnVoidHit += HandleVoidHit;

    }

    private void OnDisable()
    {
        TouchManager.OnObjectHit -= HandleObjectHit;
        TouchManager.OnVoidHit += HandleVoidHit;

    }

    public override void Awake()
    {
        Footer = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("footer");
        CelestialBodyNameInput = Footer.Q<Label>("name");
        CelestialBodyAzimuthAltitudeInput = Footer.Q<Label>("coordinates");

        base.Awake();

    }
    public override void UpdateViewFromModel()
    {
        CelestialBodyNameInput.text = Model.Name;
        CelestialBodyAzimuthAltitudeInput.text = Model.Coordinates;
    }

    public void ShowFooter()
    {
        Footer.style.display = DisplayStyle.Flex;
        Footer.AddToClassList("bottomsheet--up");
    }

    public void HideFooter()
    {
        Footer.style.display = DisplayStyle.None;
        Footer.RemoveFromClassList("bottomsheet--up");
    }

    public void HandleObjectHit(RaycastHit hitObject)
    {
        Controller.UpdateModel(hitObject);
        UpdateViewFromModel();
        ShowFooter();
    }

    public void HandleVoidHit()
    {
        HideFooter();
    }
}

public class FooterModel : BaseModel
{
    public string Coordinates { get; set; }
    public string Name { get; set; }
}

public class FooterController : BaseController<FooterModel>
{
    public void UpdateModel(RaycastHit objectHit)
    {
        var data = Helpers.GetAssociatedObject(objectHit);

        Model.Name = data.Name;
        Model.Coordinates = $"{Helpers.Deg2Str(data.Azimuth)} x {Helpers.Deg2Str(data.Altitude)}";
    }

}
