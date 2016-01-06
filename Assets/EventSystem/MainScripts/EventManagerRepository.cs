using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public sealed class EventManagerRepository
{
	private Dictionary<Type, EventManager> eventManagers = new Dictionary<Type, EventManager>();
	
	static EventManagerRepository instance;
	static readonly System.Object padlock = new System.Object();
	
	private EventManagerRepository(){}
	
	public static EventManagerRepository Instance
	{
		get
		{
			if(instance == null) //If instance hasn't been found/created yet
			{
				lock(padlock)
				{
					if(instance == null)
					{
						instance = new EventManagerRepository();
						EventManagerRepositoryCleaner cleaner = (EventManagerRepositoryCleaner)Object.FindObjectOfType(typeof(EventManagerRepositoryCleaner));
						if(cleaner == null)
							(new GameObject("Repository Cleaner")).AddComponent<EventManagerRepositoryCleaner>().Initialize(instance.CleanupRepository);
					}
				}
			}
			return instance;
		}
	}
	
	
	private void CleanupRepository()
	{
		List<Type> keysBuffer = new List<Type>(eventManagers.Keys);
		List<EventManager> valuesBuffer = new List<EventManager>(eventManagers.Values);
		for(int i = 0; i < keysBuffer.Count; i++)
		{
			valuesBuffer[i].RemoveInterimEvents(); //Call cleanup on each object
			if(!valuesBuffer[i].SaveAllEvents && !valuesBuffer[i].HasPersistentEvents)
				eventManagers.Remove(keysBuffer[i]);				
		}
	}
	
	
	//For retrieving event managers
	public EventManager<TEventType> GetSingleInstanceEventManager<TEventType>() where TEventType : struct, IComparable, IConvertible, IFormattable
	{
		EventManager eventManager;
		
		eventManagers.TryGetValue(typeof(TEventType), out eventManager);
		if(eventManager == null)
		{
			EventManager<TEventType> tempManager = new EventManager<TEventType>();
			if(tempManager != null) //In case construction throws an error
				eventManagers.Add(typeof(TEventType), tempManager);
			return tempManager;
		}
		else
			return (EventManager<TEventType>)eventManager;
	}
	
	//Removes the event manager from the collection of single instance event managers.
	//Be cautious when using this. If some other script expects the manager to be there and it has been removed,
	//a new manager will be created, which may result in buggy code. Be certain you no longer need the manager, or that
	//you want a fresh version of the manager the next time it's accessed!
	public bool RemoveEventManager<TEventType>()
	{
		return eventManagers.Remove(typeof(TEventType));
	}
}