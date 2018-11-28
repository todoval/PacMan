using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.IO;

namespace PacMan
{
    public partial class Board : UserControl
    {
        //inicializacia globalnych premennych
        private Timer timer; //casovac
        public Players[] players = new Players[10]; //pole vsetkych hracov ulozenych v tabulke s high score
        public int momentalnyLevel; //level v ktorom sa prave hrac nachadza
        public int userScore; //celkove skore nahrane hracom
        public int zostavajuciPocetPenazi; //zostavajuci pocet penazi na ploche
        private PacMan pacman = new PacMan (360, 520, 0); //deklarovanie pacmana a pozicii, otoceny hore
        //deklarovanie troch duchov
        public Ghost Red, Yellow, Pink;

        //deklarovanie celej mapy
        // 0 - prazdne policko
        // 1 - stena
        public int[,] map = new int [20,20]{ 
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,1,1,1,0,1,1,1,0,1,1,1,1,0,1,1,1,0,1},
            {1,0,1,1,0,0,0,1,1,0,1,1,1,0,0,0,1,1,0,1},
            {1,0,1,1,0,1,0,0,0,0,0,0,0,0,1,0,1,1,0,1},
            {1,0,1,1,0,1,0,1,1,1,1,1,1,0,1,0,0,0,0,1},
            {1,0,0,0,0,1,0,0,1,1,1,1,1,0,1,0,1,1,0,1},
            {1,0,1,1,1,1,1,0,0,0,0,0,0,0,1,0,1,1,0,1},
            {1,0,0,0,0,1,1,0,1,1,1,0,1,0,1,0,1,1,0,1},
            {1,0,1,1,0,1,1,0,0,0,1,0,1,0,1,0,1,1,0,1},
            {1,0,1,1,0,1,1,0,1,1,1,0,1,0,1,0,1,1,0,1},
            {1,0,1,1,0,1,0,0,0,0,0,0,0,0,1,0,1,1,0,1},
            {1,0,1,1,0,1,0,1,0,1,0,1,1,0,1,0,1,1,0,1},
            {1,0,0,0,0,1,0,1,0,1,0,1,1,0,1,0,1,1,0,1},
            {1,0,1,1,0,1,0,1,0,1,0,1,1,0,1,0,0,0,0,1},
            {1,0,1,1,0,1,0,1,0,1,0,1,1,0,1,0,1,1,0,1},
            {1,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,1},
            {1,0,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,1,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};
        

