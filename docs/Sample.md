# Sample Source Code

This page provide some sample Silk scripts (source code).

### Payment Calculator Example

This example calculates the principle and interest payment amount on a loan. It assumes the host application defines the functions `Print()`, `PrintLine()`, `ReadLine()`, `ReadKey()`, `ClearScreen()` and `SetColor()`. It also assumed the host application has defined color variables for user with `SetColor()`.

```
///////////////////////////////////////////////////////////////
// Silk - Sample script
//

main()
{
    setcolor white, darkblue
    clearscreen

    print "Enter loan amount: "
    loanamount = readline()

    print "Enter number of years: "
    payments = readline()

    print "Enter the interest rate percent: "
    interestrate = readline()

    print "Payment (principle and interest) is: "
    print "$", round(calculatepayment(loanamount, payments * 12, interestrate / 100 / 12), 2)
    printline
}

calculatepayment(loanamount, payments, interestrate)
{
    if payments = 0
        return loanamount

    if interestrate = 0
        return loanamount / payments

    temp = pow(interestrate + 1.0, payments)

    return -(-(loanamount * temp) / ((temp - 1) / interestRate))
}
```

### Prime Numbers Example

This example displays all the prime numbers within the specified range. It assumes the host application defines the functions `Print()`, `PrintLine()`, `ReadLine()`, `ReadKey()`, `ClearScreen()` and `SetColor()`. It also assumed the host application has defined color variables for user with `SetColor()`.

```
///////////////////////////////////////////////////////////////
// Silk - Sample script
//

main()
{
    // Set colors and clear screen
    setcolor white, darkblue
    clearscreen

    // Get start number
    print "Enter starting number: "
    start = readline()
    if start = ""
        return

    // Get end number
    print "Enter ending number: "
    end = readline()
    if end = ""
        return

    for i = start to end
    {
        flag_var = 0
        for j = 2 to i / 2
        {
            if(i % j = 0)
            {
                flag_var = 1
                break
            }
        }
        if flag_var = 0
            printline i
    }
}
```

