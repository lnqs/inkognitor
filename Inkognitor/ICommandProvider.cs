namespace Inkognitor
{
    public interface ICommandProvider
    {
        CommandProviderCallback Dispatch { get; set; }
    }

    public delegate string CommandProviderCallback(string[] command);
}
