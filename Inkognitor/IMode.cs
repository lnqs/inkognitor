namespace Inkognitor
{
    public interface IMode
    {
        void Enter(MainWindow window);
        void Exit();
    }
}
