using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views.TreeGameContecst
{
    public class TreeOfUser
    {
        public int lvlOfTree { get; private set; }
        public int amountOfGrow { get; private set; }
        public int amountToLvlUp { get; private set; }
        public string nameOfTree { get; private set; }
        public string nameOfImage { get; private set; }
        public float offsetX { get; private set; }
        public float offsetY { get; private set; }
        public TreeOfUser()
        {
            lvlOfTree = 0;
            amountOfGrow = 0;
            amountToLvlUp = 0;
            nameOfTree = "default";
            offsetX = 0;
            offsetY = 0;
            nameOfImage = "lvl_1.png";
        }
    }
}
