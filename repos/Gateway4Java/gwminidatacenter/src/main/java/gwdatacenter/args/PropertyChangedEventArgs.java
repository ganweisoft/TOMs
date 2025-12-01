package gwdatacenter.args;

import gwdatacenter.Properties;

public class PropertyChangedEventArgs extends sharpSystem.EventArgs
{
	private Properties properties;
	private String key;
	private Object newValue;
	private Object oldValue;

	/** @return 
	 returns the changed property object
	 
	*/
	public final Properties getProperties()
	{
		return properties;
	}

	/** @return 
	 The key of the changed property
	 
	*/
	public final String getKey()
	{
		return key;
	}

	/** @return 
	 The new value of the property
	 
	*/
	public final Object getNewValue()
	{
		return newValue;
	}

	/** @return 
	 The new value of the property
	 
	*/
	public final Object getOldValue()
	{
		return oldValue;
	}

	public PropertyChangedEventArgs(Properties properties, String key, Object oldValue, Object newValue)
	{
		this.properties = properties;
		this.key = key;
		this.oldValue = oldValue;
		this.newValue = newValue;
	}
}