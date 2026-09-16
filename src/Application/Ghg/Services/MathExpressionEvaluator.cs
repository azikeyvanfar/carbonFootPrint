using System;
using System.Collections.Generic;

namespace ContractorBackend.Application.Ghg.Services
{
    /// <summary>
    /// موتور ارزیابی عبارات ریاضی فرمول‌های محاسبه انتشار.
    /// از عملگرهای + - * / % ^ ، پرانتز، منفی یکانی، اعداد و متغیرها و
    /// توابع Abs, Min, Max, Round, Sqrt, Pow, Exp, Ln, Log10, Floor, Ceiling, If پشتیبانی می‌کند.
    /// بدون وابستگی خارجی - فرمول‌ها در بخش تنظیمات به صورت متن ذخیره و با این موتور اجرا می‌شوند.
    /// </summary>
    public static class MathExpressionEvaluator
    {
        /// <summary>
        /// ارزیابی عبارت با متغیرهای داده شده
        /// </summary>
        public static double Evaluate(string expression, IDictionary<string, double> variables)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("عبارت فرمول خالی است.", nameof(expression));

            var tokens = Tokenize(expression);
            var parser = new Parser(tokens, variables ?? new Dictionary<string, double>());
            var result = parser.Parse();
            parser.EnsureEnd();
            return result;
        }

