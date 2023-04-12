namespace Nekai.Common;

/// <summary>
/// Wrapper of the <see cref="IObservable{T}"/> interface that defines a thread-safe default implementation of the
/// <see cref="IObservable{T}.Subscribe(IObserver{T})"/> method.
/// </summary>
/// <inheritdoc cref="IObservable{T}"/>
public abstract class ObservableObject<T> : IObservable<T>
{
	/// <summary>
	/// Container of the subscribed observers to notify.
	/// </summary>
	protected ICollection<IObserver<T>> Observers { get; } = new ConcurrentList<IObserver<T>>();

	/// <summary>
	/// Add the <paramref name="observer"/> to the list of objects to notify.
	/// </summary>
	/// <param name="observer"> The object to subscribe for notification. </param>
	/// <returns> An object that removes the <paramref name="observer"/> from the list of objects to notify
	/// upon disposal. </returns>
	public IDisposable Subscribe(IObserver<T> observer)
	{
		Observers.Add(observer);
		return new ObserverUnsubscriber<T, IObserver<T>>(Observers, observer);
	}

	protected virtual void Notify(T value)
	{
		foreach(var observer in Observers)
		{
			observer.OnNext(value);
		}
	}

	protected virtual void NotifyError(Exception error)
	{
		foreach(var observer in Observers)
		{
			observer.OnError(error);
		}
	}

	protected virtual void NotifyCompleted()
	{
		foreach(var observer in Observers)
		{
			observer.OnCompleted();
		}
	}
}