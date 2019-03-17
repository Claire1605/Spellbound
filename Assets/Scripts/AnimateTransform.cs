using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimateTransform : MonoBehaviour
{
	public bool animatePosition = false;

	public float positionAmount = 0.1f;
	public Vector3 positionAxes = Vector3.up;
	public float positionSpeed = 1.0f;
	public AnimationCurve positionCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
	public float positionTime = 0.0f;
	public float positionTimeOffset = 0.0f;

	public bool animateRotation = false;

	public float rotationAmount = 36.0f;
	public Vector3 rotationAxes = Vector3.up;
	public float rotationSpeed = 1.0f;
	public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
	public float rotationTime = 0.0f;
	public float rotationTimeOffset = 0.0f;

	public bool animateScale = false;

	public float scaleAmount = 0.1f;
	public Vector3 scaleAxes = Vector3.up;
	public float scaleSpeed = 1.0f;
	public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
	public float scaleTime = 0.0f;
	public float scaleTimeOffset = 0.0f;

	private Vector3 initialPosition;

	private Vector3 initialRotation;

	private Vector3 initialScale;

	void Start ()
	{
		initialPosition = transform.localPosition;
		initialRotation = transform.localEulerAngles;
		initialScale = transform.localScale;
	}
	
	void Update ()
	{
		if (animatePosition)
		{
			positionTime = positionTime + Time.deltaTime * positionSpeed;

			float t = positionCurve.Evaluate(Mathf.Repeat(positionTime + positionTimeOffset, 1.0f));

			transform.localPosition = initialPosition + t * positionAxes * positionAmount;
		}

		if (animateRotation)
		{
			rotationTime = rotationTime + Time.deltaTime * rotationSpeed;

			float t = rotationCurve.Evaluate(Mathf.Repeat(rotationTime + rotationTimeOffset, 1.0f));

			transform.localEulerAngles = initialRotation + t * rotationAxes * rotationAmount;
		}

		if (animateScale)
		{
			scaleTime = scaleTime + Time.deltaTime * scaleSpeed;

			float t = scaleCurve.Evaluate(Mathf.Repeat(scaleTime + scaleTimeOffset, 1.0f));

			transform.localScale = initialScale + t * scaleAxes * scaleAmount;
		}
	}
}
