using TMPro;

using UnityEngine;
using UnityEngine.UI;

using YaEm.Core;
using YaEm.Health;

namespace YaEm.GUI
{
	public class HealthDisplay : GUIElement
	{
		[SerializeField, Range(0, 1)] private float _fillStart;
		[SerializeField, Range(0,1)] private float _fillEnd;
		[SerializeField, Range(0,1)] private float _fillBackSpeed = 0.05f;
		[SerializeField] private Image _fill;
		[SerializeField] private Image _fillBack;
		[SerializeField] private TextMeshProUGUI _text;
		private IHealth _health;

		private void Update()
		{
			//equalazing fills
			_fillBack.fillAmount = Mathf.Lerp(_fillBack.fillAmount, _fill.fillAmount, _fillBackSpeed);
		}

		protected override void PlayerChanged(IActor player)
		{
			if (player == null) return;

			if(player is IProvider<IHealth> prov)
			{
				_health = prov.Value;
				_health.OnDamage += Damaged;
				_health.OnDeath += Died;

				_text.text = _health.CurrentHealth.ToString();
				_fill.fillAmount = _fillBack.fillAmount = _fillEnd;
			}
		}

		private void Died(DamageArgs obj)
		{
			_health.OnDeath -= Died;
			_health.OnDamage -= Damaged;
			_fill.fillAmount = _fillStart;
			_text.text = "DEAD";
		}

		private void Damaged(DamageArgs obj)
		{
			_fill.fillAmount = Mathf.Lerp(_fillStart, _fillEnd, _health.Delta());
			_text.text = _health.CurrentHealth.ToString();
		}
	}
}