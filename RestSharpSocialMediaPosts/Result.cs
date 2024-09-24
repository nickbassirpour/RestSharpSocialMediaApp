namespace RestSharpSocialMediaPosts;

public class Result<T1, T2>
{
    private readonly T1? _value1;
    private readonly T2? _value2;
    private readonly bool _isValue1;

    private Result(T1 value)
    {
        _value1 = value;
        _isValue1 = true;
    }

    private Result(T2 value)
    {
        _value2 = value;
        _isValue1 = false;
    }

    public static implicit operator Result<T1, T2>(T1 value)
    {
        return new Result<T1, T2>(value);
    }

    public static implicit operator Result<T1, T2>(T2 value)
    {
        return new Result<T1, T2>(value);
    }

    public T Match<T>(Func<T1, T> f1, Func<T2, T> f2)
    {
        return _isValue1 ? f1(_value1) : f2(_value2);
    }
}
