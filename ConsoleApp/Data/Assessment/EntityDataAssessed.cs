using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AssessmentTools.Data
{
    public class EntityDataAssessed : IEnumerable<AssessableCriterionRegistry>
    {
        private Dictionary<string, AssessableCriterionRegistry> AssessmentCriteria { get; set; }
        public IEntityInfoToAssess Entity { get; private set; }
        public int CriteriaCount { get => AssessmentCriteria.Count; }
        public double Grade { get => AssessmentCriteria.Values.Select(ac => ac.GradeRelativeToWeight).Sum(); }
        public List<AssessableCriterion> Criteria 
        {
            get { return AssessmentCriteria.Values.Select(acr => acr.Criterion).ToList(); }
        }

        public AssessableCriterionRegistry this[string criteria]
        {
            get
            {
                if (!AssessmentCriteria.ContainsKey(criteria))
                    throw new ArgumentException($"{criteria} is not a valid assessable criteria-");

                return AssessmentCriteria[criteria];
            }
        }
        public EntityDataAssessed(IEntityInfoToAssess entity)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            AssessmentCriteria = new Dictionary<string, AssessableCriterionRegistry>();
        }
        public void Add(AssessableCriterionRegistry criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (!AssessmentCriteria.ContainsKey(criteria.Criterion.Name))
                AssessmentCriteria.Add(criteria.Criterion.Name, criteria.Clone() as AssessableCriterionRegistry);
            else
                throw new ArgumentException($"The criteria {criteria} already exists in the the assessment criteria for {Entity.Name}.");
        }

        public override string ToString()
        {
            string text = $"Assess data for {Entity.Name}\n";
            foreach (var ac in AssessmentCriteria)
                text += $"\t{ac}\n";
            return text;
        }

        public IEnumerator<AssessableCriterionRegistry> GetEnumerator()
        {
            return AssessmentCriteria.Values.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AssessmentCriteria.Values.ToList().GetEnumerator();
        }
    }
}
