using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject ObjectToFollow;
    void Update()
    {
        transform.position = ObjectToFollow.transform.position;
    }
}