        public Board()
        {
            InitializeComponent();
            DoubleBuffered = true; //pouziva druhy buffer aby sme predisli blikaniu

            //nastavi level, rozmiesti peniaze na level 1
            momentalnyLevel = 1;
            userScore = 0;
            RozmiestniPeniaze(50);

            //deklarovanie troch duchov
            Red = new Ghost(360, 720, 3, Properties.Resources.red_guy, this);
            Yellow = new Ghost(360, 360, 0, Properties.Resources.yellow_guy, this);
            Pink = new Ghost(360, 40, 1, Properties.Resources.pink_guy, this);

            //inicializuje pole players
            for (int i = 0; i<10; i++)
            {
                players[i] = new Players("unknown", 0);
            }

            //nastavenie casovaca
            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 300;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e) //vsetko co sa udeje pocas jedneho ticku timeru
        {
            int pacManX, pacManY; //premenne ktore hovoria na ktorom indexe na mape map pacman stoji
            pacManX = pacman.PM_X / 40; //preratavanie suradnice X
            pacManY = pacman.PM_Y / 40; //preratavanie suradnice Y

            if (TestCollision() == true)
            {
                goToMenu(); //ak nastala kolizia ukonci hru
                return;
            }

            //skontroluj ci nezjedol mincu a vymaz ju
            if (map[pacManX, pacManY] == 2)
            {
                map[pacManX, pacManY] = 0;
                zostavajuciPocetPenazi--;
                userScore++;
            }

            //posun pacmana
            if ((pacman.PM_direction == 0) && (map[pacManX, pacManY-1] != 1))
            {
                pacman.PM_Y = pacman.PM_Y - 40;
            }
            if ((pacman.PM_direction == 1) && (map[pacManX - 1, pacManY] != 1))
            {
                pacman.PM_X = pacman.PM_X - 40;
            }
            if ((pacman.PM_direction == 2) && (map[pacManX, pacManY + 1] != 1))
            {
                pacman.PM_Y = pacman.PM_Y + 40;
            }
            if ((pacman.PM_direction == 3) && (map[pacManX + 1, pacManY] != 1))
            {
                pacman.PM_X = pacman.PM_X + 40;
            }

            //skontroluj koliziu ducha a pacmana
            if (TestCollision() == true)
            {
                goToMenu(); //ak nastala kolizia ukonci hru
                return;
            }

            //pohyb duchov, cerveny duch chodi dokola po krajoch, zlty a ruzovy sa pohybuju nahodne
            //funkcia OneStep() vrati true ak sa pohli
            Random rnd = new Random(); //pre zlteho a ruzoveho ducha

            //cerveny duch
            if (Red.OneStep() == false) //ak sa cerveny neposunul, otoci sa dolava a posunie sa
            {
                if (Red.direction == 0) Red.direction = 1;
                else if (Red.direction == 1) Red.direction = 2;
                else if (Red.direction == 2) Red.direction = 3;
                else if (Red.direction == 3) Red.direction = 0;
                Red.OneStep();
            }

            //zlty duch
            if (Yellow.OneStep() == false) //ak sa zlty neposunul, otoci sa nahodne a pokracuje
            {
                while (Yellow.OneStep() == false) //kym sa nemoze posunut, toci sa
                {
                    Yellow.direction = rnd.Next(0, 4);
                }
            }
            else //nahodne sa moze otocit aj ked nenarazi na stenu, je tu 30 percentna sanca, inak by sa po case jeho trasa zmenila na trasu cerveneho
            {
                int zmena = rnd.Next(0, 10);
                if (zmena < 3) Yellow.direction = rnd.Next(0, 4);
            }

            //ruzovy duch, robi to iste co zlty
            if (Pink.OneStep() == false) //ak sa ruzovy neposunul, otoci sa nahodne a pokracuje
            {
                while (Pink.OneStep() == false) //kym sa nemoze posunut, toci sa
                {
                    Pink.direction = rnd.Next(0, 4);
                }
            }
            else //nahodne sa moze otocit aj ked nenarazi na stenu, je tu 30 percentna sanca, inak by sa po case jeho trasa zmenila na trasu cerveneho
            {
                int zmena = rnd.Next(0, 10);
                if (zmena < 3) Pink.direction = rnd.Next(0, 4);
            }

            //skontroluj ci hrac nepresiel level
            if (zostavajuciPocetPenazi == 0)
            {
                if (momentalnyLevel == 3) //vyhral hru
                {
                    //vypyta si username a ulozi ho do tabulky (cize do textfile Score.txt v priecinku Debug) na prve miesto
                    timer.Stop();
                    string username = Microsoft.VisualBasic.Interaction.InputBox("Zadaj svoje meno:", "Gratulujem! Vyhral si hru so skore 300!", "unknown", -1, -1);
                    UpdateScore(300, username); //300 je high score (maximalne 50+100+150 penazi moze pozbierat)
                    goToMenu(); //vrati sa do menu
                }
                else //nastavi ho o level vyssie
                {
                    timer.Stop();
                    MessageBox.Show ("Gratulujem! Postupujes do dalsieho levelu!");
                    timer.Start();
                    momentalnyLevel++;
                    zostavajuciPocetPenazi = momentalnyLevel * 50;
                    RozmiestniPeniaze(zostavajuciPocetPenazi); //rozmiestni mince nahodne
                    timer.Interval = timer.Interval - 100; //nastavi hru rychlejsie
                }
            }
            Invalidate(); //updatnutie celej mapy
        }

        public Boolean TestCollision() //kolizia ducha a pacmana, pokial nastane, vrati true a updatne tabulku, pokial nenastane, vrati false
        {
            //ak sa suradnice PacMana rovnaju suradniciam aspon jedneho z tychto troch duchov, hra sa ukonci
            if ((pacman.PM_X == Red.ghostX && pacman.PM_Y == Red.ghostY) ||
                (pacman.PM_X == Yellow.ghostX && pacman.PM_Y == Yellow.ghostY) ||
                (pacman.PM_X == Pink.ghostX && pacman.PM_Y == Pink.ghostY))
            {
                timer.Stop(); //koniec hry v Boarde
                //InputBox, vypytanie si username, aby sme mohli hraca zapisat do tabulky
                string username = Microsoft.VisualBasic.Interaction.InputBox("Zadaj svoje meno:", "Chytil ta duch! Skus znova! Tvoje dosiahnute skore je: " + userScore, "unknown", -1, -1);
                UpdateScore(userScore, username); //updatneme skore tabulky Score.txt so skore nasho hraca
                return true;
            }
            else return false;
        }

