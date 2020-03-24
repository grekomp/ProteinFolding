using System;
using UnityEngine;

public class LatticePointVisual : MonoBehaviour
{
	[Header("Components")]
	public RectTransform rectTransform;

	public GameObject bindingUp;
	public GameObject bindingRight;
	public GameObject bindingDown;
	public GameObject bindingLeft;

	public GameObject pointFillHydrophobic;
	public GameObject pointFillPolar;

	[Header("Variables")]
	[SerializeField] protected bool isActive;
	[SerializeField] protected bool isHydrophobic;
	[SerializeField] protected Direction bindingDirection;
	public int x;
	public int y;

	public int debugOriginalX;
	public int debugOriginalY;

	public float gridSpacing;
	public int index;
	public int value;


	#region Public properties and methods
	public bool IsActive {
		get => isActive;
		set {
			isActive = value;
			UpdatePointIcon();
		}
	}
	public bool IsHydrophobic {
		get => isHydrophobic;
		set {
			isHydrophobic = value;
			UpdatePointIcon();
		}
	}
	public Direction BindingDirection {
		get => bindingDirection;
		set {
			bindingDirection = value;

			switch (value)
			{
				case Direction.None:
					bindingUp.SetActive(false);
					bindingRight.SetActive(false);
					bindingDown.SetActive(false);
					bindingLeft.SetActive(false);
					break;
				case Direction.Up:
					bindingUp.SetActive(true);
					bindingRight.SetActive(false);
					bindingDown.SetActive(false);
					bindingLeft.SetActive(false);
					break;
				case Direction.Right:
					bindingUp.SetActive(false);
					bindingRight.SetActive(true);
					bindingDown.SetActive(false);
					bindingLeft.SetActive(false);
					break;
				case Direction.Down:
					bindingUp.SetActive(false);
					bindingRight.SetActive(false);
					bindingDown.SetActive(true);
					bindingLeft.SetActive(false);
					break;
				case Direction.Left:
					bindingUp.SetActive(false);
					bindingRight.SetActive(false);
					bindingDown.SetActive(false);
					bindingLeft.SetActive(true);
					break;
			}
		}
	}

	public void UpdatePositionSize()
	{
		rectTransform.anchoredPosition = new Vector3(x * gridSpacing, -y * gridSpacing, 0);
		rectTransform.sizeDelta = new Vector2(gridSpacing, gridSpacing);
		// TODO: Update size
	}
	#endregion


	#region Helper methods
	private void UpdatePointIcon()
	{
		if (isActive == false)
		{
			pointFillHydrophobic.SetActive(false);
			pointFillPolar.SetActive(false);
			return;
		}

		pointFillHydrophobic.SetActive(isHydrophobic);
		pointFillPolar.SetActive(isHydrophobic == false);
	}
	#endregion
}