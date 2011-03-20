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
using System.Diagnostics;

namespace TicTacToeLib
{
    /// <summary>
    /// A simple game of Tic-Tac-Toe.
    /// </summary>
    public class TicTacToeGame
    {
        public SquareTypes[,] Board { get; set; }

        public TicTacToeGame()
        {
            Board = new SquareTypes[3, 3];
        }

        /// <summary>
        /// Resets the current game by clearing the game board.
        /// </summary>
        public void ResetGame()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Board[i, j] = SquareTypes.N;
        }

        /// <summary>
        /// Checks if the square located at [x,y] on the game board is empty.
        /// </summary>
        public bool IsEmpty(int x, int y)
        {
            Debug.Assert(x < 3 && x >= 0);
            Debug.Assert(y < 3 && y >= 0);
            return Board[x, y] == SquareTypes.N;
        }

        /// <summary>
        /// Gets the winner of the current game.
        /// </summary>
        public SquareTypes GetWinner()
        {
            return GetWinner(Board);
        }

        /// <summary>
        /// A helper method for competing two players against each other.
        /// </summary>
        /// <param name="xPlayer">The player to act first.</param>
        /// <param name="oPlayer">The player to act second.</param>
        /// <returns>
        /// The square type of the winner, or SquareTypes.N if the game
        /// was a draw.
        /// </returns>
        public static SquareTypes PlayGameToEnd(IPlayer xPlayer, IPlayer oPlayer)
        {
            TicTacToeGame game = new TicTacToeGame();
            SquareTypes winner = SquareTypes.N;

            // Play until we have a winner or the game board is full.
            for (int moveNum = 0; moveNum < 9 && winner == SquareTypes.N; moveNum++)
            {
                IPlayer curPlayer;
                SquareTypes curSquareType;

                // Determine if it's X's or O's turn to act.
                if (moveNum % 2 == 0)
                {
                    curPlayer = xPlayer;
                    curSquareType = SquareTypes.X;
                }
                else
                {
                    curPlayer = oPlayer;
                    curSquareType = SquareTypes.O;
                }
                
                // Get the next move from the current player.
                var move = curPlayer.GetMove(game.Board);

                // Check to make sure the player's move is legal.
                Debug.Assert(game.IsEmpty(move.X, move.Y), "Player tried to make an illegal move!");

                // Set the board to the player's move.
                game.Board[move.X, move.Y] = curSquareType;

                // Start checking if we have a winner once xPlayer has made
                // at least 3 moves.
                if(moveNum > 3)
                    winner = game.GetWinner();
            }

            // Return the square type of the winner, or SquareTypes.N if it was a draw.
            return winner;
        }

        /// <summary>
        /// Gets the winner based on the specified board.
        /// </summary>
        /// <param name="Board"></param>
        /// <returns>
        /// The SquareType of the winner, or SquareTypes.N 
        /// if no one has won yet or there's a draw.
        /// </returns>
        public static SquareTypes GetWinner(SquareTypes[,] Board)
        {
            if (Board[0, 0] != SquareTypes.N)
            {
                var type = Board[0, 0];
                //top left to bottom left
                if (type == Board[0, 1] && type == Board[0, 2])
                    return type;

                //top left to top right
                if (type == Board[1, 0] && type == Board[2, 0])
                    return type;

                //top left to bottom right
                if (type == Board[1, 1] && type == Board[2, 2])
                    return type;
            }

            if (Board[0, 2] != SquareTypes.N)
            {
                var type = Board[0, 2];

                //bottom left to top right
                if (type == Board[1, 1] && type == Board[2, 0])
                    return type;

                //bottom left to bottom right
                if (type == Board[1, 2] && type == Board[2, 2])
                    return type;
            }

            if (Board[0, 1] != SquareTypes.N)
            {
                var type = Board[0, 1];

                //middle left to middle right
                if (type == Board[1, 1] && type == Board[2, 1])
                    return type;
            }

            if (Board[1, 0] != SquareTypes.N)
            {
                var type = Board[1, 0];

                //middle top to middle bottom
                if (type == Board[1, 1] && type == Board[1, 2])
                    return type;
            }

            if (Board[2, 0] != SquareTypes.N)
            {
                var type = Board[2, 0];

                //top right to bottom right
                if (type == Board[2, 1] && type == Board[2, 2])
                    return type;
            }

            return SquareTypes.N;
        }

        /// <summary>
        /// Gets a random game board played for a specified
        /// number of moves.
        /// </summary>
        public static SquareTypes[,] GetRandomBoard(int moves)
        {
            Debug.Assert(moves < 10);

            SquareTypes[,] board = new SquareTypes[3, 3];

            Random random = new Random();
            for (int i = 0; i < moves; i++)
            {
                int next = random.Next(9);
                while (board[next / 3, next % 3] != SquareTypes.N)
                    next = random.Next(9);

                board[next / 3, next % 3] = i % 2 == 0 ? SquareTypes.X : SquareTypes.O;
            }

            return board;
        }

        /// <summary>
        /// Takes a string description of a game board and returns the board array.
        /// A valid board example is:
        /// X|O|X
        ///  |X| 
        /// O|X|O
        /// 
        /// Note that there should be a space in blank squares.
        /// </summary>
        public static SquareTypes[,] GetBoardFromString(string boardString)
        {
            var lineTokens = boardString.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Debug.Assert(lineTokens.Length == 3, "Invalid board string:\n" + boardString + "\n");

            var tokens = new string[9];
            
            var line = lineTokens[0].Trim().Split('|');
            Debug.Assert(line.Length == 3);
            line.CopyTo(tokens, 0);
            
            line = lineTokens[1].Trim().Split('|');
            Debug.Assert(line.Length == 3);
            line.CopyTo(tokens, 3);

            line = lineTokens[2].Trim().Split('|');
            Debug.Assert(line.Length == 3);
            line.CopyTo(tokens, 6);
            
            Debug.Assert(tokens.Length == 9, "Invalid board string:\n" + boardString + "\n");

            SquareTypes[,] board = new SquareTypes[3, 3];

            board[0, 0] = getSquareType(tokens[0]);
            board[1, 0] = getSquareType(tokens[1]);
            board[2, 0] = getSquareType(tokens[2]);
            board[0, 1] = getSquareType(tokens[3]);
            board[1, 1] = getSquareType(tokens[4]);
            board[2, 1] = getSquareType(tokens[5]);
            board[0, 2] = getSquareType(tokens[6]);
            board[1, 2] = getSquareType(tokens[7]);
            board[2, 2] = getSquareType(tokens[8]);
            return board;
        }

        // Converts a string into a SquareType.
        private static SquareTypes getSquareType(string squareString)
        {
            switch(squareString.Trim())
            {
                case "X" : return SquareTypes.X;
                case "O" : return SquareTypes.O;
                case "N":
                case "": return SquareTypes.N;
                default: throw new Exception("Unknown square string: " + squareString);
            }
        }
    }
}
