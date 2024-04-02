// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SilkPlatforms;

namespace SilkExamples.Examples
{
    public class PaymentCalculatorExample : IExample
    {
        public string Description => "Payment Calculator Example";

        public string SourceCode => """
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
            """;

        public SilkPlatform Platform { get; init; } = SilkPlatform.Console;
    }
}
