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
using TicTacToeLib;

namespace TicTacToeEvolution
{
    /// <summary>
    /// Class used to evaluate neural networks that play Tic-Tac-Toe against
    /// random and optimal agents.
    /// </summary>
    public class TicTacToeEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;
        private bool _stopConditionSatisfied;

        #region IPhenomeEvaluator<IBlackBox> Members

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
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox against the random tic-tac-toe player and return its fitness score.
        /// Each network plays 10 games against the random player and two games against the expert player.
        /// Half of the games are played as circle and half are played as x.
        /// 
        /// A win is worth 10 points, a draw is worth 1 point, and a loss is worth 0 points.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            double fitness = 0;
            SquareTypes winner;
            OptimalPlayer optimalPlayer = new OptimalPlayer(SquareTypes.O);
            RandomPlayer randomPlayer = new RandomPlayer();
            NeatPlayer neatPlayer = new NeatPlayer(box, SquareTypes.X);
            
            
            // Play 50 games as X against a random player
            for (int i = 0; i < 50; i++)
            {
                // Compete the two players against each other.
                winner = TicTacToeGame.PlayGameToEnd(neatPlayer, randomPlayer);

                // Update the fitness score of the network
                fitness += getScore(winner, neatPlayer.SquareType);
            }

            // Play 50 games as O against a random player
            neatPlayer.SquareType = SquareTypes.O;
            for (int i = 0; i < 50; i++)
            {
                // Compete the two players against each other.
                winner = TicTacToeGame.PlayGameToEnd(randomPlayer, neatPlayer);

                // Update the fitness score of the network
                fitness += getScore(winner, neatPlayer.SquareType);
            }

            // Play 1 game as X against an optimal player
            neatPlayer.SquareType = SquareTypes.X;
            optimalPlayer.SquareType = SquareTypes.O;
            
            // Compete the two players against each other.
            winner = TicTacToeGame.PlayGameToEnd(neatPlayer, optimalPlayer);
            
            // Update the fitness score of the network
            fitness += getScore(winner, neatPlayer.SquareType);


            // Play 1 game as O against an optimal player
            neatPlayer.SquareType = SquareTypes.O;
            optimalPlayer.SquareType = SquareTypes.X;

            // Compete the two players against each other.
            winner = TicTacToeGame.PlayGameToEnd(optimalPlayer, neatPlayer);

            // Update the fitness score of the network
            fitness += getScore(winner, neatPlayer.SquareType);

            // Update the evaluation counter.
            _evalCount++;

            // If the network plays perfectly, it will beat the random player
            // and draw the optimal player.
            if (fitness >= 1002)
                _stopConditionSatisfied = true;

            // Return the fitness score
            return new FitnessInfo(fitness, fitness);
        }

        /// <summary>
        /// Returns the score for a game. Scoring is 10 for a win, 1 for a draw
        /// and 0 for a loss. Note that scores cannot be smaller than 0 because
        /// NEAT requires the fitness score to be positive.
        /// </summary>
        private int getScore(SquareTypes winner, SquareTypes neatSquareType)
        {
            if (winner == neatSquareType)
                return 10;
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
        #endregion
    }
}
