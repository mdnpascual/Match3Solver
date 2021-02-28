using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Match3Solver
{
    public interface SolverInterface
    {

        public struct Score
        {
            public int BrokenHeart;
            public int Heart;
            public int Stamina;
            public int Sentiment;
            public int Blue;
            public int Red;
            public int Green;
            public int Gold;
            public int Bell;

            public int staminaCost;
            public int chains;
            public Boolean wasChanged;

            public Score(int whyDoINeedThis)
            {
                this.BrokenHeart = 0;
                this.Heart = 0;
                this.Stamina = 0;
                this.Sentiment = 0;
                this.Blue = 0;
                this.Red = 0;
                this.Green = 0;
                this.Gold = 0;
                this.Bell = 0;
                wasChanged = false;
                this.staminaCost = 0;
                this.chains = 0;
            }

            public void addScoreFromValue(int value)
            {
                wasChanged = true;
                switch (value % 10)
                {
                    case 0:
                        this.BrokenHeart++;
                        break;
                    case 1:
                        this.Heart++;
                        break;
                    case 2:
                        this.Stamina++;
                        break;
                    case 3:
                        this.Sentiment++;
                        break;
                    case 4:
                        this.Blue++;
                        break;
                    case 5:
                        this.Red++;
                        break;
                    case 6:
                        this.Green++;
                        break;
                    case 7:
                        this.Gold++;
                        break;
                    case 8:
                        this.Bell++;
                        break;
                }
            }

            public void resetWasChanged()
            {
                this.wasChanged = false;
            }

            public Boolean hasScore()
            {
                return (this.BrokenHeart + this.Heart + this.Stamina + this.Sentiment + this.Blue + this.Red + this.Green + this.Gold + this.Bell > 0);
            }

            public int getTotal()
            {
                return this.Heart + this.Stamina + this.Sentiment + this.Blue + this.Red + this.Green + this.Gold + this.Bell - this.BrokenHeart;
            }

            public int getTotalNoBroken()
            {
                return this.Heart + this.Stamina + this.Sentiment + this.Blue + this.Red + this.Green + this.Gold + this.Bell;
            }
        }

        public struct Movement
        {
            public int xPos;
            public int yPos;
            public Boolean isVertical;
            public int amount;
            public Score score;
            public int boardHash;

            public Movement(int xPos, int yPos, Boolean isVertical, int amount, Score score, int boardhash)
            {
                this.xPos = xPos;
                this.yPos = yPos;
                this.isVertical = isVertical;
                this.amount = amount;
                this.score = score;
                this.boardHash = boardhash;
            }
        }
    }
}
