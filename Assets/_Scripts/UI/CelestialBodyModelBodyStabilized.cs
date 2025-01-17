
using UnityEngine;

public class PlaceholderManagerView : BaseView<PlaceholderManageModel, PlaceholderManageController>
{
    public GameObject placeholderModelPrefab;
    [SerializeField] private LayerMask layerMask;

    private void OnEnable()
    {
        ArInfoPanelView.OnUserSpacePanelReady += HandleObjectHit;
    }

    private void OnDisable()
    {
        ArInfoPanelView.OnUserSpacePanelReady -= HandleObjectHit;

    }

    private void HandleObjectHit(RaycastHit hitObject)
    {
        Debug.Log("tap detected");
        Controller.UpdateMaterialFromObject(hitObject);
        UpdateViewFromModel();
    }

    public override void UpdateViewFromModel()
    {
        Debug.Log("Instantiating new prefab...");

        Vector3 spawnPosition = placeholderModelPrefab.transform.position;
        Transform spawnParent = placeholderModelPrefab.transform.parent;

        Destroy(placeholderModelPrefab);

        var spawn = GameObject.Instantiate(Model.Prefab, spawnParent);
        spawn.transform.position = spawnPosition;
        spawn.transform.localScale = new Vector3(100, 100, 100);

        int layer = Mathf.FloorToInt(Mathf.Log(layerMask.value, 2));
        spawn.layer = layer;

        spawn.AddComponent<Rotatable>();

        placeholderModelPrefab = spawn;
    }
}


public class PlaceholderManageModel : BaseModel
{
    public GameObject Prefab { get; set; }

}


public class PlaceholderManageController : BaseController<PlaceholderManageModel>
{
    public void UpdateMaterialFromObject(RaycastHit objTransform)
    {
        var gameObject = Helpers.GetAssociatedObject(objTransform);
        Model.Prefab = gameObject.GetPrefab(true);
    }
}

