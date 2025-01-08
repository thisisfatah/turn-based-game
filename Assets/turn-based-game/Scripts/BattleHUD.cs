using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI nameText;
	[SerializeField]
	private Image healthImage;

	private Unit unit;

	public void SetHUD(Unit unit)
	{
		this.unit = unit;
		nameText.text = unit.unitName;
		healthImage.fillAmount = unit.currentHP / unit.maxHP;
	}

	public void SetHealth()
	{
		healthImage.fillAmount = ((float)unit.currentHP / unit.maxHP);
	}
}
