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
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Decoders;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Phenomes;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.SpeciationStrategies;
using System.Xml;
using TicTacToeCoevolution;
using TicTacToeLib;


namespace TicTacToeCoevolutionMultiPop
{
    public class TicTacToeHostParasiteExperiment
    {
        NeatEvolutionAlgorithmParameters _eaParams;
        NeatGenomeParameters _neatGenomeParams;
        string _name;
        int _populationSize;
        int _specieCount;
        NetworkActivationScheme _activationScheme;
        string _complexityRegulationStr;
        int? _complexityThreshold;
        string _description;
        ParallelOptions _parallelOptions;
        int _parasiteCount;
        int _championCount;

        #region INeatExperiment Members
        public int InputCount { get { return 9; } }
        public int OutputCount { get { return 9; } }

        public string Description
        {
            get { return _description; }
        }

        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the default population size to use for the experiment.
        /// </summary>
        public int DefaultPopulationSize
        {
            get { return _populationSize; }
        }

        /// <summary>
        /// Gets the NeatEvolutionAlgorithmParameters to be used for the experiment. Parameters on this object can be 
        /// modified. Calls to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in 
        /// at the time of the call.
        /// </summary>
        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
        {
            get { return _eaParams; }
        }

        /// <summary>
        /// Gets the NeatGenomeParameters to be used for the experiment. Parameters on this object can be modified. Calls
        /// to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in at the time of the call.
        /// </summary>
        public NeatGenomeParameters NeatGenomeParameters
        {
            get { return _neatGenomeParams; }
        }

        /// <summary>
        /// Initialize the experiment with some optional XML configutation data.
        /// </summary>
        public void Initialize(string name, XmlElement xmlConfig)
        {
            _name = name;
            _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
            _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
            _parasiteCount = XmlUtils.GetValueAsInt(xmlConfig, "ParasiteCount");
            _championCount = XmlUtils.GetValueAsInt(xmlConfig, "ChampionCount");
            _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
            _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
            _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
            _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");
            _parallelOptions = ExperimentUtils.ReadParallelOptions(xmlConfig);

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
        }

        /// <summary>
        /// Load a population of genomes from an XmlReader and returns the genomes in a new list.
        /// The genome2 factory for the genomes can be obtained from any one of the genomes.
        /// </summary>
        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            return NeatGenomeUtils.LoadPopulation(xr, false, this.InputCount, this.OutputCount);
        }

        /// <summary>
        /// Save a population of genomes to an XmlWriter.
        /// </summary>
        public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
        {
            // Writing node IDs is not necessary for NEAT.
            NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
        }

        /// <summary>
        /// Create a genome factory for the experiment.
        /// Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
        /// </summary>
        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new NeatGenomeFactory(InputCount, OutputCount, _neatGenomeParams);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload requires no parameters and uses the default population size.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome>[] CreateEvolutionAlgorithms()
        {
            return CreateEvolutionAlgorithms(DefaultPopulationSize);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a population size parameter that specifies how many genomes to create in an initial randomly
        /// generated population.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome>[] CreateEvolutionAlgorithms(int populationSize)
        {
            // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory1 = CreateGenomeFactory();

            // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory2 = CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            List<NeatGenome> genomeList1 = genomeFactory1.CreateGenomeList(populationSize, 0);
            List<NeatGenome> genomeList2 = genomeFactory2.CreateGenomeList(populationSize, 0);

            // Create evolution algorithm.
            return CreateEvolutionAlgorithms(genomeFactory1, genomeList1, genomeFactory2, genomeList2);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a pre-built genome2 population and their associated/parent genome2 factory.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome>[] CreateEvolutionAlgorithms(IGenomeFactory<NeatGenome> genomeFactory1, List<NeatGenome> genomeList1, 
                                                                              IGenomeFactory<NeatGenome> genomeFactory2, List<NeatGenome> genomeList2)
        {
            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric1 = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy1 = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric1, _parallelOptions);

            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric2 = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy2 = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric2, _parallelOptions);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy1 = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy2 = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

            // Create the evolution algorithm.
            NeatEvolutionAlgorithm<NeatGenome> ea1 = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy1, complexityRegulationStrategy1);
            NeatEvolutionAlgorithm<NeatGenome> ea2 = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy2, complexityRegulationStrategy2);

            // Create genome decoder.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder1 = CreateGenomeDecoder();
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder2 = CreateGenomeDecoder();

            // Create phenome evaluators. Note we are evolving one population of X players and one of O players.
            ICoevolutionPhenomeEvaluator<IBlackBox> phenomeEvaluator1 = new TicTacToeHostParasiteEvaluator(SquareTypes.X);
            ICoevolutionPhenomeEvaluator<IBlackBox> phenomeEvaluator2 = new TicTacToeHostParasiteEvaluator(SquareTypes.O);

            // Create a genome list evaluator. This packages up the genome decoder with the phenome evaluator.
            HostParasiteCoevolutionListEvaluator<NeatGenome, IBlackBox> genomeListEvaluator1 = new HostParasiteCoevolutionListEvaluator<NeatGenome, IBlackBox>(_parasiteCount, _championCount, ea2, genomeDecoder1, phenomeEvaluator1);
            HostParasiteCoevolutionListEvaluator<NeatGenome, IBlackBox> genomeListEvaluator2 = new HostParasiteCoevolutionListEvaluator<NeatGenome, IBlackBox>(_parasiteCount, _championCount, ea1, genomeDecoder2, phenomeEvaluator2);

            // Initialize the evolution algorithms.
            ea1.Initialize(genomeListEvaluator1, genomeFactory1, genomeList1);
            ea2.Initialize(genomeListEvaluator2, genomeFactory2, genomeList2);

            // Set the evolution algorithms to update every generation.
            ea1.UpdateScheme = new UpdateScheme(1);
            ea2.UpdateScheme = new UpdateScheme(1);

            // Finished. Return the evolution algorithms
            return new []{ ea1, ea2 };
        }

        /// <summary>
        /// Creates a new genome decoder that can be used to convert a genome into a phenome.
        /// </summary>
        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return new NeatGenomeDecoder(_activationScheme);
        }
        #endregion
    }
}
