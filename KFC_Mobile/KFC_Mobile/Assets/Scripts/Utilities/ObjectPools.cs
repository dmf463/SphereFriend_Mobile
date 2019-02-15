using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPools
{

	Dictionary<string, ObjectPool> object_pools;
	
	// Use this for initialization
	public void Initialize ()
	{
		object_pools = new Dictionary<string, ObjectPool>();
	}

	public void Destroy()
	{
		Dictionary<string, ObjectPool>.ValueCollection values = object_pools.Values;

		foreach (ObjectPool pool in values)
		{
			pool.DestroyAll();
		}
		
		object_pools = null;
	}

	public bool CreateObjectPool(string name, int size, GameObject prefab)
	{
		if (!object_pools.ContainsKey(name))
		{
			object_pools.Add(name, new ObjectPool(name, size, prefab));
			return true;
		}
		return false;
	}

	public GameObject Get(string name)
	{
		if (object_pools.ContainsKey(name))
		{
			return object_pools[name].Next();
		}

		return null;
	}
}

public class ObjectPool {

	List<GameObject> pool;
	private int current_location;
	private int size;
	
	public ObjectPool(int size_in, GameObject prefab){
		pool = new List<GameObject>();
		size = size_in;
		
		for(int i = 0 ; i < size; i++){
			GameObject obj = GameObject.Instantiate(prefab);
			pool.Add(obj);
			obj.SetActive(false);
		}

		current_location = 0;
	}
	
	public ObjectPool(string name, int size_in, GameObject prefab){
		pool = new List<GameObject>();
		size = size_in;
		GameObject holder = new GameObject(name);
		
		for(int i = 0 ; i < size; i++){
			GameObject obj = GameObject.Instantiate(prefab);
			pool.Add(obj);
			obj.transform.SetParent(holder.transform);
			obj.SetActive(false);
		}

		current_location = 0;
	}
	
	public GameObject Next(){
		if(pool.Count > 0){ 
			current_location = (current_location + 1) % size;
			pool[current_location].SetActive(true);
			return pool[current_location];
		}
		return null;
	}

	public void DestroyAll()
	{
		foreach (GameObject obj in pool)
		{
			GameObject.Destroy(obj);
		}
	}
}