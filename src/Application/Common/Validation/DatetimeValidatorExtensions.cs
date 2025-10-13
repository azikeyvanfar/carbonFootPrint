using System;
using FluentValidation;

namespace ContractorBackend.Application.Common.Validation
{
    public static class DateTimeExtensions
    {
        public static IRuleBuilderOptions<T, DateTime?> DateTimeNullableCustomValidator<T>(this IRuleBuilder<T, DateTime?> ruleBuilder, string message1 = "", string Message2 = "")
        {
            return ruleBuilder
                .GreaterThan(new DateTime(1900, 01, 01))
                //.WithMessage(message1)
                .LessThan(new DateTime(2200, 01, 01))
                //.WithMessage(Message2)
                ;
        }
        public static IRuleBuilderOptions<T, DateTime> DateTimeCustomValidator<T>(this IRuleBuilder<T, DateTime> ruleBuilder, string message1 = "", string Message2 = "")
        {
            return ruleBuilder
                .GreaterThan(new DateTime(1900, 01, 01))
                //.WithMessage(message1)
                .LessThan(new DateTime(2200, 01, 01))
                //.WithMessage(Message2)
                ;
        }

    }
}
