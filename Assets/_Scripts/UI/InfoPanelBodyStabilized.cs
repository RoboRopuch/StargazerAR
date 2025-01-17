using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArInfoPanelView : BaseView<ArInfoPanelModel, ArInfoPanelController>
{
    public GameObject welcomeMessage;
    public GameObject infoPanel;
    public Transform table;
    public TextMeshProUGUI description;
    public TextMeshProUGUI title;
    public GameObject _rowTemplatePrefab;
    public static event Action<RaycastHit> OnUserSpacePanelReady;

    private bool toggled = false;

    private void OnEnable()
    {
        TouchManager.OnObjectHit += HandleObjectHit;
    }

    private void OnDisable()
    {
        TouchManager.OnObjectHit -= HandleObjectHit;
    }

    private void HandleObjectHit(RaycastHit hitObject)
    {
        Controller.UpdateModelFromObject(hitObject);
        UpdateViewFromModel();
        OnUserSpacePanelReady?.Invoke(hitObject);

    }

    public override void UpdateViewFromModel()
    {
        description.text = Model.Description;
        title.text = Model.Title;

        ResetTable();

        foreach (var tableData in Model.TableData)
        {
            GameObject row = Instantiate(_rowTemplatePrefab, table);
            TextMeshProUGUI keyColumn = row.transform.Find("KeyColumn")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI valueColumn = row.transform.Find("ValueColumn")?.GetComponent<TextMeshProUGUI>();
            keyColumn.text = tableData.Key;
            valueColumn.text = tableData.Value;
        }

        TogglePanelsVisibility();

    }

    private void ResetTable()
    {
        foreach (Transform child in table)
        {
            Destroy(child.gameObject);
        }
    }

    private void TogglePanelsVisibility(bool onlyOnce = true)
    {
        if (onlyOnce && toggled)
        {
            return;
        }

        welcomeMessage.SetActive(!welcomeMessage.activeSelf);
        infoPanel.SetActive(!welcomeMessage.activeSelf);

        toggled = true;
    }


    void Update()
    {
        // Quaternion rotation = Input.gyro.attitude;
        // Vector3 stabilizedPosition = Camera.main.transform.position + rotation * Vector3.one;
        // transform.position = stabilizedPosition;
        // transform.rotation = rotation;


        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 offset = new(0, -2, 0);
        transform.position = cameraPosition + offset;

    }

}

public class ArInfoPanelModel : BaseModel
{
    public string Description { get; set; }
    public string Title { get; set; }
    public Dictionary<string, string> TableData { get; set; }
}


public class ArInfoPanelController : BaseController<ArInfoPanelModel>
{
    public void UpdateModelFromObject(RaycastHit objectToGetDataFrom)
    {
        var data = ExtractDataFromObject(objectToGetDataFrom);

        if (data == null) return;

        Model.Description = data.Description;
        Model.Title = data.Name;
        Model.TableData = data.GetTabularizedData();
    }

    private SkyObject ExtractDataFromObject(RaycastHit objectToGetDataFrom)
    {
        SkyObject data = Helpers.GetAssociatedObject(objectToGetDataFrom);
        return data;
    }
}
