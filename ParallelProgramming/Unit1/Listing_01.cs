using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharedData
{
    class Listing_01
    {
        public static void NewTaskStart()
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Hello World.");
            });

            // Wait for input before exiting.
            Console.WriteLine("Main method complete. Press enter to finish");
            Console.ReadLine();
        }

        // add a count to analyse the task execute order is random.
        public static void CreateSimpleTask()
        {
            // use an Action delegate and a named method
            Task task1 = new Task(new Action(printMessage));

            // use a anonymous delegate
            Task task2 = new Task(delegate
            {
                printMessage();
            });

            // use a lambda expression and a named method
            Task task3 = new Task(() => printMessage());

            // use a lambda expression and an anonymous method
            Task task4 = new Task(() =>
            {
                printMessage();
            });

            task1.Start();
            task2.Start();
            task3.Start();
            task4.Start();

            // Wait for input before exiting.
            Console.WriteLine("Main method complete. Press enter to finish");
            Console.ReadLine();
        }

        private static volatile int count = 0;

        static void printMessage()
        {
            count++;
            Console.WriteLine("Hello World" + count);
        }

        public static void CreateSimpleTaskWithParams()
        {
            // use an Action delegate and a named method
            Task task1 = new Task(new Action<object>(printMessage), "First Task");

            // use a anonymous delegate
            Task task2 = new Task(delegate (object obj)
            {
                printMessage(obj);
            }, "Second Task");

            // use a lambda expression and a named method
            Task task3 = new Task((obj) => printMessage(obj), "Third Task");

            // use a lambda expression and an anonymous method
            Task task4 = new Task((obj) =>
            {
                printMessage(obj);
            }, "Fourth Task");

            task1.Start();
            task2.Start();
            task3.Start();
            task4.Start();

            // Wait for input before exiting.
            Console.WriteLine("Main method complete. Press enter to finish");
            Console.ReadLine();
        }

        static void printMessage(object message)
        {
            Console.WriteLine("Message: {0}", message);
        }

        public static void ReturnValueOfTask()
        {
            Task<int> task1 = new Task<int>(() =>
            {
                int sum = 0;
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                }
                return sum;
            });

            // Start the task
            task1.Start();

            // write out the result
            Console.WriteLine("Result 1:{0}", task1.Result);

            // create the task using state
            Task<int> task2 = new Task<int>(obj =>
            {
                int sum = 0;
                int max = (int)obj;
                for (int i = 0; i < max; i++)
                {
                    sum += i;
                }
                return sum;
            }, 100);

            // Start the task
            task2.Start();

            // write out the result
            Console.WriteLine("Result 2: {0}", task2.Result);

            // wait for input before exiting
            Console.WriteLine("main method complete. Press enter to finish.");
            Console.ReadLine();
        }

        public static void StartTaskWithFactory()
        {
            Task<int> task = Task.Factory.StartNew<int>(() =>
            {
                return 1000;
            });

            Console.WriteLine("Result : {0}", task.Result);
            // wait for input before exiting
            Console.WriteLine("main method complete. Press enter to finish.");
            Console.ReadLine();
        }

        static void StartPrograme()
        {
            // wait for input before we start the task
            Console.WriteLine("Press enter to start task");
            Console.WriteLine("Press enter again to cancel task");
            Console.ReadLine();
        }

        static void EndOfPrograme()
        {
            // wait for input before exiting
            Console.WriteLine("main method complete. Press enter to finish.");
            Console.ReadLine();
        }

        public static void CancelTaskByPolling()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken token = tokenSource.Token;

            Task task = new Task(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Task cancel detected.");
                        throw new OperationCanceledException(token);
                    }
                    else
                    {
                        Console.WriteLine("Int value {0}", i);
                    }
                }
            }, token);

            // wait for input before we start the task
            Console.WriteLine("Press enter to start task");
            Console.WriteLine("Press enter again to cancel task");
            Console.ReadLine();

            task.Start();

            Console.ReadLine();

            Console.WriteLine("Cancelling task");
            tokenSource.Cancel();

            EndOfPrograme();
        }

        /**
         *  add a register function to solve the exception of cancel. (wrong)
         *  without debug no exception would be breaked.
         **/
        public static void CancelTaskWithDelegate()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken token = tokenSource.Token;

            Task task = new Task(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Task cancel detected.");
                        throw new OperationCanceledException(token);
                    }
                    else
                    {
                        Console.WriteLine("Int value {0}", i);
                    }
                }
            }, token);

            // register a cancellation delegate
            token.Register(() =>
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>> Delegate Invoked\n");
            });

            // wait for input before we start the task
            Console.WriteLine("Press enter to start task");
            Console.WriteLine("Press enter again to cancel task");
            Console.ReadLine();

            task.Start();

            Console.ReadLine();

            Console.WriteLine("Cancelling task");
            tokenSource.Cancel();

            EndOfPrograme();
        }

        public static void CancelTaskWithWaitHandle()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            Task task1 = new Task(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Task cancel detected.");
                        throw new OperationCanceledException(token);
                    }
                    else
                    {
                        Console.WriteLine("Int value {0}", i);
                    }
                }
            });

            Task task2 = new Task(() =>
            {
                bool check = token.WaitHandle.WaitOne();
                if (check)
                    Console.WriteLine(">>>>>>>>>>>>>>>> Wait handle released {0}", check);
                else
                    Console.WriteLine("<<<<<<<<<<<<<< common use. {0}", check);
            });

            StartPrograme();

            task1.Start();
            task2.Start();

            Console.ReadLine();

            Console.WriteLine("Cancel Task");
            tokenSource.Cancel();

            EndOfPrograme();
        }

        /**
         *  here you can't control the endofprogram execute after every task being canceled. 
         */
        public static void CancelMultiTask()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            Task task1 = new Task(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine("Task 1 - Int value {0}", i);
                }
            }, token);

            Task task2 = new Task(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine("Task 2 - Int value {0}", i);
                }
            });

            StartPrograme();

            task1.Start();
            task2.Start();

            Console.ReadLine();

            Console.WriteLine("Cancelling task...");
            tokenSource.Cancel();

            EndOfPrograme();
        }

        public static void CreateCompositeCancelToken()
        {
            CancellationTokenSource tokenSource1 = new CancellationTokenSource();
            CancellationTokenSource tokenSource2 = new CancellationTokenSource();
            CancellationTokenSource tokenSource3 = new CancellationTokenSource();

            CancellationToken token = tokenSource2.Token;
            token.Register(() =>
            {
                Console.WriteLine(">>>> token 2 invoke to cancel the task");
            });

            CancellationTokenSource compositeSource = CancellationTokenSource.CreateLinkedTokenSource(
                tokenSource1.Token, tokenSource2.Token, tokenSource3.Token);

            Task task = new Task(() =>
            {
                compositeSource.Token.WaitHandle.WaitOne();
                throw new OperationCanceledException(compositeSource.Token);
            }, compositeSource.Token);

            task.Start();

            tokenSource2.Cancel();

            EndOfPrograme();
        }

        public static void DetermineTaskIsCancelled()
        {
            CancellationTokenSource tokenSource1 = new CancellationTokenSource();
            CancellationTokenSource tokenSource2 = new CancellationTokenSource();

            CancellationToken token1 = tokenSource1.Token;
            CancellationToken token2 = tokenSource2.Token;

            // if task not add token using default, it would stop task2, but iscanceled will not be true.
            Task task1 = new Task(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    token1.ThrowIfCancellationRequested();
                    Console.WriteLine("Task 1 - Int value {0}", i);
                }
                //});
            }, token1);

            Task task2 = new Task(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    token2.ThrowIfCancellationRequested();
                    Console.WriteLine("Task 2 - Int value {0}", i);
                }
            }, token2);

            task1.Start();
            task2.Start();

            tokenSource1.Cancel();
            Console.WriteLine("Task  cancel check:");

            Console.WriteLine("Task 1 cancelled? {0}", task1.IsCanceled);
            Console.WriteLine("Task 2 cancelled? {0}", task2.IsCanceled);

            EndOfPrograme();
        }

        public static void CancelTaskWithWaitHandleTimeSpan()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            Task task = new Task(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    bool cancel = token.WaitHandle.WaitOne(2000);

                    Console.WriteLine("Task value - {0}. Cancelled? {1}", i, cancel);

                    if (cancel) 
                    {
                        Console.WriteLine("now it's the time to cancel.");
                        throw new OperationCanceledException(token);
                    }
                }
            }, token);

            task.Start();

            Console.ReadLine();  // without this, it would be termined immediately. not even into the queue of task.

            tokenSource.Cancel();

            EndOfPrograme();
        }

        public static void UsingClassicSleep()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            Task task = new Task(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    Thread.Sleep(2000);

                    Console.WriteLine("Task value - {0}. ", i);

                    token.ThrowIfCancellationRequested();
                }
            }, token);

            task.Start();

            Console.ReadLine();  // without this, it would be termined immediately. not even into the queue of task.

            tokenSource.Cancel();

            EndOfPrograme();
        }
    }
}
