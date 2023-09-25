using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CalculatorAPI.Controllers
{
    [Route("api/calculator")]
    [ApiController]
    public class CalculatorController : ControllerBase
    {
        [HttpPost("evaluate")]
        public IActionResult EvaluateExpression([FromBody] string expression)
        {
            try
            {
                double result = Evaluate(expression);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private double Evaluate(string expression)
        {
            // Remove spaces from the expression
            expression = expression.Replace(" ", "");

            // Split the expression into tokens
            List<string> tokens = new List<string>();
            int i = 0;
            while (i < expression.Length)
            {
                if (char.IsDigit(expression[i]))
                {
                    int j = i + 1;
                    while (j < expression.Length && (char.IsDigit(expression[j]) || expression[j] == '.'))
                    {
                        j++;
                    }
                    tokens.Add(expression.Substring(i, j - i));
                    i = j;
                }
                else if (expression[i] == '+' || expression[i] == '-' || expression[i] == '*' || expression[i] == '/')
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                else
                {
                    throw new Exception($"Invalid character in expression: {expression[i]}");
                }
            }

            // Initialize a stack to hold numbers and operators
            Stack<double> numberStack = new Stack<double>();
            Stack<char> operatorStack = new Stack<char>();

            // Define operator precedence
            Dictionary<char, int> precedence = new Dictionary<char, int>
            {
                { '+', 1 },
                { '-', 1 },
                { '*', 2 },
                { '/', 2 }
            };

            foreach (string token in tokens)
            {
                if (double.TryParse(token, out double number))
                {
                    numberStack.Push(number);
                }
                else if (token == "+" || token == "-" || token == "*" || token == "/")
                {
                    while (operatorStack.Count > 0 && precedence[operatorStack.Peek()] >= precedence[token[0]])
                    {
                        double b = numberStack.Pop();
                        double a = numberStack.Pop();
                        char op = operatorStack.Pop();

                        double result = ApplyOperator(a, b, op);
                        numberStack.Push(result);
                    }
                    operatorStack.Push(token[0]);
                }
                else
                {
                    throw new Exception($"Invalid token: {token}");
                }
            }

            while (operatorStack.Count > 0)
            {
                double b = numberStack.Pop();
                double a = numberStack.Pop();
                char op = operatorStack.Pop();

                double result = ApplyOperator(a, b, op);
                numberStack.Push(result);
            }

            return numberStack.Pop();
        }

        private double ApplyOperator(double a, double b, char op)
        {
            switch (op)
            {
                case '+':
                    return a + b;
                case '-':
                    return a - b;
                case '*':
                    return a * b;
                case '/':
                    if (b == 0)
                        throw new Exception("Division by zero");
                    return a / b;
                default:
                    throw new Exception($"Invalid operator: {op}");
            }
        }
    }
}