        public void goToMenu() //zrusi vsetky procesy a vrati sa do menu
        {
            this.Parent.ClientSize = new Size(800, 1444); //velkost Form1 sa zmeni naspat na povodnu
            
            //zviditelnenie vsetkych User Controls
            foreach (Control c in Parent.Controls)
            {
                c.Visible = true;
            }
            ((Form1)Parent).WriteScore(); //zavola sa funkcia Form1.WriteScore(), updatne sa High Score v tabulke
            this.Parent.Controls.Remove(this); //Board sa sam odstrani
        }


        public void UpdateScore(int score, string username) //updatne tabulku s high score na zaklade toho, ake skore a aky username dostane
        {
            //nacita vsetky skore zo Score.txt do pola players
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("Score.txt")) //otvori file Score.txt
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                //nacitanie vsetkych riadkov do pola players
                for (int k = 0; k<10; k++)
                {
                    line = streamReader.ReadLine();
                    players[k].name = line;
                    line = streamReader.ReadLine();
                    players[k].score = Int32.Parse(line);
                }
                streamReader.Close(); //zatvori file Score.txt

                //zapis novej tabulky
                int i = 0;
                int place = -1; //miesto na ktorom sa nachadza novy hrac, ak je -1, nenachadza sa v tabulke
                while (i < 10) //cyklus, ktory zistuje, na ktorom mieste v tabulke sa nachadza nas hrac
                {
                    if (score >= players[i].score)
                    {
                        place = i; //ulozime miesto do premennej place
                        i = 10; //ukoncime while cyklus
                    }
                    i++;
                }
                if (place != -1) //ak sa nachadza v tabulke
                {
                    for (int j = 9; j > place; j--) //for cyklus ktory posunie vsetkych hracov horsich ako nas hrac o jedno miesto dalej a odstrani z tabulky posledneho hraca
                    {
                        players[j].score = players[j - 1].score;
                        players[j].name = players[j - 1].name;
                    }
                    //prida nasho hraca do tabulky
                    players[place].score = score;
                    players[place].name = username;
                }
                //v poli players mame teraz uz updatnutu tabulku

