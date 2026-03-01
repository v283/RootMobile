using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RootMobile.Views.TreeGameContecst;
namespace RootMobile.Views;

public partial class TreeGameView : ContentPage
{
    private TreeOfUser treeOfUser;
    private WateringCan wateringCan;
    public TreeGameView()
    {
        treeOfUser = new TreeOfUser();
        wateringCan = new WateringCan();
        InitializeComponent();
        TreeSprite.Source = treeOfUser.nameOfImage;
    }
}