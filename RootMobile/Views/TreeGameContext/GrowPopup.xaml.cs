using CommunityToolkit.Maui.Views;

namespace RootMobile.Views.TreeGameContext;

public partial class GrowPopup : Popup
{
    public GrowPopup()
    {
        InitializeComponent();
    }

    void OnClose(object sender, EventArgs e)
    {
        Close();
    }
}