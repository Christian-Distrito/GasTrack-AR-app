using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[DisallowMultipleComponent]
[RequireComponent(typeof(ARRaycastManager))]
public class ARObjectInteract : MonoBehaviour
{
    // Assign your 3 size Prefabs here, in the Inspector, in this exact
    // order: [0] = 2.7kg, [1] = 11kg, [2] = 50kg — the size-selection
    // buttons below call SelectSize() with these same index numbers, so
    // the order here must match the order your UI buttons use.
    [SerializeField] private GameObject[] lpgPrefabs;

    private int selectedIndex = 0; // defaults to the first size (2.7kg)

    private ARRaycastManager raycastManager;
    private GameObject spawnedLpg;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    /// <summary>
    /// Called by the UI buttons' OnClick events (wired up in the
    /// Inspector, see setup notes). Switches which size gets spawned on
    /// the NEXT tap — it does not resize/replace a model already placed.
    /// </summary>
    public void SelectSize(int index)
    {
        if (index < 0 || index >= lpgPrefabs.Length)
        {
            Debug.LogWarning($"SelectSize called with invalid index {index}");
            return;
        }
        selectedIndex = index;
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        bool hitSomething = raycastManager.Raycast(
            touch.position,
            hits,
            TrackableType.PlaneWithinPolygon
        );

        if (!hitSomething) return;

        Pose hitPose = hits[0].pose;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (spawnedLpg == null)
                {
                    spawnedLpg = Instantiate(lpgPrefabs[selectedIndex], hitPose.position, hitPose.rotation);
                }
                break;

            case TouchPhase.Moved:
                if (spawnedLpg != null)
                {
                    spawnedLpg.transform.position = hitPose.position;
                }
                break;
        }
    }
}
