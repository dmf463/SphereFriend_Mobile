using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Services {

    private static PlayerAI _playerAI;
    public static PlayerAI PlayerAI
    {
        get
        {
            Debug.Assert(_playerAI != null);
            return _playerAI;
        }
        set
        {
            _playerAI = value;
        }
    }

    private static TouchManager _touch;
    public static TouchManager Touch
    {
        get
        {
            Debug.Assert(_touch != null);
            return _touch;
        }
        set
        {
            _touch = value;
        }
    }

    private static MyGameManager _gm;
    public static MyGameManager GM
    {
        get
        {
            Debug.Assert(_gm != null);
            return _gm;
        }
        set
        {
            _gm = value;
        }
    }

	private static AudioSystem _audio;
	public static AudioSystem Audio     
	{         
		get 
		{             
			Debug.Assert(_audio != null);             
			return _audio;
		}         
		set 
		{ 
			_audio = value; 
		}     
	}

    private static PrefabDB _prefabDB;
    public static PrefabDB PrefabDB
    {
        get
        {
            Debug.Assert(_prefabDB != null);
            return _prefabDB;
        }
        set
        {
            _prefabDB = value;
        }
    }

    private static MyEventManager _event_manager;
	public static MyEventManager Event_Manager
	{
		get
		{
			Debug.Assert (_event_manager != null);
			return _event_manager;
		}
		set{
			_event_manager = value;
		}
	}

	private static LevelManager _level_manager;
	public static LevelManager Level_Manager
	{
		get
		{
			Debug.Assert (_level_manager != null);
			return _level_manager;
		}
		set{
			_level_manager = value;
		}
	}

	private static ObjectPools _objects;
	public static ObjectPools Objects
	{
		get
		{
			Debug.Assert(_objects != null);
			return _objects;
		}
		set
		{
			_objects = value;
		}
	}

	private static Config _config;
	public static Config Config
	{
		get
		{
			Debug.Assert(_config != null);
			return _config;
		}
		set
		{
			_config = value;
		}
		
	}


}
