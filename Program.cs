// Year & Rates
using System.Configuration;
using System.Numerics;

var currYear = DateTime.Now.Year;
var interestRate = 0.07;
var conversionTaxRate = 0.12;
var inflationRate = 0.04;

// Your current age
int currAge = 39;
if (!int.TryParse(ConfigurationManager.AppSettings["CurrentAge"], out currAge))
{
    throw new InvalidOperationException("Invalid CurrentAge in app.config");
}

var earlyRetirementAge = 45;
if (!int.TryParse(ConfigurationManager.AppSettings["EarlyRetirementAge"], out earlyRetirementAge))
{
    throw new InvalidOperationException("Invalid EarlyRetirementAge in app.config");
}

var retirementAge = 65;
if (!int.TryParse(ConfigurationManager.AppSettings["TraditionalRetirementAge"], out retirementAge))
{
    throw new InvalidOperationException("Invalid TraditionalRetirementAge in app.config");
}

// Set your current balances
double brokerageBalance = 0;
if (!double.TryParse(ConfigurationManager.AppSettings["BrokerageBalance"], out brokerageBalance))
{
    throw new InvalidOperationException("Invalid BrokerageBalance in app.config");
}

double cryptoBalance = 0;
if (!double.TryParse(ConfigurationManager.AppSettings["CryptoBalance"], out cryptoBalance))
{
    throw new InvalidOperationException("Invalid CryptoBalance in app.config");
}

double tradBalance = 1000000;
if (!double.TryParse(ConfigurationManager.AppSettings["TraditionalBalance"], out tradBalance))
{
    throw new InvalidOperationException("Invalid TraditionalBalance in app.config");
}

double rothBalance = 0;
if (!double.TryParse(ConfigurationManager.AppSettings["RothBalance"], out rothBalance))
{
    throw new InvalidOperationException("Invalid RothBalance in app.config");
}


// Ladder needs 5 years to ramp, then you can start taking Roth distributions.
// If you want to  early retire @ 45, you must start converting Trad=>Roth for retirement @ 40
var rothLadderStartAge = earlyRetirementAge - 5;
var rothLadderStartDistributionAge = earlyRetirementAge;
var rothLadderEndAge = 50;

if (earlyRetirementAge <= currAge || earlyRetirementAge <= (currAge + 5))
{
    Console.WriteLine("Retirement age must be at least 5 years in the future to use the ladder calculator.");
}

// Controls how much money (in today's dollars) you want to distribute each year
double rothLadderStartingDistributionAmt = 55000 * (1 + conversionTaxRate);

// How much to distribute in retirement?
double retirementRothDistribution = 25000;
double retirementTradDistribution = 175000;

// Calculate Years
var startRothYear = currYear + (rothLadderStartAge - currAge);
var endRothYear = currYear + (rothLadderEndAge - currAge);
var distributionYear = currYear + (rothLadderStartDistributionAge - currAge);
var retirementYear = currYear + (retirementAge - currAge);

// Init
var yearlyBalances = new Dictionary<int, YearlyBalance>();
double rothLadderConversionAmt = rothLadderStartingDistributionAmt;
double rothDistributionAmt = 0;
double tradDistributionAmt = 0;
double previousTotal = 0;

