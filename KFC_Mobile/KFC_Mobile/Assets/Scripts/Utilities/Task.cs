using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task {

	public enum TaskStatus : byte 
	{     
		Detached, // Task has not been attached to a TaskManager     
		Pending, // Task has not been initialized     
		Working, // Task has been initialized     
		Success, // Task completed successfully     
		Fail, // Task completed unsuccessfully     
		Aborted // Task was aborted 
	}  

	public TaskStatus Status { get; private set; }  

	public bool IsDetached { get { return Status == TaskStatus.Detached;}}
	public bool IsAttached { get { return Status != TaskStatus.Detached;}}
	public bool IsPending { get { return Status == TaskStatus.Pending; } }
	public bool IsWorking { get { return Status == TaskStatus.Working; } }
	public bool IsSuccessful {get{ return Status == TaskStatus.Success;}} 
	public bool IsFailed { get { return Status == TaskStatus.Fail; } }
	public bool IsAborted { get { return Status == TaskStatus.Aborted; } } 
	public bool IsFinished { get { return (Status == TaskStatus.Fail || Status == TaskStatus.Success || Status == TaskStatus.Aborted); } }  

	public void Abort() 
	{     
		SetStatus(TaskStatus.Aborted); 
	}

	internal void SetStatus(TaskStatus newStatus) 
	{     
		if (Status == newStatus) return;      
		Status = newStatus;      
		switch (newStatus)     
		{         
			case TaskStatus.Working:             
				Init();             
				break;
			case TaskStatus.Success:
				OnSuccess();             
				CleanUp();             
				break;          
			case TaskStatus.Aborted:             
				OnAbort();             
				CleanUp();             
				break;
			case TaskStatus.Fail:             
				OnFail();             
				CleanUp();             
				break;          
			// These are "internal" states that are relevant for         
			// the task manager         
			case TaskStatus.Detached:
			case TaskStatus.Pending:
				break;
			default:
				throw new System.ArgumentOutOfRangeException(newStatus.ToString(), newStatus, null);     
		} 
	}

	protected virtual void OnAbort() {}
	protected virtual void OnSuccess() {}  
	protected virtual void OnFail() {}

	// Override this to handle initialization of the task. 
	// This is called when the task enters the Working state 

	protected virtual void Init() { }  

	// Called whenever the TaskManager updates. Your tasks' work 
	// generally goes here 

	internal virtual void Update() { }  

	// This is called when the tasks completes (i.e. is aborted, 
	// fails, or succeeds). It is called after the status change 
	// handlers are called 

	protected virtual void CleanUp() { }

	// Assign a task to be run if this task runs successfully 
	public Task NextTask { get; private set; }  

	// Sets a task to be automatically attached when this one completes successfully 
	// NOTE: if a task is aborted or fails, its next task will not be queued 
	// NOTE: **DO NOT** assign attached tasks with this method. 
	public Task Then(Task task) 
	{     
		Debug.Assert(!task.IsAttached);     
		NextTask = task;     
		return task; 
	}

}

public class ActionTask : Task {
	private readonly System.Action _action;

	public ActionTask(System.Action action)     
	{         
		_action = action;     
	}      

	protected override void Init()     
	{         
		SetStatus(TaskStatus.Success);       
		_action();     
	} 
}

public class OnGoingTask : Task {
	// Get the timestamp in floating point milliseconds from the Unix epoch     

	private static readonly System.DateTime UnixEpoch = new System.DateTime(1970, 1, 1);   

	private static double GetTimestamp()     
	{         
		return (System.DateTime.UtcNow - UnixEpoch).TotalMilliseconds;     
	}      

	private readonly System.Action _action;
	private readonly double _duration;     
	private double _startTime;      

	public OnGoingTask(double duration, System.Action action)     
	{         
		this._duration = duration;
		this._action = action;
	}

	public OnGoingTask (float duration, System.Action action)
	{
		this._duration = duration*1000;
		this._action = action;
	} 

	public OnGoingTask (int duration, System.Action action)
	{
		this._duration = duration*1000;
		this._action = action;
	}  

	protected override void Init()     
	{         
		_startTime = GetTimestamp();     
	}      

	internal override void Update()     
	{         
		var now = GetTimestamp();
		var durationElapsed = (now - _startTime) > _duration;         

		if (durationElapsed)         
		{             
			SetStatus(TaskStatus.Success);         
		}
		_action();
	} 
}

public class WaitTask : Task
{
    // Get the timestamp in floating point milliseconds from the Unix epoch   
    private static readonly System.DateTime UnixEpoch = new System.DateTime(1970, 1, 1);

