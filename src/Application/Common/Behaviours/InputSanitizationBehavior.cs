using ContractorBackend.Application.Common.Exceptions;
using MediatR;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ContractorBackend.Application.Common.Behaviours
{
    public class InputSanitizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var stringProperties = request
                .GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(string));

            foreach (var prop in stringProperties)
            {
                var value = prop.GetValue(request) as string;
                if (string.IsNullOrWhiteSpace(value)) continue;

                if (IsPotentiallyMalicious(value))
                    throw new CustomException("داده ی ورودی نامعتبر می باشد.");

            }

            return await next();
        }

        private bool IsPotentiallyMalicious(string input)
        {
            // الگوهای خطرناک ساده برای جلوگیری از XSS / Command Injection
            var patterns = new[]
                {
                @"<script.*?>",    // XSS
                @"</script>",
                @"(;|\||&&)",      // Shell chaining
                @"\b(rm|chmod|curl|wget|sudo|bash|sh)\b", // Shell commands
                @"\$\(",           // Command substitution
                @"eval\s*\(",      // JS eval
                @"exec\s*\("
            };

            return patterns.Any(pattern => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
        }
    }
}
