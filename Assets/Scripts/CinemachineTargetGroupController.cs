using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using Fusion;

public class CinemachineTargetGroupController : MonoBehaviour
{
    private CinemachineTargetGroup _targetGroup;
    private readonly HashSet<Transform> _addedTargets = new();

    private void Awake()
    {
        _targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Update()
    {
        if (_addedTargets.Count >= 2)
            return;

        var controllers = FindObjectsOfType<GameplayController>();

        foreach (var controller in controllers)
        {
            // Skip if online and this is not the local player
            if (GameManager.Instance.IsOnline)
            {
                if (controller.Object == null || !controller.Object.HasInputAuthority)
                    continue;
            }

            // Use the CinemachineCameraRef anchor point
            var cameraAnchor = controller.transform.Find("CinemachineCameraRef");
            if (cameraAnchor == null || _addedTargets.Contains(cameraAnchor))
                continue;

            _targetGroup.AddMember(cameraAnchor, 1f, 2f);
            _addedTargets.Add(cameraAnchor);

            if (_addedTargets.Count >= 2)
                break;
        }
    }
}
