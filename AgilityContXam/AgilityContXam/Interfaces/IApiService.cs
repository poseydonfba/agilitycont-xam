namespace AgilityContXam.Interfaces
{
    public interface IApiService
    {
        IApi Speculative { get; }
        IApi UserInitiated { get; }
        IApi Background { get; }
    }
}
