using System;
using System.Collections.Generic;
using System.Linq;
using RFCredit;

public class RandomForestClassifier
{
    private readonly int nTrees; // Number of trees in the forest
    private readonly int maxDepth; // Maximum depth of each tree
    private readonly int maxFeatures; // Number of features to use per tree
    private readonly Random random; // Random number generator
    private List<DecisionTree> trees; // Ensemble of decision trees

    public RandomForestClassifier(int nTrees = 50, int maxDepth = 1, int maxFeatures = 5)
    {
        this.nTrees = nTrees;
        this.maxDepth = maxDepth;
        this.maxFeatures = maxFeatures > 0 ? maxFeatures : -1; // Use all features if not specified
        this.random = new Random();
        this.trees = new List<DecisionTree>();
    }

    public void Train(double[][] features, double[] labels)
    {
        if (features.Length == 0 || labels.Length == 0 || features.Length != labels.Length)
        {
            throw new ArgumentException("Features and labels must be non-empty and have the same length.");
        }

        Console.WriteLine("Training Random Forest...");

        for (int i = 0; i < nTrees; i++)
        {
            // Generate bootstrap sample
            var (bootstrapFeatures, bootstrapLabels) = BootstrapSample(features, labels);

            // Train a tree on the bootstrap sample
            DecisionTree tree = new DecisionTree(maxDepth);
            var reducedFeatures = SelectRandomFeatures(bootstrapFeatures);
            tree.Train(reducedFeatures, bootstrapLabels);

            trees.Add(tree);

            Console.WriteLine($"Tree {i + 1}/{nTrees} trained.");
        }
    }

    public double Predict(double[] features)
    {
        // Aggregate predictions from all trees
        var predictions = trees.Select(tree => tree.Predict(features)).ToList();

        // Return the majority vote
        return predictions.GroupBy(p => p).OrderByDescending(g => g.Count()).First().Key;
    }

    private (double[][], double[]) BootstrapSample(double[][] features, double[] labels)
    {
        int nSamples = features.Length;
        var bootstrapFeatures = new double[nSamples][];
        var bootstrapLabels = new double[nSamples];

        for (int i = 0; i < nSamples; i++)
        {
            int randomIndex = random.Next(nSamples);
            bootstrapFeatures[i] = features[randomIndex];
            bootstrapLabels[i] = labels[randomIndex];
        }

        return (bootstrapFeatures, bootstrapLabels);
    }

    private double[][] SelectRandomFeatures(double[][] features)
    {
        int nFeatures = features[0].Length;
        int featuresToSelect = maxFeatures > 0 ? Math.Min(maxFeatures, nFeatures) : nFeatures;

        var selectedFeatureIndices = Enumerable.Range(0, nFeatures)
                                               .OrderBy(_ => random.Next())
                                               .Take(featuresToSelect)
                                               .ToArray();

        var reducedFeatures = features.Select(row => selectedFeatureIndices.Select(i => row[i]).ToArray()).ToArray();
        return reducedFeatures;
    }
}
