package gwdatacenter.Interface;

import gwdatacenter.args.PropertyChangedEventArgs;

@FunctionalInterface
public interface PropertyChangedEventHandler
{
	void invoke(Object sender, PropertyChangedEventArgs e);
}