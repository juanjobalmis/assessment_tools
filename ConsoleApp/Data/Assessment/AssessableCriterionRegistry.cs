using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentTools.Data
{
    public class AssessableCriterionRegistry : ICloneable
    {
        public AssessableCriterion Criterion { get; private set; }
        public double GradeRelativeToWeight { get; private set; }
        public double Grade { get => Criterion.WeightPercentage > 0 ? GradeRelativeToWeight * 100 / Criterion.WeightPercentage : 0D; }
        public string Note { get; private set; }
        public AssessableCriterionRegistry(string name, ushort weightPercentage, double grade, string note)
        {
            Criterion = new AssessableCriterion(name, weightPercentage);
            GradeRelativeToWeight = grade;
            Note = note;
        }
        public AssessableCriterionRegistry(string name, ushort weightPercentage, double grade)
        {
            Criterion = new AssessableCriterion(name, weightPercentage);
            GradeRelativeToWeight = grade;
            Note = null;
        }

        public object Clone()
        {
            return new AssessableCriterionRegistry(Criterion.Name, Criterion.WeightPercentage, this.GradeRelativeToWeight, this.Note);
        }

        public override string ToString()
        {
            return $"{Criterion} of {Grade,5:F2} = {GradeRelativeToWeight,5:F2}";
        }
    }
}
