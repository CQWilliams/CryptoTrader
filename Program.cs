using System;
using System.Net;//Added for WebClient
using Newtonsoft.Json.Linq;//Added NuGet package, for JObject

/*
This program is a simple mock-up of a cryptocurrency trading application. 

    */

namespace CryptoTrader
{
    class Program
    {
        #region Global Variables
        //Here we give the user default values for their account, future plans would include making this dynamic in some way
        static decimal accBal = 5000.00m;
        static decimal ownedBitcoin = 0;
        static decimal ownedEtherium = 0;
        static decimal ownedLitecoin = 0;

        //The commented values were for testing purposes
        static decimal bitcoinPrice;// = 3360.04m;
        static decimal etheriumPrice;// = 88.60m;
        static decimal litecoinPrice;// = 36.73m;
        #endregion

        static void Main(string[] args)
        {
            #region Crypto API
            //In this region we retreive data from a web API that tracks the current price of the three cryptocurrency our program references.
            //This only occurs on startup, could probably move this into a method to be called throughout the program to keep the prices current.
            var client = new WebClient();

            //Taking the information returned by the API as a string
            string fullResponse = client.DownloadString("https://min-api.cryptocompare.com/data/pricemulti?fsyms=BTC,ETH,LTC&tsyms=USD,EUR&api_key=190265afb032f6fd2987082c24beb6a38d0cf9912c457eb65818a435f65dae8f");
            
            //Converting that string to a json object to make it easier to make use of in the program
            var json = JObject.Parse(fullResponse);

            //Here we grab the values that we care about from the data returned to us by the API, we are using USD but could configure the request to return 
            //many currencies from around the world should we decide to expand this functionality in the future.
            bitcoinPrice = Convert.ToDecimal(json["BTC"]["USD"]);
            etheriumPrice = Convert.ToDecimal(json["ETH"]["USD"]);
            litecoinPrice = Convert.ToDecimal(json["LTC"]["USD"]);

            client.Dispose(); //Cleaning up the WebClient when it is no longer needed
            #endregion

            //Our running variable to keep the application running until the users wish to exit
            bool exit = false;


            do //This is a loop for our main menu
            {
                Console.WriteLine("~/--MAIN MENU--\\~\n (P)urchase\n (S)ell\n (C)heck Balances\n (E)xit");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.P:
                        PurchaseMenu();
                        break;
                    case ConsoleKey.S:
                        //When choosing Sell we check to see if the user possesses any crypto to be sold, if not they are given a message stating that they 
                        //have nothing to sell at this time
                        if (ownedBitcoin > 0 || ownedEtherium > 0 || ownedLitecoin > 0)
                        {
                            SellMenu();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("You do not currently have any cryptocurrency to sell...");
                        }
                        break;
                    case ConsoleKey.C:
                        //Places their current balances above the main menu for easy viewing
                        Console.Clear();
                        Console.WriteLine("Current Balance: {0:c}\nBitcoins owned: {1}\nEtherium owned: {2}\nLitecoin owned: {3}\n\n",
                            accBal, ownedBitcoin, ownedEtherium, ownedLitecoin);
                        break;

                    case ConsoleKey.E:
                        //Flips our menu variable allowing users to return the main menu
                        exit = true;
                        break;

                    default:
                        //On invalid key press the menu refreshes with an error message
                        Console.Clear();
                        Console.WriteLine("Invalid key press, please try again...");
                        break;
                }

            } while (!exit);//End main menu
        }//end Main()

