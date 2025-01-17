
using UnityEngine;

public class CelestialBodyDummyView : BaseView<CelestialBodyDummyModel, CelestialBodyDummyController>
{
    public GameObject placeholderModelPrefab;
    [SerializeField] private LayerMask layerMask;

    private void OnEnable()
    {
        ArInfoPanelView.OnInfoPanelReady += HandleinfoPanelReady;
    }

    private void OnDisable()
    {
        ArInfoPanelView.OnInfoPanelReady -= HandleinfoPanelReady;

    }

    private void HandleinfoPanelReady(RaycastHit hitObject)
    {
        Controller.UpdateMaterialFromObject(hitObject);
        UpdateViewFromModel();
    }

    public override void UpdateViewFromModel()
    {
        Vector3 spawnPosition = placeholderModelPrefab.transform.position;
        Transform spawnParent = placeholderModelPrefab.transform.parent;

        Destroy(placeholderModelPrefab);

        GameObject spawn = GameObject.Instantiate(Model.Prefab, spawnParent);
        spawn.transform.position = spawnPosition;
        spawn.transform.localScale = new Vector3(100, 100, 100);

        int layer = Mathf.FloorToInt(Mathf.Log(layerMask.value, 2));
        spawn.layer = layer;

        spawn.AddComponent<Dummy>();

        placeholderModelPrefab = spawn;
    }
}


public class CelestialBodyDummyModel : BaseModel
{
    public GameObject Prefab { get; set; }

}


public class CelestialBodyDummyController : BaseController<CelestialBodyDummyModel>
{
    public void UpdateMaterialFromObject(RaycastHit objTransform)
    {
        var gameObject = Helpers.GetAssociatedObject(objTransform);
        Model.Prefab = gameObject.GetPrefab(true);
    }
}

