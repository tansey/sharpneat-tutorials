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
using SharpNeat.Phenomes;
using TicTacToeLib;
using SharpNeat.Core;
using TicTacToeEvolution;

namespace TicTacToeCoevolution
{
    public class TicTacToeCoevolutionEvaluator : ICoevolutionPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return false; }
        }

        /// <summary>
        /// Evaluate the two black boxes by playing them against each other in a
        /// game of Tic-Tac-Toe. All permutations of size 2 are going to be
        /// evaluated, so we only play one game: box1 is X, box2 is O.
        /// </summary>
        public void Evaluate(IBlackBox box1, IBlackBox box2,
                            out FitnessInfo fitness1, out FitnessInfo fitness2)
        {
            // box1 plays as X.
            NeatPlayer player1 = new NeatPlayer(box1, SquareTypes.X);

            // box2 plays as O.
            NeatPlayer player2 = new NeatPlayer(box2, SquareTypes.O);

            // Play the two boxes against each other.
            var winner = TicTacToeGame.PlayGameToEnd(player1, player2);

            // Score box1
            double score1 = getScore(winner, SquareTypes.X);
            fitness1 = new FitnessInfo(score1, score1);

            // Score box2
            double score2 = getScore(winner, SquareTypes.O);
            fitness2 = new FitnessInfo(score2, score2);

            // Update the evaluation counter.
            _evalCount++;
        }

        /// <summary>
        /// Returns the score for a game. Scoring is 10 for a win, 1 for a draw
        /// and 0 for a loss. Note that scores cannot be smaller than 0 because
        /// NEAT requires the fitness score to be positive.
        /// </summary>
        private double getScore(SquareTypes winner, SquareTypes neatSquareType)
        {
            if (winner == neatSquareType)
                return neatSquareType == SquareTypes.X ? 2 : 10;
            if (winner == SquareTypes.N)
                return neatSquareType == SquareTypes.X ? 1 : 2;
            return 0;
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// Note. The TicTacToe problem domain has no internal state. This method does nothing.
        /// </summary>
        public void Reset()
        {
        }
    }
}
