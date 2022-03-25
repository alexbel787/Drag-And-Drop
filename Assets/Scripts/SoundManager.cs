using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioSource soundSource;
	public static SoundManager instance = null;     
	[SerializeField] private float lowPitchRange = .9f;
	[SerializeField] private float highPitchRange = 1.05f;            

	[Header("Item Sounds")]
	public AudioClip[] itemSounds;
	public AudioClip[] fallingSounds;
	[Header("Voice Sounds")]
	public AudioClip noSound;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void PlaySingle(float volume, AudioClip clip)
	{
		soundSource.pitch = Random.Range(lowPitchRange, highPitchRange);
		soundSource.PlayOneShot(clip, volume);
	}

}

