using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using System.IO;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.XR.Interaction.Toolkit;
using CSVhelper;
using TrialHelper;
using Logging;

public class Controller : MonoBehaviour
{

	public FaceManager face;
	public AudioManager audio;
	public VideoManager video;
	public Animator animator;
	public GameObject camera;

	[ContextMenuItem("negative", "setConditionNegative")]
	[ContextMenuItem("positive", "setConditionPositive")]
	[SerializeField]
	public string condition;
	public void setConditionNegative() { condition = "negative"; }
	public void setConditionPositive() { condition = "positive"; }

	private static string visemes_path = @".\Assets\Data\InputData\visemes\visemes_story_{0}_{1}.csv";
	private static string blinking_path = @".\Assets\Data\InputData\blinking\blinking_story_{0}_{1}.csv";

	private int[,][,] visemes;
	private int[,][,] blinking;

	private int[] lessons;
	private Stack<Trial> trials = new Stack<Trial>();
	private Trial trial;
	private (int s, string e) t = (-1, "");

	private int input = -1;
	private int waitFrames = 60;
	private string participant_id = "";
	private Logging.Logger logger = new Logging.Logger();
	private int startFrame;

	void Awake()
    {
		LoadData();

		lessons = new int[] { 1, 2, 3, 4, 5 };
		System.Random random = new System.Random();
		lessons = lessons.OrderBy(x => random.Next()).ToArray();

		trials.Push(new TrialSpecial(10, "neutral"));
		for (int i = lessons.Length - 1; i >= 0; i--)
        {
			if (i < lessons.Length - 1)
				trials.Push(new TrialSpecial(11, condition));
			trials.Push(new Trial(lessons[i], condition));
		}
		trials.Push(new TrialSpecial(9, "neutral"));
	}

	void Start()
    {
		SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressedLeft, SteamVR_Input_Sources.LeftHand);
		SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressedRight, SteamVR_Input_Sources.RightHand);

		trial = trials.Pop();
    }

	void Update()
	{
		if (String.IsNullOrEmpty(participant_id))
        {
			if (Input.GetKeyDown(KeyCode.F))
            {
				participant_id = Guid.NewGuid().ToString();
				startFrame = Time.frameCount;
				logger.Start();
				logger.Log("participant_id", participant_id);
				logger.Log("condition", condition);
				logger.Log("lessons", string.Join(";", lessons));
				Debug.Log("participant ID: " + participant_id);
				Debug.Log("order lessons: " + string.Join(";", lessons));
				Debug.Log("condition: " + condition);
			}
			return;
		}
		else
        {
			if ((Time.frameCount - startFrame) % 90 == 0) logger.Log("hmd_pitch", "" + camera.transform.localEulerAngles[0]);
			if (!(trials.Count == 0) && waitFrames == 0 && !audio.IsPlaying()) PlayNext();
			if (waitFrames > 0 && !audio.IsPlaying()) waitFrames--;
		}
	}

	private void LoadData()
	{
		string[] e = new string[] { "neutral", "positive", "negative" };
		visemes = new int[11, 3][,];
		blinking = new int[11, 3][,];
		for (int i = 0; i < 11; i++)
        {
			for (int j = 0; j < 3; j++)
            {
				string pathViseme = String.Format(visemes_path, i + 1, e[j]);
                if (File.Exists(pathViseme))
                {
					visemes[i, j] = CSVreader.LoadVisemeData(pathViseme, "viseme");
				}

				string pathBlinking = String.Format(blinking_path, i + 1, e[j]);
				if (File.Exists(pathBlinking))
				{
					blinking[i, j] = CSVreader.LoadVisemeData(pathBlinking, "blinkingStates");
				}
			}
		}
	}

	private void PlayNext()
    {
		t = trial.Next();
		if (t.s == 0)
        {
			if (input == -1) return;
			t = trial.Response(input);
			logger.Log("repeat_lesson", "" + (input == 0));
			Debug.Log("repeat lesson: " + ((input == 0) ? "repeat" : "next"));
			input = -1;
		}
		else if (t.s == -1)
        {
			trial = trials.Pop();
			if (trial != null) PlayNext();
			return;
        }		

		waitFrames = 60;
		logger.Log("starting_story", t.s + ";" + t.e);
		PlayStory(t.s, t.e);
	}

	private void PlayStory(int s, string e)
    {
		string code = "" + s + "_" + e;

		video.Play(code);
		audio.Play(code);
		animator.Play(code);

		if (e == "neutral")
			face.Play(visemes[s - 1, 0], blinking[s - 1, 0], e);
		else if (e == "positive")
			face.Play(visemes[s - 1, 1], blinking[s - 1, 1], e);
		else
			face.Play(visemes[s - 1, 2], blinking[s - 1, 2], e);
	}

	private void TriggerPressedLeft(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
	{
		input = t.s == 0 ? 0 : -1;
		logger.Log("button_pressed", "left;repeat");
		Debug.Log("Left Pressed");
		Debug.Log(input);
	}

	private void TriggerPressedRight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
	{
		input = t.s == 0 ? 1 : -1;
		logger.Log("button_pressed", "right;next");
		Debug.Log("Right Pressed");
		Debug.Log(input);
	}

	public void OnApplicationQuit()
    {
		if (!String.IsNullOrEmpty(participant_id))
        {
			Debug.Log("participant ID: " + participant_id);
			logger.Save("experiment_data_participant_" + participant_id);
		}
	}

}
