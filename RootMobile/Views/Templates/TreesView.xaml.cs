using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views.Templates;

public partial class TreesView : ContentView
{
    public event EventHandler<ItemsViewScrolledEventArgs> Scrolled;

    public TreesView()
    {
        InitializeComponent();
    }

    private void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        Scrolled?.Invoke(this, e);
    }
}