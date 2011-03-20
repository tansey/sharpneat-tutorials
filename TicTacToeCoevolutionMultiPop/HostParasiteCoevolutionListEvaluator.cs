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
using TicTacToeCoevolution;
using SharpNeat.Core;
using System.Threading.Tasks;
using System.Diagnostics;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Utility;
using System.Threading;

namespace TicTacToeCoevolutionMultiPop
{
    /// <summary>
    /// Implements host-parasite coevolution evaluation. Assumes there is another
    /// NEAT evolutionary algorithm running in parallel, evolving "parasites"
    /// simulataneously. 
    /// 
    /// For more information, see the following technical report:
    /// Stanley, K.O. and Miikkulainen, R. "Competitive Coevolution through 
    /// Evolutionary Complexification", Technical Report AI2002-298, 
    /// Department of Computer Sciences, The University of Texas at Austin, 2004.
    /// </summary>
    public class HostParasiteCoevolutionListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, global::SharpNeat.Core.IGenome<TGenome>
        where TPhenome : class
    {
        readonly int _parasiteGenomesPerEvaluation;
        readonly int _hallOfFameGenomesPerEvaluation;
        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        readonly ICoevolutionPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        readonly ParallelOptions _parallelOptions;
        readonly FastRandom _random;

        List<TGenome> _hallOfFame;
        List<TGenome> _parasiteGenomes;

        #region Constructors
        /// <summary>
        /// Construct with the provided IGenomeDecoder and ICoevolutionPhenomeEvaluator. 
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        public HostParasiteCoevolutionListEvaluator(int parasiteGenomesPerEvaluation,
                                           int hallOfFameGenomesPerEvaluation,
                                           IEvolutionAlgorithm<TGenome> eaParasite,
                                           IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
                                           ICoevolutionPhenomeEvaluator<TPhenome> phenomeEvaluator)
            : this(parasiteGenomesPerEvaluation,hallOfFameGenomesPerEvaluation, eaParasite,
                    genomeDecoder, phenomeEvaluator, new ParallelOptions())
        { 
        }

        /// <summary>
        /// Construct with the provided IGenomeDecoder, ICoevolutionPhenomeEvaluator and ParalleOptions.
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        public HostParasiteCoevolutionListEvaluator(int parasiteGenomesPerEvaluation,
                                           int hallOfFameGenomesPerEvaluation,
                                           IEvolutionAlgorithm<TGenome> eaParasite,
                                           IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
                                           ICoevolutionPhenomeEvaluator<TPhenome> phenomeEvaluator,
                                           ParallelOptions options)
        {
            Debug.Assert(parasiteGenomesPerEvaluation >= 0);
            Debug.Assert(hallOfFameGenomesPerEvaluation >= 0);

            _parasiteGenomesPerEvaluation = parasiteGenomesPerEvaluation;
            _hallOfFameGenomesPerEvaluation = hallOfFameGenomesPerEvaluation;
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _parallelOptions = options;

            _hallOfFame = new List<TGenome>();
            _parasiteGenomes = new List<TGenome>();
            _random = new FastRandom();

            eaParasite.UpdateEvent += new EventHandler(eaParasite_UpdateEvent);
        }
        #endregion

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
            _parasiteGenomes.Clear();
            _hallOfFame.Clear();
            _phenomeEvaluator.Reset();
        }

