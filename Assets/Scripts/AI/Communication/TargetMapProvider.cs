using System;
using System.Collections.Generic;
using UnityEngine;
using YaEm;

public class TargetMapProvider : MonoBehaviour, IService
{
    [SerializeField] private float _updateTime;
    private float _elapsed;
    private Dictionary<int, TargetMap> _targetMaps = new Dictionary<int, TargetMap>();

    public event Action OnMapsClear;

	private void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed > _updateTime)
        {
            Clear();
            _elapsed = 0f;
        }
    }
    private void Awake()
    {
        if (ServiceLocator.TryGet<TargetMapProvider>(out var provider))
        {
            Debug.Log("Replacing");
            provider.UpdateProvider(provider);
        }
        else
        {
            Debug.Log("Adding");
            ServiceLocator.Register<TargetMapProvider>(this);
        }
    }

    private void Clear()
    {
        _targetMaps = new Dictionary<int, TargetMap>();
    }

    public void AddTarget(int selfTeamNumber, Vector2 worldPosition)
    {
        var map = GetMap(selfTeamNumber);

        map.AddTarget(worldPosition);
    }
    public void RemoveTarget(int selfTeamNumber, Vector2 worldPosition)
    {
        var map = GetMap(selfTeamNumber);

        map.RemoveTarget(worldPosition);
    }

    public bool ContainsTarget(int selfTeamNumber, Vector2 worldPosition)
    {
        return GetMap(selfTeamNumber).ContainsTarget(worldPosition);
    }
    public bool GetRandomTarget(int selfTeamNumber, out Vector2 target)
    {
        var map = GetMap(selfTeamNumber);
        return map.TryGetRandomTarget(out target);
    }

    private TargetMap GetMap(int teamNum)
    {
        TargetMap map = default;
        if (!_targetMaps.TryGetValue(teamNum, out map))
        {
            _targetMaps[teamNum] = new TargetMap();
            map = _targetMaps[teamNum];
        }
        return map;
    }

    private void UpdateProvider(TargetMapProvider provider)
    {
        provider._updateTime = _updateTime;
        provider._targetMaps = new Dictionary<int, TargetMap>();
    }
}
