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
    public partial class frmMain : Form
    {
        // global variable need for program
        const int cols = 40, rows = 25, snakeSize = 20;
        List<Label> labelList = new List<Label>();
        Queue<Point> snakeBody = new Queue<Point>();
        Point headPoint, tailPoint, foodPoint;
        bool[,] visit;
        bool[] move = new bool[4];
        int score;
        Timer timer = new Timer();

        enum Direction
        {
            up, down, left, right
        }
        Direction currentDirection = Direction.up;

        public frmMain()
        {
            InitializeComponent();
            startTimer();
            txtMove.KeyDown += TxtMove_KeyDown;
            
        }

        private void TxtMove_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    if (currentDirection == Direction.down) return;
                    currentDirection = Direction.up;
                    break;
                case Keys.Down:
                case Keys.S:
                    if (currentDirection == Direction.up) return;
                    currentDirection = Direction.down;
                    break;
                case Keys.Left:
                case Keys.A:
                    if (currentDirection == Direction.right) return;
                    currentDirection = Direction.left;
                    break;
                case Keys.Right:
                case Keys.D:
                    if (currentDirection == Direction.left) return;
                    currentDirection = Direction.right;
                    break;
            }
        }

        private void startTimer()
        {
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            snakeMove(currentDirection);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            for(int i = 0; i<cols; i++) {
                for(int j=0; j<rows; j++)
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

            visit = new bool[cols, rows];
            score = 0;

            // add snake tail to queue
            tailPoint = new Point(20, 11);
            snakeBody.Enqueue(tailPoint);

            // add snake head to queue
            headPoint = new Point(20, 10);
            snakeBody.Enqueue(headPoint);

            Label head = getLabelByPositon(headPoint);
            visit[headPoint.X, headPoint.Y] = true;
            head.BackColor = Color.Red;

            Label tail = getLabelByPositon(tailPoint);
            visit[tailPoint.X, tailPoint.Y] = true;
            tail.BackColor = Color.Red;
            
            generateFood();
            
        }

        void snakeMove(Direction direction)
        {
            Point temp = new Point(headPoint.X, headPoint.Y);
            switch (direction)
            {
                case Direction.up:
                    temp.Y--;
                    break;
                case Direction.down:
                    temp.Y++;
                    break;
                case Direction.left:
                    temp.X--;
                    break;
                case Direction.right:
                    temp.X++;
                    break;
            }
            headPoint = temp;
            snakeBody.Enqueue(headPoint);

            if (checkGameOver(temp))
            {
                timer.Stop();
                MessageBox.Show("Game over");
                return;
            }

            Label head = getLabelByPositon(headPoint);
            head.BackColor = Color.Red;
            visit[headPoint.X, headPoint.Y] = true;

            if(headPoint.X == foodPoint.X && headPoint.Y == foodPoint.Y)
            {
                lblScore.Text = "Score: " + ++score;
                generateFood();
            }
            else
            {
                tailPoint = snakeBody.Dequeue();
                Label tail = getLabelByPositon(tailPoint);
                tail.BackColor = Color.Gray;
                visit[tailPoint.X, tailPoint.Y] = false;
            }
        }

        private bool checkGameOver(Point temp)
        {
            int x = temp.X;
            int y = temp.Y;
            return x < 0 || x >= cols || y < 0 || y >= rows || visit[x, y];
        }

        void generateFood()
        {
            Random random = new Random();
            int foodX, foodY;
            do
            {
                foodX = random.Next(cols);
                foodY = random.Next(rows);

            } while (visit[foodX, foodY]);

            foodPoint = new Point(foodX, foodY);
            Label food = getLabelByPositon(foodPoint);
            food.BackColor = Color.Orange;
        }

        Label getLabelByPositon(int x, int y)
        {
            return labelList[x * rows + y];
        }

        Label getLabelByPositon(Point p)
        {
            return getLabelByPositon(p.X, p.Y);
        }
    }
}
