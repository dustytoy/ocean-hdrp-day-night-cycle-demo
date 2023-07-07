public abstract class DayNightHandle<T>
{
    public T cachedValue { get; protected set; }
    public abstract T Evaluate(float t);
}
