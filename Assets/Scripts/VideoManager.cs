using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{

	public static VideoManager instance;

	public VideoPlayer video;

	public VideoClip[] clips;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void Play(string clip)
	{
		VideoClip c = Array.Find(clips, item => item.name == clip);
		if (c == null)
		{
			Debug.LogWarning("Clip: " + clip + " not found!");
			return;
		}

		video.clip = c;
		video.Play();
	}

}