        static void PurchaseMenu()
        {
            /*
            In this method we tie together all functionality related to purchasing. It first displays a menu prompting the user to decide what
            they wish to purchase, upon selecting a cryptocurrency they are asked which of their balances they would like to spend. 
            Choosing which balance to use allows users to trade their cryptocurrencies for one another based upon their current USD value. 
            The menu presented will refresh itself until the user decides to return to the main menu, or close the program.
            */

            bool exit = false;//This bool keeps users in the purchase menu until the opt to return to the main menu
            Console.Clear();//Clearing screen to improve readability

            do//Begin purchase menu
            {
                //Prompts the user to choose a currency, also provides an updateable price in USD for each currency
                Console.WriteLine($"What would you like to buy?\n (B)itcoin  - {bitcoinPrice:c}\n (E)therium - {etheriumPrice:c}\n (L)itecoin - {litecoinPrice:c}\n (R)eturn to main menu\n");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.B:
                        PurchaseBitcoin();
                        break;
                    case ConsoleKey.E:
                        PurchaseEtherium();
                        break;
                    case ConsoleKey.L:
                        PurchaseLitecoin();
                        break;
                    case ConsoleKey.R:
                        //On choosing return, flips the variable that refreshes the menu
                        exit = true;
                        break;
                    default:
                        //On invalid key press the menu refreshes with an error message
                        Console.Clear();
                        Console.WriteLine("Invalid key press, please try again...");
                        break;
                }
            } while (!exit);//end purchase menu
            Console.Clear();//Clearing the screen for readability
        }//end PurchaseMenu()

        static void SellMenu()
        {
            /*
            In this method we handle all functonality realted to a user "cashing out" or selling their cryptocurrency for USD. The users are given
            all relevant information in the menu, and are prevented from selling more currency than they currently own.
            */

            bool exit = false;
            do //begin sell menu
            {
                Console.Clear();//Clearing screen for readability

                //This menu gives users their current balances for each currency and the current price in USD for each cryptocurrency
                Console.WriteLine($"Current Account Balance: {accBal:c}" +
                    $"\nWhat would you like to sell?\n (B)itcoin  - {bitcoinPrice:c} | Currently Own: {ownedBitcoin}" +
                    $"\n (E)therium - {etheriumPrice:c}    | Currently Own: {ownedEtherium}" +
                    $"\n (L)itecoin - {litecoinPrice:c}    | Currently Own: {ownedLitecoin}" +
                    "\n (R)eturn to main menu");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.B:
                        SellBitcoin();
                        break;
                    case ConsoleKey.E:
                        SellEtherium();
                        break;
                    case ConsoleKey.L:
                        SellLitecoin();
                        break;
                    case ConsoleKey.R:
                        //Flips our menu variable allowing users to return the main menu
                        exit = true;
                        break;
                    default:
                        //On invalid key press the menu refreshes with an error message
                        Console.Clear();
                        Console.WriteLine("Invalid key press, please try again...");
                        break;
                }
            } while (!exit);//end sell menu
            Console.Clear();//Clearing screen for readability
        }//end SellMenu()

        /* Old Trade Menu
        This was our original code enabling users to trade currencies for one another, this has been implemented in the purchasing options instead.
        Should we decide to create a seperate trade menu once more, we would likely take a more dynamic approach as the below code would require us to write a
        a new menu option/switch case for each new trade we wished to implement
        
        static void TradeMenu()
        {
            Console.Clear();
            bool exit = false;
            do
            {
                Console.WriteLine("What would you like to trade?");
                Console.WriteLine("1) Bitcoin for Etherium");
                Console.WriteLine("2) Bitcoin for Litecoin");
                Console.WriteLine("3) Etherium for Litecoin");
                Console.WriteLine("4) Etherium for Bitcoin");
                Console.WriteLine("5) Litecoin for Bitcoin");
                Console.WriteLine("6) Litecoing for Etherium");
                Console.WriteLine("(R)eturn to main menu");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.WriteLine("1");
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        Console.WriteLine("2");
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Console.WriteLine("3");
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        Console.WriteLine("4");
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        Console.WriteLine("5");
                        break;

                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        Console.WriteLine("6");
                        break;

                    case ConsoleKey.R:
                        exit = true;
                        break;

                    default:
                        break;
                }
            } while (!exit);
            Console.Clear();
        }*/

        static void PurchaseBitcoin()
        {

            decimal userInput = 0; //Initializing our variable to take user input

            //Displays the user's current balances that can be spent to purchase bitcoin
            Console.WriteLine("What would you like to spend to purchase bitcoin?"
                + $"\n (U)SD      - {accBal:c}"
                + $"\n (E)therium - {ownedEtherium:n2}"
                + $"\n (L)itecoin - {ownedLitecoin:n2}"
                + "\n (R)eturn to puchase menu\n");


            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.U:
                    #region Purchase Bitcoin w/ USD
                    //In this region we once again display the user's balance (for USD specifically) and take input for how many
                    //bitcoins they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu. 
                    Console.Write($"You currently have {accBal:c}. " + "How many bitcoin would you like to buy with USD? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (accBal - (bitcoinPrice * userInput) > 0)
                    {
                        accBal -= bitcoinPrice * userInput; //Subtracts the cost of the purchase from user's balance
                        ownedBitcoin += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} bitcoins purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion
                    break;

                case ConsoleKey.E:
                    #region Purchase Bitcoin w/ Etherium
                    //In this region we once again display the user's balance (for Etherium specifically) and take input for how many
                    //bitcoins they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu. 
                    Console.Write($"You currently have {ownedEtherium:n2} Etherium. " + "How many bitcoin would you like to buy with Etherium? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (ownedEtherium - ((bitcoinPrice * userInput) / etheriumPrice) > 0)
                    {
                        ownedEtherium -= (bitcoinPrice * userInput) / etheriumPrice; //Subtracts the cost of the purchase from user's balance
                        ownedBitcoin += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} bitcoins purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion
                    break;

                case ConsoleKey.L:
                    #region Purchase Bitcoin w/ Litecoin
                    //In this region we once again display the user's balance (for Litecoin specifically) and take input for how many
                    //bitcoins they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu. 
                    Console.Write("How many bitcoin would you like to buy? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (ownedLitecoin - ((bitcoinPrice * userInput) / litecoinPrice) > 0)
                    {
                        ownedLitecoin -= (bitcoinPrice * userInput) / litecoinPrice; //Subtracts the cost of the purchase from user's balance
                        ownedBitcoin += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} bitcoins purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion
                    break;

                case ConsoleKey.R:
                    //This case is simply to escape the menu, it does not repeat so we allow the user to fall through.
                    //In this case they fall back to the purchase menu
                    break;

                default:
                    //Error message and prompt if the users enter an invalid key during the purchase
                    Console.WriteLine("Invalid key pressed, press any button to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }

        }
        static void PurchaseEtherium()
        {
            decimal userInput; //Initializing our variable to take user input

            //Displays the user's current balances that can be spent to purchase etherium
            Console.WriteLine("What would you like to spend to purchase Etherium?"
                + $"\n (U)SD      - {accBal:c}"
                + $"\n (B)itcoin  - {ownedBitcoin:n2}"
                + $"\n (L)itecoin - {ownedLitecoin:n2}"
                + $"\n (R)eturn to puchase menu\n");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.U:
                    #region Purcahse Etherium w/ USD
                    //In this region we once again display the user's balance (for USD specifically) and take input for how many
                    //etherium they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu. 
                    Console.Write($"You currently have {accBal:c}. " + "How many etherium would you like to buy? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (accBal - (etheriumPrice * userInput) > 0)
                    {
                        accBal -= etheriumPrice * userInput; //Subtracts the cost of the purchase from user's balance
                        ownedEtherium += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} etherium purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion
                    break;

                case ConsoleKey.B:
                    #region Purcahse Etherium w/ Bitcoin
                    //In this region we once again display the user's balance (for Bitcoin specifically) and take input for how many
                    //etherium they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu. 
                    Console.Write($"You currently have {ownedBitcoin:n2} bitcoins. " + "How many etherium would you like to buy? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (ownedBitcoin - ((etheriumPrice * userInput) / bitcoinPrice) > 0)
                    {
                        ownedBitcoin -= (etheriumPrice * userInput) / bitcoinPrice; //Subtracts the cost of the purchase from user's balance
                        ownedEtherium += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} etherium purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion
                    break;

                case ConsoleKey.L:
                    #region Purcahse Etherium w/ Litecoin
                    //In this region we once again display the user's balance (for Litecoin specifically) and take input for how many
                    //etherium they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu.
                    Console.Write($"You currently have {ownedLitecoin:n2} litecoins. " + "How many etherium would you like to buy? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (ownedLitecoin - ((etheriumPrice * userInput) / litecoinPrice) > 0)
                    {
                        ownedLitecoin -= (etheriumPrice * userInput) / litecoinPrice; //Subtracts the cost of the purchase from user's balance
                        ownedEtherium += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} etherium purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion
                    break;

                case ConsoleKey.R:
                    //This case is simply to escape the menu, it does not repeat so we allow the user to fall through.
                    //In this case they fall back to the purchase menu
                    break;
                default:
                    //Error message and prompt if the users enter an invalid key during the purchase
                    Console.WriteLine("Invalid key pressed, press any button to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }
        }
        static void PurchaseLitecoin()
        {
            decimal userInput; //Initializing our variable to take user input

            //Displays the user's current balances that can be spent to purchase etherium
            Console.WriteLine("What would you like to spend to purchase Litecoin?"
                + $"\n (U)SD      - {accBal:c}"
                + $"\n (B)itcoin  - {ownedBitcoin:n2}"
                + $"\n (E)therium - {ownedEtherium:n2}"
                + $"\n (R)eturn to puchase menu\n");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.U:
                    #region Purchase Litecoin w/ USD
                    //In this region we once again display the user's balance (for USD specifically) and take input for how many
                    //litecoins they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu.
                    Console.Write($"You currently have {accBal:c}. " + "How many litecoins would you like to buy? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (accBal - (litecoinPrice * userInput) > 0)
                    {
                        accBal -= litecoinPrice * userInput; //Subtracts the cost of the purchase from user's balance
                        ownedLitecoin += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} litecoins purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion

                    break;

                case ConsoleKey.B:
                    #region Purchase Litecoin w/ Bitcoin
                    //In this region we once again display the user's balance (for Bitcoin specifically) and take input for how many
                    //litecoins they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu.
                    Console.Write($"You currently have {ownedBitcoin:n2} bitcoins. " + "How many litecoins would you like to buy? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (ownedBitcoin - ((litecoinPrice * userInput) / bitcoinPrice) > 0)
                    {
                        ownedBitcoin -= (litecoinPrice * userInput) / bitcoinPrice; //Subtracts the cost of the purchase from user's balance
                        ownedLitecoin += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} litecoins purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion

                    break;

                case ConsoleKey.E:
                    #region Purchase Litecoin w/ Etherium
                    //In this region we once again display the user's balance (for Etherium specifically) and take input for how many
                    //litecoins they would like to purchase. After recieving the input we check to see if they can afford the purchase/
                    //trade they are attempting to make. If the trade would set their balance below 0 they are informed they cannot 
                    //afford the purchase and returned the the purchasing menu.
                    Console.Write($"You currently have {ownedEtherium:n2} bitcoins. " + "How many litecoins would you like to buy? ");
                    userInput = Convert.ToDecimal(Console.ReadLine());

                    //Checking if the user can afford the purchase/trade
                    if (ownedEtherium - ((litecoinPrice * userInput) / etheriumPrice) > 0)
                    {
                        ownedEtherium -= (litecoinPrice * userInput) / etheriumPrice; //Subtracts the cost of the purchase from user's balance
                        ownedLitecoin += userInput; //Increases the balance of purchased cryptocurrency
                        Console.WriteLine($"{userInput} litecoins purchased. Press any key to continue..."); //Confirmation message
                    }
                    else
                    {
                        //Error message if user attempts a purchase/trade they cannot afford
                        Console.WriteLine("You cannot afford this purchase. Press any key to continue...");
                    }
                    Console.ReadKey(); //Delaying the program so users can see the error/confirmation
                    #endregion

                    break;

                case ConsoleKey.R:
                    //This case is simply to escape the menu, it does not repeat so we allow the user to fall through.
                    //In this case they fall back to the purchase menu
                    break;

                default:
                    //Error message and prompt if the users enter an invalid key during the purchase
                    Console.WriteLine("Invalid key pressed, press any button to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }
        }

        static void SellBitcoin()
        {
            //After the user has chosen a currency we ask for the amount they'd like to sell
            Console.Write($"How many bitcoins would you like to sell? ");
            decimal userInput = Convert.ToDecimal(Console.ReadLine());

            //Here we check if they are attempting to sell more of the currency than they currently own
            if ((ownedBitcoin - userInput) >= 0)
            {
                accBal += bitcoinPrice * userInput; //Updating their USD balance
                ownedBitcoin -= userInput; //Subtracting the currency they've sold
                Console.WriteLine($"{userInput} bitcoins sold. Press any key to continue..."); //Confirmation message
            }
            else
            {
                //Error message and prompt if they've attempted to sell more than they currently posses
                Console.WriteLine("You do not have that many bitcoins to sell. Press any key to continue...");
            }
            Console.ReadKey(); //Awaits key press to improve user experience
        }
        static void SellEtherium()
        {
            //After the user has chosen a currency we ask for the amount they'd like to sell
            Console.Write($"How many etherium would you like to sell? ");
            decimal userInput = Convert.ToDecimal(Console.ReadLine());

            //Here we check if they are attempting to sell more of the currency than they currently own
            if ((ownedEtherium - userInput) >= 0)
            {
                accBal += etheriumPrice * userInput; //Updating their USD balance
                ownedEtherium -= userInput; //Subtracting the currency they've sold
                Console.WriteLine($"{userInput} etherium sold. Press any key to continue..."); //Confirmation message
            }
            else
            {
                //Error message and prompt if they've attempted to sell more than they currently posses
                Console.WriteLine("You do not have that many etherium to sell. Press any key to continue...");
            }
            Console.ReadKey(); //Awaits key press to improve user experience
        }
        static void SellLitecoin()
        {
            //After the user has chosen a currency we ask for the amount they'd like to sell
            Console.Write($"How many litecoins would you like to sell? ");
            decimal userInput = Convert.ToDecimal(Console.ReadLine());

            //Here we check if they are attempting to sell more of the currency than they currently own
            if ((ownedLitecoin - userInput) >= 0)
            {
                accBal += litecoinPrice * userInput; //Updating their USD balance
                ownedLitecoin -= userInput; //Subtracting the currency they've sold
                Console.WriteLine($"{userInput} litecoins sold. Press any key to continue..."); //Confirmation message
            }
            else
            {
                //Error message and prompt if they've attempted to sell more than they currently posses
                Console.WriteLine("You do not have that many litecoins to sell. Press any key to continue...");
            }
            Console.ReadKey(); //Awaits key press to improve user experience
        }
    }
}
