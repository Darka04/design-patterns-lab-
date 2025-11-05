public interface IBeverage
{
    double GetCost();
    string GetDescription();
}
public class Coffee : IBeverage
{
    public double GetCost()
    {
        return 50.0;
    }
    public string GetDescription()
    {
        return "Coffee";
    }
}
public abstract class BeverageDecorator : IBeverage
{
    protected IBeverage _beverage;

    public BeverageDecorator(IBeverage beverage)
    {
        _beverage = beverage;
    }

    public virtual double GetCost()
    {
        return _beverage?.GetCost() ?? 0;
    }

    public virtual string GetDescription()
    {
        return _beverage?.GetDescription() ?? "Unknown Beverage";
    }
}
public class MilkDecorator : BeverageDecorator
{
    public MilkDecorator(IBeverage beverage) : base(beverage) { }

    public override double GetCost()
    {
        return base.GetCost() + 10.0;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + ", Milk";
    }
}

public class SugarDecorator : BeverageDecorator
{
    public SugarDecorator(IBeverage beverage) : base(beverage) { }

    public override double GetCost()
    {
        return base.GetCost() + 5.0;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + ", Sugar";
    }
}

public class ChocolateDecorator : BeverageDecorator
{
    public ChocolateDecorator(IBeverage beverage) : base(beverage) { }

    public override double GetCost()
    {
        return base.GetCost() + 15.0;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + ", Chocolate";
    }
}

public interface IPaymentProcessor
{
    void ProcessPayment(double amount);
    void RefundPayment(double amount);
}

public class InternalPaymentProcessor : IPaymentProcessor
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Обработка платежа на сумму {amount} Тг через ВНУТРЕННЮЮ систему.");
    }

    public void RefundPayment(double amount)
    {
        Console.WriteLine($"Возврат средств на сумму {amount} Тг через ВНУТРЕННЮЮ систему.");
    }
}

public class ExternalPaymentSystemA
{
    public void MakePayment(double amount)
    {
        Console.WriteLine($"Платеж на сумму {amount} Тг проведен через ВНЕШНЮЮ Систему A.");
    }

    public void MakeRefund(double amount)
    {
        Console.WriteLine($"Возврат средств на сумму {amount} Тг проведен через ВНЕШНЮЮ Систему A.");
    }
}

public class ExternalPaymentSystemB
{
    public void SendPayment(double amount)
    {
        Console.WriteLine($"Платеж на сумму {amount} Тг отправлен через ВНЕШНЮЮ Систему B.");
    }

    public void ProcessRefund(double amount)
    {
        Console.WriteLine($"Возврат средств на сумму {amount} Тг обработан через ВНЕШНЮЮ Систему B.");
    }
}

public class PaymentAdapterA : IPaymentProcessor
{
    private readonly ExternalPaymentSystemA _externalSystemA;

    public PaymentAdapterA(ExternalPaymentSystemA externalSystemA)
    {
        _externalSystemA = externalSystemA;
    }

    public void ProcessPayment(double amount)
    {
        _externalSystemA.MakePayment(amount);
    }

    public void RefundPayment(double amount)
    {
        _externalSystemA.MakeRefund(amount);
    }
}

public class PaymentAdapterB : IPaymentProcessor
{
    private readonly ExternalPaymentSystemB _externalSystemB;

    public PaymentAdapterB(ExternalPaymentSystemB externalSystemB)
    {
        _externalSystemB = externalSystemB;
    }

    public void ProcessPayment(double amount)
    {
        _externalSystemB.SendPayment(amount);
    }

    public void RefundPayment(double amount)
    {
        _externalSystemB.ProcessRefund(amount);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Паттерн ДЕКОРАТОР (Кафе) ===");

        IBeverage myBeverage = new Coffee();
        PrintBeverage(myBeverage);

        myBeverage = new MilkDecorator(myBeverage);
        PrintBeverage(myBeverage);

        myBeverage = new SugarDecorator(myBeverage);
        PrintBeverage(myBeverage);

        myBeverage = new ChocolateDecorator(myBeverage);
        PrintBeverage(myBeverage);

        Console.WriteLine("\n--- Другой заказ ---");
        IBeverage anotherBeverage = new ChocolateDecorator(new Coffee());
        PrintBeverage(anotherBeverage);

        IBeverage doubleChocolate = new ChocolateDecorator(new ChocolateDecorator(new Coffee()));
        PrintBeverage(doubleChocolate);

        Console.WriteLine("\nНажмите любую клавишу для перехода к паттерну АДАПТЕР...");
        Console.ReadKey();
        Console.Clear();

        Console.WriteLine("=== Паттерн АДАПТЕР (Платежные системы) ===");

        Console.WriteLine("\n--> Используем внутреннюю систему:");
        IPaymentProcessor processor = new InternalPaymentProcessor();
        processor.ProcessPayment(1000);
        processor.RefundPayment(200);

        Console.WriteLine("\n--> Используем Внешнюю Систему A через адаптер:");
        ExternalPaymentSystemA systemA = new ExternalPaymentSystemA();
        processor = new PaymentAdapterA(systemA);
        processor.ProcessPayment(1500);
        processor.RefundPayment(300);

        Console.WriteLine("\n--> Используем Внешнюю Систему B через адаптер:");
        ExternalPaymentSystemB systemB = new ExternalPaymentSystemB();
        processor = new PaymentAdapterB(systemB);
        processor.ProcessPayment(2000);
        processor.RefundPayment(500);

        Console.WriteLine("\n--> Логика выбора системы по валюте (USD -> System A, EUR -> System B):");

        string currency = "USD";
        double amount = 75.50;

        IPaymentProcessor selectedProcessor = GetProcessorForCurrency(currency);
        Console.Write($"Валюта {currency}: ");
        selectedProcessor.ProcessPayment(amount);

        currency = "EUR";
        amount = 65.30;
        selectedProcessor = GetProcessorForCurrency(currency);
        Console.Write($"Валюта {currency}: ");
        selectedProcessor.ProcessPayment(amount);

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static void PrintBeverage(IBeverage beverage)
    {
        Console.WriteLine($"{beverage.GetDescription()} | Cost: {beverage.GetCost()} Тг");
    }

    static IPaymentProcessor GetProcessorForCurrency(string currency)
    {
        switch (currency.ToUpper())
        {
            case "USD":
                return new PaymentAdapterA(new ExternalPaymentSystemA());
            case "EUR":
                return new PaymentAdapterB(new ExternalPaymentSystemB());
            default:
                return new InternalPaymentProcessor();
        }
    }
}