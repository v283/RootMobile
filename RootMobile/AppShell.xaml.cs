using RootMobile.Views;

namespace RootMobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("botpage", typeof(ConversationView));
    }
}