        /// <summary>
        /// استخراج نام متغیرهای استفاده شده در عبارت
        /// </summary>
        public static IReadOnlyList<string> ExtractVariables(string expression)
        {
            var variables = new List<string>();
            var tokens = Tokenize(expression);
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.Identifier && !IsFunction(token.Text) && !variables.Contains(token.Text))
                {
                    variables.Add(token.Text);
                }
            }
            return variables;
        }

        private static bool IsFunction(string name) =>
            name.Equals("Abs", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Min", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Max", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Round", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Sqrt", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Pow", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Exp", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Ln", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Log", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Log10", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Floor", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Ceiling", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("If", StringComparison.OrdinalIgnoreCase);

        #region Tokenizer

        private enum TokenType
        {
            Number,
            Identifier,
            Plus,
            Minus,
            Multiply,
            Divide,
            Modulo,
            Power,
            LeftParen,
            RightParen,
            Comma
        }

        private sealed class Token
        {
            public TokenType Type { get; set; }
            public string Text { get; set; } = string.Empty;
            public double Number { get; set; }
        }

        private static List<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            int i = 0;
            int n = expression.Length;

            while (i < n)
            {
                char c = expression[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (char.IsDigit(c) || (c == '.' && i + 1 < n && char.IsDigit(expression[i + 1])))
                {
                    int start = i;
                    while (i < n && (char.IsDigit(expression[i]) || expression[i] == '.' || expression[i] == 'e' || expression[i] == 'E'))
                    {
                        // scientific notation like 1E-06
                        if ((expression[i] == 'e' || expression[i] == 'E') && i + 1 < n && (expression[i + 1] == '-' || expression[i + 1] == '+' || char.IsDigit(expression[i + 1])))
                        {
                            i++;
                            if (expression[i] == '-' || expression[i] == '+') i++;
                            continue;
                        }
                        i++;
                    }

                    var text = expression.Substring(start, i - start);
                    if (!double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var value))
                        throw new FormatException($"عدد نامعتبر در فرمول: {text}");

                    tokens.Add(new Token { Type = TokenType.Number, Text = text, Number = value });
                    continue;
                }

                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    while (i < n && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                        i++;

                    tokens.Add(new Token { Type = TokenType.Identifier, Text = expression.Substring(start, i - start) });
                    continue;
                }

                switch (c)
                {
                    case '+': tokens.Add(new Token { Type = TokenType.Plus }); i++; break;
                    case '-': tokens.Add(new Token { Type = TokenType.Minus }); i++; break;
                    case '*': tokens.Add(new Token { Type = TokenType.Multiply }); i++; break;
                    case '/': tokens.Add(new Token { Type = TokenType.Divide }); i++; break;
                    case '%': tokens.Add(new Token { Type = TokenType.Modulo }); i++; break;
                    case '^': tokens.Add(new Token { Type = TokenType.Power }); i++; break;
                    case '(': tokens.Add(new Token { Type = TokenType.LeftParen }); i++; break;
                    case ')': tokens.Add(new Token { Type = TokenType.RightParen }); i++; break;
                    case ',': tokens.Add(new Token { Type = TokenType.Comma }); i++; break;
                    default:
                        throw new FormatException($"کاراکتر نامعتبر در فرمول: '{c}' در موقعیت {i}");
                }
            }

            return tokens;
        }

        #endregion

        #region Parser

        private sealed class Parser
        {
            private readonly List<Token> _tokens;
            private readonly IDictionary<string, double> _variables;
            private int _position;

            public Parser(List<Token> tokens, IDictionary<string, double> variables)
            {
                _tokens = tokens;
                _variables = variables;
            }

            public double Parse()
            {
                return ParseExpression();
            }

            public void EnsureEnd()
            {
                if (_position < _tokens.Count)
                    throw new FormatException($"توکن اضافی در انتهای فرمول: '{_tokens[_position].Text}'");
            }

            // expression = term (('+' | '-') term)*
            private double ParseExpression()
            {
                var left = ParseTerm();
                while (_position < _tokens.Count)
                {
                    var op = _tokens[_position];
                    if (op.Type == TokenType.Plus)
                    {
                        _position++;
                        left += ParseTerm();
                    }
                    else if (op.Type == TokenType.Minus)
                    {
                        _position++;
                        left -= ParseTerm();
                    }
                    else break;
                }
                return left;
            }

            // term = factor (('*' | '/' | '%') factor)*
            private double ParseTerm()
            {
                var left = ParseUnary();
                while (_position < _tokens.Count)
                {
                    var op = _tokens[_position];
                    if (op.Type == TokenType.Multiply)
                    {
                        _position++;
                        left *= ParseUnary();
                    }
                    else if (op.Type == TokenType.Divide)
                    {
                        _position++;
                        var right = ParseUnary();
                        if (right == 0) throw new DivideByZeroException("تقسیم بر صفر در فرمول محاسبه.");
                        left /= right;
                    }
                    else if (op.Type == TokenType.Modulo)
                    {
                        _position++;
                        var right = ParseUnary();
                        if (right == 0) throw new DivideByZeroException("تقسیم بر صفر در فرمول محاسبه.");
                        left %= right;
                    }
                    else break;
                }
                return left;
            }

            // unary = ('-' | '+') unary | power
            private double ParseUnary()
            {
                if (_position < _tokens.Count && _tokens[_position].Type == TokenType.Minus)
                {
                    _position++;
                    return -ParseUnary();
                }
                if (_position < _tokens.Count && _tokens[_position].Type == TokenType.Plus)
                {
                    _position++;
                    return ParseUnary();
                }
                return ParsePower();
            }

            // power = atom ('^' unary)?
            private double ParsePower()
            {
                var left = ParseAtom();
                if (_position < _tokens.Count && _tokens[_position].Type == TokenType.Power)
                {
                    _position++;
                    var right = ParseUnary(); // right associative
                    return Math.Pow(left, right);
                }
                return left;
            }

            // atom = number | variable | function '(' args ')' | '(' expression ')'
            private double ParseAtom()
            {
                if (_position >= _tokens.Count)
                    throw new FormatException("فرمول ناقص است - انتظار عدد یا متغیر.");

                var token = _tokens[_position];

                if (token.Type == TokenType.Number)
                {
                    _position++;
                    return token.Number;
                }

                if (token.Type == TokenType.Identifier)
                {
                    _position++;

                    if (_position < _tokens.Count && _tokens[_position].Type == TokenType.LeftParen)
                    {
                        return CallFunction(token.Text);
                    }

                    if (_variables.TryGetValue(token.Text, out var value))
                        return value;

                    throw new KeyNotFoundException($"متغیر '{token.Text}' در فرمول تعریف نشده یا مقداردهی نشده است.");
                }

                if (token.Type == TokenType.LeftParen)
                {
                    _position++;
                    var value = ParseExpression();
                    Expect(TokenType.RightParen, "')'");
                    return value;
                }

                throw new FormatException($"توکن غیرمنتظره '{token.Text}' در فرمول.");
            }

            private double CallFunction(string name)
            {
                Expect(TokenType.LeftParen, "'('");

                var args = new List<double>();
                if (_position < _tokens.Count && _tokens[_position].Type != TokenType.RightParen)
                {
                    args.Add(ParseExpression());
                    while (_position < _tokens.Count && _tokens[_position].Type == TokenType.Comma)
                    {
                        _position++;
                        args.Add(ParseExpression());
                    }
                }

                Expect(TokenType.RightParen, "')'");

                if (name.Equals("Abs", StringComparison.OrdinalIgnoreCase)) return CheckArgs(name, args, 1) ? Math.Abs(args[0]) : 0;
                if (name.Equals("Min", StringComparison.OrdinalIgnoreCase))
                {
                    CheckArgs(name, args, 2);
                    return Math.Min(args[0], args[1]);
                }
                if (name.Equals("Max", StringComparison.OrdinalIgnoreCase))
                {
                    CheckArgs(name, args, 2);
                    return Math.Max(args[0], args[1]);
                }
                if (name.Equals("Round", StringComparison.OrdinalIgnoreCase))
                {
                    CheckArgs(name, args, 1);
                    var digits = args.Count > 1 ? (int)args[1] : 0;
                    return Math.Round(args[0], digits);
                }
                if (name.Equals("Sqrt", StringComparison.OrdinalIgnoreCase)) { CheckArgs(name, args, 1); return Math.Sqrt(args[0]); }
                if (name.Equals("Pow", StringComparison.OrdinalIgnoreCase))
                {
                    CheckArgs(name, args, 2);
                    return Math.Pow(args[0], args[1]);
                }
                if (name.Equals("Exp", StringComparison.OrdinalIgnoreCase)) { CheckArgs(name, args, 1); return Math.Exp(args[0]); }
                if (name.Equals("Ln", StringComparison.OrdinalIgnoreCase)) { CheckArgs(name, args, 1); return Math.Log(args[0]); }
                if (name.Equals("Log", StringComparison.OrdinalIgnoreCase) || name.Equals("Log10", StringComparison.OrdinalIgnoreCase))
                {
                    CheckArgs(name, args, 1);
                    return Math.Log10(args[0]);
                }
                if (name.Equals("Floor", StringComparison.OrdinalIgnoreCase)) { CheckArgs(name, args, 1); return Math.Floor(args[0]); }
                if (name.Equals("Ceiling", StringComparison.OrdinalIgnoreCase)) { CheckArgs(name, args, 1); return Math.Ceiling(args[0]); }
                if (name.Equals("If", StringComparison.OrdinalIgnoreCase))
                {
                    CheckArgs(name, args, 3);
                    return args[0] != 0 ? args[1] : args[2];
                }

                throw new FormatException($"تابع '{name}' در موتور فرمول پشتیبانی نمی‌شود.");
            }

            private bool CheckArgs(string name, List<double> args, int expected)
            {
                if (args.Count < expected)
                    throw new FormatException($"تابع '{name}' حداقل {expected} آرگومان نیاز دارد.");
                return true;
            }

            private void Expect(TokenType type, string description)
            {
                if (_position >= _tokens.Count || _tokens[_position].Type != type)
                    throw new FormatException($"انتظار {description} در فرمول بود.");
                _position++;
            }
        }

        #endregion
    }
}
