// See https://aka.ms/new-console-template for more information
internal class YearlyBalance
{
    public int Year;
    public int Age;
    public List<string> Actions = new List<string>();
    public double CryptoBalance;
    public double BrokerageBalance;
    public double TradBalance;
    public double RothBalance;

    public double TotalBalance => (RothBalance + TradBalance + BrokerageBalance + CryptoBalance);

    public override string ToString()
    {
        return $"{Year,4} [{Age,3} years old] (T) {TradBalance,14:C0} (R) {RothBalance,14:C0} (B) {BrokerageBalance,14:C0} (C) {CryptoBalance,14:C0} | Total {TotalBalance,14:C0}  || | ||  {String.Join(" | ", Actions)}";
    }
}