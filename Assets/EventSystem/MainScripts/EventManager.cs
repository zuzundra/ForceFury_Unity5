/*
 * Advanced Generic C# messenger
 * Note, this script is heavily modified from the original Advanced C# messenger script found on the wiki
 * 
 * Based on Ilya Suzdalnitski's Advanced C# messenger, which is in turn based on
 * Rod Hyde's "CSharpMessenger" and Magnus Wolffelt's "CSharpMessenger Extended".
 * 
 * Features:
 	* Create an event manager for any Enum based on the System.Int32 (int) type
 	* Option to log all messages
 	* Use of generics provides error detection and safeguards
 	* Create global event managers that can be accessed by any script (by using the repository)
 * 
 * Usage examples:
 	1. To create a new event (this assumes you have an enum named GlobalEvent):
 		A) EventManager<GlobalEvent> globalEventManager = new EventManager<GlobalEvent>();
 		
 	2. To add a listener to an eventManager
 		A) globalEventManager.AddListener(GlobalEvent.DoSomething, FunctionToHandleDoSomethingEvent);
 		B) globalEventManager.AddListener<float>(GlobalEvent.DoSomething, FunctionToHandleDoSomethingEventWithFloatParamater);
 	
 	3. To broadcast an event
 		A) globalEventManager.Broadcast(GlobalEvent.DoSomething);
 		B) globalEventManager.Broadcast<float>(GlobalEvent.DoSomething, 4f);
 
 	4. Accessing a single instance event via the EventManagerRepository. Single instance events are useful if you need to be able to
 	   access the same event manager from many different scripts.
 	   	
 		A) EventManagerRepository.Instance.GetSingleInstanceEventManager<GlobalEvent>().Broadcast<float>(GlobalEvent.DoSomething, 4f);
 		
 		If you are accessing an event manager in the repository frequently, store a reference in the script where you need it instead of
 		using the GetSingleInstanceEventManager method.
 * 
 * The repository cleans up all event managers automatically when a new level is loaded. By default, all events are cleared from each
 * event manager and the managers are removed from the repository. 
 * 
 * If you want an event to survive a level load, mark it as persistent via the MarkEventAsPersistent method, like so:
 * 		A) globalEventManager.MarkEventAsPersistent(GlobalEvent.DoSomething);
 * 
 * If later you want the event to not survive a level load, you can use the MarkEventAsInterim method in the same manner.
 * 
 * Alternatively, you can set the SaveAllEvents value of an event manager to true if you want all events to survive a level load, like so:
 * 		A) globalEventManager.SaveAllEvents = true;
 * 
 * Please note, this does not mark each event as persistent, instead it simply tells the event manager to skip the cleanup phase when a level is loaded.
 * So later, if you set SaveAllEvents to false, and there are no persistent messages, the event manageer will be removed
 * from the repository when a new level is loaded. Regardless of SaveAllEvents value, you can change an event to/from persistent/interim
 * at any time (but note an events persistent/interim status only matters when SaveAllEvents is false).
 * 
 * One final caveat. The repository removes event managers automatically upon level load 
 * (as long as the manager has no persistent events and its SaveAllEvents value is set to false), 
 * or when RemoveEventManager is called, but this does not clear the event manager from memory. 
 * If you have a script which references the eventManager, it will remain in memory. Always make sure to get a fresh single instance
 * event manager from the repository if you expect a new version, and null out any references to event managers when you no longer are
 * using them. The cleanup process is designed to try and limit errors between different levels, but it is not completely error proof. Some
 * knowledge of delegates/c# memory management is required to avoid exceptions.
 */
 
//#define LOG_ALL_MESSAGES
//#define LOG_ADD_LISTENER
//#define LOG_BROADCAST_MESSAGE

