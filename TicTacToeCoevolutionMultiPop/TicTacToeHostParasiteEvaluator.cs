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
using SharpNeat.Core;
using SharpNeat.Phenomes;
using TicTacToeEvolution;
using TicTacToeLib;
using TicTacToeCoevolution;

namespace TicTacToeCoevolutionMultiPop
{
    public class TicTacToeHostParasiteEvaluator : ICoevolutionPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;
        private SquareTypes _playerOneSquareType;

        public TicTacToeHostParasiteEvaluator(SquareTypes playerOneSquareType)
        {
            _playerOneSquareType = playerOneSquareType;
        }


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
        /// Evaluate the two black boxes by playing them against each other in 2
        /// games of Tic-Tac-Toe. Each player plays each side once.
        /// </summary>
        public void Evaluate(IBlackBox box1, IBlackBox box2,
                            out FitnessInfo fitness1, out FitnessInfo fitness2)
        {
            double score1, score2;

            if (_playerOneSquareType == SquareTypes.X)
                // Evaluate with box1 as X.
                evaluate(box1, box2, out score1, out score2);
            else
                // Evaluate with box1 as O.
                evaluate(box2, box1, out score2, out score1);

            // Set the final fitness values
            fitness1 = new FitnessInfo(score1, score1);
            fitness2 = new FitnessInfo(score2, score2);
            
            // Update the evaluation counter.
            _evalCount++;
        }

        private void evaluate(IBlackBox box1, IBlackBox box2,
                            out double score1, out double score2)
        {
            // box1 plays as X.
            NeatPlayer player1 = new NeatPlayer(box1, SquareTypes.X);

            // box2 plays as O.
            NeatPlayer player2 = new NeatPlayer(box2, SquareTypes.O);

            // Play the two boxes against each other.
            var winner = TicTacToeGame.PlayGameToEnd(player1, player2);

            // Score box1
            score1 = getScore(winner, SquareTypes.X);

            // Score box2
            score2 = getScore(winner, SquareTypes.O);
        }

        /// <summary>
        /// Returns the score for a game. Scoring is 10 for a win, 1 for a draw
        /// and 0 for a loss. Note that scores cannot be smaller than 0 because
        /// NEAT requires the fitness score to be positive.
        /// </summary>
        private double getScore(SquareTypes winner, SquareTypes neatSquareType)
        {
            if (winner == neatSquareType)
                return 2;
            if (winner == SquareTypes.N)
                return 1;
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
