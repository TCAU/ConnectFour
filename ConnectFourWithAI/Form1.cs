using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectFour
{
    public partial class Form1 : Form
    {

        Dictionary<Keys,bool> input = new Dictionary<Keys, bool>();
        int[,] slots;
        int entryDisc;
        int emptySlots;
        bool isGameOver;
        bool isPlayersTurn;

        Brush bWhite = Brushes.White; 
        Brush bBlack = Brushes.Black;
        Brush bRed = Brushes.Red;     
        Brush bGray = Brushes.Gray;

        Random random;

        public Form1()
        {
            InitializeComponent();

            input.Add(Keys.Left, false);
            input.Add(Keys.Right, false);
            input.Add(Keys.Enter, false);

            random = new Random();
            isGameOver = false;
            isPlayersTurn = true;
            slots = new int[7, 6];
            emptySlots = 42;
            ResetGrid();

            timer1.Interval = 50;
            timer1.Tick += PlayerInput;
            timer1.Start();
        }

        private void ResetGrid()
        {
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 6; j++)
                    slots[i, j] = 0;

            entryDisc = 0;
            emptySlots = 42;
        }

        private void PlayerInput(object sender, EventArgs e)
        {
            if (isGameOver && input[Keys.Enter])
            {
                label1.Visible = false;
                ResetGrid();
                isGameOver = false;
            }
            else
            { 
                if (isPlayersTurn)
                {
                    if (input[Keys.Left] && !input[Keys.Right] && entryDisc > 0 && findAndSwapEmptyLeftSlot(entryDisc, out entryDisc)) { }
                    else if (input[Keys.Right] && !input[Keys.Left] && entryDisc < 6 && findAndSwapEmptyRightSlot(entryDisc, out entryDisc)) { }

                        if (input[Keys.Enter])
                        {
                            DropDisc(1);
                            PlaceNewEntryDisc();
                            isPlayersTurn = false;
                        }
                }
                else
                {
                    do
                    {
                        entryDisc = random.Next(0, 7);
                    } while (slots[entryDisc, 0] != 0);

                    DropDisc(-1);
                    isPlayersTurn = true;
                    if (emptySlots > 0)
                    {
                        PlaceNewEntryDisc();
                    }
                }
                checkForGameOver();
            }
            pbCanvas.Invalidate();
        }

        private void checkForGameOver()
        {
            if (emptySlots==0)
            {
                EndGame("No Winners");
            }
            else 
            {
                for (int x = 0; x < 7; x++)
                {
                    for (int y = 0; y < 6; y++)
                    {
                        if (x <= 3)
                        {
                            int sumHorizontal = 0;
                            for (int i = x; i < x+4; i++)
                                sumHorizontal += slots[i, y];
                            checkScore(sumHorizontal);
                        }
                        if (y <=2) 
                        {
                            int sumVertical = 0;
                            for (int i = y; i < y + 4; i++)
                                sumVertical += slots[x, i];
                            checkScore(sumVertical);	 
                        }
                        if (x <= 3 && y <=2) 
                        {
                            int sumDiag1 = 0;
                            for (int i = x, j = y; i < x + 4; i++, j++)
                                sumDiag1 += slots[i, j];
                            checkScore(sumDiag1);
                        }
                        if (x >= 3 && y >= 3)
                        {
                            int sumDiag2 = 0;
                            for (int i = x, j = y; j > y - 4; i--, j--)
                                sumDiag2 += slots[i, j];
                            checkScore(sumDiag2);
                        }
                    }
                }
            }
        }

        private void checkScore(int x)
        {
            if (x == 4)
                EndGame("You Won!");
            if (x == -4)
                EndGame("Computer Wins!");
        }

        private void EndGame(string message)
        {
            isGameOver = true;
            label1.Visible = true;
            label1.Text = "Game over - "+ message + "\nPress Enter to restart";
            isPlayersTurn = true;
        }

        private void PlaceNewEntryDisc()
        {
            entryDisc = 0;

            while (slots[entryDisc, 0] != 0)
                entryDisc++;
        }

        private void DropDisc(int playerValue)
        {
            emptySlots--;
            int index = 5;
            while (slots[entryDisc, index] != 0)
            {
                index--;
            }
            slots[entryDisc, index] = playerValue;
        }

        private bool findAndSwapEmptyLeftSlot(int input, out int output)
        {
            int i;
            for (i = input -1; i >= 0; i--)
            {
                if (slots[i, 0] == 0)
                {
                    output = i;
                    return true;
                }
            }
            output = input;
            return false;
        }

        private bool findAndSwapEmptyRightSlot(int input, out int output)
        {
            int i;
            for (i = input + 1; i < 7; i++)
            {
                if (slots[i, 0] == 0)
                {
                    output = i;
                    return true;
                }
            }
            output = input;
            return false;
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 6; j++)
                {
                    if (slots[i, j] == 0) { g.FillEllipse(bWhite, new Rectangle(i * 90, j * 90, 90, 90)); }
                    else if (slots[i, j] == 1) { g.FillEllipse(bRed, new Rectangle(i * 90, j * 90, 90, 90)); }
                    else if (slots[i, j] == -1) { g.FillEllipse(bBlack, new Rectangle(i * 90, j * 90, 90, 90)); }
                    else if (slots[i, j] == 2) { g.FillEllipse(bGray, new Rectangle(i * 90, j * 90, 90, 90)); }

                    g.FillEllipse(bGray, new Rectangle(entryDisc * 90, 0, 90, 90));
                }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            input[e.KeyCode] = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            input[e.KeyCode] = false;
        }

    }
}