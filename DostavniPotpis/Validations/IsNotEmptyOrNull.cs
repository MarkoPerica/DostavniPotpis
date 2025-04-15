using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Validations
{
    internal class IsNotEmptyOrNull<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }
        public bool Check(T value) =>
            value is string str &&
            !string.IsNullOrWhiteSpace(str);
    }
}
