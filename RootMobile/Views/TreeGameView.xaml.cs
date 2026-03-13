using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RootMobile.Services;
using RootMobile.Views.TreeGameContecst;
namespace RootMobile.Views;

public partial class TreeGameView : ContentPage
{
    private TreeOfUser treeOfUser;
    private WateringCan wateringCan;
    private DataService _dataService;

    public TreeGameView(IDataService dataService)
    {
        _dataService = (DataService)dataService;

        DataService _dataServise;
        treeOfUser = new TreeOfUser();
        wateringCan = new WateringCan();
        InitializeComponent();
        TreeSprite.Source = treeOfUser.nameOfImage;
    }
}