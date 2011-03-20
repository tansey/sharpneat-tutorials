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

namespace TicTacToeLib
{
    /// <summary>
    /// A perfect Tic-Tac-Toe player.
    /// </summary>
    public class OptimalPlayer : IPlayer
    {
        public SquareTypes SquareType { get; set; }

        public OptimalPlayer(SquareTypes type)
        {
            SquareType = type;
        }

        public Move GetMove(SquareTypes[,] board)
        {
            TicTacToeGame.GetWinner(board);
            int moveNum = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i, j] != SquareTypes.N)
                        moveNum++;

            //first move is always a corner
            if (moveNum == 0)
                return new Move(0, 0);

            //second move should be the center if free, else a corner
            if (moveNum == 1)
            {
                if (board[1, 1] == SquareTypes.N)
                    return new Move(1, 1);

                return new Move(0, 0);
            }

            //make a winning move if possible
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != SquareTypes.N)
                        continue;

                    board[i, j] = SquareType;
                    var winner = TicTacToeGame.GetWinner(board);
                    board[i, j] = SquareTypes.N;
                    if (winner == SquareType)
                        return new Move(i, j);
                }

            //if we can't win, check if there are any moves that we have to make
            //to prevent ourselves from losing
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != SquareTypes.N)
                        continue;

                    //set the move to the opponent's type
                    board[i, j] = SquareType == SquareTypes.X ? SquareTypes.O : SquareTypes.X;
                    var winner = TicTacToeGame.GetWinner(board);
                    board[i, j] = SquareTypes.N;

                    //if the opponent will win by moving here, move here to block them
                    if (winner != SquareTypes.N)
                        return new Move(i, j);
                }

            //if we're here, that means we have made at least 1 move already and can't win
            //nor lose in 1 move, so just make the optimal play which would be to a free
            //corner that isn't blocked
            Move move = null;
            int max = -1;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != SquareTypes.N)
                        continue;

                    board[i, j] = SquareType;
                    int count = 0;
                    for (int m = 0; m < 3; m++)
                        for (int n = 0; n < 3; n++)
                        {
                            if (board[m, n] != SquareTypes.N)
                                continue;

                            board[m, n] = SquareType;
                            var winner = TicTacToeGame.GetWinner(board);
                            board[m, n] = SquareTypes.N;
                            if (winner == SquareType)
                                count++;
                        }
                    board[i, j] = SquareTypes.N;
                    if (count > max)
                    {
                        move = new Move(i, j);
                        max = count;
                    }
                }

            return move;
        }
    }
}
