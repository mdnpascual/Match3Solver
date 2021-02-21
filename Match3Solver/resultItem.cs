using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Solver
{
    class resultItem
    {
        public string Position { get; set; }
        public int Amount { get; set; }
        public string Direction { get; set; }
        public int sHeart { get; set; }
        public int sStam { get; set; }
        public int sSent { get; set; }
        public int sBlue { get; set; }
        public int sRed { get; set; }
        public int sGreen { get; set; }
        public int sGold { get; set; }
        public int sBell { get; set; }
        public int sBHeart { get; set; }
        public Boolean isVertical { set; get; }
        public int xPos { set; get; }
        public int yPos { set; get; }
        public int StaminaCost { set; get; }
        public int Chain { set; get; }
        public int Total { set; get; }
        public int TotalWBroken { set; get; }

        public resultItem(SolverInterface.Movement input)
        {
            Position = "[" + input.yPos + "," + input.xPos + "]";
            Amount = input.amount;
            Direction = getDirection(input);
            sHeart = input.score.Heart;
            sStam = input.score.Stamina;
            sSent = input.score.Sentiment;
            sBlue = input.score.Blue;
            sRed = input.score.Red;
            sGreen = input.score.Green;
            sGold = input.score.Gold;
            sBell = input.score.Bell;
            sBHeart = input.score.BrokenHeart;
            isVertical = input.isVertical;
            xPos = input.xPos;
            yPos = input.yPos;
            StaminaCost = input.score.staminaCost;
            Chain = input.score.chains;
            Total = input.score.getTotal();
            TotalWBroken = input.score.getTotal() - (2 * input.score.BrokenHeart);
        }

        private string getDirection(SolverInterface.Movement input)
        {
            if (!input.isVertical && input.amount > 0)
            {
                return "⇒";
            }
            else if (!input.isVertical && input.amount < 0)
            {
                return "⇐";
            }
            else if (input.isVertical && input.amount > 0)
            {
                return "⇓";
            }
            else if (input.isVertical && input.amount < 0)
            {
                return "⇑";
            }
            return "?";
        }

        public override string ToString()
        {
            return Direction;
        }
    }
}
