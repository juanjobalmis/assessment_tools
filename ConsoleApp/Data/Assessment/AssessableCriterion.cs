using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentTools.Data
{
    public class AssessableCriterion : IComparable<AssessableCriterion>, ICloneable
    {
        public string Name { get; private set; }
        public ushort WeightPercentage { get; private set; }
        public AssessableCriterion(string name, ushort weightPercentage)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            WeightPercentage = weightPercentage;
        }

        public object Clone()
        {
            return new AssessableCriterion(this.Name, this.WeightPercentage);
        }

        public override string ToString()
        {
            return $"{Name,-50}{WeightPercentage,4:D}%";
        }

        public int CompareTo(AssessableCriterion other)
        {
            int d = Name.CompareTo(other.Name);
            return (d == 0) ? WeightPercentage - other.WeightPercentage : d;
        }
        public override bool Equals(object obj)
        {
            return CompareTo(obj as AssessableCriterion) == 0;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator==(AssessableCriterion o1, AssessableCriterion o2)
        {
            return o1.Equals(o2);
        }
        public static bool operator!=(AssessableCriterion o1, AssessableCriterion o2)
        {
            return !o1.Equals(o2);
        }
    }
}
