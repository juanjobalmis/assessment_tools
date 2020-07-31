using System;
using System.Text;

namespace AssessmentTools.Commands.AG
{
    public class AssessmentPlatform
    {
        public enum Target { Moodle, Gmail, Xlsx }
        private Target target;

        public Target TargetPlatForm
        {
            get
            {
                return target;
            }
        }

        private string TargetText
        {
            set 
            {
                StringBuilder v = null;
                bool valid = value != null && value.Length > 0;
                if (valid)
                {
                    v = new StringBuilder(value.ToLower());
                    v[0] = char.ToUpper(v[0]);
                    valid = Enum.IsDefined(typeof(Target), v.ToString());
                }

                if (valid)
                    target = (Target)Enum.Parse(typeof(Target), v.ToString());
                else
                {
                    string message = $"{value} is not valid as assesment platform.\n" +
                                      "Use: " + string.Join(", ", Enum.GetNames(typeof(Target)));
                    throw new ArgumentException(message);
                }
            }
        }

        public AssessmentPlatform(string platform)
        {
            TargetText = platform;
        }
    }
}
