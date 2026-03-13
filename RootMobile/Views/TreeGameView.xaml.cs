using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.Views.TreeGameContecst;
namespace RootMobile.Views;

public partial class TreeGameView : ContentPage
{
    private UserAnimalModel _userTreeModel;
    private TreeOfUser treeOfUser;
    private WateringCan wateringCan;
    private DataService _dataService;

    public TreeGameView(IDataService dataService)
    {
        _dataService = (DataService)dataService;
       // _userTreeModel =  _dataService.GetUserCurrentTree().Result;
        //AnimalTypeModel _treeLevelModel = _userTreeModel.TreeLevel;
        //treeOfUser = new TreeOfUser(_treeLevelModel.Lvl,_userTreeModel.CurrentGrowProgress,_treeLevelModel.AmountForLevelUp,_userTreeModel.Name,_treeLevelModel.Lvl,0,0);
        wateringCan = new WateringCan();
        InitializeComponent();
        //TreeSprite.Source = treeOfUser.nameOfImage;
    }
}