using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionWorkBench
{
    class Program
    {
        static void Main(string[] args)
        {
            ExpressionTreeIntroduction();
            ExpressionTreeAPIIntroduction();
            DynamicQueryExpressionTree();

            Console.ReadLine();
        }

        /// <summary>
        /// Introduction to Expression Trees via Lambda Expressions
        /// </summary>
        private static void ExpressionTreeIntroduction()
        {
            Console.WriteLine("Expression Tree Introduction");
            Console.WriteLine("> Creating an expression tree to represent \"num => num > 5\" using a one line lambda expression.");
            Expression<Func<int, bool>> lambda = num => num < 5;

            var testValue = 6;

            var isGreatThanFive = lambda.Compile()(testValue);
            Console.WriteLine($"> Is {testValue} greater than five? {isGreatThanFive}\n");
        }

        /// <summary>
        /// How to create expression trees using the expression tree api
        /// </summary>
        private static void ExpressionTreeAPIIntroduction()
        {
            Console.WriteLine("Expression Tree API Introduction");
            Console.WriteLine("> Creating an expression tree using the API to represent \"num => num > 5\"");

            // Add the following using directive to your code file:  
            // using System.Linq.Expressions;  

            // Manually build the expression tree for   
            // the lambda expression num => num < 5.  
            ParameterExpression numParam = Expression.Parameter(typeof(int), "num");
            ConstantExpression five = Expression.Constant(5, typeof(int));
            BinaryExpression numLessThanFive = Expression.LessThan(numParam, five);
            Expression<Func<int, bool>> lambda1 =
                Expression.Lambda<Func<int, bool>>(
                    numLessThanFive,
                    new ParameterExpression[] { numParam });

            var testValue = 6;

            var isGreatThanFive = lambda1.Compile()(testValue);

            Console.WriteLine($"> Is {testValue} greater than five? {isGreatThanFive}\n");
        }

        /// <summary>
        /// Create a dynamic linq expression
        /// </summary>
        private static void DynamicQueryExpressionTree()
        {
            Console.WriteLine("Introduction to Expression Trees");

            // Add a using directive for System.Linq.Expressions.  

            string[] companies = { "Consolidated Messenger", "Alpine Ski House", "Southridge Video", "City Power & Light",
                   "Coho Winery", "Wide World Importers", "Graphic Design Institute", "Adventure Works",
                   "Humongous Insurance", "Woodgrove Bank", "Margie's Travel", "Northwind Traders",
                   "Blue Yonder Airlines", "Trey Research", "The Phone Company",
                   "Wingtip Toys", "Lucerne Publishing", "Fourth Coffee" };

            // The IQueryable data to query.  
            IQueryable<String> queryableData = companies.AsQueryable();

            // Compose the expression tree that represents the parameter to the predicate.  
            ParameterExpression pe = Expression.Parameter(typeof(string), "company");

            // ***** Where(company => (company.ToLower() == "coho winery" || company.Length > 16)) *****  
            // Create an expression tree that represents the expression 'company.ToLower() == "coho winery"'.  

            Expression left = Expression.Call(pe, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            Expression right = Expression.Constant("coho winery");
            Expression e1 = Expression.Equal(left, right);

            // Create an expression tree that represents the expression 'company.Length > 16'. 
            
            left = Expression.Property(pe, typeof(string).GetProperty("Length"));
            right = Expression.Constant(16, typeof(int));
            Expression e2 = Expression.GreaterThan(left, right);

            // Combine the expression trees to create an expression tree that represents the  
            // expression '(company.ToLower() == "coho winery" || company.Length > 16)'.  

            Expression predicateBody = Expression.OrElse(e1, e2);

            // Create an expression tree that represents the expression  
            // 'queryableData.Where(company => (company.ToLower() == "coho winery" || company.Length > 16))'  

            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { queryableData.ElementType },
                queryableData.Expression,
                Expression.Lambda<Func<string, bool>>(predicateBody, new ParameterExpression[] { pe }));

            // ***** End Where *****  

            // ***** OrderBy(company => company) *****  
            // Create an expression tree that represents the expression  
            // 'whereCallExpression.OrderBy(company => company)'

            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new Type[] { queryableData.ElementType, queryableData.ElementType },
                whereCallExpression,
                Expression.Lambda<Func<string, string>>(pe, new ParameterExpression[] { pe }));

            // ***** End OrderBy *****  

            // Create an executable query from the expression tree.  
            IQueryable<string> results = queryableData.Provider.CreateQuery<string>(orderByCallExpression);

            // Enumerate the results.  
            foreach (string company in results)
            {
                Console.WriteLine($"> {company}");
            }

            /*  This code produces the following output:  

                Blue Yonder Airlines  
                City Power & Light  
                Coho Winery  
                Consolidated Messenger  
                Graphic Design Institute  
                Humongous Insurance  
                Lucerne Publishing  
                Northwind Traders  
                The Phone Company  
                Wide World Importers  
            */

            Console.WriteLine();
        }
    }
}