    private static double GetTimestamp()
    {
        return (System.DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
    }

    private readonly double _duration; //how long does this wait for
    private double _startTime; //when did we start waiting

    public WaitTask(double duration)
    {
        this._duration = duration;
    }

    protected override void Init()
    {
        _startTime = GetTimestamp();
    }

    internal override void Update()
    {
        var now = GetTimestamp(); //use var for a) less typing, b) if it changes from float, to int, to double, etc.
        var durationElapsed = (now - _startTime) > _duration;

        if (durationElapsed)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}

// A base class for tasks that track time. Use it to make things like
// Wait, ScaleUpOverTime, etc. tasks
public abstract class TimedTask : Task
{
    public float Duration { get; private set; }
    public float StartTime { get; private set; }

    protected TimedTask(float duration)
    {
        Debug.Assert(duration > 0, "Cannot create a timed task with duration less than 0");
        Duration = duration;
    }

    protected override void Init()
    {
        StartTime = Time.time;
    }

    internal override void Update()
    {
        var now = Time.time;
        var elapsed = now - StartTime;
        var t = Mathf.Clamp01(elapsed / Duration);
        OnTick(t);
        if (t >= 1)
        {
            OnElapsed();
        }
    }

    // t is the normalized time for the task. E.g. if half the task's duration has elapsed then t == 0.5
    // This is where subclasses will do most of their work
    protected virtual void OnTick(float t) { }

    // Default to being successful if we get to the end of the duration
    protected virtual void OnElapsed()
    {
        SetStatus(TaskStatus.Success);
    }

}



// A VERY simple wait task
public class Wait : TimedTask
{
    public Wait(float duration) : base(duration) { }
}


////////////////////////////////////////////////////////////////////////
// GAME OBJECT TASKS
////////////////////////////////////////////////////////////////////////

// Base classes for tasks that operate on a game object.
// Since C# doesn't allow multiple inheritance we'll make two versions - one timed and one untimed
public abstract class GOTask : Task
{
    protected readonly GameObject gameObject;

    protected GOTask(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
}


public abstract class TimedGOTask : TimedTask
{
    protected readonly GameObject gameObject;

    protected TimedGOTask(GameObject gameObject, float duration) : base(duration)
    {
        this.gameObject = gameObject;
    }
}

// A task to teleport a gameobject
public class SetPos : GOTask
{
    private readonly Vector3 _pos;

    public SetPos(GameObject gameObject, Vector3 pos) : base(gameObject)
    {
        _pos = pos;
    }

    protected override void Init()
    {
        gameObject.transform.position = _pos;
        SetStatus(TaskStatus.Success);
    }
}


// A task to lerp a gameobject's position
public class LerpPos : TimedGOTask
{
    public Vector3 Start { get; private set; }
    public Vector3 End { get; private set; }

    public LerpPos(GameObject gameObject, Vector3 start, Vector3 end, float duration) : base(gameObject, duration)
    {
        Start = start;
        End = end;
    }

    protected override void OnTick(float t)
    {
        gameObject.transform.position = Vector3.Lerp(Start, End, t);
    }
}

// A task to lerp a gameobject's position
public class WanderAround : TimedGOTask
{
    public Vector3 Start { get; private set; }
    public Vector3 End { get; private set; }

    public WanderAround(GameObject gameObject, Vector3 start, Vector3 end, float duration) : base(gameObject, duration)
    {
        Start = start;
        End = end;
    }

    protected override void OnTick(float t)
    {
        gameObject.transform.position = Vector3.Lerp(Start, End, t);
        if (gameObject.GetComponent<MyCritter>().nearestEnemy != null || Services.Touch.touchCount > 0) Abort();
    }

    protected override void OnSuccess()
    {
        gameObject.GetComponent<MyCritter>().wandering = false;
    }

    protected override void OnAbort()
    {
        gameObject.GetComponent<MyCritter>().wandering = false;
    }
}

public class Jump : TimedGOTask
{
    public Vector3 Start { get; private set; }
    public Vector3 End { get; private set; }

    public Jump(GameObject gameObject, Vector3 start, Vector3 end, float duration) : base(gameObject, duration)
    {
        Start = start;
        End = end;
    }

    protected override void OnTick(float t)
    {
        gameObject.transform.position = Vector3.Lerp(Start, End, t);
    }
}

public class LerpRotationBetweenTwoSetQuaternions : TimedGOTask
{
    private Quaternion Start;
    private Quaternion End;

    public LerpRotationBetweenTwoSetQuaternions(GameObject gameObject, Quaternion _start, Quaternion _end, float duration) : base(gameObject, duration)
    {
        Start = _start;
        End = _end;
    }

    protected override void OnTick(float t)
    {
        gameObject.transform.rotation = Quaternion.Lerp(Start, End, t);
    }
}

public class LerpEulerAngles : TimedGOTask
{
    private Vector3 Start;
    private Vector3 End;

    public LerpEulerAngles(GameObject gameObject, Vector3 _start, Vector3 _end, float duration) : base(gameObject, duration)
    {
        Start = _start;
        End = _end;
    }

    protected override void OnTick(float t)
    {
        gameObject.transform.eulerAngles = Vector3.Lerp(Start, End, t);
    }
}


// A task to lerp a gameobject's scale
public class Scale : TimedGOTask
{
    public Vector3 Start { get; private set; }
    public Vector3 End { get; private set; }

    public Scale(GameObject gameObject, Vector3 start, Vector3 end, float duration) : base(gameObject, duration)
    {
        Start = start;
        End = end;
    }

    protected override void OnTick(float t)
    {
        gameObject.transform.localScale = Vector3.Lerp(Start, End, t);
    }
}

// A task to lerp a Canvas Alpha
public class FadeUIAlpha : TimedGOTask
{
    private Color _color;
    private float _startAlpha;
    private float _endAlpha;

    public FadeUIAlpha(GameObject gameObject, Color color, float startAlpha, float endAlpha, float duration) : base(gameObject, duration)
    {
        _color = color;
        _startAlpha = startAlpha;
        _endAlpha = endAlpha;
        
    }

    protected override void OnTick(float t)
    {
        gameObject.GetComponent<Image>().color = Color.Lerp(
                                                        new Color(_color.r, _color.g, _color.b, _startAlpha),
                                                        new Color(_color.r, _color.g, _color.b, _endAlpha), t);
    }
}
