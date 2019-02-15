using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem {
	
	// *************************** EXPLANATION *************************** 
	// How to reference:
	// Almost always, there will be a static member variable in "Services"
	// for the current AudioSystem.  In most cases, you can access this
	// through "Services.Audio.(whatever function call)", although for 
	// sound effects in particular, they should be referenced here.
	//
	// Adding Sound Effects:
	// 		Inside _RegisterSoundEffects, add:
	//
	// 		RegisterEffect<[EVENT NAME]>			or 				RegisterMusic<[EVENT NAME]>
	// 			("[NAME OF EFFECT]");									("[NAME OF EFFECT]");
	//
	//		Replacing [EVENT NAME] with the name of the event the sound is attached to.
	//		And "[NAME OF EFFECT]" with a string of the name of the vent.
	//
	//		Inside _UnregisterSoundEffects, add:
	//
	//		UnregisterEffect<[EVENT NAME]> 			or 				UnregisterMusic<[EVENT NAME]>
	// 			("[NAME OF EFFECT]")									("[NAME OF EFFECT]");
	//	
	// Other functions / variables:
	// Initialize() - reads in sound effects, and creates the audiosources
	// Update() - updates the fader task manager
	// ChangeMusic(string) - changes the music to input string by fading out current music.
	// FadeOut() - fades out current music.
	// StartMusic(string) - changes the music to input string immediately
	// StopMusic() - stops current music
	// PlaySoundEffect(string) - plays string sound effect.
	// ******************************************************************* 

	private AudioSource music_player, fade_player;
	private List<AudioClip> music_sources;
	private List<AudioClip> effect_sources;
	private List<AudioSource> effect_players;
	private TaskManager _fader;
	private float fade_start_time;
	private GameObject AudioSourceHolder;

	private static float _fade_speed = 2.0f;
	public static float Fade_Speed {
		get {
			return _fade_speed;
		}
		set {
			_fade_speed = value;
		}
	}

	// Use this for initialization
	public void Initialize (float fade_speed) {
		AudioSourceHolder = GameObject.Find("Audio Source Holder");
		effect_players = new List<AudioSource>();

		if (AudioSourceHolder == null)
		{
			AudioSourceHolder = new GameObject("Audio Source Holder");
			GameObject.DontDestroyOnLoad(AudioSourceHolder);
			music_player = AudioSourceHolder.AddComponent<AudioSource>();
			fade_player = AudioSourceHolder.AddComponent<AudioSource>();
		}
		else
		{
			AudioSource[] sources = AudioSourceHolder.GetComponents<AudioSource>();

			switch (sources[0].isPlaying)
			{
				case (true) :
				{
					music_player = sources[0];
					fade_player = sources[1];
					break;
				}
				case (false):
				{
					music_player = sources[1];
					fade_player = sources[0];
					break;
				}
			}
		}
		
		
		music_sources = new List<AudioClip>();
		effect_sources = new List<AudioClip>();
		_PopulateEffects();
		_PopulateMusic();

		_fader = new TaskManager();
		Fade_Speed = fade_speed;
	}

	public void Update()
	{
		_fader.Update();
		AudioSourceHolder.transform.position = Camera.main.transform.position;

		List<AudioSource> to_destroy = new List<AudioSource>();
		
		foreach (AudioSource player in effect_players)
		{
			if (!player.isPlaying)
			{
				effect_players.Remove(player);
				to_destroy.Add(player);
			}
		}

		foreach (AudioSource player in to_destroy)
		{
			GameObject.Destroy(player);
		}
		
	}

	public void DestroyHolder()
	{
		GameObject.Destroy(AudioSourceHolder);
	}

	public void Destroy()
	{
		
	}

	internal void _PopulateEffects ()
	{
		Object[] effects = Resources.LoadAll ("Effects", typeof(AudioClip));

		foreach (Object o in effects) {
			effect_sources.Add ((AudioClip)o);
		}
	}

	internal void _PopulateMusic ()
	{
		Object[] music = Resources.LoadAll ("Music", typeof(AudioClip));

		foreach (Object o in music) {
			music_sources.Add ((AudioClip)o);
		}
	}

	public void ChangeMusic (string music_name_in)
	{
		fade_start_time = Time.unscaledTime;
		AudioClip found_music = _FindMusic (music_name_in);
		if (found_music != null) {
			fade_player.clip = found_music;
			fade_player.loop = true;
			fade_player.Play ();
			fade_player.volume = 0.0f;
		}

		OnGoingTask cross_fade = new OnGoingTask(_fade_speed, _CrossFade);

		cross_fade
			.Then(new ActionTask(_SwitchPlayers));

		_fader.Do(cross_fade);
	}

	private System.Action<Event> _SendHandler(System.Action<Event, string> function, string input_string)
	{
		System.Action<Event> to_return = (Event e) =>
		{
			function(e, input_string);
		};
		return to_return;
	}

	private void RegisterEffect<T>(string effect_name) where T : MyEvent
    {
		System.Action<MyEvent> to_register = (MyEvent e) =>
		{
			EventSoundEffect(e, effect_name);
		};
		
		Services.Event_Manager.Register<T>(to_register.Invoke);
	}

	private void UnregisterMusic<T>(string music_name) where T : MyEvent
    {
		System.Action<MyEvent> to_unregister = (MyEvent e) =>
		{
			EventChangeMusic(e, music_name);
		};
		
		Services.Event_Manager.Unregister<T>(to_unregister.Invoke);
	}
	
	private void RegisterMusic<T>(string music_name) where T : MyEvent
    {
		System.Action<MyEvent> to_register = (MyEvent e) =>
		{
			EventChangeMusic(e, music_name);
		};
		
		Services.Event_Manager.Register<T>(to_register.Invoke);
	}
	
	private void UnregisterEffect<T>(string effect_name) where T : MyEvent
    {
		System.Action<MyEvent> to_unregister = (MyEvent e) =>
		{
			EventChangeMusic(e, effect_name);
		};
		
		Services.Event_Manager.Unregister<T>(to_unregister.Invoke);
	}

	private void EventSoundEffect(MyEvent e, string effect_name)
	{
		PlaySoundEffect(effect_name);
	}
	
	private void EventChangeMusic(MyEvent e, string music_name)
	{
		ChangeMusic(music_name);
	}
	
	public void StartMusic (string music_name)
	{
		AudioClip found_music = _FindMusic (music_name);
		if (found_music != null) {
			music_player.clip = found_music;	
			music_player.loop = true;
			music_player.Play ();
		}
	}

	public void StopMusic ()
	{
		if (music_player.isPlaying) {
			music_player.Stop();
		}

		if (fade_player.isPlaying) {
			fade_player.Stop();
		}
	}

	public void StopMusicEvent(MyEvent e)
	{
		StopMusic();
	}

	public void FadeOutEvent(MyEvent e)
	{
		FadeOut();
	}

	public void FadeOut() {
		fade_start_time = Time.unscaledTime;
		fade_player.volume = 0.0f;

		OnGoingTask fade_out = new OnGoingTask(_fade_speed, _FadeOut);

		_fader.Do(fade_out);
	}

	public void PlaySoundEffect (string effect_name)
	{
		var effect_player = AudioSourceHolder.AddComponent<AudioSource>();
		effect_player.clip = _FindEffect(effect_name);
		effect_player.loop = false;
		effect_player.Play();
		effect_players.Add(effect_player);
	}

	private void _CrossFade() {
		music_player.volume = Mathf.Lerp (1f, 0f, (Time.unscaledTime - fade_start_time)/_fade_speed);
		fade_player.volume = Mathf.Lerp (0f, 1f, (Time.unscaledTime - fade_start_time)/_fade_speed);
	}

	private void _SwitchPlayers() {
		GameObject.Destroy(music_player);
		music_player = fade_player;
		fade_player = AudioSourceHolder.AddComponent<AudioSource>();
	}

	private void _FadeOut() {
		music_player.volume = Mathf.Lerp (1f, 0f, (Time.unscaledTime - fade_start_time)/_fade_speed);
	}

	private AudioClip _FindEffect (string name)
	{
		foreach (AudioClip clip in effect_sources) {
			if (clip.name == name) {
				return clip;
			}
		}
		Debug.Log("Couldn't find clip " + name);
		return null;
	}

	private AudioClip _FindMusic (string name)
	{
		foreach (AudioClip clip in music_sources) {
			if (clip.name == name) {
				return clip;
			}
		}
		Debug.Log("Couldn't find clip " + name);
		return null;
	}

	public void StopSoundEffect(string name)
	{
		var clip = _FindEffect(name);
		
		for (int i = 0; i < AudioSourceHolder.transform.childCount; i++)
		{
			var player = AudioSourceHolder.transform.GetChild(i).GetComponent<AudioSource>();
			if (player.clip == clip)
			{
				player.Stop();
			}
		}
	}
}




