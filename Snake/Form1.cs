using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;

namespace Snake
{
    public partial class frmMain : Form
    {
        // global variable need for program
        const int cols = 40, rows = 25, snakeSize = 20, marginTop = 70;
        List<Label> labelList = new List<Label>(); // label list hold label play gruond
        Queue<Point> snakeBody = new Queue<Point>(); // snakebody
        Point headPoint, tailPoint, foodPoint;
        bool[,] visit; // ground
        bool keyDelay; // prevent some dumb die
        int score, difficulty;
        Timer timer = new Timer();
        string path; // path to program file

        enum Direction
        {
            up, down, left, right
        }
        Direction currentDirection; // move direction

        public frmMain()
        {
            InitializeComponent();
            this.KeyDown += FrmMain_KeyDown;
            setPath();
        }

        private void setPath()
        {
            // set program path to map and sfx
            path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            List<string> l = path.Split("\\").ToList();
            l.Remove(l[l.Count - 1]);
            path = "";
            foreach(string s in l)
            {
                path += s + "\\";
            }
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (keyDelay) return;
            if (!timer.Enabled) return;
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

            keyDelay = true;
        }

        private void startTimer()
        {
            timer.Interval = 250 - 25 * difficulty;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            snakeMove();
            keyDelay = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createGround();
            cbbMap.SelectedIndex = 0; // select default map
            createSnakeBody();
            timer.Tick += Timer_Tick;
            keyDelay = false;
        }

        private void createSnakeBody()
        {
            snakeBody.Clear();

            // add snake tail to queue
            tailPoint = new Point(cols / 2, rows - 2);
            snakeBody.Enqueue(tailPoint);

            // add snake head to queue
            headPoint = new Point(cols / 2, rows - 3);
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
                    label.Location = new Point(i * snakeSize, j * snakeSize + marginTop);
                    label.BackColor = Color.Green;
                    label.Size = new Size(snakeSize, snakeSize);
                    label.Enabled = false;
                    label.MouseDown += Label_MouseDown; // for developer develop map only

                    labelList.Add(label);
                    this.Controls.Add(label);
                }
            }
            
            difficulty = Convert.ToInt32(lblDifficulty.Text);
            visit = new bool[cols, rows];
        }

        private void Label_MouseDown(object sender, MouseEventArgs e)
        {
            // for develop map only
            Label label = (Label)sender;
            if(e.Button == MouseButtons.Left)
            {
                label.BackColor = Color.Black;
                int x = label.Location.X / snakeSize;
                int y = (label.Location.Y - marginTop) / snakeSize;
                visit[x, y] = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                label.BackColor = Color.Green;
                int x = label.Location.X / snakeSize;
                int y = (label.Location.Y - marginTop) / snakeSize;
                visit[x, y] = false;
            }
        }

        private void btnDecrease_Click(object sender, EventArgs e)
        {
            if (difficulty == 1) return;
            difficulty--;
            lblDifficulty.Text = difficulty.ToString();
        }

        private void btnIncrease_Click(object sender, EventArgs e)
        {
            if (difficulty == 9) return;
            difficulty++;
            lblDifficulty.Text = difficulty.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            createObtancle();
            createSnakeBody();
            generateFood();
            startTimer();
            // disable something
            btnDecrease.Enabled = false;
            btnIncrease.Enabled = false;
            btnStart.Enabled = false;
            cbbMap.Enabled = false;
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

            currentDirection = Direction.up; // default direction
            snakeBody.Clear();
            score = 0;
            lblScore.Text = "Score: 0";
        }

        private void cbbMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetGround();
            createObtancle();
            createSnakeBody();
        }

        void createObtancle(int x, int y)
        {
            // create obtancle at location x y
            Label obtancle = getLabelByPositon(new Point(x, y));
            visit[x, y] = true;
            obtancle.BackColor = Color.Black;
        }

        private void btnCreateMap_Click(object sender, EventArgs e)
        {
            // for develop map only
            Button button = (Button)sender;
            if(button.Text == "Create New Map")
            {
                button.Text = "Done";
                foreach(Label label in labelList)
                {
                    label.Enabled = true;
                }
                return;
            }

            button.Text = "Create New Map";
            string mapPath = path + "map\\" + "newmap.txt";

            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        if (visit[i, j]) sw.WriteLine(i + " " + j);
                    }
                }
            }
        }

        void createObtancle()
        {
            // create full map on ground

            // get the path to map file
            string mapPath = path + "map\\"; 
            mapPath += cbbMap.SelectedItem.ToString() + ".txt";

            int x, y;
            resetGround();

            if (!File.Exists(mapPath)) return;
            // read file
            // an obtancle in file is write x y
            using (StreamReader sr = File.OpenText(mapPath))
            {
                string s;
                List<string> lists = new List<string>();
                while((s = sr.ReadLine()) != null)
                {
                    lists = s.Split(" ").ToList();
                    x = Convert.ToInt32(lists[0]);
                    y = Convert.ToInt32(lists[1]);
                    createObtancle(x,y);
                }
                
            }
            
        }

        void snakeMove()
        {
            // create point have same location with head point
            Point temp = new Point(headPoint.X, headPoint.Y);
            // new location of temp base on direction
            switch (currentDirection)
            {
                case Direction.up:
                    temp.Y--;
                    if (temp.Y < 0) temp.Y = rows - 1;
                    break;
                case Direction.down:
                    temp.Y++;
                    if (temp.Y >= rows) temp.Y = 0;
                    break;
                case Direction.left:
                    temp.X--;
                    if (temp.X < 0) temp.X = cols - 1;
                    break;
                case Direction.right:
                    temp.X++;
                    if (temp.X >= cols) temp.X = 0;
                    break;
            }
            headPoint = temp;
            // put snake head to queue
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
                // snake eat food
                string eatPath = path + "sound\\" + "eat.wav";
                SoundPlayer soundPlayer = new SoundPlayer(eatPath);
                soundPlayer.Play();

                score += difficulty;
                lblScore.Text = "Score: " + score;
                generateFood();
            }
            else
            {
                // snake not eat anything
                tailPoint = snakeBody.Dequeue();
                Label tail = getLabelByPositon(tailPoint);
                tail.BackColor = Color.Green;
                visit[tailPoint.X, tailPoint.Y] = false;
            }
        }

        private void gameOver()
        {
            timer.Stop();
            string hitPath = path + "sound\\" + "hit.wav";
            SoundPlayer soundPlayer = new SoundPlayer(hitPath);
            soundPlayer.Play();
            MessageBox.Show("Game over\n" + lblScore.Text);
            btnStart.Enabled = true;
            btnDecrease.Enabled = true;
            btnIncrease.Enabled = true;
            cbbMap.Enabled = true;
        }

        private bool checkGameOver(Point point)
        {
            return visit[point.X, point.Y];
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
