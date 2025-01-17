using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArInfoPanelView : BaseView<ArInfoPanelModel, ArInfoPanelController>
{
    public GameObject WelcomeMessage;
    public GameObject InfoPanel;
    public Transform Table;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Title;
    public GameObject RowTemplatePrefab;
    public static event Action<RaycastHit> OnInfoPanelReady;

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
        OnInfoPanelReady?.Invoke(hitObject);

    }

    public override void UpdateViewFromModel()
    {
        Description.text = Model.Description;
        Title.text = Model.Title;

        ResetTable();

        foreach (var data in Model.TableData)
        {
            GameObject row = Instantiate(RowTemplatePrefab, Table);
            TextMeshProUGUI keyColumn = row.transform.Find("KeyColumn")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI valueColumn = row.transform.Find("ValueColumn")?.GetComponent<TextMeshProUGUI>();
            keyColumn.text = data.Key;
            valueColumn.text = data.Value;
        }

        TogglePanelsVisibility();

    }

    private void ResetTable()
    {
        foreach (Transform child in Table)
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

        WelcomeMessage.SetActive(!WelcomeMessage.activeSelf);
        InfoPanel.SetActive(!WelcomeMessage.activeSelf);

        toggled = true;
    }


    void Update()
    {
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

    private CelestialBody ExtractDataFromObject(RaycastHit objectToGetDataFrom)
    {
        CelestialBody data = Helpers.GetAssociatedObject(objectToGetDataFrom);
        return data;
    }
}
