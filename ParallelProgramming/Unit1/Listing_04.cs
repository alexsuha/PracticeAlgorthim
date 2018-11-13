using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SharedData
{
    class Deposit
    {
        public int Amount { get; set; }
    }

    class Listing_04
    {
        private static void EndofProgram()
        {
            // wait for input before existing
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public static void TaskContinuation()
        {
            Task<BankAccount> task = new Task<BankAccount>(() =>
            {
                BankAccount account = new BankAccount();

                for (int i = 0; i < 1000; i++)
                {
                    account.Balance++;
                }

                return account;
            });

            task.ContinueWith((Task<BankAccount> antecedent) =>
            {
                Console.WriteLine("Final Balance {0}", antecedent.Result.Balance);
            });

            task.Start();

            EndofProgram();
        }

        public static void SimpleChildTask()
        {
            Task parentTask = new Task(() =>
            {
                Task childTask = new Task(() =>
                {
                    Console.WriteLine("Child task running");
                    Task.Delay(1000);
                    Console.WriteLine("Child task finished");
                    throw new Exception();
                });

                Console.WriteLine("Starting child task...");
                childTask.Start();
            });

            parentTask.Start();
            Console.WriteLine("Waiting for parent task");
            parentTask.Wait();
            Console.WriteLine("Parent task finished");

            EndofProgram();
        }

        public static void AttachedChildTask()
        {
            Task parentTask = new Task(() =>
            {
                Task childTask = new Task(() => {
                    Console.WriteLine("Child 1 running");
                    Task.Delay(1000);
                    Console.WriteLine("Child 1 finished");
                    throw new Exception();
                }, TaskCreationOptions.AttachedToParent);

                childTask.ContinueWith(antecedent => {
                    Console.WriteLine("Continuation running");
                    Task.Delay(1000);
                    Console.WriteLine("Continuation finished");
                },
                TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);

                Console.WriteLine("Starting child task...");
                childTask.Start();
            });

            parentTask.Start();

            try
            {
                Console.WriteLine("Waiting for parent task");
                parentTask.Wait();
                Console.WriteLine("Parent task finished");
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Exception {0}", ex.InnerException.GetType());
            }

            EndofProgram();
        }

        public static void UsingBarrierClass()
        {
            BankAccount[] accounts = new BankAccount[5];
            for(int i = 0; i < accounts.Length; i++)
            {
                accounts[i] = new BankAccount();
            }

            int totalBalance = 0;

            Barrier barrier = new Barrier(5, (myBarrier) =>
            {
                totalBalance = 0;
                foreach(BankAccount account in accounts)
                {
                    totalBalance += account.Balance;
                }
                Console.WriteLine("Total balance {0}", totalBalance);
            });

            Task[] tasks = new Task[5];

            for(int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task((stateObj) =>
                {
                    BankAccount account = (BankAccount)stateObj;

                    Random rnd = new Random();
                    for(int j = 0; j < 1000; j++)
                    {
                        account.Balance += rnd.Next(1, 100);
                    }

                    Console.WriteLine("Task {0}, phase {1} ended", Task.CurrentId, barrier.CurrentPhaseNumber);

                    barrier.SignalAndWait();

                    account.Balance -= (totalBalance - account.Balance) / 10;

                    Console.WriteLine("Task {0}, phase {1} ended", Task.CurrentId, barrier.CurrentPhaseNumber);

                    barrier.SignalAndWait();
                }, accounts[i]);
            }

            foreach(Task t in tasks)
            {
                t.Start();
            }

            Task.WaitAll(tasks);

            EndofProgram();
        }

        public static void ReduceParticipation()
        {
            // only the count is equal removecount + 1 works fine
            Barrier barrier = new Barrier(3);

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Good task starting phase 0");
                barrier.SignalAndWait();
                Console.WriteLine("Good task starting phase 1");
                barrier.SignalAndWait();
                Console.WriteLine("Good task starting phase 2");
                barrier.SignalAndWait();
                Console.WriteLine("Good task completed");
            });

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Bad task 1 throwing exception");
                throw new Exception();
            }).ContinueWith(antecedent =>
            {
                Console.WriteLine("Reducing the barrier participant count");
                barrier.RemoveParticipant();
                barrier.RemoveParticipant();
            }, TaskContinuationOptions.OnlyOnFaulted);

            EndofProgram();
        }

        public static void UsingCancellationDealingWithExceptions()
        {
            // only 1 works, 2 original failed.
            Barrier barrier = new Barrier(1);
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Good task starting phase 0");
                barrier.SignalAndWait(tokenSource.Token);
                Console.WriteLine("Good task starting phase 1");
                barrier.SignalAndWait(tokenSource.Token);
                Console.WriteLine("Good task starting phase 2");
            }, tokenSource.Token);

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Bad task 1 throwing exception");
                throw new Exception();
            }, tokenSource.Token).ContinueWith(antecedent =>
            {
                Console.WriteLine("Cancelling the token");
                tokenSource.Cancel();
            }, TaskContinuationOptions.OnlyOnFaulted);

            EndofProgram();
        }

        public static void UsingCountDownEvent()
        {
            CountdownEvent cdevent = new CountdownEvent(5);

            Random rnd = new Random();

            Task[] tasks = new Task[6];
            for(int i = 0; i < 5; i++)
            {
                tasks[i] = new Task(() =>
                {
                    Thread.Sleep(rnd.Next(500, 1000));
                    Console.WriteLine("Task {0} signalling event", Task.CurrentId);
                    cdevent.Signal();
                });
            }

            tasks[5] = new Task(() =>
            {
                Console.WriteLine("Rendezvous task waiting");
                cdevent.Wait();
                Console.WriteLine("Event has been set");
            });

            foreach(Task t in tasks)
            {
                t.Start();
            }
            Task.WaitAll(tasks);

            EndofProgram();
        }

        public static void UsingManualResetEventSlim()
        {
            ManualResetEventSlim manualResetEvent = new ManualResetEventSlim();

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task waitingTask = Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    manualResetEvent.Wait(tokenSource.Token);
                    Console.WriteLine("Waiting task active");
                }
            }, tokenSource.Token);

            Task signallingTask = Task.Factory.StartNew(() =>
            {
                Random rnd = new Random();
                while(!tokenSource.Token.IsCancellationRequested)
                {
                    tokenSource.Token.WaitHandle.WaitOne(rnd.Next(500, 2000));
                    manualResetEvent.Set();
                    Console.WriteLine("Event set");
                    tokenSource.Token.WaitHandle.WaitOne(rnd.Next(500, 2000));
                    manualResetEvent.Reset();
                    Console.WriteLine("Event reset");
                }
                tokenSource.Token.ThrowIfCancellationRequested();
            }, tokenSource.Token);

            Console.WriteLine("Press enter to cancel tasks");
            Console.ReadLine();

            tokenSource.Cancel();
            try
            {
                Task.WaitAll(waitingTask, signallingTask);
            } catch (AggregateException)
            {
                // discard exception
            }

            EndofProgram();
        }

        public static void UsingAutoResetEvent()
        {
            AutoResetEvent arEvent = new AutoResetEvent(false);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            for (int i = 0; i < 3; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    while(!tokenSource.Token.IsCancellationRequested)
                    {
                        arEvent.WaitOne();
                        Console.WriteLine("Task {0} released", Task.CurrentId);
                    }
                    tokenSource.Token.ThrowIfCancellationRequested();
                }, tokenSource.Token);
            }

            Task signallingTask = Task.Factory.StartNew(() =>
            {
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    tokenSource.Token.WaitHandle.WaitOne(500);
                    arEvent.Set();
                    Console.WriteLine("Event Set");
                }
                tokenSource.Token.ThrowIfCancellationRequested();
            }, tokenSource.Token);

            Console.WriteLine("Press enter to cancel tasks");
            Console.ReadLine();

            tokenSource.Cancel();

            EndofProgram();
        }

        public static void UsingSemaphoreSlim()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(2);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            for(int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("Semaphore task running.");
                        semaphore.Wait(tokenSource.Token);
                        Console.WriteLine("Task {0} released", Task.CurrentId);
                    }
                }, tokenSource.Token);
            }

            Task signallingTask = Task.Factory.StartNew(() =>
            {
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    tokenSource.Token.WaitHandle.WaitOne(500);
                    semaphore.Release(2);
                    Console.WriteLine("Semaphore released");
                }
                tokenSource.Token.ThrowIfCancellationRequested();
            }, tokenSource.Token);

            Console.WriteLine("Press enter to cancel tasks");
            Console.ReadLine();

            tokenSource.Cancel();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public static void UsingParallelProducerConsumer()
        {
            BlockingCollection<Deposit> blockingCollection = new BlockingCollection<Deposit>();
            Task[] producers = new Task[3];
            for (int i = 0; i < 3; i++)
            {
                producers[i] = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("task {0} start", Task.CurrentId);
                    for (int j = 0; j < 20; j++)
                    {
                        Deposit deposit = new Deposit { Amount = 100 };
                        blockingCollection.Add(deposit);
                    }
                    Console.WriteLine("task {0} end", Task.CurrentId);
                });
            }

            Task.Factory.ContinueWhenAll(producers, antecedents =>
            {
                Console.WriteLine("Signalling production end");
                blockingCollection.CompleteAdding();
            });

            BankAccount account = new BankAccount();

            Task consumer = Task.Factory.StartNew(() =>
            {
                while (!blockingCollection.IsCompleted)
                {
                    Console.WriteLine("consumer running: {0}", Task.CurrentId);
                    Deposit deposit;
                    if (blockingCollection.TryTake(out deposit))
                    {
                        account.Balance += deposit.Amount;
                    }
                }
                Console.WriteLine("Final Balance: {0}", account.Balance);
            });

            consumer.Wait();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public static void UsingMultipleBlockingCollection()
        {
            BlockingCollection<string> bc1 = new BlockingCollection<string>();
            BlockingCollection<string> bc2 = new BlockingCollection<string>();

            BlockingCollection<string> bc3 = new BlockingCollection<string>();

            BlockingCollection<string>[] bc1andbc2 = { bc1, bc2 };
            BlockingCollection<string>[] bcAll = { bc1, bc2, bc3 };

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            for(int i = 0; i < 5; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    while(!tokenSource.IsCancellationRequested)
                    {
                        string message = String.Format("Message from task {0}", Task.CurrentId);
                        BlockingCollection<string>.AddToAny(bc1andbc2, message, tokenSource.Token);
                        tokenSource.Token.WaitHandle.WaitOne(1000);
                    }
                }, tokenSource.Token);
            }

            for (int i = 0; i < 3; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    while(!tokenSource.IsCancellationRequested)
                    {
                        string warning = String.Format("Warning from task {0}", Task.CurrentId);
                        bc3.Add(warning, tokenSource.Token);
                        tokenSource.Token.WaitHandle.WaitOne(500);
                    }
                }, tokenSource.Token);
            }

            for (int i = 0; i < 2; i++)
            {
                Task consumer = Task.Factory.StartNew(() =>
                {
                    string item;
                    while (!tokenSource.IsCancellationRequested)
                    {
                        int bcid = BlockingCollection<string>.TakeFromAny(bcAll, out item, tokenSource.Token);
                        Console.WriteLine("From collection {0} : {1}", bcid, item);
                    }
                }, tokenSource.Token);
            }

            Console.WriteLine("Press enter to cancel tasks");
            Console.ReadLine();

            tokenSource.Cancel();

            EndofProgram();
        }

        public static void UsingCustomScheduler()
        {
            int procCount = System.Environment.ProcessorCount;

            CustomScheduler scheduler = new CustomScheduler(procCount);

            Console.WriteLine("Custom scheduler ID: {0}", scheduler.Id);
            Console.WriteLine("Default scheduler ID: {0}", TaskScheduler.Default.Id);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task task1 = new Task(() =>
            {
                Console.WriteLine("Task {0} executed by scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id);

                // Create a child task - this will use the same scheduler as its parent
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Task {0} executed by scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id);
                });

                // create a child and specify the default scheduler
                Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Task {0} executed by scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id);
                    }, tokenSource.Token, TaskCreationOptions.None, TaskScheduler.Default);
            });

            task1.Start(scheduler);

            task1.ContinueWith(antecedent =>
            {
                Console.WriteLine("Task {0} continue executed by scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id);
            });

            task1.ContinueWith(antecedent =>
            {
                Console.WriteLine("Task {0} continue executed by scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id);
            }, scheduler);
        }

        public static void InconsistentCancellation()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task<int> task1 = new Task<int>(() =>
            {
                Console.WriteLine("Task 1 execute.");
                tokenSource.Token.WaitHandle.WaitOne();
                tokenSource.Token.ThrowIfCancellationRequested();
                // this function never reached but is required to satisfy the compiler.
                return 100;
            }, tokenSource.Token);

            task1.Start();

            Task task2 = task1.ContinueWith((Task<int> antecedent) =>
            {
                // read the antecedent result without checking the status of the task
                Console.WriteLine("task 2 Antecedent result: {0}", antecedent.Result);
            });

            Task task3 = task1.ContinueWith((Task<int> antecedent) =>
            {
                // this task will never be executed
            }, tokenSource.Token);

            Task task4 = task1.ContinueWith((Task<int> antecedent) =>
            {
                if (antecedent.Status == TaskStatus.Canceled)
                {
                    Console.WriteLine("task 4 Antecedent cancelled");
                }
                else
                {
                    Console.WriteLine("task 4 Antecedent Result: {0}", antecedent.Result);
                }
            });
            
            Console.WriteLine("Press enter to cancel token");
            Console.ReadLine();
            // All Result output not execute except the task 4 cancelled notice.
            tokenSource.Cancel();

            EndofProgram();
        }

        public static void AssumingWaitAnyStatus()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task<int>[] tasks = new Task<int>[2];

            tasks[0] = new Task<int>(() =>
            {
                while(true)
                {
                    Thread.Sleep(100000);
                }
            });

            tasks[1] = new Task<int>(() =>
            {
                tokenSource.Token.WaitHandle.WaitOne();
                tokenSource.Token.ThrowIfCancellationRequested();
                return 200;
            }, tokenSource.Token);

            Task.Factory.ContinueWhenAny(tasks, (Task<int> antecedent) =>
            {
                Console.WriteLine("Result of antecedent is {0}", antecedent.Result);
            });

            tasks[1].Start();
            tasks[0].Start();

            Console.WriteLine("Press enter to cancel token");
            Console.ReadLine();
            tokenSource.Cancel();

            EndofProgram();
        }

        public static void TryingToTakeConcurrently()
        {
            BlockingCollection<int> blockingCollection = new BlockingCollection<int>();
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    blockingCollection.Add(i);
                }
                blockingCollection.CompleteAdding();
            });

            Task.Factory.StartNew(() =>
            {
                while (!blockingCollection.IsCompleted)
                {
                    int item = blockingCollection.Take();
                    Console.WriteLine("Item {0}", item);
                }
            });

            EndofProgram();
        }

        public static void ReusingObjectsinProducers()
        {
            BlockingCollection<DataItem> blockingCollection = new BlockingCollection<DataItem>();

            Task consumer = Task.Factory.StartNew(() =>
            {
                DataItem item;
                while (!blockingCollection.IsCompleted)
                {
                    if (blockingCollection.TryTake(out item))
                    {
                        Console.WriteLine("Item counter {0}", item.Counter);
                    }
                }
            });

            Task.Factory.StartNew(() =>
            {
                DataItem item = new DataItem();
                for (int i = 0; i < 100; i++)
                {
                    item.Counter = i;
                    blockingCollection.Add(item);
                }
                blockingCollection.CompleteAdding();
            });

            consumer.Wait();

            EndofProgram();
        }

        public static void UsingBlockingCollectionAsIEnum()
        {
            BlockingCollection<int> blockingCollection = new BlockingCollection<int>();
            Task producer = Task.Factory.StartNew(() =>
            {
                //System.Threading.Thread.Sleep(500);
                for (int i = 0; i < 100; i++)
                {
                    blockingCollection.Add(i);
                }
                blockingCollection.CompleteAdding();
            });

            Task consumer = Task.Factory.StartNew(() =>
            {
                while (!blockingCollection.IsCompleted)
                {
                    int item;
                    if (blockingCollection.TryTake(out item))
                    {
                        Console.WriteLine("Item counter {0}", item);
                    }
                }
                //foreach (int i in blockingCollection)
                //{
                //    Console.WriteLine("item {0}", i);
                //}
                Console.WriteLine("Collection is fully consumed.");
            });

            producer.Wait();
            //consumer.Wait();

            EndofProgram();
        }

        public static void TestDeadlockedTaskScheduler()
        {
            Deadlocked_Task_Scheduler scheduler = new Deadlocked_Task_Scheduler(5);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task[] tasks = new Task[6];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew((object stateObj) =>
                {
                    int index = (int)stateObj;
                    if (index < tasks.Length - 1)
                    {
                        Console.WriteLine("Task {0} waiting for {1}", index, index + 1);
                        tasks[index + 1].Wait();
                    }
                    Console.WriteLine("Task {0} complete", index);
                }, i, tokenSource.Token, TaskCreationOptions.None, scheduler);
            }

            Task.WaitAll(tasks);
            Console.WriteLine("All tasks complete");

            EndofProgram();
        }
    }

    class Deadlocked_Task_Scheduler : TaskScheduler, IDisposable
    {
        private BlockingCollection<Task> taskQueue;
        private Thread[] threads;

        public Deadlocked_Task_Scheduler(int concurrency)
        {
            taskQueue = new BlockingCollection<Task>();
            threads = new Thread[concurrency];

            for (int i = 0; i < threads.Length; i++)
            {
                (threads[i] = new Thread(() =>
                {
                    foreach(Task t in taskQueue.GetConsumingEnumerable())
                    {
                        TryExecuteTask(t);
                    }
                })).Start();
            }
        }

        protected override void QueueTask(Task task)
        {
            if (task.CreationOptions.HasFlag(TaskCreationOptions.LongRunning))
            {
                new Thread(() =>
                {
                    TryExecuteTask(task);
                }).Start();
            }
            else
            {
                taskQueue.Add(task);
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // disallow all inline execution
            return false;
        }

        public override int MaximumConcurrencyLevel
        {
            get
            {
                return threads.Length;
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return taskQueue.ToArray();
        }

        public void Dispose()
        {
            taskQueue.CompleteAdding();
            foreach(Thread t in threads)
            {
                t.Join();
            }
        }
    }

    public class DataItem
    {
        public int Counter { get; set; }
    }

    public class CustomScheduler : TaskScheduler, IDisposable
    {
        private BlockingCollection<Task> taskQueue;
        private Thread[] threads;

        public CustomScheduler(int concurrency)
        {
            taskQueue = new BlockingCollection<Task>();
            threads = new Thread[concurrency];
            for(int i = 0; i < threads.Length; i++)
            {
                (threads[i] = new Thread(() =>
                {
                    foreach (Task t in taskQueue.GetConsumingEnumerable())
                    {
                        TryExecuteTask(t);
                    }
                })).Start();
            }
        }

        protected override void QueueTask(Task task)
        {
            if (task.CreationOptions.HasFlag(TaskCreationOptions.LongRunning))
            {
                new Thread(() => {
                        TryExecuteTask(task);
                }).Start();
            }
            else {
                taskQueue.Add(task);
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (threads.Contains(Thread.CurrentThread))
            {
                return TryExecuteTask(task);
            }
            else
            {
                return false;
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return taskQueue.ToArray();
        }

        public void Dispose()
        {
            taskQueue.CompleteAdding();
            foreach (Thread t in threads)
            {
                t.Join();
            }
        }
    }
}
