/* ***************************************************************************
 * This file is part of the NashCoding tutorial on SharpNEAT 2.
 * 
 * Copyright 2010, Wesley Tansey (wes@nashcoding.com)
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
using System.Linq;
using System.Text;
using TicTacToeLib;
using SharpNeat.Phenomes;

namespace TicTacToeEvolution
{
    /// <summary>
    /// A neural network Tic-Tac-Toe player.
    /// </summary>
    public class NeatPlayer : IPlayer
    {
        /// <summary>
        /// The neural network that this player uses to make its decision.
        /// </summary>
        public IBlackBox Brain { get; set; }

        /// <summary>
        /// The square type of this player (X or O).
        /// </summary>
        public SquareTypes SquareType { get; set; }

        /// <summary>
        /// Creates a new NEAT player with the specified brain.
        /// </summary>
        public NeatPlayer(IBlackBox brain, SquareTypes squareType)
        {
            Brain = brain;
            SquareType =squareType;
        }

        #region IPlayer Members

        /// <summary>
        /// Gets the next move as dictated by the neural network.
        /// </summary>
        public Move GetMove(SquareTypes[,] board)
        {
            // Clear the network
            Brain.ResetState();

            // Convert the game board into an input array for the network
            setInputSignalArray(Brain.InputSignalArray, board);

            // Activate the network
            Brain.Activate();

            // Find the highest-scoring available move
            Move move = null;
            double max = double.MinValue;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    // If the square is taken, skip it.
                    if (board[i, j] != SquareTypes.N)
                        continue;

                    // Set the score for this square.
                    double score = Brain.OutputSignalArray[i * 3 + j];

                    // If this is the first available move we've found, 
                    // set it to the current best.
                    if (move == null)
                    {
                        move = new Move(i, j);
                        max = score;
                    }
                    // If this square has a higher score than any we've
                    // found, set it to the current best.
                    else if (max < score)
                    {
                        move.X = i;
                        move.Y = j;
                        max = score;
                    }
                }

            return move;
        }

        // Loads the board into the input signal array.
        // This just flattens the 2d board into a 1d array.
        private void setInputSignalArray(ISignalArray inputArr, SquareTypes[,] board)
        {
            inputArr[0] = squareToInt(board[0, 0]);
            inputArr[1] = squareToInt(board[1, 0]);
            inputArr[2] = squareToInt(board[2, 0]);
            inputArr[3] = squareToInt(board[0, 1]);
            inputArr[4] = squareToInt(board[1, 1]);
            inputArr[5] = squareToInt(board[2, 1]);
            inputArr[6] = squareToInt(board[0, 2]);
            inputArr[7] = squareToInt(board[1, 2]);
            inputArr[8] = squareToInt(board[2, 2]);
        }

        // Converts the square type into an integer value.
        // We use 1 for our squares, -1 for opponent squares,
        // and 0 for empty squares.
        private int squareToInt(SquareTypes square)
        {
            if (square == SquareType)
                return 1;

            if (square == SquareTypes.N)
                return 0;

            return -1;
        }

        #endregion
    }
}