//Uncomment this if you want A broadcast method call to produce an error if there are no listeners
//#define REQUIRE_LISTENER

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//Every generic event manager derives from this abstract class. This allows
//the repository to handle each manager no matter it's <Type>.
public abstract class EventManager
{
	public abstract bool SaveAllEvents { get; set; }
	public abstract bool HasPersistentEvents 	{ get; }	
	
	public abstract void RemoveInterimEvents(); 
}

//We're trying to constrain the generic to an enum, but that is not possible in .Net currently.
//We will settle for using some constraints particular to an enum
public class EventManager<TEventType> : EventManager where TEventType : struct, IComparable, IConvertible, IFormattable
{ 
	private int numOfEventsOfThisType;
	private Delegate[] eventHandlers;
	
	//Function that allows us to convert an enum to int.
	//It's necessary because you can't cast a generic type to an int normally.
	//Convert.ToInt32(enum) and other methods work but they are a lot slower
	//than this dynamically created function.
	private Func<TEventType, int> ConvertEnumToInt;
		
	//Message handlers that should never be removed, regardless of calling RemoveInterimEvents
	private HashSet<int> persistentEvents;
	
	private bool saveAllEvents;
	private bool hasPersistentEvents;
	
	//Implementation of the base class, allows the EventManagerRepository to check whether
	//this object should be destroyed
	public override bool SaveAllEvents		{ get { return saveAllEvents; } set { saveAllEvents = value; } }
	public override bool HasPersistentEvents		{ get { return hasPersistentEvents; } }

	//Used to initialize our static variables, called the first time a method/variable is accessed
	public EventManager()
	{
		if(!typeof(TEventType).IsEnum) 
			throw new InvalidGenericTypeException("TEventType for generic class 'Event Manager' must be an enum!");

		hasPersistentEvents = false;
		saveAllEvents = false;
		persistentEvents = new HashSet<int>();
#if UNITY_WEBPLAYER && !UNITY_EDITOR
		ConvertEnumToInt = (TEventType t) => { return (int)(Enum.Parse(t.GetType(), Enum.GetName(t.GetType(), t))); };	//spike
#else
		ConvertEnumToInt = EnumConverterCreator.CreateFromEnumConverter<TEventType, int>();
#endif
		
		//Get the max number of events
		numOfEventsOfThisType = Enum.GetNames (typeof(TEventType)).Length;
		
		//Create the delegate array to hold the events -- they are all initialized to null to begin with
		eventHandlers = new Delegate[numOfEventsOfThisType];		
	}
	//Marks a certain message as permanent.
	public void MarkEventAsPersistent(TEventType eventName) 
	{
		int eventID = ConvertEnumToInt(eventName); 
		#if LOG_ALL_MESSAGES
		Debug.Log("GlobalEventManager MarkAsPermanent \t\"" + eventID + "\"");
		#endif
		
 		//Add the eventID to the hashset of persistent events. Duplicates will be ignored
		//Set hasPersistentEvents to true if the ID isn't a duplicate.
		if(persistentEvents.Add(eventID)) hasPersistentEvents = true;
	}
	
	//Removes event from the list of persistent events, which effectively 'marks' it as interim.
	public void MarkEventAsInterim(TEventType eventName)
	{
		int eventID = ConvertEnumToInt(eventName);
		
		//Tries to remove the event from the persistentEvents HashSet. If the event
		//is successfully removed and the hashset is empty after removal, set hasPersistentEvents to false.
		if(persistentEvents.Remove(eventID) && persistentEvents.Count == 0)
			hasPersistentEvents = false;
	}
 
 
	public override void RemoveInterimEvents()
	{
		#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER RemoveInterimEvents. Make sure that none of necessary listeners are removed.");
		#endif
 		
		if(saveAllEvents) //If save all events is set to true, do not remove interimEvents.
			return;
		
		//A new delegate array that will hold only those eventHandlers marked as permanent
 		Delegate[] tempGlobalEventHolder = new Delegate[numOfEventsOfThisType];
 
		//For every id stored in the list of persistentEvents, use that id to place a copy of the delegate stored in the current events in the tempGlobalEventHolder
		foreach (int eventID in persistentEvents) { tempGlobalEventHolder[eventID] = eventHandlers[eventID]; }
		
		//Replace the eventHandlers array with the temp one we just created
		eventHandlers = tempGlobalEventHolder;
	}
 
