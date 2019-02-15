using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager {

	protected int _stored_int;

	public void Initialize() {
		_stored_int = 0;

		RegisterLevelChange<OpeningOver>("Main");
		RegisterLevelChange<GameWon>("Won");
		RegisterLevelChange<GameLost>("Lost");
		RegisterLevelChange<RestartGame>("Opening");
		RegisterLevelChange<GameStarted>("Opening");
		RegisterLevelChange<ReturnToTitle>("Title Screen");
		Services.Event_Manager.Register<Quit>(QuitEvent);
	}

	public void Destroy ()
	{
		UnregisterLevelChange<OpeningOver>("Main");
		UnregisterLevelChange<GameWon>("Won");
		UnregisterLevelChange<GameLost>("Lost");
		UnregisterLevelChange<RestartGame>("Opening");
		UnregisterLevelChange<GameStarted>("Opening");
		UnregisterLevelChange<ReturnToTitle>("Title Screen");
		Services.Event_Manager.Unregister<Quit>(QuitEvent);
	}

	public void LoadLevel (string name)
	{
		SceneManager.LoadScene(name);
	}

	public void LoadLevel (string name, int f_in)
	{
		SceneManager.LoadScene(name);
		_stored_int = f_in;
	}

	public int ReturnNumber ()
	{
		return _stored_int;
	} 

	public void QuitRequest () {
		Application.Quit();
	}
	
	public void QuitEvent (MyEvent e) {
		Application.Quit();
	}

	public void LoadNextLevelByIndex ()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
	
	internal void ChangeLevel(MyEvent e, string level_name)
	{
		LoadLevel(level_name);
	}
	
	private void UnregisterLevelChange<T>(string level_name) where T : MyEvent
    {
		System.Action<MyEvent> to_unregister = (MyEvent e) =>
		{
			ChangeLevel(e, level_name);
		};
		
		Services.Event_Manager.Unregister<T>(to_unregister.Invoke);
	}
	
	private void RegisterLevelChange<T>(string level_name) where T : MyEvent
    {
		System.Action<MyEvent> to_register = (MyEvent e) =>
		{
			ChangeLevel(e, level_name);
		};
		
		Services.Event_Manager.Register<T>(to_register.Invoke);
	}

}
