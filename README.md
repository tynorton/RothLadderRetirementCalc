# Roth Ladder Retirement Calculator (.NET 7.0)

# Overview 
This is a simple command line calculator to calculate Roth Ladder Conversions.

You input a series of balances (Traditional, Roth, Brokerage, and Crypto) into `appsetting.config` and the app calculates balances over time.

This differs from normal online calculators in that it is intended to help plan for early-retirement. 
To achieve this, we use a technique known as a "**Roth Ladder Conversion**".

### Important assumptions (currently facts as of 2023)
* Roth yearly **conversion** maximum is unlimited. Not to be confused with the **contribution** maximum of $7,500/y (2023)
  * For large conversions to work, you must have already amassed a large "Traditional" balance. i.e. Traditional 401K, IRA, 403b. to convert into Roth.
* Roth distributions do not count as income (So you many take advantage of cheap healthcare via ACA subsidies)
* To achieve 12% tax rate, your income (normal income, and ladder conversions) must be meet the following criteria, or be subjexct to 22% rates.
  * Single: $44,725 (Default)
  * Married filing jointly: $89,450
* Roth **contributions** may be withdrawn at any point after 5 years without penalty. Gains may not be withdrawn until 59 <sup>1/2</sup>

Take the number of years from now until a traditional retirement (59 <sup>1/2</sup> +) and divide it in two. If you convert some amount from Traditional => Roth for the first half, and take the distributions for those same amounts 5 years later, you will be able to tap retirement funds before you are 59 <sup>1/2</sup>, while keeping healthcare costs minimized.

This calculator takes the above assumptions into account, and tries to simulate a "complete" scenario using the "5 years until retirement" roth ladder strategy.

#### Variables
* Age: 39 (Default)
* Early-Retire: 45 (Default)
* Traditional Retire: 67 (Default)

#### Balances
* Traditional: $1,000,000 (Default)
* Roth: $0 (Default)

### Screenshot
<img width="1419" alt="image" src="https://github.com/tynorton/RothLadderRetirementCalc/assets/811086/012ab8ce-77cc-41ab-b4a8-6f4141098751">

