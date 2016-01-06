using System;

//Provides a standard way to open a channel.
//When opening a channel, this pass should be passed as an argument, example:
//EventManagerRepository.Instance.GetSingleInstanceEventManager<OpenChannelEvent>
//			.Broadcast<OpenChannelEventArgs>(OpenChannelEvent.OpenChannel, new OpenChannelEventArgs(channelManager, 1));
//Very long, but doing it in this way allows us to add more variables to the open channel event args without breaking
//any code.
public class OpenChannelEventArgs : EventArgs
{
	public OpenChannelEventArgs(EventManager channelManager, int channelID)
	{
		ChannelManager = channelManager;
		ChannelID = channelID;
	}
	
	public EventManager ChannelManager 	{ get; private set; }
	public int ChannelID				{ get; private set; }
}