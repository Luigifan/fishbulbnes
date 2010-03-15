using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace InstiBulb.ValidationRules
{
    public class GameGenieValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string code = value as string;
            if (String.IsNullOrEmpty( code ))
                return new ValidationResult(false, "No code entered.");

            code = code.ToUpper();
            if (code.Length != 6 && code.Length != 8)
            {
                return new ValidationResult(false, "Game Genie codes are 6 or 8 characters.");
            }

            bool validCode = true;
            foreach (char c in code)
            {
                switch (c)
                {
                    case 'A':
                    case 'P':
                    case 'Z':
                    case 'L':
                    case 'G':
                    case 'I':
                    case 'T':
                    case 'Y':
                    case 'E':
                    case 'O':
                    case 'X':
                    case 'U':
                    case 'K':
                    case 'S':
                    case 'V':
                    case 'N':
                        break;
                    default:
                        validCode = false;
                        break;
                }
            }

            if (!validCode)
            {

                return new ValidationResult(false, "Game Genie codes contain only APZLGITYEOXUKSVN.");
            }

            return new ValidationResult(true, null);

        }
    }
}
