/* ***************************************************************************
 * This file is part of the NashCoding tutorial on SharpNEAT 2.
 * 
 * Copyright 2010, Wesley Tansey (wes@nashcoding.com)
 * 
 * Some code in this file may have been copied directly from SharpNEAT,
 * for learning purposes only. Any code copied from SharpNEAT 2 is 
 * copyright of Colin Green (sharpneat@gmail.com).
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
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using System.Diagnostics;

namespace TicTacToeCoevolution
{
    /// <summary>
    /// Represents a Hall of Fame evaluator.
    /// 
    /// Champion genomes are periodically stored to create a Hall of Fame. All successive
    /// generations are required to evaluate against both their own population and
    /// the hall of fame. A genome's overall fitness is a weighted sum of its
    /// fitness against the population and the hall of fame.
    /// </summary>
    public class ParallelHallOfFameListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, global::SharpNeat.Core.IGenome<TGenome>
        where TPhenome : class
    {
        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        readonly ICoevolutionPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        readonly ParallelOptions _parallelOptions;
        readonly uint _generationsPerChampion;
        readonly IGenomeListEvaluator<TGenome> _innerEvaluator;
        readonly double _hallOfFameWeight;

        uint _lastUpdate;
        List<TGenome> _hallOfFame;

        #region Constructors

        /// <summary>
        /// Construct with the provided number of generations per champion, weight to the hall
        /// of fame fitness, and other parameters. 
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        public ParallelHallOfFameListEvaluator(uint generationsPerChampion,
                                           double hallOfFameWeight,
                                           AbstractGenerationalAlgorithm<TGenome> ea,
                                           IGenomeListEvaluator<TGenome> innerEvaluator,
                                           IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
                                           ICoevolutionPhenomeEvaluator<TPhenome> phenomeEvaluator)
            : this(generationsPerChampion, hallOfFameWeight, ea, innerEvaluator, genomeDecoder, phenomeEvaluator, new ParallelOptions())
        { 
        }

        /// <summary>
        /// Construct with the provided number of generations per champion, weight to the hall
        /// of fame fitness, parallel options, and other parameters. 
        /// </summary>
        public ParallelHallOfFameListEvaluator(uint generationsPerChampion,
                                           double hallOfFameWeight,
                                           AbstractGenerationalAlgorithm<TGenome> ea,
                                           IGenomeListEvaluator<TGenome> innerEvaluator,
                                           IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
                                           ICoevolutionPhenomeEvaluator<TPhenome> phenomeEvaluator,
                                           ParallelOptions options)
        {
            Debug.Assert(hallOfFameWeight >= 0d);
            Debug.Assert(hallOfFameWeight <= 1d);

            _generationsPerChampion = generationsPerChampion;
            _hallOfFameWeight = hallOfFameWeight;
            _innerEvaluator = innerEvaluator;
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _parallelOptions = options;

            _hallOfFame = new List<TGenome>();
            ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
        }
        #endregion

        #region IGenomeListEvaluator<TGenome> Members

        /// <summary>
        /// Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _innerEvaluator.EvaluationCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _innerEvaluator.StopConditionSatisfied; }
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
            _hallOfFame.Clear();
            _lastUpdate = 0;
            _phenomeEvaluator.Reset();
        }
        
        /// <summary>
        /// Main genome evaluation loop with no phenome caching (decode on each evaluation).
        /// Individuals are competed pairwise against every champion in the hall of fame.
        /// The final fitness score is the weighted sum of the fitness versus the champions
        /// and the fitness score by the inner evaluator.
        /// </summary>
        public void Evaluate(IList<TGenome> genomeList)
        {
            _innerEvaluator.Evaluate(genomeList);

            //Create a temporary list of fitness values with the scores of the inner evaluator.
            FitnessInfo[] results = new FitnessInfo[genomeList.Count];
            for (int i = 0; i < results.Length; i++)
                results[i] = new FitnessInfo(genomeList[i].EvaluationInfo.Fitness * (1.0 - _hallOfFameWeight),
                                             genomeList[i].EvaluationInfo.AlternativeFitness * (1.0 - _hallOfFameWeight));

            // Calculate how much each champion game is worth
            double championGameWeight = _hallOfFameWeight / (double)_hallOfFame.Count;

            // Exhaustively compete individuals against each other.
            Parallel.For(0, genomeList.Count, delegate(int i)
            {
                // Decode the first genome.
                TPhenome phenome1 = _genomeDecoder.Decode(genomeList[i]);

                // Check that the first genome is valid.
                if (phenome1 == null)
                    return;

                for (int j = 0; j < _hallOfFame.Count; j++)
                {
                    // Decode the second genome.
                    TPhenome phenome2 = _genomeDecoder.Decode(_hallOfFame[j]);

                    // Check that the second genome is valid.
                    if (phenome2 == null)
                        continue;

                    // Compete the two individuals against each other and get the results.
                    FitnessInfo fitness1, fitness2;
                    _phenomeEvaluator.Evaluate(phenome1, phenome2, out fitness1, out fitness2);

                    // Add the results to each genome's overall fitness.
                    // Note that we need to use a lock here because
                    // the += operation is not atomic.
                    // ENHANCEMENT: I don't think this lock is necessary here since the hall of fame
                    //              is our inner loop.
                    lock (results)
                    {
                        results[i]._fitness += fitness1._fitness * championGameWeight;
                        results[i]._alternativeFitness += fitness1._alternativeFitness * championGameWeight;
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

        private void ea_UpdateEvent(object sender, EventArgs e)
        {
            // Make sure that the event sender is an EA.
            Debug.Assert(sender is AbstractGenerationalAlgorithm<TGenome>);

            // Cast the EA so we can access the current champion.
            AbstractGenerationalAlgorithm<TGenome> ea = (AbstractGenerationalAlgorithm<TGenome>)sender;

            // Update every few generations.
            if (ea.CurrentGeneration < (_generationsPerChampion + _lastUpdate))
                return;

            // Update the update counter.
            _lastUpdate = ea.CurrentGeneration;

            // Add the genome to the hall of fame.
            _hallOfFame.Add(ea.CurrentChampGenome);

            Console.WriteLine("Hall of Fame Updated. Size: {0}", _hallOfFame.Count);
        }
    }
}
