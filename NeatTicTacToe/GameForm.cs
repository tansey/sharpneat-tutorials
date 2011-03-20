/* ***************************************************************************
 * This file is part of the NashCoding tutorial on SharpNEAT 2.
 * 
 * Copyright 2010, Wesley Tansey (wes@nashcoding.com)
 * 
 * Some code in this file may have been copied directly from SharpNEAT,
 * for learning purposes only. Any code copied from SharpNEAT 2 is 
 * copyright of Colin Green (sharpneat@gmail.com).
 *
 * Both SharpNEAT and this tutorial are free software: you can redistribute
 * it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the 
 * License, or (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TicTacToeLib;
using TicTacToeEvolution;
using SharpNeat.Genomes.Neat;
using System.Xml;
using SharpNeat.Decoders.Neat;
using System.IO;
using TicTacToeHyperNeat;
using SharpNeat.Phenomes;
using SharpNeat.Core;

namespace NeatTicTacToe
{
    public partial class GameForm : Form
    {
        IPlayer _ai;
        RandomPlayer _randomPlayer;
        OptimalPlayer _optimalPlayer;
        NeatPlayer _neatPlayer;
        TicTacToeExperiment _experiment;
        TicTacToeHyperNeatExperiment _hyperNeatExperiment;
        SquareTypes _humanSquareType;
        SquareTypes _aiSquareType;
        TicTacToeGame _game;
        bool _playing;
        int _movesInCurGame;

        public GameForm()
        {
            InitializeComponent();
            _randomPlayer = new RandomPlayer();
            _optimalPlayer = new OptimalPlayer(SquareTypes.O);
            _neatPlayer = new NeatPlayer(null, SquareTypes.O);
            _aiSquareType = SquareTypes.O;
            _humanSquareType = SquareTypes.X;
            _game = new TicTacToeGame();

            // Set the AI to the random player by default.
            _ai = _randomPlayer;

            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            _experiment = new TicTacToeExperiment();
            _hyperNeatExperiment = new TicTacToeHyperNeatExperiment();

            // Load config XML for the NEAT experiment.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("tictactoe.config.xml");
            _experiment.Initialize("TicTacToe", xmlConfig.DocumentElement);

            // Load config XML for the HyperNEAT experiment.
            xmlConfig = new XmlDocument();
            xmlConfig.Load("hyperneat.config.xml");
            _hyperNeatExperiment.Initialize("TicTacToe", xmlConfig.DocumentElement);
        }

        private void randomPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the AI to a random player.
            _ai = _randomPlayer;

            // Update the check marks.
            randomPlayerToolStripMenuItem.Checked = true;
            optimalPlayerToolStripMenuItem.Checked = false;
            neatPlayerToolStripMenuItem.Checked = false;
        }

        private void optimalPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the AI to an optimal player.
            _ai = _optimalPlayer;

            // Update the check marks.
            randomPlayerToolStripMenuItem.Checked = false;
            optimalPlayerToolStripMenuItem.Checked = true;
            neatPlayerToolStripMenuItem.Checked = false;
        }

        private void neatPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the AI to a neural network player.
            _ai = _neatPlayer;

            // Update the check marks.
            randomPlayerToolStripMenuItem.Checked = false;
            optimalPlayerToolStripMenuItem.Checked = false;
            neatPlayerToolStripMenuItem.Checked = true;
        }

        private void loadNEATPlayerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Loads a NEAT genome from file.
            // You don't have to read in function IDs
            // for NEAT genomes.
            loadGenomeFromFile(false);
        }


        private void loadHyperNEATPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Loads a HyperNEAT genome from file.
            // You need to read in function IDs
            // for HyperNEAT genomes because they
            // evolve the functions to use.
            loadGenomeFromFile(true);
        }

        private void loadGenomeFromFile(bool readFunctionIds)
        {
            // Have the user choose the genome XML file.
            var result = openFileDialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
                return;


            NeatGenome genome = null;

            // Try to load the genome from the XML document.
            try
            {
                using (XmlReader xr = XmlReader.Create(openFileDialog.FileName))
                    genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, readFunctionIds)[0];
            }
            catch (Exception e1)
            {
                MessageBox.Show("Error loading genome from file!\nLoading aborted.\n" + e1.Message);
                return;
            }

            // Get a genome decoder that can convert genomes to phenomes.
            IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder = null;
            
            if (!readFunctionIds)
                // If we don't need to read function IDs, get a NEAT decoder.
                genomeDecoder = _experiment.CreateGenomeDecoder();
            else
                // Reading in function IDs implies that we need a HyperNEAT decoder.
                genomeDecoder = _hyperNeatExperiment.CreateGenomeDecoder();

            // Decode the genome into a phenome (neural network).
            var phenome = genomeDecoder.Decode(genome);

            // Set the NEAT player's brain to the newly loaded neural network.
            _neatPlayer.Brain = phenome;

            // Show the option to select the NEAT player
            if (neatPlayerToolStripMenuItem.Enabled == false)
                neatPlayerToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Sets the human's square type to X and the AI's square type to O.
        /// </summary>
        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _humanSquareType = SquareTypes.X;
            _optimalPlayer.SquareType = SquareTypes.O;
            _neatPlayer.SquareType = SquareTypes.O;
            _aiSquareType = SquareTypes.O;
            xToolStripMenuItem.Checked = true;
            oToolStripMenuItem.Checked = false;
        }

        /// <summary>
        /// Sets the human's square type to O and the AI's square type to X.
        /// </summary>
        private void oToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _humanSquareType = SquareTypes.O;
            _optimalPlayer.SquareType = SquareTypes.X;
            _neatPlayer.SquareType = SquareTypes.X;
            _aiSquareType = SquareTypes.X;
            xToolStripMenuItem.Checked = false;
            oToolStripMenuItem.Checked = true;
        }

        /// <summary>
        /// Resets the board and starts a new game.
        /// </summary>
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _playing = true;
            _game.ResetGame();
            _movesInCurGame = 0;

            // If the AI acts first, get its action
            if (_humanSquareType == SquareTypes.O)
                getComputerOpponentMove();

            Invalidate();
        }

        /// <summary>
        /// Gets the next move from the AI.
        /// </summary>
        private void getComputerOpponentMove()
        {
            // Get the next move.
            var move = _ai.GetMove(_game.Board);

            // Add the first move to the game board.
            _game.Board[move.X, move.Y] = _aiSquareType;

            // Increment the moves counter.
            _movesInCurGame++;
        }

        /// <summary>
        /// Fills in the square that the human clicked on,
        /// gets the AI's response, and checks for a winner.
        /// </summary>
        private void GameForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!_playing)
                return;

            #region Get the game board location that the user clicked on.
            var x = 0;
            if (e.X > 2 * this.ClientRectangle.Width / 3)
                x = 2;
            else if (e.X > this.ClientRectangle.Width / 3)
                x = 1;

            var y = 0;
            if (e.Y > 2 * this.ClientRectangle.Height / 3)
                y = 2;
            else if (e.Y > this.ClientRectangle.Height / 3)
                y = 1;
            #endregion
            
            // If the user didn't click on a valid square, assume it was
            // a misclick and disregard it.
            if (!_game.IsEmpty(x, y))
                return;

            // Increment the move counter.
            _movesInCurGame++;

            // Set the human's move.
            _game.Board[x, y] = _humanSquareType;

            // Check if the game is over
            var winner = _game.GetWinner();

            // If the game isn't over yet, get the AI's response.
            if (winner == SquareTypes.N && _movesInCurGame < 9)
            {
                // If the game isn't over, then get the AI's response.
                getComputerOpponentMove();

                // Check again if the game is over.
                winner = _game.GetWinner();
            }

            // Draw the move on the game board.
            Invalidate();

            // Check if the game is over.
            if(winner == _aiSquareType)
                endGame("You Lost!");
            else if(winner == _humanSquareType)
                endGame("You Won!");
            else if(_movesInCurGame >= 9)
                endGame("You Tied!");
        }

        /// <summary>
        /// Ends the game and displays a pop-up message to the player.
        /// </summary>
        private void endGame(string message)
        {
            MessageBox.Show(message);
            _playing = false;
        }
        
        #region Drawing crap
        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var blackPen = new Pen(Color.Black);
            var x1 = this.ClientRectangle.Width / 3;
            var x2 = 2 * this.ClientRectangle.Width / 3;
            g.DrawLine(blackPen, x1, 0, x1, this.ClientRectangle.Bottom);
            g.DrawLine(blackPen, x2, 0, x2, this.ClientRectangle.Bottom);


            var y1 = this.ClientRectangle.Height / 3;
            var y2 = 2 * this.ClientRectangle.Height / 3;
            g.DrawLine(blackPen, 0, y1, this.ClientRectangle.Right, y1);
            g.DrawLine(blackPen, 0, y2, this.ClientRectangle.Right, y2);

            blackPen.Dispose();

            const int buffer = 15;
            var xPen = new Pen(Color.Red, 3);
            var circlePen = new Pen(Color.Blue, 3);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (_game.Board[i, j] == SquareTypes.O)
                        g.DrawEllipse(circlePen, x1 * i + buffer, y1 * j + buffer,
                                                 x1 - buffer * 2, y1 - buffer * 2);
                    else if (_game.Board[i, j] == SquareTypes.X)
                    {
                        g.DrawLine(xPen, x1 * i + buffer, y1 * j + buffer,
                                                 x1 * (i + 1) - buffer * 2, y1 * (j + 1) - buffer * 2);

                        g.DrawLine(xPen, x1 * (i + 1) - buffer * 2, y1 * j + buffer,
                                                  x1 * i + buffer, y1 * (j + 1) - buffer * 2);
                    }
                }
            base.OnPaint(e);
        }
        #endregion

    }
}
