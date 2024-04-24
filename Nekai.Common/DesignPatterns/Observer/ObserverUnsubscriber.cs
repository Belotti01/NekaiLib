namespace Nekai.Common;

/// <inheritdoc/>
// Barebones implementation
public class ObserverUnsubscriber<T> : ObserverUnsubscriber<T, IObserver<T>>
{
	/// <inheritdoc/>
	public ObserverUnsubscriber(ICollection<IObserver<T>> observers, IObserver<T> observer) : base(observers, observer) { }
}

/// <summary>
/// Basic generic implementation of the Unsubscriber component of the Observer design pattern.
/// </summary>
/// <typeparam name="T">The Type of the object that provides notification information to the
/// <typeparamref name="TObserver"/> implementation. </typeparam>
/// <typeparam name="TObserver">The implementation used for the Observer object.</typeparam>
public class ObserverUnsubscriber<T, TObserver> : IDisposable
	where TObserver : IObserver<T>
{
	/// <summary>
	/// Reference to the container of the subscribed observers to be notified (including the <see cref="Observer"/>).
	/// </summary>
	protected ICollection<TObserver>? Subscribers { get; }

	/// <summary>
	/// The <see cref="IObserver{T}"/> managed by this instance during disposal.
	/// </summary>
	protected TObserver Observer { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ObserverUnsubscriber{T, TObserver}"/> class.
	/// </summary>
	/// <param name="subscribersCollection"> The reference to the observable object's subscribers container. </param>
	/// <param name="observer"> The subscribed observer to be managed. </param>
	public ObserverUnsubscriber(ICollection<TObserver>? subscribersCollection, TObserver observer)
	{
		Subscribers = subscribersCollection;
		Observer = observer;
	}

	/// <summary>
	/// Unsubscribe the <typeparamref name="TObserver"/> from the <see cref="IObservable{T}"/>.
	/// </summary>
	public void Dispose()
	{
		Subscribers?.Remove(Observer);
		GC.SuppressFinalize(this);
	}
}