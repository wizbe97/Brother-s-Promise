using System.Collections;
using UnityEngine;
using Cinemachine;
using Fusion;

public class PlayerCinemachineTargetGroup : NetworkBehaviour
{
    public override void Spawned()
    {
        if (Runner.IsClient)
            StartCoroutine(RegisterWithCinemachineWhenReady());
    }

    private IEnumerator RegisterWithCinemachineWhenReady()
    {
        yield return new WaitUntil(() => FindObjectOfType<CinemachineTargetGroup>() != null);

        var targetGroup = FindObjectOfType<CinemachineTargetGroup>();
        if (targetGroup != null && !IsAlreadyInGroup(targetGroup, transform))
        {
            targetGroup.AddMember(transform, 1f, 2f);
            Debug.Log($"[Cinemachine] Added {name} to target group at {transform.position}");
        }
    }

    private bool IsAlreadyInGroup(CinemachineTargetGroup group, Transform t)
    {
        foreach (var m in group.m_Targets)
        {
            if (m.target == t)
                return true;
        }
        return false;
    }

}
