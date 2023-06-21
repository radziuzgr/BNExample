class BayesianNetworkExample
{
    public void Program()
    {
        Init();

        var resultProbability = Enumerate(probabilities, question.EvidenceVariables, question.QueryVariables);

        Console.WriteLine(resultProbability.ToString());
    }

    class ProbabilityItem
    {
        public string NodeName { get; set; }
        public Dictionary<string, bool> Combination { get; set; }
        public double Probability { get; set; }
    }

    class QuestionParseResult
    {
        public List<Dictionary<string, bool>> QueryVariables { get; set; }
        public List<Dictionary<string, bool>> EvidenceVariables { get; set; }
    }

    const string umbrellaNode = "Umbrella";
    const string wetNode = "Wet";
    List<ProbabilityItem> probabilities = new List<ProbabilityItem>();
    QuestionParseResult question = new QuestionParseResult();

    void Init()
    {
        question.EvidenceVariables = new List<Dictionary<string, bool>>();
        var dict1 = new Dictionary<string, bool>
    {
        { umbrellaNode, true }
    };
        question.EvidenceVariables.Add(dict1);

        question.QueryVariables = new List<Dictionary<string, bool>>();
        var dict2 = new Dictionary<string, bool>
    {
        { wetNode, true }
    };
        question.QueryVariables.Add(dict2);

        var firstCombination = new Dictionary<string, bool>
    {
        {wetNode,true }
    };
        probabilities.Add(new ProbabilityItem { Combination = firstCombination, NodeName = wetNode, Probability = 0.3 });

        var secondCombination = new Dictionary<string, bool>
    {
        {wetNode,false }
    };
        probabilities.Add(new ProbabilityItem { Combination = secondCombination, NodeName = wetNode, Probability = 0.7 });

        var thirdCombination = new Dictionary<string, bool>
    {
        { wetNode,false },
        { umbrellaNode, true }
    };
        probabilities.Add(new ProbabilityItem { Combination = thirdCombination, NodeName = umbrellaNode, Probability = 0.2 });

        var fourthCombination = new Dictionary<string, bool>
    {
        { wetNode,true },
        { umbrellaNode, false }
    };
        probabilities.Add(new ProbabilityItem { Combination = fourthCombination, NodeName = umbrellaNode, Probability = 0.8 });

        var fifthCombination = new Dictionary<string, bool>
    {
        { wetNode,true },
        { umbrellaNode, true }
    };
        probabilities.Add(new ProbabilityItem { Combination = fifthCombination, NodeName = umbrellaNode, Probability = 0.8 });

        var sixCombination = new Dictionary<string, bool>
    {
        { wetNode, false },
        { umbrellaNode, false }
    };
        probabilities.Add(new ProbabilityItem { Combination = sixCombination, NodeName = umbrellaNode, Probability = 0.2 });
    }
    double Enumerate(List<ProbabilityItem> probabilityDistributions, List<Dictionary<string, bool>> evidenceVariables, List<Dictionary<string, bool>> queryVariables)
    {
        // If there are no more query variables to evaluate
        if (queryVariables.Count == 0)
        {
            // Calculate the joint probability of evidence variables
            var jointProbability = CalculateJointProbability(probabilityDistributions, evidenceVariables);
            return jointProbability;
        }

        // Get the first query variable to evaluate
        var queryVariable = queryVariables[0];
        // Get the remaining query variables
        var remainingQueryVariables = queryVariables.Skip(1).ToList();

        // Generate assignments for evidence variables where the query variable is true
        var evidenceVariableAssignmentsTrue = evidenceVariables.Select(e => e.Concat(queryVariable)).ToList();
        // Generate assignments for evidence variables where the query variable is false
        var evidenceVariableAssignmentsFalse = evidenceVariables.Select(e => e.Concat(Negate(queryVariable))).ToList();

        // Initialize the sum of probabilities
        var sum = 0.0;

        // Enumerate over evidence variable assignments where the query variable is true
        foreach (var assignment in evidenceVariableAssignmentsTrue)
        {
            // Create a copy of evidence variables and add the current assignment
            var updatedEvidenceVariables = new List<Dictionary<string, bool>>(evidenceVariables);
            updatedEvidenceVariables.Add(assignment.ToDictionary(x => x.Key, x => x.Value));
            // Recursively call Enumerate with updated evidence variables and remaining query variables
            var probability = Enumerate(probabilityDistributions, updatedEvidenceVariables, remainingQueryVariables);
            // Add the probability to the sum
            sum += probability;
        }

        // Enumerate over evidence variable assignments where the query variable is false
        foreach (var assignment in evidenceVariableAssignmentsFalse)
        {
            // Create a copy of evidence variables and add the current assignment
            var updatedEvidenceVariables = new List<Dictionary<string, bool>>(evidenceVariables);
            updatedEvidenceVariables.Add(assignment.ToDictionary(x => x.Key, x => x.Value));
            // Recursively call Enumerate with updated evidence variables and remaining query variables
            var probability = Enumerate(probabilityDistributions, updatedEvidenceVariables, remainingQueryVariables);
            // Add the probability to the sum
            sum += probability;
        }

        // Return the sum of probabilities
        return sum;
    }

    double CalculateJointProbability(List<ProbabilityItem> probabilityDistributions, List<Dictionary<string, bool>> evidenceVariables)
    {
        // Initialize the joint probability to 1.0
        var jointProbability = 1.0;

        // Enumerate over the evidence variables
        foreach (var evidence in evidenceVariables)
        {
            // Find probability distributions that match the evidence
            var matchingProbabilityItems = probabilityDistributions
                .Where(p => p.Combination.OrderBy(kv => kv.Key).SequenceEqual(evidence.OrderBy(kv => kv.Key)));

            // If there are matching probability items
            if (matchingProbabilityItems.Any())
            {
                var evidenceProbability = 0.0;

                // Sum the probabilities of matching probability items
                foreach (var matchingItem in matchingProbabilityItems)
                {
                    evidenceProbability += matchingItem.Probability;
                }

                // Multiply the joint probability by the evidence probability
                jointProbability *= evidenceProbability;
            }
        }

        // Return the joint probability
        return jointProbability;
    }

    Dictionary<string, bool> Negate(Dictionary<string, bool> variable)
    {
        // Create a new dictionary for negated variable
        var negatedVariable = new Dictionary<string, bool>();
        // Negate each value in the variable
        foreach (var kvp in variable)
        {
            negatedVariable[kvp.Key] = !kvp.Value;
        }
        // Return the negated variable
        return negatedVariable;
    }

}
