using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using TicTacToeEvolution;
using SharpNeat.Phenomes;

namespace TicTacToeExhaustiveEvolution
{
    public class TicTacToeExhaustiveExperiment : SimpleNeatExperiment
    {
        /// <summary>
        /// Gets the Tic-Tac-Toe evaluator that scores individuals.
        /// </summary>
        public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator
        {
            get { return new TicTacToeExhaustiveEvaluator(); }
        }

        /// <summary>
        /// Defines the number of input nodes in the neural network.
        /// The network has one input for each square on the board,
        /// so it has 9 inputs total.
        /// </summary>
        public override int InputCount
        {
            get { return 9; }
        }

        /// <summary>
        /// Defines the number of output nodes in the neural network.
        /// The network has one output for each square on the board,
        /// so it has 9 outputs total.
        /// </summary>
        public override int OutputCount
        {
            get { return 9; }
        }

        /// <summary>
        /// Defines whether all networks should be evaluated every
        /// generation, or only new (child) networks. For Tic-Tac-Toe,
        /// we're evolving against a random player, so it's a good
        /// idea to evaluate individuals again every generation,
        /// to make sure it wasn't just luck.
        /// </summary>
        public override bool EvaluateParents
        {
            get { return true; }
        }
    }
}
