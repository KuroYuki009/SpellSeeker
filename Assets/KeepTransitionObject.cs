using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepTransitionObject : MonoBehaviour
{
	public static KeepTransitionObject Instance
	{
		get; private set;
	}

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
