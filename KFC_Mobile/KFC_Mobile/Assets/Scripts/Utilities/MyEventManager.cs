using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEventManager {

	private Dictionary<System.Type, MyEvent.Handler> registered_handlers;

	public void Initialize() {
		registered_handlers = new Dictionary<System.Type, MyEvent.Handler>();
	}

	public void Register<T>(MyEvent.Handler handler) where T : MyEvent
    {
		System.Type type = typeof(T);
		if (registered_handlers.ContainsKey(type)) {
			if (!IsEventHandlerRegistered(type, handler))
				registered_handlers[type] += handler;         
		} else {
			registered_handlers.Add(type, handler);         
		}     
	} 

	public void Unregister<T>(MyEvent.Handler handler) where T : MyEvent
    {         
		System.Type type = typeof(T);
        MyEvent.Handler handlers;         
		if (registered_handlers.TryGetValue(type, out handlers)) {             
			handlers -= handler;             
			if (handlers == null) {                 
				registered_handlers.Remove(type);             
			} else {
				registered_handlers[type] = handlers;             
			}         
		}     
	}      
		
	public void Fire(MyEvent e) 
	{         
		System.Type type = e.GetType();
        MyEvent.Handler handlers;   
		
		if (registered_handlers.TryGetValue(type, out handlers)) {             
			handlers(e);
		}     
	} 

	public bool IsEventHandlerRegistered (System.Type type_in, System.Delegate prospective_handler)
	{   
		foreach (System.Delegate existingHandler in registered_handlers[type_in].GetInvocationList()) {
			if (existingHandler == prospective_handler) {
				return true;
			}
		}
	    return false;
	}

}

public abstract class MyEvent {
	public readonly float creation_time;

	public MyEvent()
	{
		creation_time = Time.time;
	}

	public delegate void Handler (MyEvent e);
}

public class UnderWater : MyEvent { }

public class AboveWater : MyEvent { }

public class GameStarted : MyEvent { }

public class ReturnToTitle : MyEvent { }

public class Quit : MyEvent { }

public class OpeningOver : MyEvent { }

public class RingHit : MyEvent { }

public class BallHit : MyEvent { }

public class GameWon : MyEvent { }

public class LevelStarted : MyEvent { }

public class GameLost : MyEvent { }

public class TimeRunningOut : MyEvent { }

public class RestartGame : MyEvent { }