using System.Text.RegularExpressions;
using FluentValidation;

namespace ContractorBackend.Application.Common.Validation
{
    public static class PhoneExtensions
    {
        /// <summary>
        /// ولیدیشن شماره های موبایل 10 رقمی
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, long?> PhoneNullableCustomValidator<T>(this IRuleBuilder<T, long?> ruleBuilder)
        {
            var regex = new Regex(@"\b\d{10}\b");
            var res = ruleBuilder.Must((a) => a.HasValue ? regex.IsMatch(a.Value.ToString() ?? "") : true);

            return res;
        }

        /// <summary>
        /// ولیدیشن شماره های داخلی فولاد 5 رقمی
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, long?> InternalPhoneNullableCustomValidator<T>(this IRuleBuilder<T, long?> ruleBuilder)
        {
            var regex = new Regex(@"\b\d{5}\b");
            var res = ruleBuilder.Must((a) => a.HasValue ? regex.IsMatch(a.Value.ToString() ?? "") : true);

            return res;
        }

        //public static IRuleBuilderOptions<T, long> longCustomValidator<T>(this IRuleBuilder<T, long> ruleBuilder, string message1 = "", string Message2 = "")
        //{
        //    return ruleBuilder 
        //        .GreaterThan(new long(1900, 01, 01))
        //        //.WithMessage(message1)
        //        .LessThan(new long(2200, 01, 01))
        //        //.WithMessage(Message2)
        //        ;
        //}

    }
}
