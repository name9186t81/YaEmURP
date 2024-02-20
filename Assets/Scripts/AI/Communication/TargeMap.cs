using System.Collections.Generic;
using UnityEngine;

public class TargetMap
{
    private List<Vector2> _targets = new List<Vector2>();

    public void AddTarget(Vector2 target)
    {
        _targets.Add(target);
    }
    public void RemoveTarget(Vector2 target)
    {
        _targets.Remove(target);
    }
    public bool TryGetRandomTarget(out Vector2 target)
    {
        if (_targets.Count > 0)
        {
            target = _targets[Random.Range(0, _targets.Count)];
            return true;
        }
        target = Vector2.zero;
        return false;
    }
    public bool ContainsTarget(Vector2 target)
    {
        return _targets.Contains(target);
    }

    public void Clear()
    {
        _targets.Clear();
    }
}
