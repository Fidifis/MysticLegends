using System.Windows;

namespace MysticLegendsClient;

public abstract class NpcWindow : Window, ISingleInstanceWindow
{
    protected readonly int NpcId;

    public NpcWindow(int npcId)
    {
        NpcId = npcId;
    }

    public virtual void ShowWindow()
    {
        Show();
        if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;
        Activate();
    }

    protected abstract void SetSplashImage(string image);
}
