/*  
   PacMan
   Lucia Tódová
   letný semester 2016/17, krúžok 41
   Programování
*/

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

namespace PacMan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WriteScore(); //vypíšeme hneď na začiatku skóre, ktoré je uložené v tabuľke Score.txt v priečinku Debug
        }

        private void button1_Click(object sender, EventArgs e) //tlačítko HRA
        {
            //zneviditelni User Controls na Form1
            foreach (Control c in this.Controls)
            {
                c.Visible = false;
            }

            //inicializujeme novy board a prida ho na form
            Board board = new Board();
            board.Dock = DockStyle.Fill;
            this.Controls.Add(board);
            this.KeyPreview = true;
            board.Focus(); 

            //zmeni form na velkost, do ktorej sa presne zmesti board
            this.Size = new Size(817, 840);
          
        }

        public void WriteScore() //vypise skore zo subore Score.txt do tabulky High Score zobrazenej na Form1
        {
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("Score.txt")) //otvori subor Score.txt
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize)) //nacitanie vsetkych riadkov zo suboru Score.txt do textboxov
            {
                String line;
                line = streamReader.ReadLine();
                string nameOne = line;
                line = streamReader.ReadLine();
                string scoreOne = line;
                line = streamReader.ReadLine();
                string nameTwo = line;
                line = streamReader.ReadLine();
                string scoreTwo = line;
                line = streamReader.ReadLine();
                string nameThree = line;
                line = streamReader.ReadLine();
                string scoreThree = line;
                line = streamReader.ReadLine();
                string nameFour = line;
                line = streamReader.ReadLine();
                string scoreFour = line;
                line = streamReader.ReadLine();
                string nameFive = line;
                line = streamReader.ReadLine();
                string scoreFive = line;
                line = streamReader.ReadLine();
                string nameSix = line;
                line = streamReader.ReadLine();
                string scoreSix = line;
                line = streamReader.ReadLine();
                string nameSeven = line;
                line = streamReader.ReadLine();
                string scoreSeven = line;
                line = streamReader.ReadLine();
                string nameEight = line;
                line = streamReader.ReadLine();
                string scoreEight = line;
                line = streamReader.ReadLine();
                string nameNine = line;
                line = streamReader.ReadLine();
                string scoreNine = line;
                line = streamReader.ReadLine();
                string nameTen = line;
                line = streamReader.ReadLine();
                string scoreTen = line;
                textBox11.Text = scoreOne;
                textBox12.Text = scoreTwo;
                textBox13.Text = scoreThree;
                textBox14.Text = scoreFour;
                textBox15.Text = scoreFive;
                textBox16.Text = scoreSix;
                textBox17.Text = scoreSeven;
                textBox18.Text = scoreEight;
                textBox19.Text = scoreNine;
                textBox20.Text = scoreTen;
                textBox1.Text = nameOne;
                textBox2.Text = nameTwo;
                textBox3.Text = nameThree;
                textBox4.Text = nameFour;
                textBox5.Text = nameFive;
                textBox6.Text = nameSix;
                textBox7.Text = nameSeven;
                textBox8.Text = nameEight;
                textBox9.Text = nameNine;
                textBox10.Text = nameTen;
                streamReader.Close(); //zatvorenie suboru
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) //ak stlacime escape, cely program sa ukonci
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e) //tlacitko vynuluj skore
            //vezme textfile Score.txt a prepise kazde meno na unknown so skore 0, vzapati tabulku vypise
        {
            //InputBox, ktory zisti ci naozaj chceme skore vynulovat
            string ack = Microsoft.VisualBasic.Interaction.InputBox("Vsetky nahrane body budu vynulovane. Si si isty, ze chces pokracovat? Pre vynulovanie tabulky odpovedz Y:", "Upozornenie", "Y/N", -1, -1);
            if (ack == "Y") //ak je odpoved ano
            {
                System.IO.File.Delete("Score.txt"); //vymazeme cely file Score.txt
                using (StreamWriter sw = File.CreateText("Score.txt")) //vytvorime novy file Score.txt
                {
                    //do Score.txt pridame 10 dvojriadkov s unknown menami hracov a nahranym skore 0
                    for (int i = 0; i < 9; i++)
                    {
                        sw.WriteLine("unknown");
                        sw.WriteLine("0");
                    }
                    sw.WriteLine("unknown");
                    sw.Write("0");
                    sw.Close(); //zatvorime Score.txt
                    WriteScore(); //Score.txt sa vypise do High Score tabulky (v opacnom pripade by textboxy zostali prazdne)
                }
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }
    }
}
