using System.Windows.Input;
using CommunityToolkit.Maui.Views;

namespace RootMobile.Views.Templates;

public partial class EmailCkeckPopup : Popup
{
    public DateTime SelectedDate { get; private set; }

    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

    public EmailCkeckPopup()
    {
        InitializeComponent();
        SaveCommand = new Command(() => Close(isEmail.IsChecked));
        CloseCommand = new Command(() => Close(null));
        BindingContext = this;
    }
}
