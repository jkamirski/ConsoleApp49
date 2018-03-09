using System;


namespace Cfinnance
{
    /*class Maina
    {
        static void Main()
        { }


    }*/

    public interface IOptionFactory
    { // An interface consists of abstract methods
        Option create();
    }
    public class ConsoleEuropeanOptionFactory : IOptionFactory
    {
        public Option create()
        {
            Console.Write("\n***; Data for option object ***\n");
            double r; // Interest rate
            double sig; // Volatility
            double K; // Strike price
            double T; // Expiry date

            double b; // Cost of carry
            string type; // Option name (call, put)
            Console.Write("Strike: ");
            K = Convert.ToDouble(Console.ReadLine());
            Console.Write("Volatility: ");
            sig = Convert.ToDouble(Console.ReadLine());
            Console.Write("Interest rate: ");
            r = Convert.ToDouble(Console.ReadLine());
            Console.Write("Cost of carry: ");
            b = Convert.ToDouble(Console.ReadLine());
            Console.Write("Expiry date: ");
            T = Convert.ToDouble(Console.ReadLine());
            Console.Write("1. Call, 2. Put: ");
            type = Convert.ToString(Console.ReadLine());
            Option opt = new Option(type, T, K, b, r, sig);
            return opt;
        }
    }

    public class Option
    {
        private double r; // Interest rate
        private double sig; // Volatility
        private double K; // Strike price
        private double T; // Expiry date
        private double b; // Cost of carry
        private string type; // Option name (call, put)
                             // Kernel Functions (Haug)
        private double CallPrice(double U)
        {
            double tmp = sig * Math.Sqrt(T);
            double d1 = (Math.Log(U / K) + (b + (sig * sig) * 0.5) * T) / tmp;
            double d2 = d1 - tmp;
            return (U * Math.Exp((b - r) * T) * SpecialFunctions.N(d1))
            - (K * Math.Exp(-r * T) * SpecialFunctions.N(d2));
        }

        private double PutPrice(double U)
        {
            double tmp = sig * Math.Sqrt(T);
            double d1 = (Math.Log(U / K) + (b + (sig * sig) * 0.5) * T) / tmp;
            double d2 = d1 - tmp;
            return (K * Math.Exp(-r * T) * SpecialFunctions.N(-d2))
            - (U * Math.Exp((b - r) * T) * SpecialFunctions.N(-d1));
        }
        public void init()
        { // Initialize all default values

            r = 0.08;
            sig = 0.30;
            K = 65.0;
            T = 0.25;
            b = r;
            type = "C";
        }
        public Option()
        { // Default call option
            init();
        }
        public Option(string optionType)
        { // Create option instance of given type and default values
            init();
            type = optionType;
            // Finger trouble option
            if (type == "c")
                type = "C";
        }
        public Option(string optionType, double expiry, double strike,
        double costOfCarry, double interest, double volatility)
        { // Create option instance
            type = optionType;
            T = expiry;
            K = strike;
            b = costOfCarry;
            r = interest;
            sig = volatility;
        }
        public Option(string optionType, string underlying)
        { // Create option type
            init();
            type = optionType;
        }
        // Functions that calculate option price and sensitivities
        public double Price(double U)
        {
            if (type == "1")
            {
                return CallPrice(U);
            }
            else
                return PutPrice(U);
        }
    }
    public class SpecialFunctions
    {
        //////////// Gaussian functions /////////////////////////////////
        static public double n(double x)
        {
            double A = 1.0 / Math.Sqrt(2.0 * 3.1415);
            return A * Math.Exp(-x * x * 0.5); // Math class in C#
        }
        static public double N(double x)
        { // The approximation to the cumulative normal distribution
            double a1 = 0.4361836;
            double a2 = -0.1201676;
            double a3 = 0.9372980;
            double k = 1.0 / (1.0 + (0.33267 * x));
            if (x >= 0.0)
            {
                return 1.0 - n(x) * (a1 * k + (a2 * k * k) + (a3 * k * k * k));
            }
            else
            {
                return 1.0 - N(-x);
            }
        }
    }
    public struct Mediator
    { // The class that directs the program flow, from data initialisation,
      // computation and presentation
        static IOptionFactory getFactory()
        {
            return new ConsoleEuropeanOptionFactory();
        }
        public void calculate()
        {
            // 1. Choose how the data in the option will be created
            IOptionFactory fac = getFactory();
            // 2. Create the option
            Option myOption = fac.create();
            // 3. Get the price
            Console.Write("Give the underlying price: ");
            double S = Convert.ToDouble(Console.ReadLine());
            // 4. Display the result
            Console.WriteLine("Price: {0}", myOption.Price(S));
            Console.ReadKey();
        }
    }
    class TestOption
    {
        static void Main()
        { // All options are European
          // Major client delegates to the mediator (aka sub-contractor)
            Mediator med = new Mediator();
            med.calculate();
        }
    }
}