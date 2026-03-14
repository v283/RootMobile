using RootMobile.ViewModels;

namespace RootMobile.Views;

[QueryProperty(nameof(Question), "question")]

public partial class ConversationView : ContentPage
{
    private string question;
    public string Question
    {
        get => question;
        set
        {
            question = value;
            QuestionEntry.Text = value;
        }
    }
    public ConversationView(ConversationViewModel vm)
    {
        InitializeComponent();
       
        BindingContext = vm;

        vm.CollectionView = myCollectionView;
        vm.ConversationView = mainPage;
        
    }

}