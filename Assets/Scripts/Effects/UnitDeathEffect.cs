using UnityEngine;

using YaEm.Core;
using YaEm;
using YaEm.Health;

public class UnitDeathEffect : MonoBehaviour, IActorComponent
{
	[SerializeField] private Transform _spawnTransform;
	[SerializeField] private UnitDisolveEffect _prefab;
	[SerializeField] private float _lifeTime;
	[SerializeField] private float _randomVelocity;
	[SerializeField] private float _randomVelocityMin;

	[SerializeField] private AnimationCurve _rotationCurve;
	[SerializeField] private float _curveMultiplayer;
	[SerializeField, Range(0, 1)] private float _randomFactor;
	[SerializeField] private float _maxDamage;
	[SerializeField] private Sprite _texture;
	private IActor _actor;

	public IActor Actor { set => _actor = value; }

	public void Init(IActor actor)
	{
		if(actor is IProvider<IHealth> prov)
		{
			prov.Value.OnDeath += Death;
		}
		_actor = actor;
	}

	private void Death(DamageArgs obj)
	{
		if(_actor is IProvider<IHealth> prov)
		{
			prov.Value.OnDeath -= Death;
		}

		var instance = Instantiate(_prefab, _spawnTransform.position, Quaternion.Euler(0, 0, _spawnTransform.eulerAngles.z), null);
		if(instance.TryGetComponent<SpriteRenderer>(out var renderer))
		{
			renderer.sprite = _texture;
			if(_actor is ITeamProvider prov2)
			{
				renderer.color = ServiceLocator.Get<ColorTable>().GetColor(prov2.TeamNumber);
			}
		}
		else
		{
			Debug.LogWarning("Cannot find sprite renderer on " + _prefab.name);
		}

		float delta = Mathf.Clamp01(obj.Damage / _maxDamage);
		float vel = _rotationCurve.Evaluate(Mathf.Clamp01(delta + Random.Range(-_randomFactor, _randomFactor))) * _curveMultiplayer;

		//todo: make special method to transfer all these paramethers
		instance.RotationVelocity = vel;
		instance.Direction = obj.HitPosition.GetDirectionNormalized(_actor.Position);
		instance.RandomVelocity = Random.Range(_randomVelocityMin, _randomVelocity);
	}
}
