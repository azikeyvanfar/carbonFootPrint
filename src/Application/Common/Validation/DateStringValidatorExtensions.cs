using System.Text.RegularExpressions;
using FluentValidation;

namespace ContractorBackend.Application.Common.Validation
{
    public static class DateStringValidatorExtensions
    {

        /// <summary>
        /// validate dates in string with just month like 140205
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="message">Error Message to show</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string?> DateStringMonthValidator<T>(this IRuleBuilder<T, string?> ruleBuilder, string message = null)
        {
            var regex = new Regex(@"^(13|14|15)[0-9][0-9](0[1-9]|1[0-2])$");
            var res = ruleBuilder.Must((a) => string.IsNullOrEmpty(a) ? true : regex.IsMatch(a ?? ""));

            if (!string.IsNullOrWhiteSpace(message))
            {
                res.WithMessage(message);
            }
            return res;

            //var res = ruleBuilder.Must(x => true);
            //return res;
        }
    }
}
