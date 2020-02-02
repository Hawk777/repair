using UnityEngine;

public class MusicManager : MonoBehaviour {
	[Tooltip("The main music layers")]
	public AudioClip[] main = new AudioClip[6];

	[Tooltip("The EuroBeat music layers")]
	public AudioClip[] euro = new AudioClip[6];

	[Tooltip("The template AudioSource")]
	public GameObject sourceTemplate;

	// The one and only instance.
	private static MusicManager instance;

	// The audio sources.
	private AudioSource[] mainSources, euroSources;

	// Gets the MusicManager.
	public static MusicManager Get() {
		return instance;
	}

	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
			mainSources = MakeSources(main);
			euroSources = MakeSources(euro);
		} else {
			Destroy(gameObject);
		}
	}

	void Start() {
		if(instance == this) { /* not sure if destruction happens between Awake and Start or after Start */
			double startTime = AudioSettings.dspTime + 0.5;
			foreach(AudioSource i in mainSources) {
				i.PlayScheduled(startTime);
			}
			foreach(AudioSource i in euroSources) {
				i.PlayScheduled(startTime);
			}
			mainSources[0].volume = 1f;
		}
	}

	private AudioSource[] MakeSources(AudioClip[] clips) {
		AudioSource[] sources = new AudioSource[clips.Length];
		for(uint i = 0; i != sources.Length; ++i) {
			sources[i] = Instantiate(sourceTemplate).GetComponent<AudioSource>();
			sources[i].clip = clips[i];
		}
		return sources;
	}
}
