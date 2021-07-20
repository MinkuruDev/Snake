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
        int score, difficulity;
        Timer timer = new Timer();

        enum Direction
        {
            up, down, left, right
        }
        Direction currentDirection;

        public frmMain()
        {
            InitializeComponent();
            this.KeyDown += FrmMain_KeyDown;
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
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
            timer.Interval = 300 - 30 * difficulity;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            snakeMove();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createGround();
            createSnakeBody();
            timer.Tick += Timer_Tick;
        }

        private void createSnakeBody()
        {
            // add snake tail to queue
            tailPoint = new Point(cols / 2, rows - 1);
            snakeBody.Enqueue(tailPoint);

            // add snake head to queue
            headPoint = new Point(cols / 2, rows - 2);
            snakeBody.Enqueue(headPoint);

            Label head = getLabelByPositon(headPoint);
            visit[headPoint.X, headPoint.Y] = true;
            head.BackColor = Color.Red;

            Label tail = getLabelByPositon(tailPoint);
            visit[tailPoint.X, tailPoint.Y] = true;
            tail.BackColor = Color.Red;
        }

        private void createGround()
        {
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Label label = new Label();
                    label.Location = new Point(i * snakeSize, j * snakeSize + 50);
                    label.BackColor = Color.Green;
                    label.Size = new Size(snakeSize, snakeSize);
                    label.Enabled = false;

                    labelList.Add(label);
                    this.Controls.Add(label);
                }
            }
            
            difficulity = Convert.ToInt32(lblDifficulity.Text);
            visit = new bool[cols, rows];
        }

        private void btnDecrease_Click(object sender, EventArgs e)
        {
            if (difficulity == 1) return;
            difficulity--;
            lblDifficulity.Text = difficulity.ToString();
        }

        private void btnIncrease_Click(object sender, EventArgs e)
        {
            if (difficulity == 9) return;
            difficulity++;
            lblDifficulity.Text = difficulity.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            resetGround();
            createSnakeBody();
            generateFood();
            startTimer();
            btnDecrease.Enabled = false;
            btnIncrease.Enabled = false;
            btnStart.Enabled = false;
        }

        private void resetGround()
        {
            foreach(Label label in labelList)
            {
                label.BackColor = Color.Green;
            }
            
            for(int i = 0; i<cols; i -= -1)
            {
                for(int j = 0; j<rows; j -= -1)
                {
                    visit[i, j] = false;
                }
            }

            currentDirection = Direction.up;
            snakeBody.Clear();
            score = 0;
            lblScore.Text = "Score: 0";
        }  

        void snakeMove()
        {
            Point temp = new Point(headPoint.X, headPoint.Y);
            switch (currentDirection)
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
                gameOver();
                return;
            }

            Label head = getLabelByPositon(headPoint);
            head.BackColor = Color.Red;
            visit[headPoint.X, headPoint.Y] = true;

            if(headPoint.X == foodPoint.X && headPoint.Y == foodPoint.Y)
            {
                score += difficulity;
                lblScore.Text = "Score: " + score;
                generateFood();
            }
            else
            {
                tailPoint = snakeBody.Dequeue();
                Label tail = getLabelByPositon(tailPoint);
                tail.BackColor = Color.Green;
                visit[tailPoint.X, tailPoint.Y] = false;
            }
        }

        private void gameOver()
        {
            timer.Stop();
            MessageBox.Show("Game over\n" + lblScore.Text);
            btnStart.Enabled = true;
            btnDecrease.Enabled = true;
            btnIncrease.Enabled = true;
        }

        private bool checkGameOver(Point point)
        {
            int x = point.X;
            int y = point.Y;
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

        Label getLabelByPositon(Point p)
        {
            return labelList[p.X * rows + p.Y];
        }
    }
}
