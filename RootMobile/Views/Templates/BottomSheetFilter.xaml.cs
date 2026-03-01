using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;

namespace RootMobile.Views.Templates;

public partial class BottomSheetFilter : Popup
{
    int _current;
    public BottomSheetFilter(int current)
    {
        InitializeComponent();

        BindingContext = this;
        _current = current;

        CurrentSelection(current);
    }



    private void OnCheck(object sender, CheckedChangedEventArgs args)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked)
        {
            int selectedValue = int.Parse(radioButton.Value.ToString());

            Close(Convert.ToInt32(selectedValue));
        }

    }
    private async void Popup_Opened(object sender, EventArgs e)
    {
        BottomSheetBorder.TranslationY = 500;
        await BottomSheetBorder.TranslateTo(0, 0, 300, Easing.CubicOut);

    }


    private void CurrentSelection(int number)
    {
        foreach (var item in stackBtn)
        {
            if (item is RadioButton radioButton)
            {
                if (int.Parse(radioButton.Value.ToString()) == number)
                {
                    radioButton.IsChecked = true;
                }
                radioButton.CheckedChanged += OnCheck;
            }
        }
    }

}
