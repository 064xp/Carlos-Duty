using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : ScriptableObject {
	public AudioClip[] clips;

	public RangedFloat volume;

	public RangedFloat pitch;

	public void Play(AudioSource source) {
		if (clips.Length == 0) return;

		source.clip = clips[Random.Range(0, clips.Length)];
		source.volume = Random.Range(volume.minValue, volume.maxValue);
		source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
		source.Play();
	}
}