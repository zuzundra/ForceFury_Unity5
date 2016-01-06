public class EventsAggregator {
	private static EventManager<ENetworkEvent> _network = new EventManager<ENetworkEvent>();
	public static EventManager<ENetworkEvent> Network { get { return _network; } }

	private static EventManager<EPlayerEvent> _player = new EventManager<EPlayerEvent>();
	public static EventManager<EPlayerEvent> Player { get { return _player; } }

	private static EventManager<EFightEvent> _fight = new EventManager<EFightEvent>();
	public static EventManager<EFightEvent> Fight { get { return _fight; } }

	private static EventManager<EItemEvent> _items = new EventManager<EItemEvent>();
	public static EventManager<EItemEvent> Items { get { return _items; } }

	private static EventManager<EUnitEvent> _units = new EventManager<EUnitEvent>();
	public static EventManager<EUnitEvent> Units { get { return _units; } }

	private static EventManager<ECityEvent> _city = new EventManager<ECityEvent>();
	public static EventManager<ECityEvent> City { get { return _city; } }

	private static EventManager<EUIEvent> _ui = new EventManager<EUIEvent>();
	public static EventManager<EUIEvent> UI { get { return _ui; } }
}