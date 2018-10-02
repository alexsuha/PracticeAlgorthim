using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedData
{
    class BankAccount
    {
        public int Balance
        {
            get;
            set;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Listing_01.NewTaskStart();
            //Listing_01.CreateSimpleTask();
            //Listing_01.CreateSimpleTaskWithParams();
            //Listing_01.ReturnValueOfTask();
            //Listing_01.CancelTaskByPolling();
            //Listing_01.CancelTaskWithDelegate();
            //Listing_01.CancelTaskWithWaitHandle();
            //Listing_01.CancelMultiTask();
            //Listing_01.CreateCompositeCancelToken();
            //Listing_01.DetermineTaskIsCancelled();
            //Listing_01.CancelTaskWithWaitHandleTimeSpan();
            //Listing_01.UsingClassicSleep();
            //Listing_01.WaitingForSingleTask();
            //Listing_01.WaitingForOneOfManyTasks();
            //Listing_01.HandleExceptionInTask();
            //Listing_01.ReadTaskProperties();
            //Listing_01.UsingEscalationPolicy();
            //Listing_01.LazyTaskExecution();
            Listing_01.TaskDependencyDeadlock();

            //// Create the bank account instance
            //BankAccount account = new BankAccount();

            //// create an array of tasks
            //Task<int>[] tasks = new Task<int>[10];

            //// create the thread local storage
            //ThreadLocal<int> tls = new ThreadLocal<int>(() => {
            //    Console.WriteLine("Value factory called for value: {0}", account.Balance);
            //    return account.Balance;
            //});

            //for (int i = 0; i < 10; i++)
            //{
            //    // create a new task
            //    tasks[i] = new Task<int>(() =>
            //    {
            //        // enter a loop for 1000 balance updates
            //        for (int j = 0; j < 1000; j++)
            //        {
            //            // update the TLS balance
            //            tls.Value++;
            //            //Console.WriteLine("tls value : {0}", tls.Value);
            //        }

            //        // return the updated balance
            //        return tls.Value;
            //    });

            //    // start the new task
            //    tasks[i].Start();
            //}

            //// get the result from each task and add it to the balance
            //for (int i = 0; i < 10; i++)
            //{
            //    account.Balance += tasks[i].Result;
            //}

            //// write out the counter value
            //Console.WriteLine("Expected value {0}, Balance: {1}", 10000, account.Balance);

            //// wait for input before exiting
            //Console.WriteLine("Press enter to finish");
            //Console.ReadLine();
        }
    }
}