// Lets not look past 100 years
var Centennial = DateTime.Now.Year + (100 - currAge);
for (var i = currYear; i < Centennial; i++)
{
    bool isTradDistributionYear = false;
    bool isRothDistributionYear = false;
    List<string> actions =  new List<string>();

    var tradInterestGrowth = tradBalance * interestRate;
    var rothInterestGrowth = rothBalance * interestRate;
    var brokerageInterestGrowth = brokerageBalance * interestRate;
    var cryptoInterestGrowth = cryptoBalance * 0.04; // Current ETH staked rate

    // Starting balances shouldn't get a "bonus" year of interest
    if (i != currYear)
    {
        tradBalance += tradInterestGrowth;
        rothBalance += rothInterestGrowth;
        brokerageBalance += brokerageInterestGrowth;
        cryptoBalance += cryptoInterestGrowth;
    }

    // Reduce conversion after we've converted more than 50% needed to get to 59 1/2 years old
    if (i > endRothYear && i < retirementYear)
    {
        rothLadderConversionAmt = rothLadderStartingDistributionAmt / 5;
    }

    // Conversion Years
    if (i >= startRothYear && i < retirementYear)
    {
        tradBalance -= rothLadderConversionAmt;
        rothBalance += rothLadderConversionAmt - (rothLadderConversionAmt * conversionTaxRate);

        // Bump up by expected inflation rate for next year
        rothLadderConversionAmt += (rothLadderConversionAmt * inflationRate);
    }

    // Distribution Years
    if (i >= distributionYear && i < retirementYear)
    {
        if (rothDistributionAmt == 0) rothDistributionAmt = rothLadderStartingDistributionAmt;

        rothDistributionAmt = rothDistributionAmt * (1 + inflationRate);

        isRothDistributionYear = true;
        rothBalance -= rothLadderStartingDistributionAmt;
    }

    if (i >= retirementYear)
    {
        if (tradDistributionAmt == 0) tradDistributionAmt = retirementTradDistribution;
        if (rothDistributionAmt == 0) rothDistributionAmt = retirementRothDistribution;

        tradDistributionAmt = (tradDistributionAmt * (1 + inflationRate));
        rothDistributionAmt = (rothDistributionAmt * (1 + inflationRate));

        isTradDistributionYear = true;
        tradBalance -= retirementTradDistribution;

        isRothDistributionYear = true;
        rothBalance -= retirementRothDistribution;

        rothLadderStartingDistributionAmt = retirementRothDistribution;
    }

    actions.Add($"Convert: (T=>R) {rothLadderConversionAmt,8:C0}");

    if (isRothDistributionYear)
    {
        actions.Add($"DIST (R) {(rothDistributionAmt * -1),14:C0}");
    }
    else
    {
        actions.Add($"DIST (R) {(0),14:C00}");
    }


    if (isTradDistributionYear)
    {
        actions.Add($"DIST (T) {(tradDistributionAmt * -1),14:C0}");
    }
    else
    {
        actions.Add($"DIST (T) {(0),14:C00}");
    }

    if (i >= retirementYear || isTradDistributionYear || isRothDistributionYear)
    {
        actions.Add($"Total {((rothDistributionAmt + tradDistributionAmt) * -1),14:C0}");
    }
    else
    {
        actions.Add($"Total {(0),14:C00}");
    }

    var yearBalance = new YearlyBalance
    {
        Year = i,
        Age = currAge++,
        Actions = actions,
        BrokerageBalance = brokerageBalance,
        CryptoBalance = cryptoBalance,
        RothBalance = rothBalance,
        TradBalance = tradBalance
    };

    yearlyBalances.Add(i, yearBalance);

    if (previousTotal == 0) previousTotal = yearBalance.TotalBalance;

    var changed = (yearBalance.TotalBalance - previousTotal) / yearBalance.TotalBalance;

    actions.Add($"Change {changed:P}");

    previousTotal = yearBalance.TotalBalance;
}

foreach (var kvp in yearlyBalances)
{
    Console.ForegroundColor = ConsoleColor.DarkGreen;

    if (kvp.Key >= startRothYear && kvp.Key < endRothYear)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
    } 
    else if (kvp.Key >= distributionYear && kvp.Key < retirementYear)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }

    // Negative is bad
    if (kvp.Value.TotalBalance <= 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }

    Console.WriteLine($"{kvp.Value}");
}

Console.WriteLine($"\n\t\t\t\t(T)raditional | (R)oth | (B)rokerage | (C)rypto");

Console.ForegroundColor = ConsoleColor.White;