using System;

public class ListenerException : Exception 
{
    public ListenerException(string msg)
        : base(msg) {}
}

public class BroadcastException : Exception 
{
    public BroadcastException(string msg)
        : base(msg) {}
}

public class InvalidGenericTypeException : Exception
{
	public InvalidGenericTypeException(string msg)
		: base(msg){}
}