using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace SpaceShooterGame
{
    public partial class SpaceShooterGame : Form
    {
        PictureBox[] stars;
        int backgroundSpeed, playerSpeed;

        PictureBox[] munitions;
        int munitionSpeed;

        PictureBox[] enemies;
        int enemiesSpeed;

        PictureBox[] enemiesMunition;
        int enemiesMunitionSpeed;

        Random rnd;

        WindowsMediaPlayer gameMedia;
        WindowsMediaPlayer shootMedia;
        WindowsMediaPlayer boomMedia;

        int score, level, difficulty;
        bool pause, gameIsOver;

        public SpaceShooterGame()
        {
            InitializeComponent();
        }

        private void SpaceShooterGame_Load(object sender, EventArgs e)
        {
            backgroundSpeed = 4;
            playerSpeed = 4;
            enemiesSpeed = 4;
            munitionSpeed = 20;
            enemiesMunitionSpeed = 4;

            pause = false;
            gameIsOver = false;
            score = 0;
            level = 1;
            difficulty = 9;

            munitions = new PictureBox[3];
            Image munition = Image.FromFile(@"asserts\munition.png");

            Image enemi1 = Image.FromFile("asserts\\E1.png");
            Image enemi2 = Image.FromFile("asserts\\E2.png");
            Image enemi3 = Image.FromFile("asserts\\E3.png");
            Image boss1 = Image.FromFile("asserts\\Boss1.png");
            Image boss2 = Image.FromFile("asserts\\Boss2.png");

            enemies = new PictureBox[10];

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new PictureBox();
                enemies[i].Size = new Size(44, 44);
                enemies[i].SizeMode = PictureBoxSizeMode.Zoom;
                enemies[i].BorderStyle = BorderStyle.None;
                enemies[i].Visible = false;
                this.Controls.Add(enemies[i]);
                enemies[i].Location = new Point ((i + 1) * 50, -50); 
            }

            for (int i = 0; i < munitions.Length; i++)
            {
                munitions[i] = new PictureBox();
                munitions[i].Size = new Size(8, 8);
                munitions[i].Image = munition;
                munitions[i].SizeMode = PictureBoxSizeMode.Zoom;
                munitions[i].BorderStyle = BorderStyle.None;
                this.Controls.Add(munitions[i]);
            }

            enemies[0].Image = boss1;
            enemies[1].Image = enemi3;
            enemies[2].Image = enemi2;
            enemies[3].Image = enemi3;
            enemies[4].Image = enemi1;
            enemies[5].Image = enemi3;
            enemies[6].Image = enemi1;
            enemies[7].Image = enemi2;
            enemies[8].Image = boss2;

            stars = new PictureBox[10];
            rnd = new Random();

            for(int i = 0;  i < stars.Length; i++)
            {
                stars[i] = new PictureBox();
                stars[i].BorderStyle = BorderStyle.None;
                stars[i].Location = new Point(rnd.Next(20, 500), rnd.Next(-10, 400));
                if (i % 2 == 1)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[i].BackColor = Color.Wheat;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }
                this.Controls.Add(stars[i]);
            }

            enemiesMunition = new PictureBox[10]; 

            for (int i = 0; i < enemiesMunition.Length; i++)
            {
                enemiesMunition[i] = new PictureBox();
                enemiesMunition[i].Size = new Size(2, 25);
                enemiesMunition[i].Visible = false;
                enemiesMunition[i].BackColor = Color.Gold;
                int x = rnd.Next(0, 10);
                enemiesMunition[i].Location = new Point(enemies[x].Location.X, enemies[x].Location.Y - 20);
                this.Controls.Add(enemiesMunition[i]);
            }

            gameMedia = new WindowsMediaPlayer();
            shootMedia = new WindowsMediaPlayer();
            boomMedia = new WindowsMediaPlayer();

            gameMedia.URL = "songs\\GameSong.mp3";
            shootMedia.URL = "songs\\shoot.mp3";
            boomMedia.URL = "songs\\boom.mp3"; 

            gameMedia.settings.setMode("loop", true);
            gameMedia.settings.volume = 4;
            shootMedia.settings.volume = 1;
            boomMedia.settings.volume = 5;
  
            gameMedia.controls.play();
        }

        private void MoveBgTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < stars.Length/2; i++)
            {
                stars[i].Top += backgroundSpeed;
                if (stars[i].Top >= this.Height)
                    stars[i].Top = -stars[i].Height;
            }

            for (int i = stars.Length / 2; i < stars.Length; i++)
            {
                stars[i].Top += backgroundSpeed-2;
                if (stars[i].Top >= this.Height)
                    stars[i].Top = -stars[i].Height;
            }
        }

        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Left > 10)
                Player.Left -= playerSpeed;
        }

        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Right < 480)
                Player.Left += playerSpeed;
        }

        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top < 340)
                Player.Top += playerSpeed;
        }

        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 10)
                Player.Top -= playerSpeed; 
        }

        private void SpaceShooterGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pause)
            {
                if (e.KeyCode == Keys.Right)
                    RightMoveTimer.Start();
                if (e.KeyCode == Keys.Left)
                    LeftMoveTimer.Start();
                if (e.KeyCode == Keys.Down)
                    DownMoveTimer.Start();
                if (e.KeyCode == Keys.Up)
                    UpMoveTimer.Start();
            }
        }

        private void SpaceShooterGame_KeyUp(object sender, KeyEventArgs e)
        {
            RightMoveTimer.Stop();
            LeftMoveTimer.Stop();
            DownMoveTimer.Stop();
            UpMoveTimer.Stop();

            if (e.KeyCode == Keys.Space)
            {
                if(!gameIsOver)
                {
                    if(pause)
                    {
                        StartTimers();
                        label1.Visible = false;
                        gameMedia.controls.play();
                        pause = false;
                    }
                    else
                    {
                        label1.Location = new Point(this.Width / 2 - 90, 120);
                        label1.Text = "PAUSED";
                        label1.Visible = true;
                        gameMedia.controls.pause();
                        StopTimers();
                        pause = true;   
                    }
                }
            }
        }

        private void MoveEnemiesTimer_Tick(object sender, EventArgs e)
        {
            MoveEnemies(enemies, enemiesSpeed);
        }

        private void MoveEnemies(PictureBox[] array, int speed)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Visible = true;
                array[i].Top += speed;
                if (array[i].Top > this.Height)
                    array[i].Location = new Point((i + 1) * 50, -200);
            }
        }

        private void MoveMunitionTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < munitions.Length; i++)
            {
                shootMedia.controls.play();
                if (munitions[i].Top > 0)
                {
                    munitions[i].Visible = true;
                    munitions[i].Top -= munitionSpeed;

                    Collision();
                }
                else
                {
                    munitions[i].Visible = false;
                    munitions[i].Location = new Point(Player.Location.X + 20, Player.Location.Y - i * 30);
                }
            }
        }

        private void Collision()
        {

            for (int i = 0; i < enemies.Length; i++)
            {
                if(munitions[0].Bounds.IntersectsWith(enemies[i].Bounds) ||
                    munitions[1].Bounds.IntersectsWith(enemies[i].Bounds) ||
                    munitions[2].Bounds.IntersectsWith(enemies[i].Bounds)) 
                {
                    boomMedia.controls.play();
                    enemies[i].Location = new Point((i + 1) * 50, -100);

                    score += 1;
                    scorelbl.Text = (score < 10) ? "0" + score.ToString() : score.ToString();

                    if (score % 30 == 0)
                    {
                        level += 1;
                        levellbl.Text = (level < 10) ? "0"  + level.ToString() : level.ToString();

                        if (enemiesSpeed <= 10 && enemiesMunitionSpeed <= 10 && difficulty >= 0)
                        {
                            difficulty--;
                            enemiesSpeed++;
                            enemiesMunitionSpeed++;
                        }
                        if (level == 10)
                            GameOver("GREAT JOB!");
                    }
                    enemies[i].Location = new Point((i + 1) * 50, -100);
                }
                if (Player.Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    boomMedia.settings.volume = 40;
                    boomMedia.controls.play();
                    Player.Visible = false;
                    GameOver("");
                }
            }
        }

        private void GameOver(String strng)
        {
            label1.Text = strng;
            label1.Location = new Point(120, 120);
            label1.Visible = true;
            replayButton.Visible = true;
            exitButton.Visible = true;

            gameMedia.controls.stop();
            StopTimers();
        }

        private void replayButton_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeComponent();
            SpaceShooterGame_Load(e, e);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void StopTimers()
        {
            MoveBgTimer.Stop();
            MoveEnemiesTimer.Stop();
            MoveMunitionTimer.Stop();
            EnemiesMunitionTimer.Stop();
        }

        private void EnemiesMunitionTimer_Tick(object sender, EventArgs e)
        { 
            for(int i = 0; i < (enemiesMunition.Length -difficulty); i++)
            {
                if(enemiesMunition[i].Top < this.Height)
                {
                    enemiesMunition[i].Visible = true;
                    enemiesMunition[i].Top += enemiesMunitionSpeed;

                    CollisionEnemiesMunition() ;
                }
                else
                {
                    enemiesMunition[i].Visible = false;
                    int x = rnd.Next(0, 10);
                    enemiesMunition[i].Location = new Point(enemies[x].Location.X + 20, enemies[x].Location.Y + 30);
                }
            }
        }

        private void StartTimers()
        {
            MoveBgTimer.Start();
            MoveEnemiesTimer.Start();
            MoveMunitionTimer.Start();
            EnemiesMunitionTimer.Start();
        }

        private void CollisionEnemiesMunition()
        {
            for (int i = 0; i < enemiesMunition.Length; i++)
            {
                if (enemiesMunition[i].Bounds.IntersectsWith(Player.Bounds))
                {
                    enemiesMunition[i].Visible = false;
                    boomMedia.settings.volume = 40;
                    boomMedia.controls.play();
                    Player.Visible = false;
                    GameOver("GAME OVER");
                }
            }
        }
    }
}
