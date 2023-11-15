using UnityEngine;

public class PivotPoint : MonoBehaviour
{
    // Parent to the grapple gun and child to the player, marks where the gun should be
    PlayerManager Instance;
    // Object being changed
    public GameObject PivotPointObject;
    // Object distance from transform
    public float pivotDistanceFromPlayer;

    private void Start()
    {
        // Make player manager instance
        Instance = PlayerManager.Instance;
    }

    private void Update()
    {
        // Updates the position of the pivot point relative to player and mouse position
        PivotPointObject.transform.position = transform.position + Instance.RelativeDirection(transform.position, Instance.MousePosition()) * pivotDistanceFromPlayer;
    }
}
