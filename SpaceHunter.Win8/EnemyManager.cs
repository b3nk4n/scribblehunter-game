
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    /// <summary>
    /// Manages the enemies.
    /// </summary>
    class EnemyManager : ILevel
    {
        #region Members

        private Texture2D texture;

        public List<Enemy> Enemies = new List<Enemy>(64);

        private PlayerManager playerManager;

        public const int SOFT_ROCKET_EXPLOSION_RADIUS = 100;
        public const float ROCKET_POWER_AT_CENTER = 40.0f;

        public const int DAMAGE_LASER_MIN = 7;
        public const int DAMAGE_LASER_MAX = 10;

        public int MinShipsPerWave = 5;
        public int MaxShipsPerWave = 8;
        private float nextWaveTimer;
        public const float InitialNextWaveMinTimer = 5.0f;
        private float nextWaveMinTimer;
        public const float InitialNextWaveMaxTimer = 7.0f;
        private float nextWaveMaxTimer;

        private float nextSingleEnemyTimer;
        public const float InitialNextSingleEnemyMinTimer = 1.5f;
        private float nextSingleEnemyMinTimer;
        public const float InitialNextSingleEnemyMaxTimer = 3.0f;
        private float nextSingleEnemyMaxTimer;

        private List<Wave> waves = new List<Wave>(32);

        public bool IsActive = false;

        private Random rand = new Random();

        private int currentLevel;

        #endregion

        #region Constructors

        public EnemyManager(Texture2D texture, PlayerManager playerManager,
                            Rectangle screenBounds)
        {
            this.texture = texture;
            this.playerManager = playerManager;

            setUpWayPoints();

            this.currentLevel = 1;
        }

        #endregion

        #region Methods

        private void setUpWayPoints()
        {
            Wave wave1 = new Wave();
            WaveEntity entry01_1 = new WaveEntity(new Vector2(75,25));
            entry01_1.AddRoutePoint(new Vector2(75, 455));
            wave1.AddEntry(entry01_1);
            WaveEntity entry01_2 = new WaveEntity(new Vector2(150, 25));
            entry01_2.AddRoutePoint(new Vector2(150, 455));
            wave1.AddEntry(entry01_2);
            WaveEntity entry01_3 = new WaveEntity(new Vector2(225, 25));
            entry01_3.AddRoutePoint(new Vector2(225, 455));
            wave1.AddEntry(entry01_3);
            WaveEntity entry01_4 = new WaveEntity(new Vector2(300, 25));
            entry01_4.AddRoutePoint(new Vector2(300, 455));
            wave1.AddEntry(entry01_4);
            WaveEntity entry01_5 = new WaveEntity(new Vector2(375, 25));
            entry01_5.AddRoutePoint(new Vector2(375, 455));
            wave1.AddEntry(entry01_5);
            waves.Add(wave1);

            Wave wave2 = new Wave();
            WaveEntity entry02_1 = new WaveEntity(new Vector2(100, 455));
            entry02_1.AddRoutePoint(new Vector2(100, 25));
            wave2.AddEntry(entry02_1);
            WaveEntity entry02_2 = new WaveEntity(new Vector2(175, 455));
            entry02_2.AddRoutePoint(new Vector2(175, 25));
            wave2.AddEntry(entry02_2);
            WaveEntity entry02_3 = new WaveEntity(new Vector2(250, 455));
            entry02_3.AddRoutePoint(new Vector2(250, 25));
            wave2.AddEntry(entry02_3);
            WaveEntity entry02_4 = new WaveEntity(new Vector2(325, 455));
            entry02_4.AddRoutePoint(new Vector2(325, 25));
            wave2.AddEntry(entry02_4);
            WaveEntity entry02_5 = new WaveEntity(new Vector2(400, 455));
            entry02_5.AddRoutePoint(new Vector2(400, 25));
            wave2.AddEntry(entry02_5);
            waves.Add(wave2);

            Wave wave3 = new Wave();
            WaveEntity entry03_1 = new WaveEntity(new Vector2(425, 25));
            entry03_1.AddRoutePoint(new Vector2(425, 455));
            wave3.AddEntry(entry03_1);
            WaveEntity entry03_2 = new WaveEntity(new Vector2(500, 25));
            entry03_2.AddRoutePoint(new Vector2(500, 455));
            wave3.AddEntry(entry03_2);
            WaveEntity entry03_3 = new WaveEntity(new Vector2(575, 25));
            entry03_3.AddRoutePoint(new Vector2(575, 455));
            wave3.AddEntry(entry03_3);
            WaveEntity entry03_4 = new WaveEntity(new Vector2(650, 25));
            entry03_4.AddRoutePoint(new Vector2(650, 455));
            wave3.AddEntry(entry03_4);
            WaveEntity entry03_5 = new WaveEntity(new Vector2(725, 25));
            entry03_5.AddRoutePoint(new Vector2(725, 455));
            wave3.AddEntry(entry03_5);
            waves.Add(wave3);

            Wave wave4 = new Wave();
            WaveEntity entry04_1 = new WaveEntity(new Vector2(450, 455));
            entry04_1.AddRoutePoint(new Vector2(450, 25));
            wave4.AddEntry(entry04_1);
            WaveEntity entry04_2 = new WaveEntity(new Vector2(525, 455));
            entry04_2.AddRoutePoint(new Vector2(525, 25));
            wave4.AddEntry(entry04_2);
            WaveEntity entry04_3 = new WaveEntity(new Vector2(600, 455));
            entry04_3.AddRoutePoint(new Vector2(600, 25));
            wave4.AddEntry(entry04_3);
            WaveEntity entry04_4 = new WaveEntity(new Vector2(675, 455));
            entry04_4.AddRoutePoint(new Vector2(675, 25));
            wave4.AddEntry(entry04_4);
            WaveEntity entry04_5 = new WaveEntity(new Vector2(750, 455));
            entry04_5.AddRoutePoint(new Vector2(750, 25));
            wave4.AddEntry(entry04_5);
            waves.Add(wave4);

            Wave wave5 = new Wave();
            WaveEntity entry05_1 = new WaveEntity(new Vector2(100, 40));
            entry05_1.AddRoutePoint(new Vector2(700, 40));
            entry05_1.AddRoutePoint(new Vector2(700, 120));
            entry05_1.AddRoutePoint(new Vector2(100, 120));
            wave5.AddEntry(entry05_1);
            WaveEntity entry05_2 = new WaveEntity(new Vector2(100, 120));
            entry05_2.AddRoutePoint(new Vector2(700, 120));
            entry05_2.AddRoutePoint(new Vector2(700, 200));
            entry05_2.AddRoutePoint(new Vector2(100, 200));
            wave5.AddEntry(entry05_2);
            WaveEntity entry05_3 = new WaveEntity(new Vector2(100, 200));
            entry05_3.AddRoutePoint(new Vector2(700, 200));
            entry05_3.AddRoutePoint(new Vector2(700, 280));
            entry05_3.AddRoutePoint(new Vector2(100, 280));
            wave5.AddEntry(entry05_3);
            WaveEntity entry05_4 = new WaveEntity(new Vector2(100, 280));
            entry05_4.AddRoutePoint(new Vector2(700, 280));
            entry05_4.AddRoutePoint(new Vector2(700, 360));
            entry05_4.AddRoutePoint(new Vector2(100, 360));
            wave5.AddEntry(entry05_4);
            WaveEntity entry05_5 = new WaveEntity(new Vector2(100, 360));
            entry05_5.AddRoutePoint(new Vector2(700, 360));
            entry05_5.AddRoutePoint(new Vector2(700, 440));
            entry05_5.AddRoutePoint(new Vector2(100, 440));
            wave5.AddEntry(entry05_5);
            waves.Add(wave5);

            Wave wave6 = new Wave();
            WaveEntity entry06_1 = new WaveEntity(new Vector2(100, 120));
            entry06_1.AddRoutePoint(new Vector2(700, 120));
            entry06_1.AddRoutePoint(new Vector2(700, 40));
            entry06_1.AddRoutePoint(new Vector2(100, 40));
            wave6.AddEntry(entry06_1);
            WaveEntity entry06_2 = new WaveEntity(new Vector2(100, 200));
            entry06_2.AddRoutePoint(new Vector2(700, 200));
            entry06_2.AddRoutePoint(new Vector2(700, 120));
            entry06_2.AddRoutePoint(new Vector2(100, 120));
            wave6.AddEntry(entry06_2);
            WaveEntity entry06_3 = new WaveEntity(new Vector2(100, 280));
            entry06_3.AddRoutePoint(new Vector2(700, 280));
            entry06_3.AddRoutePoint(new Vector2(700, 200));
            entry06_3.AddRoutePoint(new Vector2(100, 200));
            wave6.AddEntry(entry06_3);
            WaveEntity entry06_4 = new WaveEntity(new Vector2(100, 360));
            entry06_4.AddRoutePoint(new Vector2(700, 360));
            entry06_4.AddRoutePoint(new Vector2(700, 280));
            entry06_4.AddRoutePoint(new Vector2(100, 280));
            wave6.AddEntry(entry06_4);
            WaveEntity entry06_5 = new WaveEntity(new Vector2(100, 440));
            entry06_5.AddRoutePoint(new Vector2(700, 440));
            entry06_5.AddRoutePoint(new Vector2(700, 360));
            entry06_5.AddRoutePoint(new Vector2(100, 360));
            wave6.AddEntry(entry06_5);
            waves.Add(wave6);

            Wave wave7 = new Wave();
            WaveEntity entry07_1 = new WaveEntity(new Vector2(700, 40));
            entry07_1.AddRoutePoint(new Vector2(100, 40));
            entry07_1.AddRoutePoint(new Vector2(100, 120));
            entry07_1.AddRoutePoint(new Vector2(700, 120));
            wave7.AddEntry(entry07_1);
            WaveEntity entry07_2 = new WaveEntity(new Vector2(700, 120));
            entry07_2.AddRoutePoint(new Vector2(100, 120));
            entry07_2.AddRoutePoint(new Vector2(100, 200));
            entry07_2.AddRoutePoint(new Vector2(700, 200));
            wave7.AddEntry(entry07_2);
            WaveEntity entry07_3 = new WaveEntity(new Vector2(700, 200));
            entry07_3.AddRoutePoint(new Vector2(100, 200));
            entry07_3.AddRoutePoint(new Vector2(100, 280));
            entry07_3.AddRoutePoint(new Vector2(700, 280));
            wave7.AddEntry(entry07_3);
            WaveEntity entry07_4 = new WaveEntity(new Vector2(700, 280));
            entry07_4.AddRoutePoint(new Vector2(100, 280));
            entry07_4.AddRoutePoint(new Vector2(100, 360));
            entry07_4.AddRoutePoint(new Vector2(700, 360));
            wave7.AddEntry(entry07_4);
            WaveEntity entry07_5 = new WaveEntity(new Vector2(700, 360));
            entry07_5.AddRoutePoint(new Vector2(100, 360));
            entry07_5.AddRoutePoint(new Vector2(100, 440));
            entry07_5.AddRoutePoint(new Vector2(700, 440));
            wave7.AddEntry(entry07_5);
            waves.Add(wave7);

            Wave wave8 = new Wave();
            WaveEntity entry08_1 = new WaveEntity(new Vector2(700, 120));
            entry08_1.AddRoutePoint(new Vector2(100, 120));
            entry08_1.AddRoutePoint(new Vector2(100, 40));
            entry08_1.AddRoutePoint(new Vector2(700, 40));
            wave8.AddEntry(entry08_1);
            WaveEntity entry08_2 = new WaveEntity(new Vector2(700, 200));
            entry08_2.AddRoutePoint(new Vector2(100, 200));
            entry08_2.AddRoutePoint(new Vector2(100, 120));
            entry08_2.AddRoutePoint(new Vector2(700, 120));
            wave8.AddEntry(entry08_2);
            WaveEntity entry08_3 = new WaveEntity(new Vector2(700, 280));
            entry08_3.AddRoutePoint(new Vector2(100, 280));
            entry08_3.AddRoutePoint(new Vector2(100, 200));
            entry08_3.AddRoutePoint(new Vector2(700, 200));
            wave8.AddEntry(entry08_3);
            WaveEntity entry08_4 = new WaveEntity(new Vector2(700, 360));
            entry08_4.AddRoutePoint(new Vector2(100, 360));
            entry08_4.AddRoutePoint(new Vector2(100, 280));
            entry08_4.AddRoutePoint(new Vector2(700, 280));
            wave8.AddEntry(entry08_4);
            WaveEntity entry08_5 = new WaveEntity(new Vector2(700, 440));
            entry08_5.AddRoutePoint(new Vector2(100, 440));
            entry08_5.AddRoutePoint(new Vector2(100, 360));
            entry08_5.AddRoutePoint(new Vector2(700, 360));
            wave8.AddEntry(entry08_5);
            waves.Add(wave8);

            Wave wave9 = new Wave();
            WaveEntity entry09_1 = new WaveEntity(new Vector2(100, 240));
            entry09_1.AddRoutePoint(new Vector2(100, 25));
            wave9.AddEntry(entry09_1);
            WaveEntity entry09_2 = new WaveEntity(new Vector2(200, 240));
            entry09_2.AddRoutePoint(new Vector2(200, 25));
            wave9.AddEntry(entry09_2);
            WaveEntity entry09_3 = new WaveEntity(new Vector2(300, 240));
            entry09_3.AddRoutePoint(new Vector2(300, 25));
            wave9.AddEntry(entry09_3);
            WaveEntity entry09_4 = new WaveEntity(new Vector2(400, 240));
            entry09_4.AddRoutePoint(new Vector2(400, 25));
            wave9.AddEntry(entry09_4);
            WaveEntity entry09_5 = new WaveEntity(new Vector2(500, 240));
            entry09_5.AddRoutePoint(new Vector2(500, 25));
            wave9.AddEntry(entry09_5);
            WaveEntity entry09_6 = new WaveEntity(new Vector2(600, 240));
            entry09_6.AddRoutePoint(new Vector2(600, 25));
            wave9.AddEntry(entry09_6);
            WaveEntity entry09_7 = new WaveEntity(new Vector2(700, 240));
            entry09_7.AddRoutePoint(new Vector2(700, 25));
            wave9.AddEntry(entry09_7);
            waves.Add(wave9);

            Wave wave10 = new Wave();
            WaveEntity entry10_1 = new WaveEntity(new Vector2(100, 240));
            entry10_1.AddRoutePoint(new Vector2(100, 455));
            wave10.AddEntry(entry10_1);
            WaveEntity entry10_2 = new WaveEntity(new Vector2(200, 240));
            entry10_2.AddRoutePoint(new Vector2(200, 455));
            wave10.AddEntry(entry10_2);
            WaveEntity entry10_3 = new WaveEntity(new Vector2(300, 240));
            entry10_3.AddRoutePoint(new Vector2(300, 455));
            wave10.AddEntry(entry10_3);
            WaveEntity entry10_4 = new WaveEntity(new Vector2(400, 240));
            entry10_4.AddRoutePoint(new Vector2(400, 455));
            wave10.AddEntry(entry10_4);
            WaveEntity entry10_5 = new WaveEntity(new Vector2(500, 240));
            entry10_5.AddRoutePoint(new Vector2(500, 455));
            wave10.AddEntry(entry10_5);
            WaveEntity entry10_6 = new WaveEntity(new Vector2(600, 240));
            entry10_6.AddRoutePoint(new Vector2(600, 455));
            wave10.AddEntry(entry10_6);
            WaveEntity entry10_7 = new WaveEntity(new Vector2(700, 240));
            entry10_7.AddRoutePoint(new Vector2(700, 455));
            wave10.AddEntry(entry10_7);
            waves.Add(wave10);

            Wave wave11 = new Wave();
            WaveEntity entry11_1 = new WaveEntity(new Vector2(25, 220));
            entry11_1.AddRoutePoint(new Vector2(750, 220));
            wave11.AddEntry(entry11_1);
            WaveEntity entry11_2 = new WaveEntity(new Vector2(25, 180));
            entry11_2.AddRoutePoint(new Vector2(750, 180));
            wave11.AddEntry(entry11_2);
            WaveEntity entry11_3 = new WaveEntity(new Vector2(25, 140));
            entry11_3.AddRoutePoint(new Vector2(750, 140));
            wave11.AddEntry(entry11_3);
            WaveEntity entry11_4 = new WaveEntity(new Vector2(25, 100));
            entry11_4.AddRoutePoint(new Vector2(750, 100));
            wave11.AddEntry(entry11_4);
            WaveEntity entry11_5 = new WaveEntity(new Vector2(25, 60));
            entry11_5.AddRoutePoint(new Vector2(750, 60));
            wave11.AddEntry(entry11_5);
            WaveEntity entry11_6 = new WaveEntity(new Vector2(25, 20));
            entry11_6.AddRoutePoint(new Vector2(750, 20));
            wave11.AddEntry(entry11_6);
            WaveEntity entry11_7 = new WaveEntity(new Vector2(25, 260));
            entry11_7.AddRoutePoint(new Vector2(750, 260));
            wave11.AddEntry(entry11_7);
            WaveEntity entry11_8 = new WaveEntity(new Vector2(25, 300));
            entry11_8.AddRoutePoint(new Vector2(750, 300));
            wave11.AddEntry(entry11_8);
            WaveEntity entry11_9 = new WaveEntity(new Vector2(25, 340));
            entry11_9.AddRoutePoint(new Vector2(750, 340));
            wave11.AddEntry(entry11_9);
            WaveEntity entry11_10 = new WaveEntity(new Vector2(25, 380));
            entry11_10.AddRoutePoint(new Vector2(750, 380));
            wave11.AddEntry(entry11_10);
            WaveEntity entry11_11 = new WaveEntity(new Vector2(25, 420));
            entry11_11.AddRoutePoint(new Vector2(750, 420));
            wave11.AddEntry(entry11_11);
            WaveEntity entry11_12 = new WaveEntity(new Vector2(25, 460));
            entry11_12.AddRoutePoint(new Vector2(750, 460));
            wave11.AddEntry(entry11_12);
            waves.Add(wave11);

            Wave wave12 = new Wave();
            WaveEntity entry12_1 = new WaveEntity(new Vector2(750, 220));
            entry12_1.AddRoutePoint(new Vector2(25, 220));
            wave12.AddEntry(entry12_1);
            WaveEntity entry12_2 = new WaveEntity(new Vector2(750, 180));
            entry12_2.AddRoutePoint(new Vector2(25, 180));
            wave12.AddEntry(entry12_2);
            WaveEntity entry12_3 = new WaveEntity(new Vector2(750, 140));
            entry12_3.AddRoutePoint(new Vector2(25, 140));
            wave12.AddEntry(entry12_3);
            WaveEntity entry12_4 = new WaveEntity(new Vector2(750, 100));
            entry12_4.AddRoutePoint(new Vector2(25, 100));
            wave12.AddEntry(entry12_4);
            WaveEntity entry12_5 = new WaveEntity(new Vector2(750, 60));
            entry12_5.AddRoutePoint(new Vector2(25, 60));
            wave12.AddEntry(entry12_5);
            WaveEntity entry12_6 = new WaveEntity(new Vector2(750, 20));
            entry12_6.AddRoutePoint(new Vector2(25, 20));
            wave12.AddEntry(entry12_6);
            WaveEntity entry12_7 = new WaveEntity(new Vector2(750, 260));
            entry12_7.AddRoutePoint(new Vector2(25, 260));
            wave12.AddEntry(entry12_7);
            WaveEntity entry12_8 = new WaveEntity(new Vector2(750, 300));
            entry12_8.AddRoutePoint(new Vector2(25, 300));
            wave12.AddEntry(entry12_8);
            WaveEntity entry12_9 = new WaveEntity(new Vector2(750, 340));
            entry12_9.AddRoutePoint(new Vector2(25, 340));
            wave12.AddEntry(entry12_9);
            WaveEntity entry12_10 = new WaveEntity(new Vector2(750, 380));
            entry12_10.AddRoutePoint(new Vector2(25, 380));
            wave12.AddEntry(entry12_10);
            WaveEntity entry12_11 = new WaveEntity(new Vector2(750, 420));
            entry12_11.AddRoutePoint(new Vector2(25, 420));
            wave12.AddEntry(entry12_11);
            WaveEntity entry12_12 = new WaveEntity(new Vector2(750, 460));
            entry12_12.AddRoutePoint(new Vector2(25, 460));
            wave12.AddEntry(entry12_12);
            waves.Add(wave12);

            Wave wave13 = new Wave();
            WaveEntity entry13_1 = new WaveEntity(new Vector2(380, 25));
            entry13_1.AddRoutePoint(new Vector2(380, 455));
            wave13.AddEntry(entry13_1);
            WaveEntity entry13_2 = new WaveEntity(new Vector2(340, 25));
            entry13_2.AddRoutePoint(new Vector2(340, 455));
            wave13.AddEntry(entry13_2);
            WaveEntity entry13_3 = new WaveEntity(new Vector2(300, 25));
            entry13_3.AddRoutePoint(new Vector2(300, 455));
            wave13.AddEntry(entry13_3);
            WaveEntity entry13_4 = new WaveEntity(new Vector2(260, 25));
            entry13_4.AddRoutePoint(new Vector2(260, 455));
            wave13.AddEntry(entry13_4);
            WaveEntity entry13_5 = new WaveEntity(new Vector2(220, 25));
            entry13_5.AddRoutePoint(new Vector2(220, 455));
            wave13.AddEntry(entry13_5);
            WaveEntity entry13_6 = new WaveEntity(new Vector2(180, 25));
            entry13_6.AddRoutePoint(new Vector2(180, 455));
            wave13.AddEntry(entry13_6);
            WaveEntity entry13_7 = new WaveEntity(new Vector2(140, 25));
            entry13_7.AddRoutePoint(new Vector2(140, 455));
            wave13.AddEntry(entry13_7);
            WaveEntity entry13_8 = new WaveEntity(new Vector2(100, 25));
            entry13_8.AddRoutePoint(new Vector2(100, 455));
            wave13.AddEntry(entry13_8);
            WaveEntity entry13_9 = new WaveEntity(new Vector2(60, 25));
            entry13_9.AddRoutePoint(new Vector2(60, 455));
            wave13.AddEntry(entry13_9);
            WaveEntity entry13_10 = new WaveEntity(new Vector2(420, 25));
            entry13_10.AddRoutePoint(new Vector2(420, 455));
            wave13.AddEntry(entry13_10);
            WaveEntity entry13_11 = new WaveEntity(new Vector2(460, 25));
            entry13_11.AddRoutePoint(new Vector2(460, 455));
            wave13.AddEntry(entry13_11);
            WaveEntity entry13_12 = new WaveEntity(new Vector2(500, 25));
            entry13_12.AddRoutePoint(new Vector2(500, 455));
            wave13.AddEntry(entry13_12);
            WaveEntity entry13_13 = new WaveEntity(new Vector2(540, 25));
            entry13_13.AddRoutePoint(new Vector2(540, 455));
            wave13.AddEntry(entry13_13);
            WaveEntity entry13_14 = new WaveEntity(new Vector2(580, 25));
            entry13_14.AddRoutePoint(new Vector2(580, 455));
            wave13.AddEntry(entry13_14);
            WaveEntity entry13_15 = new WaveEntity(new Vector2(620, 25));
            entry13_15.AddRoutePoint(new Vector2(620, 455));
            wave13.AddEntry(entry13_15);
            WaveEntity entry13_16 = new WaveEntity(new Vector2(660, 25));
            entry13_16.AddRoutePoint(new Vector2(660, 455));
            wave13.AddEntry(entry13_16);
            WaveEntity entry13_17 = new WaveEntity(new Vector2(700, 25));
            entry13_17.AddRoutePoint(new Vector2(700, 455));
            wave13.AddEntry(entry13_17);
            WaveEntity entry13_18 = new WaveEntity(new Vector2(740, 25));
            entry13_18.AddRoutePoint(new Vector2(740, 455));
            wave13.AddEntry(entry13_18);
            waves.Add(wave13);

            Wave wave14 = new Wave();
            WaveEntity entry14_1 = new WaveEntity(new Vector2(380, 455));
            entry14_1.AddRoutePoint(new Vector2(380, 25));
            wave14.AddEntry(entry14_1);
            WaveEntity entry14_2 = new WaveEntity(new Vector2(340, 455));
            entry14_2.AddRoutePoint(new Vector2(340, 25));
            wave14.AddEntry(entry14_2);
            WaveEntity entry14_3 = new WaveEntity(new Vector2(300, 455));
            entry14_3.AddRoutePoint(new Vector2(300, 25));
            wave14.AddEntry(entry14_3);
            WaveEntity entry14_4 = new WaveEntity(new Vector2(260, 455));
            entry14_4.AddRoutePoint(new Vector2(260, 25));
            wave14.AddEntry(entry14_4);
            WaveEntity entry14_5 = new WaveEntity(new Vector2(220, 455));
            entry14_5.AddRoutePoint(new Vector2(220, 25));
            wave14.AddEntry(entry14_5);
            WaveEntity entry14_6 = new WaveEntity(new Vector2(180, 455));
            entry14_6.AddRoutePoint(new Vector2(180, 25));
            wave14.AddEntry(entry14_6);
            WaveEntity entry14_7 = new WaveEntity(new Vector2(140, 455));
            entry14_7.AddRoutePoint(new Vector2(140, 25));
            wave14.AddEntry(entry14_7);
            WaveEntity entry14_8 = new WaveEntity(new Vector2(100, 455));
            entry14_8.AddRoutePoint(new Vector2(100, 25));
            wave14.AddEntry(entry14_8);
            WaveEntity entry14_9 = new WaveEntity(new Vector2(60, 455));
            entry14_9.AddRoutePoint(new Vector2(60, 25));
            wave14.AddEntry(entry14_9);
            WaveEntity entry14_10 = new WaveEntity(new Vector2(420, 455));
            entry14_10.AddRoutePoint(new Vector2(420, 25));
            wave14.AddEntry(entry14_10);
            WaveEntity entry14_11 = new WaveEntity(new Vector2(460, 455));
            entry14_11.AddRoutePoint(new Vector2(460, 25));
            wave14.AddEntry(entry14_11);
            WaveEntity entry14_12 = new WaveEntity(new Vector2(500, 455));
            entry14_12.AddRoutePoint(new Vector2(500, 25));
            wave14.AddEntry(entry14_12);
            WaveEntity entry14_13 = new WaveEntity(new Vector2(540, 455));
            entry14_13.AddRoutePoint(new Vector2(540, 25));
            wave14.AddEntry(entry14_13);
            WaveEntity entry14_14 = new WaveEntity(new Vector2(580, 455));
            entry14_14.AddRoutePoint(new Vector2(580, 25));
            wave14.AddEntry(entry14_14);
            WaveEntity entry14_15 = new WaveEntity(new Vector2(620, 455));
            entry14_15.AddRoutePoint(new Vector2(620, 25));
            wave14.AddEntry(entry14_15);
            WaveEntity entry14_16 = new WaveEntity(new Vector2(660, 455));
            entry14_16.AddRoutePoint(new Vector2(660, 25));
            wave14.AddEntry(entry14_16);
            WaveEntity entry14_17 = new WaveEntity(new Vector2(700, 455));
            entry14_17.AddRoutePoint(new Vector2(700, 25));
            wave14.AddEntry(entry14_17);
            WaveEntity entry14_18 = new WaveEntity(new Vector2(740, 455));
            entry14_18.AddRoutePoint(new Vector2(740, 25));
            wave14.AddEntry(entry14_18);
            waves.Add(wave14);

            /* since Version 1.3 */

            Wave wave15 = new Wave();
            WaveEntity entry15_1 = new WaveEntity(new Vector2(250, 240));
            entry15_1.AddRoutePoint(new Vector2(550, 240));
            wave15.AddEntry(entry15_1);
            WaveEntity entry15_2 = new WaveEntity(new Vector2(230, 205));
            entry15_2.AddRoutePoint(new Vector2(530, 205));
            wave15.AddEntry(entry15_2);
            WaveEntity entry15_3 = new WaveEntity(new Vector2(230, 275));
            entry15_3.AddRoutePoint(new Vector2(530, 275));
            wave15.AddEntry(entry15_3);
            WaveEntity entry15_4 = new WaveEntity(new Vector2(210, 170));
            entry15_4.AddRoutePoint(new Vector2(510, 170));
            wave15.AddEntry(entry15_4);
            WaveEntity entry15_5 = new WaveEntity(new Vector2(210, 310));
            entry15_5.AddRoutePoint(new Vector2(510, 310));
            wave15.AddEntry(entry15_5);
            WaveEntity entry15_6 = new WaveEntity(new Vector2(190, 135));
            entry15_6.AddRoutePoint(new Vector2(490, 135));
            wave15.AddEntry(entry15_6);
            WaveEntity entry15_7 = new WaveEntity(new Vector2(190, 345));
            entry15_7.AddRoutePoint(new Vector2(490, 345));
            wave15.AddEntry(entry15_7);
            WaveEntity entry15_8 = new WaveEntity(new Vector2(170, 100));
            entry15_8.AddRoutePoint(new Vector2(470, 100));
            wave15.AddEntry(entry15_8);
            WaveEntity entry15_9 = new WaveEntity(new Vector2(170, 380));
            entry15_9.AddRoutePoint(new Vector2(470, 380));
            wave15.AddEntry(entry15_9);
            WaveEntity entry15_10 = new WaveEntity(new Vector2(150, 65));
            entry15_10.AddRoutePoint(new Vector2(450, 65));
            wave15.AddEntry(entry15_10);
            WaveEntity entry15_11 = new WaveEntity(new Vector2(150, 415));
            entry15_11.AddRoutePoint(new Vector2(450, 415));
            wave15.AddEntry(entry15_11);
            waves.Add(wave15);

            Wave wave16 = new Wave();
            WaveEntity entry16_1 = new WaveEntity(new Vector2(250, 240));
            entry16_1.AddRoutePoint(new Vector2(550, 240));
            wave16.AddEntry(entry16_1);
            WaveEntity entry16_2 = new WaveEntity(new Vector2(230, 205));
            entry16_2.AddRoutePoint(new Vector2(550, 205));
            wave16.AddEntry(entry16_2);
            WaveEntity entry16_3 = new WaveEntity(new Vector2(230, 275));
            entry16_3.AddRoutePoint(new Vector2(550, 275));
            wave16.AddEntry(entry16_3);
            WaveEntity entry16_4 = new WaveEntity(new Vector2(210, 170));
            entry16_4.AddRoutePoint(new Vector2(550, 170));
            wave16.AddEntry(entry16_4);
            WaveEntity entry16_5 = new WaveEntity(new Vector2(210, 310));
            entry16_5.AddRoutePoint(new Vector2(550, 310));
            wave16.AddEntry(entry16_5);
            WaveEntity entry16_6 = new WaveEntity(new Vector2(190, 135));
            entry16_6.AddRoutePoint(new Vector2(550, 135));
            wave16.AddEntry(entry16_6);
            WaveEntity entry16_7 = new WaveEntity(new Vector2(190, 345));
            entry16_7.AddRoutePoint(new Vector2(550, 345));
            wave16.AddEntry(entry16_7);
            WaveEntity entry16_8 = new WaveEntity(new Vector2(170, 100));
            entry16_8.AddRoutePoint(new Vector2(550, 100));
            wave16.AddEntry(entry16_8);
            WaveEntity entry16_9 = new WaveEntity(new Vector2(170, 380));
            entry16_9.AddRoutePoint(new Vector2(550, 380));
            wave16.AddEntry(entry16_9);
            WaveEntity entry16_10 = new WaveEntity(new Vector2(150, 65));
            entry16_10.AddRoutePoint(new Vector2(550, 65));
            wave16.AddEntry(entry16_10);
            WaveEntity entry16_11 = new WaveEntity(new Vector2(150, 415));
            entry16_11.AddRoutePoint(new Vector2(550, 415));
            wave16.AddEntry(entry16_11);
            waves.Add(wave16);

            Wave wave17 = new Wave();
            WaveEntity entry17_1 = new WaveEntity(new Vector2(450, 240));
            entry17_1.AddRoutePoint(new Vector2(250, 240));
            wave17.AddEntry(entry17_1);
            WaveEntity entry17_2 = new WaveEntity(new Vector2(470, 205));
            entry17_2.AddRoutePoint(new Vector2(230, 205));
            wave17.AddEntry(entry17_2);
            WaveEntity entry17_3 = new WaveEntity(new Vector2(470, 275));
            entry17_3.AddRoutePoint(new Vector2(230, 275));
            wave17.AddEntry(entry17_3);
            WaveEntity entry17_4 = new WaveEntity(new Vector2(490, 170));
            entry17_4.AddRoutePoint(new Vector2(210, 170));
            wave17.AddEntry(entry17_4);
            WaveEntity entry17_5 = new WaveEntity(new Vector2(490, 310));
            entry17_5.AddRoutePoint(new Vector2(210, 310));
            wave17.AddEntry(entry17_5);
            WaveEntity entry17_6 = new WaveEntity(new Vector2(510, 135));
            entry17_6.AddRoutePoint(new Vector2(190, 135));
            wave17.AddEntry(entry17_6);
            WaveEntity entry17_7 = new WaveEntity(new Vector2(510, 345));
            entry17_7.AddRoutePoint(new Vector2(190, 345));
            wave17.AddEntry(entry17_7);
            WaveEntity entry17_8 = new WaveEntity(new Vector2(530, 100));
            entry17_8.AddRoutePoint(new Vector2(170, 100));
            wave17.AddEntry(entry17_8);
            WaveEntity entry17_9 = new WaveEntity(new Vector2(530, 380));
            entry17_9.AddRoutePoint(new Vector2(170, 380));
            wave17.AddEntry(entry17_9);
            WaveEntity entry17_10 = new WaveEntity(new Vector2(550, 65));
            entry17_10.AddRoutePoint(new Vector2(150, 65));
            wave17.AddEntry(entry17_10);
            WaveEntity entry17_11 = new WaveEntity(new Vector2(550, 415));
            entry17_11.AddRoutePoint(new Vector2(150, 415));
            wave17.AddEntry(entry17_11);
            waves.Add(wave17);

            Wave wave18 = new Wave();
            WaveEntity entry18_1 = new WaveEntity(new Vector2(540, 240));
            entry18_1.AddRoutePoint(new Vector2(250, 240));
            wave18.AddEntry(entry18_1);
            WaveEntity entry18_2 = new WaveEntity(new Vector2(560, 205));
            entry18_2.AddRoutePoint(new Vector2(230, 205));
            wave18.AddEntry(entry18_2);
            WaveEntity entry18_3 = new WaveEntity(new Vector2(560, 275));
            entry18_3.AddRoutePoint(new Vector2(230, 275));
            wave18.AddEntry(entry18_3);
            WaveEntity entry18_4 = new WaveEntity(new Vector2(540, 170));
            entry18_4.AddRoutePoint(new Vector2(210, 170));
            wave18.AddEntry(entry18_4);
            WaveEntity entry18_5 = new WaveEntity(new Vector2(540, 310));
            entry18_5.AddRoutePoint(new Vector2(210, 310));
            wave18.AddEntry(entry18_5);
            WaveEntity entry18_6 = new WaveEntity(new Vector2(560, 135));
            entry18_6.AddRoutePoint(new Vector2(190, 135));
            wave18.AddEntry(entry18_6);
            WaveEntity entry18_7 = new WaveEntity(new Vector2(560, 345));
            entry18_7.AddRoutePoint(new Vector2(190, 345));
            wave18.AddEntry(entry18_7);
            WaveEntity entry18_8 = new WaveEntity(new Vector2(540, 100));
            entry18_8.AddRoutePoint(new Vector2(170, 100));
            wave18.AddEntry(entry18_8);
            WaveEntity entry18_9 = new WaveEntity(new Vector2(540, 380));
            entry18_9.AddRoutePoint(new Vector2(170, 380));
            wave18.AddEntry(entry18_9);
            WaveEntity entry18_10 = new WaveEntity(new Vector2(560, 65));
            entry18_10.AddRoutePoint(new Vector2(150, 65));
            wave18.AddEntry(entry18_10);
            WaveEntity entry18_11 = new WaveEntity(new Vector2(560, 415));
            entry18_11.AddRoutePoint(new Vector2(150, 415));
            wave18.AddEntry(entry18_11);
            waves.Add(wave18);

            Wave wave19 = new Wave();
            WaveEntity entry19_1 = new WaveEntity(new Vector2(150, 50));
            entry19_1.AddRoutePoint(new Vector2(150, 430));
            wave19.AddEntry(entry19_1);
            WaveEntity entry19_2 = new WaveEntity(new Vector2(200, 430));
            entry19_2.AddRoutePoint(new Vector2(200, 50));
            wave19.AddEntry(entry19_2);
            WaveEntity entry19_3 = new WaveEntity(new Vector2(250, 50));
            entry19_3.AddRoutePoint(new Vector2(250, 430));
            wave19.AddEntry(entry19_3);
            WaveEntity entry19_4 = new WaveEntity(new Vector2(300, 430));
            entry19_4.AddRoutePoint(new Vector2(300, 50));
            wave19.AddEntry(entry19_4);
            WaveEntity entry19_5 = new WaveEntity(new Vector2(350, 50));
            entry19_5.AddRoutePoint(new Vector2(350, 430));
            wave19.AddEntry(entry19_5);
            WaveEntity entry19_6 = new WaveEntity(new Vector2(400, 430));
            entry19_6.AddRoutePoint(new Vector2(400, 50));
            wave19.AddEntry(entry19_6);
            WaveEntity entry19_7 = new WaveEntity(new Vector2(450, 50));
            entry19_7.AddRoutePoint(new Vector2(450, 430));
            wave19.AddEntry(entry19_7);
            WaveEntity entry19_8 = new WaveEntity(new Vector2(500, 430));
            entry19_8.AddRoutePoint(new Vector2(500, 50));
            wave19.AddEntry(entry19_8);
            WaveEntity entry19_9 = new WaveEntity(new Vector2(550, 50));
            entry19_9.AddRoutePoint(new Vector2(550, 430));
            wave19.AddEntry(entry19_9);
            WaveEntity entry19_10 = new WaveEntity(new Vector2(600, 430));
            entry19_10.AddRoutePoint(new Vector2(600, 50));
            wave19.AddEntry(entry19_10);
            WaveEntity entry19_11 = new WaveEntity(new Vector2(650, 50));
            entry19_11.AddRoutePoint(new Vector2(650, 430));
            wave19.AddEntry(entry19_11);
            waves.Add(wave19);
        }

        public void SpawnEnemy(WaveEntity waveEntry, EnemyType type)
        {
            Enemy newEnemy;

            switch (type)
            {
                case EnemyType.Medium:
                    newEnemy = Enemy.CreateMediumEnemy(texture,
                                                       waveEntry.StartPosition);
                    break;

                case EnemyType.Hard:
                    newEnemy = Enemy.CreateHardEnemy(texture,
                                                     waveEntry.StartPosition);
                    break;

                case EnemyType.Speeder:
                    newEnemy = Enemy.CreateSpeederEnemy(texture,
                                                        waveEntry.StartPosition);
                    break;

                case EnemyType.Tank:
                    newEnemy = Enemy.CreateTankEnemy(texture,
                                                     waveEntry.StartPosition);
                    break;

                default:
                    newEnemy = Enemy.CreateEasyEnemy(texture,
                                                     waveEntry.StartPosition);
                    break;
            }

            newEnemy.SetLevel(currentLevel);

            for (int x = 0; x < waveEntry.RoutePoints.Count; x++)
            {
                newEnemy.AddWayPoint(waveEntry.RoutePoints[x]);
            }

            Enemies.Add(newEnemy);

            EffectManager.AddSpawnSmoke(newEnemy.EnemySprite.Center);
        }

        public void SpawnWave(Wave wave)
        {
            EnemyType type;
            int rnd = rand.Next(0, 5);

            switch (rnd)
            {
                case 0:
                    type = EnemyType.Medium;
                    break;

                case 1:
                    type = EnemyType.Hard;
                    break;

                case 2:
                    type = EnemyType.Speeder;
                    break;

                case 3:
                    type = EnemyType.Tank;
                    break;

                default:
                    type = EnemyType.Easy;
                    break;
            }

            for (int i = 0; i < wave.Entries.Count; ++i)
            {
                SpawnEnemy(wave.Entries[i], type);
            }
        }

        public void SpawnSingleEnemy()
        {
            Enemy newEnemy;
            int rnd = rand.Next(0, 5);

            Vector2 randomPosition = getRandomPosition();

            switch (rnd)
            {
                case 0:
                    newEnemy = Enemy.CreateMediumEnemy(texture,
                                                       randomPosition);
                    break;

                case 1:
                    newEnemy = Enemy.CreateHardEnemy(texture,
                                                     randomPosition);
                    break;

                case 2:
                    newEnemy = Enemy.CreateSpeederEnemy(texture,
                                                        randomPosition);
                    break;

                case 3:
                    newEnemy = Enemy.CreateTankEnemy(texture,
                                                     randomPosition);
                    break;

                default:
                    newEnemy = Enemy.CreateEasyEnemy(texture,
                                                     randomPosition);
                    break;
            }

            newEnemy.SetLevel(currentLevel);

            Enemies.Add(newEnemy);

            EffectManager.AddSpawnSmoke(newEnemy.EnemySprite.Center);
        }

        private Vector2 getRandomPosition()
        {
            Vector2 pos;
            do
            {
                int x = rand.Next(25, 775);
                int y = rand.Next(25, 455);
                pos = new Vector2(x, y);
            } while ((playerManager.playerSprite.Center - pos).Length() < 100);

            return pos;
        }

        private void updateWaveSpawns(float elapsed)
        {
            nextWaveTimer -= elapsed;

            if (nextWaveTimer <= 0)
            {
                int rnd1 = rand.Next(0, waves.Count);

                SpawnWave(waves[rnd1]);

                nextWaveTimer = nextWaveMinTimer + (float)rand.NextDouble() * (nextWaveMaxTimer - nextWaveMinTimer);
            }
        }

        private void updateSingleEnemySpawns(float elapsed)
        {
            nextSingleEnemyTimer -= elapsed;

            if (nextSingleEnemyTimer <= 0.0f)
            {
                SpawnSingleEnemy();

                nextSingleEnemyTimer = nextSingleEnemyMinTimer + (float)rand.NextDouble() * (nextSingleEnemyMaxTimer - nextSingleEnemyMinTimer);
            }
        }

        public void Update(float elapsed)
        {
            for (int x = Enemies.Count - 1; x >= 0; --x)
            {
                Enemy enemy = Enemies[x];

                if (!playerManager.IsDestroyed)
                    enemy.CurrentTarget = playerManager.playerSprite.Center;
                else
                    enemy.NotifyPlayerKilled();

                enemy.Update(elapsed);

                if (!enemy.IsActive())
                {
                    Enemies.RemoveAt(x);
                }
            }

            if (this.IsActive)
            {
                updateWaveSpawns(elapsed);
                updateSingleEnemySpawns(elapsed);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        public void Reset()
        {
            this.Enemies.Clear();

            this.nextWaveTimer = InitialNextWaveMaxTimer;

            this.nextWaveMinTimer = InitialNextWaveMinTimer;
            this.nextWaveMaxTimer = InitialNextWaveMaxTimer;

            this.nextSingleEnemyTimer = InitialNextSingleEnemyMaxTimer;
            this.nextSingleEnemyMinTimer = InitialNextSingleEnemyMinTimer;
            this.nextSingleEnemyMaxTimer = InitialNextSingleEnemyMaxTimer;

            this.IsActive = true;
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;

            float tmpMin = (int)(InitialNextWaveMinTimer - (float)Math.Sqrt(lvl - 1) * 0.075f - 0.02 * (lvl - 1)); // 5,0 - WURZEL(A2-1) / 2 * 0,15 - 0,02 * (A2 - 1)
            this.nextWaveMinTimer = Math.Max(tmpMin, 2.5f);

            float tmpMax = (int)(InitialNextWaveMaxTimer - (float)Math.Sqrt(lvl - 1) * 0.075f - 0.01 * (lvl - 1)); // 7,0 - WURZEL(A2-1) / 2 * 0,15 - 0,01 * (A2 - 1)
            this.nextWaveMaxTimer = Math.Max(tmpMax, 4.0f);

            float tmpSingleMin = (int)(InitialNextSingleEnemyMinTimer - (float)Math.Sqrt(lvl - 1) * 0.075f - 0.0025 * (lvl - 1)); // 1,5 - WURZEL(A2-1) / 2 * 0,15 - 0,0025 * (A2 - 1)
            this.nextSingleEnemyMinTimer = Math.Max(tmpSingleMin, 1.0f);

            float tmpSingleMax = (int)(InitialNextSingleEnemyMaxTimer - (float)Math.Sqrt(lvl - 1) * 0.15f - 0.005 * (lvl - 1)); // 3 - WURZEL(A2-1) / 2 * 0,3 - 0,005 * (A2 - 1)
            this.nextSingleEnemyMaxTimer = Math.Max(tmpSingleMax, 1.5f);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            // Enemies
            int enemiesCount = Int32.Parse(data.Dequeue());
            Enemies.Clear();

            for (int i = 0; i < enemiesCount; ++i)
            {
                EnemyType type = EnemyType.Easy;
                Enemy e;

                type = (EnemyType)Enum.Parse(type.GetType(), data.Dequeue(), false);

                switch (type)
                {
                    case EnemyType.Easy:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Medium:
                        e = Enemy.CreateMediumEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Hard:
                        e = Enemy.CreateHardEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Speeder:
                        e = Enemy.CreateSpeederEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Tank:
                        e = Enemy.CreateTankEnemy(texture, Vector2.Zero);
                        break;
                    default:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                }
                e.Activated(data);

                Enemies.Add(e);
            }

            this.MinShipsPerWave = Int32.Parse(data.Dequeue());
            this.MaxShipsPerWave = Int32.Parse(data.Dequeue());

            this.nextWaveTimer = Single.Parse(data.Dequeue());
            this.nextWaveMinTimer = Single.Parse(data.Dequeue());
            this.nextWaveMaxTimer = Single.Parse(data.Dequeue());

            this.nextSingleEnemyTimer = Single.Parse(data.Dequeue());
            this.nextSingleEnemyMinTimer = Single.Parse(data.Dequeue());
            this.nextSingleEnemyMaxTimer = Single.Parse(data.Dequeue());

            this.IsActive = Boolean.Parse(data.Dequeue());

            this.currentLevel = Int32.Parse(data.Dequeue());
        }

        public void Deactivated(Queue<string> data)
        {
            //Enemies
            data.Enqueue(Enemies.Count.ToString());

            for (int i = 0; i < Enemies.Count; ++i)
            {
                data.Enqueue(Enemies[i].Type.ToString());
                Enemies[i].Deactivated(data);
            }

            data.Enqueue(MinShipsPerWave.ToString());
            data.Enqueue(MaxShipsPerWave.ToString());

            data.Enqueue(nextWaveTimer.ToString());
            data.Enqueue(nextWaveMinTimer.ToString());
            data.Enqueue(nextWaveMaxTimer.ToString());

            data.Enqueue(nextSingleEnemyTimer.ToString());
            data.Enqueue(nextSingleEnemyMinTimer.ToString());
            data.Enqueue(nextSingleEnemyMaxTimer.ToString());

            data.Enqueue(IsActive.ToString());

            data.Enqueue(currentLevel.ToString());
        }

        #endregion
    }
}
