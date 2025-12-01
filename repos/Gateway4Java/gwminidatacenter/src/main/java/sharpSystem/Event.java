package sharpSystem;

//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2022 Tangible Software Solutions, Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to convert C# events to Java.
//----------------------------------------------------------------------------------------
public final class Event<T>
{
	private java.util.Map<String, T> namedListeners = new java.util.HashMap<String, T>();
	public void addListener(String methodName, T namedEventHandlerMethod)
	{
		if (!namedListeners.containsKey(methodName))
			namedListeners.put(methodName, namedEventHandlerMethod);
	}
	public void removeListener(String methodName)
	{
		if (namedListeners.containsKey(methodName))
			namedListeners.remove(methodName);
	}

	private java.util.List<T> anonymousListeners = new java.util.ArrayList<T>();
	public void addListener(T unnamedEventHandlerMethod)
	{
		anonymousListeners.add(unnamedEventHandlerMethod);
	}

	public java.util.List<T> listeners()
	{
		java.util.List<T> allListeners = new java.util.ArrayList<T>();
		allListeners.addAll(namedListeners.values());
		allListeners.addAll(anonymousListeners);
		return allListeners;
	}
}