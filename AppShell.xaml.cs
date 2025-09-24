namespace BabyTime;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Customize tab bar appearance
        Shell.SetTabBarBackgroundColor(this, Colors.White);
        Shell.SetTabBarTitleColor(this, Colors.Gray);
        Shell.SetTabBarUnselectedColor(this, Colors.Gray);
    }
}
