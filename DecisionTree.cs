using System;
using System.Linq;
using RFCredit;
public class DecisionTree
{
    private int maxDepth;
    private int splitFeatureIndex;
    private double splitThreshold;
    private double leftValue;
    private double rightValue;

    public DecisionTree(int maxDepth)
    {
        this.maxDepth = maxDepth;
        splitFeatureIndex = -1;
    }

    public void Train(double[][] features, double[] residuals)
    {
        int nSamples = features.Length;
        int nFeatures = features[0].Length;

        double bestError = double.MaxValue;

        for (int featureIndex = 0; featureIndex < nFeatures; featureIndex++)
        {
            var sortedIndices = Enumerable.Range(0, nSamples)
                                          .OrderBy(i => features[i][featureIndex])
                                          .ToArray();

            var sortedFeatures = sortedIndices.Select(i => features[i][featureIndex]).ToArray();
            var sortedResiduals = sortedIndices.Select(i => residuals[i]).ToArray();

            double leftSum = 0.0, rightSum = residuals.Sum();
            int leftCount = 0, rightCount = nSamples;

            for (int i = 0; i < nSamples - 1; i++)
            {
                leftSum += sortedResiduals[i];
                rightSum -= sortedResiduals[i];
                leftCount++;
                rightCount--;

                if (sortedFeatures[i] == sortedFeatures[i + 1]) continue;

                double leftMean = leftSum / leftCount;
                double rightMean = rightSum / rightCount;

                double leftError = 0.0, rightError = 0.0;
                for (int j = 0; j <= i; j++) leftError += Math.Pow(sortedResiduals[j] - leftMean, 2);
                for (int j = i + 1; j < nSamples; j++) rightError += Math.Pow(sortedResiduals[j] - rightMean, 2);

                double totalError = leftError + rightError;

                if (totalError < bestError)
                {
                    bestError = totalError;
                    splitFeatureIndex = featureIndex;
                    splitThreshold = (sortedFeatures[i] + sortedFeatures[i + 1]) / 2.0;
                    leftValue = leftMean;
                    rightValue = rightMean;
                }
            }
        }

        if (splitFeatureIndex == -1)
        {
            throw new InvalidOperationException("No valid split found.");
        }
    }

    public double Predict(double[] features)
    {
        if (splitFeatureIndex == -1)
        {
            throw new InvalidOperationException("Tree has not been trained.");
        }

        return features[splitFeatureIndex] <= splitThreshold ? leftValue : rightValue;
    }
}
