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

namespace TicTacToeCoevolution
{
    /// <summary>
    /// Represents an evaluator that competes two phenomes against each other.
    /// </summary>
    public interface ICoevolutionPhenomeEvaluator<TPhenome>
    {
        /// <summary>
        /// Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        ulong EvaluationCount { get; }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        bool StopConditionSatisfied { get; }

        /// <summary>
        /// Evaluate the provided phenomes and return their fitness scores.
        /// </summary>
        void Evaluate(TPhenome phenome1, TPhenome phenome2, 
                      out FitnessInfo fitness1, out FitnessInfo fitness2);

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        void Reset();
    }
}
