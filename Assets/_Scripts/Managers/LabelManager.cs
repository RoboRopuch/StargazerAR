using UnityEngine;
using TMPro;
public class LabelManager : Singleton<LabelManager>
{
    public GameObject LabelPrefab;

    public void AttachLabel(Vector3 position, string labelMessage, Transform parent = null, Vector3 offset = default)
    {
        if (parent == null)
        {
            parent = transform;
        }

        Vector3 finalPosition = position + offset;
        GameObject label = Instantiate(LabelPrefab, finalPosition, Quaternion.identity, parent);
        label.name = $"{labelMessage}_label";
        label.GetComponentInChildren<TextMeshProUGUI>().text = labelMessage;
        label.AddComponent<Billboard>();
    }
    public void AttachCustomLabel(Vector3 position, string labelMessage, GameObject customLabelPrefab, Transform parent = null, Vector3 offset = default)
    {
        if (parent == null)
        {
            parent = transform;
        }

        Vector3 finalPosition = position + offset;
        GameObject label = Instantiate(customLabelPrefab, finalPosition, Quaternion.identity, parent);
        label.name = $"{labelMessage}_label";
        label.GetComponentInChildren<TextMeshProUGUI>().text = labelMessage;
        label.AddComponent<Billboard>();
    }

    public void ClearAllLabels()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
