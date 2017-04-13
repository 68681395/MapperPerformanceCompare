
// Test.cs
//
using System;

class Test
{
    static void Main()
    {
        double[] Y =
        {
            9999992.348,
            9999992.35,
            9999992.354,
            9999992.359,
            9999992.361,
            9999992.365,
            9999992.366,
            9999992.37,
            9999992.371,
            9999992.374,
            9999992.376,
            9999992.377,
            9999992.379,
            9999992.38,
            9999992.382,
            9999992.384,
            9999992.386,
            9999992.387,
            9999992.389,
            9999992.39,
            9999992.39,
            9999992.392,
            9999992.392
        };
        double[] X = new double[Y.Length];
        for (int i = 0; i < X.Length; i++) X[i] = i + 1;

        // f(x) = A ln(x) + B
        GaussNewton.F f = delegate (double[] coefficients, double x)
        {
            return coefficients[0] * Math.Log(x) + coefficients[1];
        };
        GaussNewton gaussNewton = new GaussNewton(2);
        gaussNewton.Initialize(Y, X, f);
        double[] answer = gaussNewton.Coefficients;     //answer[0] = 0.016 answer[1]=9999992.3386
    }

    static void PolynomialTest()
    {
        // y = -x*x + 2*x + 3
        double[] X = { 1, 2, 3, 8 };
        double[] Y = { 4, 3, 0, -45 };

        // f(x) = A*x*x + B*x + C
        GaussNewton.F f = delegate (double[] coefficients, double x)
        {
            return coefficients[0] * x * x + coefficients[1] * x + coefficients[2];
        };

        GaussNewton gaussNewton = new GaussNewton(3);
        gaussNewton.Initialize(Y, X, f);
        double[] answer = gaussNewton.Coefficients;     //A=-1 B=2 C=3
    }
}
