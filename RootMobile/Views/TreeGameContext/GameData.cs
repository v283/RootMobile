using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views.TreeGameContext
{
    class GameData
    {
        public TreeOfUser userTree { get; private set; }
        public WateringCan userWateringCan { get; private set; }
        public GameState gameState { get; private set; }
        public GameData()
        {
            userTree = new TreeOfUser();
            userWateringCan = new WateringCan();
            gameState = new GameState();
            gameState = GameState.Wellcome;
            wellcomeTextList = new List<string>() { wellcomeText, wellcomeText2, plantingText };
        }
        //Strings for popups 
        public string wellcomeText { get; private set; } = "Привіт друже ти тут новенький ?:)";
        public string wellcomeText2 { get; private set; } = "Ха-ха давай тоді посадимо твоє перше деревце разом";
        public string plantingText { get; private set; } = "Викопай для початку невелику ямку";
        public string plantingText2 { get; private set; } = "Хей в тебе чудово виходить";
        public string plantingText3 { get; private set; } = "Чудово а тепер поклади його в ямку та додай трішки добрива";
        public string plantingText5 { get; private set; } = "Це допоможе йому вирости велииииким та сильним";
        public string plantingText6 { get; private set; } = "Прямо як ці величезні дерева позаду мене";
        public string plantingText7 { get; private set; } = "Оу, вибач мені потрібно йти головне не забувай поливати своє деревце :)";
        public List<string> wellcomeTextList { get; private set; }


    }
    public enum GameState
    {
        Wellcome,
        Plant,
        Tutorial,
        Idle,
        Watering,
        Victory
    }
}
