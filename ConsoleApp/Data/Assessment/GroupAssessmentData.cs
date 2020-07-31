using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AssessmentTools.Data
{
    public class GroupAssessmentData : IEnumerable<EntityDataAssessed>
    {
        public string AssignmentName { get; private set; }
        private List<AssessableCriterion> AssessableCriteriaOfThisGroup { get; set; }

        private List<EntityDataAssessed> Data { get; set; }

        public GroupAssessmentData(string assignmentName)
        {
            AssignmentName = assignmentName;
            Data = new List<EntityDataAssessed>();
            AssessableCriteriaOfThisGroup = new List<AssessableCriterion>();
        }

        private void NomalizeToGroupCriteria(EntityDataAssessed data)
        {
            if (AssessableCriteriaOfThisGroup.Count == 0)
                throw new Exception("Assessment criteria must exist before normalize them.");

            if (data.Criteria.Count == 0)
            {
                AssessableCriteriaOfThisGroup.ForEach(ac => data.Add(new AssessableCriterionRegistry(ac.Name, ac.WeightPercentage, 0)));
            }
            else
            {
                if (!data.Criteria.SequenceEqual(AssessableCriteriaOfThisGroup))
                {
                    string message = $"The student {data.Entity.Name} has been excluded from the" +
                                        $"\n\tgroup summary for the assignment called {AssignmentName}." +
                                        $"\n\tThis assignment contains the following assessable criteria:\n\t\t{string.Join("\n\t\t", AssessableCriteriaOfThisGroup)}" +
                                        $"\n\tPlease, check the criteria in {data.Entity.Name}'s rubric.\n";
                    throw new Exception(message);
                }
            }
        }

        public void Add(AssessableCriterion criterion)
        {
            if (AssessableCriteriaOfThisGroup.FindIndex(c => c.Name.CompareTo(criterion.Name) == 0) >= 0)
                throw new ArgumentException($"The assessment criterion '{criterion.Name}' was already added to the assessment group {AssignmentName}.");
            AssessableCriteriaOfThisGroup.Add(criterion);
        }

        public void Add(EntityDataAssessed data)
        {
            if (AssessableCriteriaOfThisGroup.Count == 0)
                throw new ArgumentException("Before to add the student assessment, all the assessment criteria have to be added.");
            NomalizeToGroupCriteria(data);
            Data.Add(data ?? throw new ArgumentNullException(nameof(data)));
        }

        public override string ToString()
        {
            string text = $"Assess data for {AssignmentName} assignment:\n\n";
            Data.ForEach(e => text += $"{e}\n");
            return text;
        }

        public IEnumerator<EntityDataAssessed> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }
}
