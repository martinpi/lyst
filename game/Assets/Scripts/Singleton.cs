using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T _instance;
	public static T Instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<T>();
			
			return _instance;
		}
	}
	
	void Awake()
	{
		_instance = GameObject.FindObjectOfType<T>();
	}
	
}