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
using System.Threading.Tasks;

namespace TicTacToeCoevolution
{
    /// <summary>
    /// A concrete implementation of IGenomeListEvaluator that evaulates genomes 
    /// in parallel (on multiple execution threads). Genomes are pitted against
    /// each other in a head-to-head matchup.
    /// 
    /// Genome decoding is performed by a provided IGenomeDecoder.
    /// Phenome evaluation is performed by a provided IPhenomeEvaluator.
    /// </summary>
    public class ParallelCoevolutionListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, global::SharpNeat.Core.IGenome<TGenome>
        where TPhenome : class
    {
        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        readonly ICoevolutionPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        readonly ParallelOptions _parallelOptions;
        
        #region Constructors

        /// <summary>
        /// Construct with the provided IGenomeDecoder and ICoevolutionPhenomeEvaluator. 
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        public ParallelCoevolutionListEvaluator(IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
                                           ICoevolutionPhenomeEvaluator<TPhenome> phenomeEvaluator)
            : this(genomeDecoder, phenomeEvaluator, new ParallelOptions())
        { 
        }

        /// <summary>
        /// Construct with the provided IGenomeDecoder, ICoevolutionPhenomeEvaluator and ParalleOptions.
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        public ParallelCoevolutionListEvaluator(IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
                                           ICoevolutionPhenomeEvaluator<TPhenome> phenomeEvaluator,
                                           ParallelOptions options)
        { 
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _parallelOptions = options;
        }
        #endregion

        #region IGenomeListEvaluator<TGenome> Members

        /// <summary>
        /// Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _phenomeEvaluator.EvaluationCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }

        /// <summary>
        /// Main genome evaluation loop with no phenome caching (decode on each evaluation).
        /// Individuals are competed pairwise against every other individual in the population.
        /// Evaluations are summed to get the final genome fitness.
        /// </summary>
        public void Evaluate(IList<TGenome> genomeList)
        {
            //Create a temporary list of fitness values
            FitnessInfo[] results = new FitnessInfo[genomeList.Count];
            for (int i = 0; i < results.Length; i++)
                results[i] = FitnessInfo.Zero;

            // Exhaustively compete individuals against each other.
            Parallel.For(0, genomeList.Count, delegate(int i)
            {
                for(int j = 0; j < genomeList.Count; j++)
                {
                    // Don't bother evaluating inviduals against themselves.
                    if (i == j)
                        continue;

                    // Decode the first genome.
                    TPhenome phenome1 = _genomeDecoder.Decode(genomeList[i]);

                    // Check that the first genome is valid.
                    if (phenome1 == null)
                        continue;

                    // Decode the second genome.
                    TPhenome phenome2 = _genomeDecoder.Decode(genomeList[j]);

                    // Check that the second genome is valid.
                    if (phenome2 == null)
                        continue;

                    // Compete the two individuals against each other and get
                    // the results.
                    FitnessInfo fitness1, fitness2;
                    _phenomeEvaluator.Evaluate(phenome1, phenome2, out fitness1, out fitness2);

                    // Add the results to each genome's overall fitness.
                    // Note that we need to use a lock here because
                    // the += operation is not atomic.
                    lock (results)
                    {
                        results[i]._fitness += fitness1._fitness;
                        results[i]._alternativeFitness += fitness1._alternativeFitness;
                        results[j]._fitness += fitness2._fitness;
                        results[j]._alternativeFitness += fitness2._alternativeFitness;
                    }
                }
            });

            // Update every genome in the population with its new fitness score.
            for (int i = 0; i < results.Length; i++)
            {
                genomeList[i].EvaluationInfo.SetFitness(results[i]._fitness);
                genomeList[i].EvaluationInfo.AlternativeFitness = results[i]._alternativeFitness;
            }
        }

        #endregion
    }
}
