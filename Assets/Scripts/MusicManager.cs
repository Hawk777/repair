using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
	[Tooltip("The main music layers")]
	public AudioClip[] main = new AudioClip[6];

	[Tooltip("The EuroBeat music layers")]
	public AudioClip[] euro = new AudioClip[6];

	[Tooltip("The template AudioSource")]
	public GameObject sourceTemplate;

	[Tooltip("How long it takes to fade an audio source")]
	public float fadeTime = 1f;

	[Tooltip("How long you have to drift for before switching to Euro-beat")]
	public float driftThreshold = 8f;

	[Tooltip("How long to play the target-solved layer after a target is solved")]
	public float solveTime = 16f;

	// The one and only instance.
	private static MusicManager instance;

	// The audio sources. The major axis is the bank (main-vs-Euro); the minor
	// axis is layer.
	private AudioSource[][] sources;

	// The target enablements of the layers./
	private bool[] layersEnabled;

	// For how long the car has been continuously drifting.
	private float driftTime = 0f;

	// For how much longer the solve layer will play.
	private float solveCounter = 0f;

	// Gets the MusicManager.
	public static MusicManager Get() {
		return instance;
	}

	// Handles switching scenes.
	public void SwitchScene(string scene) {
		// After a scene switch, we are not drifting.
		driftTime = 0f;

		// Layer 0 is always enabled. Ignore it. Layer 1 is enabled if not at
		// the main menu.
		layersEnabled[1] = scene != "Start Menu";

		// Layers 2+ are always disabled initially, until enabled by some
		// external call.
		for(uint i = 2; i != layersEnabled.Length; ++i) {
			layersEnabled[i] = false;
		}
	}

	// Reports whether the car is moving.
	//
	// This should be called whenever the car starts or stops, and may also be
	// called at other times.
	public void SetCarMoving(bool moving) {
		layersEnabled[2] = moving;
	}

	// Reports whether there are any enemies near the car.
	public void SetEnemiesNearby(bool nearby) {
		layersEnabled[3] = nearby;
	}

	// Reports whether there are any targets near the car.
	public void SetTargetsNearby(bool nearby) {
		layersEnabled[4] = nearby;
	}

	// Reports that a target was just solved.
	public void TargetSolved() {
		layersEnabled[5] = true;
		solveCounter = solveTime;
	}

	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
			sources = new AudioSource[][]{MakeSources(main), MakeSources(euro)};
			layersEnabled = new bool[sources[0].Length];
			layersEnabled[0] = true;
		} else {
			Destroy(gameObject);
		}
	}

	void Start() {
		if(instance == this) { /* not sure if destruction happens between Awake and Start or after Start */
			double startTime = AudioSettings.dspTime + 0.5;
			foreach(AudioSource[] i in sources) {
				foreach(AudioSource j in i) {
					j.PlayScheduled(startTime);
				}
			}
			sources[0][0].volume = 1f;
		}
	}

	void FixedUpdate() {
		if(Input.GetButton("Fire1")) {
			driftTime += Time.fixedDeltaTime;
		} else{
			driftTime = 0f;
		}

		if(solveCounter > 0f) {
			solveCounter -= Time.fixedDeltaTime;
		} else {
			layersEnabled[5] = false;
		}

		for(uint bank = 0; bank != sources.Length; ++bank) {
			for(uint layer = 0; layer != sources[bank].Length; ++layer) {
				AudioSource source = sources[bank][layer];
				bool enabled = bank == currentBank && layersEnabled[layer];
				if(enabled) {
					source.volume = Mathf.Min(1f, source.volume + Time.fixedDeltaTime / fadeTime);
				} else {
					source.volume = Mathf.Max(0f, source.volume - Time.fixedDeltaTime / fadeTime);
				}
			}
		}
	}

	private AudioSource[] MakeSources(AudioClip[] clips) {
		AudioSource[] sources = new AudioSource[clips.Length];
		for(uint i = 0; i != sources.Length; ++i) {
			GameObject gobj = Instantiate(sourceTemplate);
			DontDestroyOnLoad(gobj);
			sources[i] = gobj.GetComponent<AudioSource>();
			sources[i].clip = clips[i];
		}
		return sources;
	}

	private uint currentBank {
		get {
			return driftTime >= driftThreshold ? 1u : 0u;
		}
	}
}