	public void PrintGlobalEventTable()
	{
		Debug.Log("MESSENGER PrintGlobalEventsTable ===");
 		Debug.Log("\n");
		
		for(int i = 0; i < eventHandlers.Length; i++) 
			Debug.Log("Event Name: " + (TEventType)(object)i + "  ||  Delegate Type: " + eventHandlers[i]);
		
		Debug.Log("\n");
	}
 	
	
	
	//Listener stuff
	
	
	//Called before a Listener is added. Used primarily to make sure the delegate supplied by the listener matches the delegate
	//in the array. If the delegate in the array at index 'eventID' is null or is of the same type as the listeners delegate, we
	//can add the listeners delegate. Otherwise an exception is thrown.
    private void OnListenerAdding(int eventID, Delegate listenerBeingAdded) 
	{
		#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
		Debug.Log("MESSENGER OnListenerAdding \t\"" + (TEventType)(object)eventID + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
		#endif

        Delegate tempDel = eventHandlers[eventID];
        if (tempDel != null && tempDel.GetType() != listenerBeingAdded.GetType()) 
		{
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", (TEventType)(object)eventID, tempDel.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }
 
	//Called from RemoveListener (when a listener request to be removed).
	//Primarily used to make sure the event the listener is trying to get removed from exist in the events array
    private void OnListenerRemoving(int eventID, Delegate listenerBeingRemoved) 
	{
		#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER OnListenerRemoving \t\"" + (TEventType)(object)eventID + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
		#endif
       
        Delegate tempDel = eventHandlers[eventID];
        /*if (tempDel == null) 
		{
            throw new ListenerException(string.Format("Attempting to remove listener for event type \"{0}\" but current listener is null.", (TEventType)(object)eventID));
        } 
		else*/
		if (tempDel != null && tempDel.GetType() != listenerBeingRemoved.GetType()) 
		{
            throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", (TEventType)(object)eventID, tempDel.GetType().Name, listenerBeingRemoved.GetType().Name));
        }
    }
 
    
 
	
	
	
	//No parameters
    public void AddListener(TEventType eventName, Action handler) 
	{
		int eventID = ConvertEnumToInt(eventName);
        OnListenerAdding(eventID, handler);
        eventHandlers[eventID] = (Action)eventHandlers[eventID] + handler;
    }
 
	//Single parameter
	public void AddListener<T>(TEventType eventName, Action<T> handler) 
	{
		int eventID = ConvertEnumToInt(eventName);
        OnListenerAdding(eventID, handler);
        eventHandlers[eventID] = (Action<T>)eventHandlers[eventID] + handler;
    }
 
	//Two parameters
	public void AddListener<T, U>(TEventType eventName, Action<T, U> handler) 
	{
		int eventID = ConvertEnumToInt(eventName);
        OnListenerAdding(eventID, handler);
        eventHandlers[eventID] = (Action<T, U>)eventHandlers[eventID] + handler;
    }
 
	//Three parameters
	public void AddListener<T, U, V>(TEventType eventName, Action<T, U, V> handler) 
	{
		int eventID = ConvertEnumToInt(eventName);
        OnListenerAdding(eventID, handler);
        eventHandlers[eventID] = (Action<T, U, V>)eventHandlers[eventID] + handler;
    }

	//Four parameters
	public void AddListener<T, U, V, C>(TEventType eventName, Action<T, U, V, C> handler) {
		int eventID = ConvertEnumToInt(eventName);
		OnListenerAdding(eventID, handler);
		eventHandlers[eventID] = (Action<T, U, V, C>)eventHandlers[eventID] + handler;
	}
	
	//No parameters
    public void RemoveListener(TEventType eventName, Action handler) 
	{
		int eventID = ConvertEnumToInt(eventName);
        OnListenerRemoving(eventID, handler);   
        eventHandlers[eventID] = (Action)eventHandlers[eventID] - handler;
    }
 
	//Single parameter
	public void RemoveListener<T>(TEventType eventName, Action<T> handler) 
	{
		int eventID = ConvertEnumToInt(eventName);
        OnListenerRemoving(eventID, handler);
        eventHandlers[eventID] = (Action<T>)eventHandlers[eventID] - handler;
    }
 
	//Two parameters
	public void RemoveListener<T, U>(TEventType eventName, Action<T, U> handler) 
	{
		int eventID = ConvertEnumToInt(eventName);
        OnListenerRemoving(eventID, handler);
        eventHandlers[eventID] = (Action<T, U>)eventHandlers[eventID] - handler;
    }
 
	//Three parameters
	public void RemoveListener<T, U, V>(TEventType eventName, Action<T, U, V> handler) 
	{
		int eventID = ConvertEnumToInt(eventName);
        OnListenerRemoving(eventID, handler);
        eventHandlers[eventID] = (Action<T, U, V>)eventHandlers[eventID] - handler;
    }

	//Four parameters
	public void RemoveListener<T, U, V, C>(TEventType eventName, Action<T, U, V, C> handler) {
		int eventID = ConvertEnumToInt(eventName);
		OnListenerRemoving(eventID, handler);
		eventHandlers[eventID] = (Action<T, U, V, C>)eventHandlers[eventID] - handler;
	}
 
	#if REQUIRE_LISTENER
    static public void OnBroadcasting(int eventID) 
	{
        if (!eventHandlers.ContainsKey(eventID)) 
		{
            throw new BroadcastException(int.Format("Broadcasting message \"{0}\" but no listener found. Try marking the message with GlobalEventManager.MarkAsPermanent.", (TEventType)(object)eventID));
        }
    }
	#endif
	
    private static BroadcastException CreateBroadcastSignatureException(int eventID) 
	{
        return new BroadcastException(string.Format("Broadcasting message for event \"{0}\" but listeners have a different signature than the broadcaster.", (TEventType)(object)eventID));
    }
	
	
	
	
	//Regular Broadcast
	
	//No parameters
    public void Broadcast(TEventType eventName) 
	{
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif
		
		#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
		#endif
		
        Delegate tempDel = eventHandlers[eventID];
        if (tempDel != null)
		{
            Action broadcastEvent = tempDel as Action;
 
            if (broadcastEvent != null) { broadcastEvent(); } 
			else { throw CreateBroadcastSignatureException(eventID); }
        }
    }
 
	//Single parameter
    public void Broadcast<T>(TEventType eventName, T arg1) 
	{
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif
        
		#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
		#endif
 
        Delegate tempDel = eventHandlers[eventID];
       	if (tempDel != null)
		{
            Action<T> broadcastEvent = tempDel as Action<T>;
 
            if (broadcastEvent != null) { broadcastEvent(arg1); } 
			else { throw CreateBroadcastSignatureException(eventID); }
        }
	}
 
	//Two parameters
    public void Broadcast<T, U>(TEventType eventName, T arg1, U arg2) 
	{
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif
        
		#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
		#endif
 
        Delegate tempDel = eventHandlers[eventID];
        if (tempDel != null)
		{
            Action<T, U> broadcastEvent = tempDel as Action<T, U>;
 
            if (broadcastEvent != null) { broadcastEvent(arg1, arg2); } 
			else { throw CreateBroadcastSignatureException(eventID); }
        }
    }
 
	//Three parameters
    public void Broadcast<T, U, V>(TEventType eventName, T arg1, U arg2, V arg3) 
	{
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif
        
		#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
		#endif
 
        Delegate tempDel = eventHandlers[eventID];
       	if (tempDel != null)
		{
            Action<T, U, V> broadcastEvent = tempDel as Action<T, U, V>;
 
            if (broadcastEvent != null) { broadcastEvent(arg1, arg2, arg3); } 
			else { throw CreateBroadcastSignatureException(eventID); }
        }
    }

	//Four parameters
	public void Broadcast<T, U, V, C>(TEventType eventName, T arg1, U arg2, V arg3, C arg4) {
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
				Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif

		#if REQUIRE_LISTENER
				OnBroadcasting(eventID);
		#endif

		Delegate tempDel = eventHandlers[eventID];
		if (tempDel != null) {
			Action<T, U, V, C> broadcastEvent = tempDel as Action<T, U, V, C>;

			if (broadcastEvent != null) { broadcastEvent(arg1, arg2, arg3, arg4); } else { throw CreateBroadcastSignatureException(eventID); }
		}
	}
	
	
	//Delayed Broadcast (using coroutines -- these must be called like so from a MonoBehaviour script : 
	//StartCoroutine(GlobalEventManager(TEventType.DoSomething, new WaitForSeconds(5f));
	
	//No parameters
    public IEnumerator Broadcast(TEventType eventName, YieldInstruction[] delayInstrunctions) 
	{
		Debug.Log("Before yielding");
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif
        
		#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
		#endif
 
        Delegate tempDel = eventHandlers[eventID];
        if (tempDel != null)
		{
            Action broadcastEvent = tempDel as Action;
 
            if (broadcastEvent != null) 
			{ 
				foreach(YieldInstruction instruction in delayInstrunctions)
					yield return instruction; 
				broadcastEvent(); 
			} 
			else { throw CreateBroadcastSignatureException(eventID); }
        }
    }
 
	//Single parameter
    public IEnumerator Broadcast<T>(TEventType eventName, T arg1, YieldInstruction[] delayInstrunctions) 
	{
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif
        
		#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
		#endif
 
        Delegate tempDel = eventHandlers[eventID];
       	if (tempDel != null)
		{
            Action<T> broadcastEvent = tempDel as Action<T>;
 
            if (broadcastEvent != null) 
			{ 
				foreach(YieldInstruction instruction in delayInstrunctions)
					yield return instruction;
				broadcastEvent(arg1); 
			} 
			else { throw CreateBroadcastSignatureException(eventID); }
        }
	}
 
	//Two parameters
    public IEnumerator Broadcast<T, U>(TEventType eventName, T arg1, U arg2, YieldInstruction[] delayInstrunctions) 
	{
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif
        
		#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
		#endif
 
        Delegate tempDel = eventHandlers[eventID];
        if (tempDel != null)
		{
            Action<T, U> broadcastEvent = tempDel as Action<T, U>;
 
            if (broadcastEvent != null) 
			{ 
				foreach(YieldInstruction instruction in delayInstrunctions)
					yield return instruction;
				broadcastEvent(arg1, arg2); 
			} 
			else { throw CreateBroadcastSignatureException(eventID); }
        }
    }
 
	//Three parameters
    public IEnumerator Broadcast<T, U, V>(TEventType eventName, T arg1, U arg2, V arg3, YieldInstruction[] delayInstrunctions) 
	{
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif
        
		#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
		#endif
 
        Delegate tempDel = eventHandlers[eventID];
       	if (tempDel != null)
		{
            Action<T, U, V> broadcastEvent = tempDel as Action<T, U, V>;
 
            if (broadcastEvent != null) 
			{ 
				foreach(YieldInstruction instruction in delayInstrunctions)
					yield return instruction;
				broadcastEvent(arg1, arg2, arg3); 
			} 
			else { throw CreateBroadcastSignatureException(eventID); }
        }
    }

	//Four parameters
	public IEnumerator Broadcast<T, U, V, C>(TEventType eventName, T arg1, U arg2, V arg3, C arg4, YieldInstruction[] delayInstrunctions) {
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
				Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif

		#if REQUIRE_LISTENER
				OnBroadcasting(eventID);
		#endif

		Delegate tempDel = eventHandlers[eventID];
		if (tempDel != null) {
			Action<T, U, V, C> broadcastEvent = tempDel as Action<T, U, V, C>;

			if (broadcastEvent != null) {
				foreach (YieldInstruction instruction in delayInstrunctions)
					yield return instruction;
				broadcastEvent(arg1, arg2, arg3, arg4);
			} else { throw CreateBroadcastSignatureException(eventID); }
		}
	}



	//Last Listener Broadcast
	//event will be sent only to last subscriber

	//No parameters
	public void BroadcastToLast(TEventType eventName) {
		int eventID = ConvertEnumToInt(eventName);
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
#endif

#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
#endif

		Delegate tempDel = eventHandlers[eventID];
		if (tempDel != null) {
			Delegate[] invocationList = tempDel.GetInvocationList();
			Action broadcastEvent = invocationList[invocationList.Length - 1] as Action;

			if (broadcastEvent != null) { broadcastEvent(); } else { throw CreateBroadcastSignatureException(eventID); }
		}
	}

	//Single parameter
	public void BroadcastToLast<T>(TEventType eventName, T arg1) {
		int eventID = ConvertEnumToInt(eventName);
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
#endif

#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
#endif

		Delegate tempDel = eventHandlers[eventID];
		if (tempDel != null) {
			Delegate[] invocationList = tempDel.GetInvocationList();
			Action<T> broadcastEvent = invocationList[invocationList.Length - 1] as Action<T>;

			if (broadcastEvent != null) { broadcastEvent(arg1); } else { throw CreateBroadcastSignatureException(eventID); }
		}
	}

	//Two parameters
	public void BroadcastToLast<T, U>(TEventType eventName, T arg1, U arg2) {
		int eventID = ConvertEnumToInt(eventName);
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
#endif

#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
#endif

		Delegate tempDel = eventHandlers[eventID];
		if (tempDel != null) {
			Delegate[] invocationList = tempDel.GetInvocationList();
			Action<T, U> broadcastEvent = invocationList[invocationList.Length - 1] as Action<T, U>;

			if (broadcastEvent != null) { broadcastEvent(arg1, arg2); } else { throw CreateBroadcastSignatureException(eventID); }
		}
	}

	//Three parameters
	public void BroadcastToLast<T, U, V>(TEventType eventName, T arg1, U arg2, V arg3) {
		int eventID = ConvertEnumToInt(eventName);
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
#endif

#if REQUIRE_LISTENER
        OnBroadcasting(eventID);
#endif

		Delegate tempDel = eventHandlers[eventID];
		if (tempDel != null) {
			Delegate[] invocationList = tempDel.GetInvocationList();
			Action<T, U, V> broadcastEvent = invocationList[invocationList.Length - 1] as Action<T, U, V>;

			if (broadcastEvent != null) { broadcastEvent(arg1, arg2, arg3); } else { throw CreateBroadcastSignatureException(eventID); }
		}
	}

	//Four parameters
	public void BroadcastToLast<T, U, V, C>(TEventType eventName, T arg1, U arg2, V arg3, C arg4) {
		int eventID = ConvertEnumToInt(eventName);
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
				Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventName + "\"");
		#endif

		#if REQUIRE_LISTENER
				OnBroadcasting(eventID);
		#endif

		Delegate tempDel = eventHandlers[eventID];
		if (tempDel != null) {
			Delegate[] invocationList = tempDel.GetInvocationList();
			Action<T, U, V, C> broadcastEvent = invocationList[invocationList.Length - 1] as Action<T, U, V, C>;

			if (broadcastEvent != null) { broadcastEvent(arg1, arg2, arg3, arg4); } else { throw CreateBroadcastSignatureException(eventID); }
		}
	}
}