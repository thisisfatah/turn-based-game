using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
	[SerializeField]
    private Image tileImage;

	private void Start()
	{
		HideImage();
	}

	public void ShowImage(Color imageColor)
	{
		tileImage.gameObject.SetActive(true);
		tileImage.color = imageColor;
	}

	public void HideImage()
	{
		tileImage.gameObject.SetActive(false);
	}
}