        /// <summary>
        /// Main genome evaluation loop with no phenome caching (decode on each evaluation).
        /// Individuals are competed pairwise against every parasite in the parasite list
        /// and against a randomly selected subset of the hall of fame.
        /// </summary>
        public void Evaluate(IList<TGenome> hostGenomeList)
        {
            //Create a temporary list of fitness values with the scores of the inner evaluator.
            FitnessInfo[] results = new FitnessInfo[hostGenomeList.Count];
            for (int i = 0; i < results.Length; i++)
                results[i] = FitnessInfo.Zero;

            // Randomly select champions from the hall of fame.
            TGenome[] champions = _hallOfFame.Count > 0 
                                    ? new TGenome[_hallOfFameGenomesPerEvaluation]
                                    : new TGenome[0];
            for (int i = 0; i < champions.Length; i++)
            {
                // Pick a random champion's index
                int hallOfFameIdx = _random.Next(_hallOfFame.Count);
                
                // Add the champion to the list of competitors.
                champions[i] = _hallOfFame[hallOfFameIdx];
            }

            // Acquire a lock on the parasite genome list.
            Monitor.Enter(_parasiteGenomes);

            // Exhaustively compete individuals against each other.
            Parallel.For(0, hostGenomeList.Count, delegate(int i)
            {
                // Decode the host genome.
                TPhenome host = _genomeDecoder.Decode(hostGenomeList[i]);

                // Check that the host genome is valid.
                if (host == null)
                    return;

                // Evaluate the host against the parasites.
                for (int j = 0; j < _parasiteGenomes.Count; j++)
                {
                    // Decode the champion genome.
                    TPhenome parasite = _genomeDecoder.Decode(_parasiteGenomes[j]);

                    // Check that the champion genome is valid.
                    if (parasite == null)
                        continue;

                    // Compete the two individuals against each other and get the results.
                    FitnessInfo hostFitness, parasiteFitness;
                    _phenomeEvaluator.Evaluate(host, parasite, out hostFitness, out parasiteFitness);

                    // Add the results to each genome's overall fitness.
                    results[i]._fitness += hostFitness._fitness;
                    results[i]._alternativeFitness += hostFitness._alternativeFitness;
                }

                // Evaluate the host against the champions.
                for (int j = 0; j < champions.Length; j++)
                {
                    // Decode the champion genome.
                    TPhenome champion = _genomeDecoder.Decode(champions[j]);

                    // Check that the champion genome is valid.
                    if (champion == null)
                        continue;

                    // Compete the two individuals against each other and get the results.
                    FitnessInfo hostFitness, championFitness;
                    _phenomeEvaluator.Evaluate(host, champion, out hostFitness, out championFitness);

                    // Add the results to each genome's overall fitness.
                    results[i]._fitness += hostFitness._fitness;
                    results[i]._alternativeFitness += hostFitness._alternativeFitness;
                }
            });

            // Release the lock on the parasite genome list.
            Monitor.Exit(_parasiteGenomes);

            // Update every genome in the population with its new fitness score.
            TGenome champ = hostGenomeList[0];
            for (int i = 0; i < results.Length; i++)
            {
                TGenome hostGenome = hostGenomeList[i];
                hostGenome.EvaluationInfo.SetFitness(results[i]._fitness);
                hostGenome.EvaluationInfo.AlternativeFitness = results[i]._alternativeFitness;

                // Check if this genome is the generational champion
                if (hostGenome.EvaluationInfo.Fitness > champ.EvaluationInfo.Fitness)
                    champ = hostGenome;
            }

            // Add the new champion to the hall of fame.
            _hallOfFame.Add(champ);
        }
        
        /// <summary>
        /// Updates the champion genomes every generation.
        /// </summary>
        private void eaParasite_UpdateEvent(object sender, EventArgs args)
        {
            // Make sure that the event sender is a NEAT EA.
            Debug.Assert(sender is NeatEvolutionAlgorithm<TGenome>);

            // Cast the EA so we can access the current champion.
            NeatEvolutionAlgorithm<TGenome> ea = (NeatEvolutionAlgorithm<TGenome>)sender;

            // Lock the list to prevent issues with the Evaluate method.

            // Acquire a lock on the parasite genome list.
            Monitor.Enter(_parasiteGenomes);

            // Clear the competitors list since we're dealing with a new population.
            _parasiteGenomes.Clear();

            // Add the best genome from every specie to the list
            foreach (var specie in ea.SpecieList)
            {
                // Find the best genome in the specie
                TGenome genome = findMax(specie.GenomeList);

                // Add it to the list
                _parasiteGenomes.Add(genome);
            }

            // Sort the list in descending order based on fitness
            _parasiteGenomes.Sort((g1, g2) =>
                        g2.EvaluationInfo.Fitness.CompareTo(g1.EvaluationInfo.Fitness));
            
            // Keep only the top genomes
            _parasiteGenomes.RemoveRange(_parasiteGenomesPerEvaluation,
                                        _parasiteGenomes.Count - _parasiteGenomesPerEvaluation);

            // Release the lock on the parasite genome list.
            Monitor.Exit(_parasiteGenomes);
        }

        /// <summary>
        /// Returns the genome in the list with the highest fitness.
        /// </summary>
        private TGenome findMax(List<TGenome> genomeList)
        {
            Debug.Assert(genomeList.Count > 0);
            TGenome max = genomeList[0];
            for (int i = 1; i < genomeList.Count; i++)
                if (genomeList[i].EvaluationInfo.Fitness > max.EvaluationInfo.Fitness)
                    max = genomeList[i];
            return max;
        }
    }
}