                //zapiseme tuto tabulku do Score.txt
                System.IO.File.Delete("Score.txt"); //vymazeme file Score.txt
                using (StreamWriter sw = File.CreateText("Score.txt")) //vytvorime novy file Score.txt
                {
                    for (int k = 0; k < 10; k++) //zapis do Score.txt
                    {
                        sw.WriteLine(players[k].name);
                        sw.WriteLine(players[k].score);
                    }
                    sw.Close(); //zatvorime file Score.txt
                }
            }
        }

        public void RozmiestniPeniaze(int pocetPenazi) //nahodne rozmiestni peniaze po mape
        {
            zostavajuciPocetPenazi = pocetPenazi; //zostavajuciPocetPenazi na celom Boarde bude taky, ako pocet rozmiestnenych penazi
            while (pocetPenazi > 0)
            {
                Random rnd = new Random();
                int indexRow = rnd.Next(0, 19); //vyberieme nahodny riadok
                int indexColumn = rnd.Next(0, 19); //vyberieme nahodny stlpec
                if (indexRow == pacman.PM_Y/40 && indexColumn == pacman.PM_X/40)
                {
                    //nerob nic, na tom mieste stoji PacMan
                }
                else if (map[indexRow, indexColumn] == 0) //ak na tomto mieste nie je stena (moze tam byt duch, po posunuti ducha sa peniaz zobrazi)
                {
                    map[indexRow, indexColumn] = 2; //daj na toto miesto peniaz
                    pocetPenazi--;
                }
            }
        }

        public void Board_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) //zisti ktoru klavesu sme stlacili a podla toho vykona udalosti
        { 
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.W: //pacman sa otoci hore 
                    pacman.PM_direction = 0;
                    break;
                case System.Windows.Forms.Keys.A: //pacman sa otoci dolava 
                    pacman.PM_direction = 1;
                    break;
                case System.Windows.Forms.Keys.S: //pacman sa otoci dole
                    pacman.PM_direction = 2;
                    break;
                case System.Windows.Forms.Keys.D: //pacman sa otoci doprava
                    pacman.PM_direction = 3;
                    break;
                case System.Windows.Forms.Keys.Back: //ak je stlaceny backspace, vratime sa naspat do menu
                    timer.Stop(); //zastavi sa timer
                    string ack = Microsoft.VisualBasic.Interaction.InputBox("Naozaj si prajes ukoncit hru?", "Upozornenie", "Y/N", -1, -1);
                    if (ack == "Y")
                    {
                        string username = Microsoft.VisualBasic.Interaction.InputBox("Tvoje dosiahnute skore je:" + userScore + "Zadaj svoje meno:", "Hra bude ukoncena", "unknown", -1, -1);
                        goToMenu(); //vratime sa do menu
                    }
                    else timer.Start(); //ak sme stlacili backspace omylom, timer sa opat spusti a hra pokracuje
                    break;
            }        
        }

        protected override void OnPaint(PaintEventArgs e) //vykreslovanie mapy
        {
            base.OnPaint(e);
            DrawMap(e.Graphics); //vykresli sa mapa
            pacman.DrawPM(e.Graphics); //vykresleny pacman
            Red.DrawGhost(e.Graphics); //vykresleny cerveny duch
            Yellow.DrawGhost(e.Graphics); //vykresleny zlty duch
            Pink.DrawGhost(e.Graphics); //vykresleny ruzovy duch
        }

        public void DrawMap(Graphics g)
        {
            Image Coin = Properties.Resources.coin;
            //dva for cykly = pre kazde jedno miesto na mape
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (map[i, j] == 0) g.FillRectangle(new SolidBrush(Color.Black), i*40, j*40, 40, 40); //ak je miesto prazdne, pozadie tohto policka sa vyplni ciernou
                    if (map[i, j] == 1) g.FillRectangle(new SolidBrush(Color.Blue), i * 40, j * 40, 40, 40); //ak je na mieste stena, pozadie tohto policka sa vyplni modrou
                    if (map[i, j] == 2) g.DrawImage(Coin, i*40, j*40, 40, 40); //ak je na mieste peniaz, vykresli sa na tieto suradnice peniaz
                }
            }
        }

        public class Ghost //trieda duch
        {
            public int ghostX; //suradnica x ducha
            public int ghostY; //suradnica y ducha
            public int direction; //smer ducha
            public Bitmap img; //obrazok z Resources, ako duch vyzera
            public Board par;

            public Ghost(int x, int y, int dir, Bitmap picture, Board parent) //konstruktor ducha
            {
                this.ghostX = x;
                this.ghostY = y;
                this.direction = dir;
                this.img = picture;
                this.par = parent;
            }

            public void DrawGhost(Graphics g) //vykreslenie ducha
            {
                g.DrawImage(img, ghostX, ghostY, 40, 40); //na suradniciach ducha sa vykresli jeho obrazok
            }

            public bool OneStep() //jeden krok ducha ak pred nim nic nie je, vracia hodnotu true ak krok vykonal, v opacnom pripade (ak zostal stat), vrati hodnotu false
            {
                if ((direction == 0) && (par.map[ghostX / 40, ghostY / 40 - 1] != 1))
                {
                    ghostY = ghostY - 40;
                    return true;
                }
                else if ((direction == 1) && (par.map[ghostX / 40 - 1, ghostY / 40] != 1))
                {
                    ghostX = ghostX - 40;
                    return true;
                }
                else if ((direction == 2) && (par.map[ghostX / 40, ghostY / 40 + 1] != 1))
                {
                    ghostY = ghostY + 40;
                    return true;
                }
                else if ((direction == 3) && (par.map[ghostX / 40 + 1, ghostY / 40] != 1))
                {
                    ghostX = ghostX + 40;
                    return true;
                }
                else return false;
            }

        }

        public class PacMan //trieda PacMan
        {
            public int PM_X; //suradnica X pacmana
            public int PM_Y; //suradnica Y pacmana
            public int PM_direction; //smer ktorym je otoceny pacman

            //obrazky pacmana v zavistlosti na tom, ktorym smerom je otoceny
            Image UP = Properties.Resources.Up;
            Image DOWN = Properties.Resources.down;
            Image LEFT = Properties.Resources.Left;
            Image RIGHT = Properties.Resources.Right;

            public PacMan(int pm_x, int pm_y, int pm_direction) //konstruktor pacmana
            {
                this.PM_X = pm_x;
                this.PM_Y = pm_y;
                this.PM_direction = pm_direction;
            }

            public void DrawPM (Graphics g) //vykresli pacmana v zavistlosti na jeho smere otocenia
            {
                if (PM_direction == 0) g.DrawImage(UP, PM_X, PM_Y, 40, 40);
                if (PM_direction == 1) g.DrawImage(LEFT, PM_X, PM_Y, 40, 40);
                if (PM_direction == 2) g.DrawImage(DOWN, PM_X, PM_Y, 40, 40);
                if (PM_direction == 3) g.DrawImage(RIGHT, PM_X, PM_Y, 40, 40);
            }
        }

        public class Players //trieda hracov ulozenych v high score tabulke
        {
            public string name; //meno hraca
            public int score; //skore hraca

            public Players(string Name, int Score) //konstruktor
            {
                this.name = Name;
                this.score = Score;
            }
        }

    }
}
