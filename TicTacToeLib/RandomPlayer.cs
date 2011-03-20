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
    /// A simple Tic-Tac-Toe player that chooses a square randomly.
    /// </summary>
    public class RandomPlayer : IPlayer
    {
        private Random random = new Random();

        public Move GetMove(SquareTypes[,] board)
        {
            int x = random.Next(3);
            int y = random.Next(3);
            while (board[x, y] != SquareTypes.N)
            {
                x = random.Next(3);
                y = random.Next(3);
            }

            return new Move(x, y);
        }
    }
}
