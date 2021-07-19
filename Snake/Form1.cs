using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        const int length = 40, width = 25, snakeSize = 20;
        List<Label> labelList = new List<Label>();
        int headX, headY, tailX, tailY, foodX, foodY;
        bool[,] visit;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            for(int i = 0; i<length; i++) {
                for(int j=0; j<width; j++)
                {
                    Label label = new Label();
                    label.Location = new Point(i * snakeSize , j * snakeSize + 50);
                    label.BackColor = Color.Gray;
                    label.Size = new Size(snakeSize, snakeSize);
                    label.Enabled = false;

                    labelList.Add(label);
                    this.Controls.Add(label);
                }
            }

            visit = new bool[length, width];
            headX = 20;
            headY = 10;
            tailX = 20;
            tailY = 11;

            Label head = getLabelByPositon(headX, headY);
            visit[headX, headY] = true;
            head.BackColor = Color.Red;

            Label tail = getLabelByPositon(tailX, tailY);
            visit[tailX, tailY] = true;
            tail.BackColor = Color.Red;

            generateFood();
        }

        void generateFood()
        {
            Random random = new Random();

            do
            {
                foodX = random.Next(length);
                foodY = random.Next(width);

            } while (visit[foodX, foodY]);

            Label food = getLabelByPositon(foodX, foodY);
            food.BackColor = Color.Orange;
        }

        Label getLabelByPositon(int x, int y)
        {
            return labelList[x * width + y];
        }

        Label getLabelByPositon(Point p)
        {
            return getLabelByPositon(p.X, p.Y);
        }
    }
}
