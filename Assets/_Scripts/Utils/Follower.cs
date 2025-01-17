using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject obectToFollow;
    void Update()
    {
        transform.position = obectToFollow.transform.position;
    }
}
