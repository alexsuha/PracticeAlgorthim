using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;

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

    class ImmutableBankAccount
    {
        public const int AccountNumber = 123456;
        public readonly int Balance;

        public ImmutableBankAccount(int initialBalance)
        {
            Balance = initialBalance;
        }

        public ImmutableBankAccount()
        {
            Balance = 0;
        }
    }

    [Synchronization]
    class SBankAccount : ContextBoundObject
    {
        private int balance = 0;

        public void IncrementBalance()
        {
            balance++;
        }

        public int GetBalance()
        {
            return balance;
        }
    }

    class Listing_03
    {
        private static void EndofProgram()
        {
            // wait for input before existing
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public static void firstSharedData()
        {
            // create the bank account instance.
            BankAccount account = new BankAccount();

            // create an array of tasks
            Task[] tasks = new Task[1];

            for (int i = 0; i < 1; i++)
            {
                // create a new task
                tasks[i] = new Task(() =>
                {
                    // enter a loop for 1000 balance updates
                    for (int j = 0; j < 1000; j++)
                    {
                        // update the balance
                        account.Balance = account.Balance + 1;
                    }
                });
                // start the new task
                tasks[i].Start();
            }

            // wait for all of the tasks to complete
            Task.WaitAll();
            //Task.WaitAll(tasks[0]); // this function not interrupt until task finished. CARE!!!, but if specify the task, it will be finished.

            //while (!tasks[0].IsCompleted) // When change the task number into 1, not show 1000 without this function.
            //{

            //}
            // write out the counter value
            Console.WriteLine("Expected value {0}, Count value: {1}", 10000, account.Balance);

            EndofProgram();
        }

        public static void CreateImutableData()
        {
            ImmutableBankAccount bankAccount1 = new ImmutableBankAccount();
            Console.WriteLine("Account number: {0}, Account balance: {1}",
                ImmutableBankAccount.AccountNumber, bankAccount1.Balance);

            ImmutableBankAccount bankAccount2 = new ImmutableBankAccount(200);
            Console.WriteLine("Account number: {0}, Account balance: {1}",
                ImmutableBankAccount.AccountNumber, bankAccount2.Balance);

            EndofProgram();
        }

        public static void ExecuteInIsolation()
        {
            // create the bank account instance
            BankAccount account = new BankAccount();

            // create an array of tasks
            Task<int>[] tasks = new Task<int>[10];

            for(int i = 0; i < 10; i++)
            {
                tasks[i] = new Task<int>((stateObject) =>
                {
                    // get the state object
                    int isolatedBalance = (int)stateObject;

                    // enter a loop for 1000 balance updates
                    for(int j = 0; j < 1000; j++)
                    {
                        isolatedBalance++;
                    }

                    return isolatedBalance;
                }, account.Balance);

                tasks[i].Start();
            }

            for (int i = 0; i < 10; i++)
            {
                account.Balance += tasks[i].Result;
            }

            Console.WriteLine("Expected value {0}, Count value: {1}", 10000, account.Balance);

            EndofProgram();
        }

        public static void ExecuteInIsolationWithTLS()
        {
            // create the bank account instance
            BankAccount account = new BankAccount();

            // create an array of tasks
            Task<int>[] tasks = new Task<int>[10];

            ThreadLocal<int> tls = new ThreadLocal<int>();

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task<int>((stateObject) =>
                {
                    // get the state object
                    tls.Value = (int)stateObject;

                    // enter a loop for 1000 balance updates
                    for (int j = 0; j < 1000; j++)
                    {
                        tls.Value++;
                    }

                    return tls.Value;
                }, account.Balance);

                tasks[i].Start();
            }

            for (int i = 0; i < 10; i++)
            {
                account.Balance += tasks[i].Result;
            }

            Console.WriteLine("Expected value {0}, Count value: {1}, Tls value: {2}",
                10000, account.Balance, tls.Value);

            EndofProgram();
        }

        public static void ExecuteInIsolationWithTLSFactory()
        {
            // create the bank account instance
            BankAccount account = new BankAccount();

            // create an array of tasks
            Task<int>[] tasks = new Task<int>[10];

            ThreadLocal<int> tls = new ThreadLocal<int>(() =>
            {
                Console.WriteLine("Value factory called for value: {0}", account.Balance);
                return account.Balance;
            });

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task<int>(() =>
                {
                    // get the state object
                   // tls.Value = (int)stateObject;

                    for (int j = 0; j < 1000; j++)
                    {
                        tls.Value++;
                    }

                    return tls.Value;
                });

                tasks[i].Start();
            }

            for (int i = 0; i < 10; i++)
            {
                account.Balance += tasks[i].Result;
            }

            Console.WriteLine("Expected value {0}, Count value: {1}, Tls value: {2}",
                10000, account.Balance, tls.Value);

            EndofProgram();
        }

        public static void AcquiringMultipleLocks()
        {
            BankAccount account1 = new BankAccount();
            BankAccount account2 = new BankAccount();

            Mutex mutex1 = new Mutex();
            Mutex mutex2 = new Mutex();

            Task task1 = new Task(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    bool lockAcquired = mutex1.WaitOne();
                    try
                    {
                        account1.Balance++;
                    }
                    finally
                    {
                        if (lockAcquired)
                        {
                            mutex1.ReleaseMutex();
                        }
                    }
                }
            });

            Task task2 = new Task(() =>
            {
                for(int j = 0; j < 1000; j++)
                {
                    bool lockAquired = mutex2.WaitOne();
                    try
                    {
                        account2.Balance++;
                    }
                    finally
                    {
                        if (lockAquired)
                        {
                            mutex2.ReleaseMutex();
                        }
                    }
                }
            });

            Task task3 = new Task(() =>
            {
                for(int j = 0; j < 1000; j++)
                {
                    bool lockAquired = Mutex.WaitAll(new WaitHandle[] { mutex1, mutex2 });
                    try
                    {
                        // simulate  a transfer between accounts
                        account1.Balance++;
                        account2.Balance--;
                    }
                    finally
                    {
                        if (lockAquired)
                        {
                            mutex1.ReleaseMutex();
                            mutex2.ReleaseMutex();
                        }
                    }
                }
            });

            task1.Start();
            task2.Start();
            task3.Start();

            Task.WaitAll(task1, task2, task3);

            Console.WriteLine("Account1 balance: {0}, Account balance: {1}",
                account1.Balance, account2.Balance);

            EndofProgram();
        }

        public static void ConfiguringInterprocessSynchronization()
        {
            string mutexName = "myApressMutex";

            Mutex namedMutex;

            try
            {
                namedMutex = Mutex.OpenExisting(mutexName);
            } catch(WaitHandleCannotBeOpenedException)
            {
                namedMutex = new Mutex(false, mutexName);
            }

            Task task = new Task(() =>
            {
                while(true)
                {
                    Console.WriteLine("Waiting to aquire Mutex");
                    namedMutex.WaitOne();
                    Console.WriteLine("Acquired Mutex - press enter to release");
                    Console.ReadLine();
                    namedMutex.ReleaseMutex();
                    Console.WriteLine("Released Mutex");
                }
            });

            task.Start();

            task.Wait();
        }

        public static void UsingDeclarativeSynchronization()
        {
            SBankAccount account = new SBankAccount();

            Task[] tasks = new Task[10];

            for(int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                {
                    for(int j = 0; j < 1000; j++)
                    {
                        account.IncrementBalance();
                    }
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Expected value {0}, Balance {1}",
                10000, account.GetBalance());

            EndofProgram();
        }

        public static void UsingSpinLock()
        {
            BankAccount account = new BankAccount();

            SpinLock spinLock = new SpinLock();

            Task[] tasks = new Task[10];

            for(int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool lockAquired = false;
                        try
                        {
                            spinLock.Enter(ref lockAquired);
                            account.Balance = account.Balance + 1;
                        }
                        finally
                        {
                            if (lockAquired)
                            {
                                spinLock.Exit();
                            }
                        }
                    }
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Expected value {0}, Balance {1}", 10000, account.Balance);

            EndofProgram();
        }

        public static void UsingMutexAndWaitHandle()
        {
            BankAccount account = new BankAccount();

            Mutex mutex = new Mutex();

            Task[] tasks = new Task[10];

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                {
                    for(int j = 0; j < 1000; j++)
                    {
                        bool lockAcquired = mutex.WaitOne();
                        try
                        {
                            account.Balance = account.Balance + 1;
                        }
                        finally
                        {
                            if (lockAcquired)
                            {
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Expected value {0}, Balance {1}", 10000, account.Balance);

            EndofProgram();
        }

        public static void UsingReaderWriterLock()
        {
            ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task[] tasks = new Task[5];

            for(int i = 0; i < 5; i++)
            {
                tasks[i] = new Task(() =>
                {
                    while(true)
                    {
                        rwLock.EnterReadLock();

                        Console.WriteLine("Read lock aquired - count: {0}", rwLock.CurrentReadCount);

                        tokenSource.Token.WaitHandle.WaitOne(1000);

                        rwLock.ExitReadLock();

                        Console.WriteLine("Read lock released - count: {0}", rwLock.CurrentReadCount);

                        tokenSource.Token.ThrowIfCancellationRequested();
                    }
                }, tokenSource.Token);
                tasks[i].Start();
            }

            Console.WriteLine("Read enter to acquire write lock");
            Console.ReadLine();

            Console.WriteLine("Request write lock");
            rwLock.EnterWriteLock();

            Console.WriteLine("Write lock acquired");
            Console.WriteLine("Press enter to release write lock");
            Console.ReadLine();
            rwLock.ExitWriteLock();

            tokenSource.Token.WaitHandle.WaitOne(2000);
            tokenSource.Cancel();

            try
            {
                Task.WaitAll(tasks);
            }
            catch(AggregateException)
            {

            }

            EndofProgram();
        }

        public static void UsingUpgradedReadWriteLock()
        {
            ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            int sharedData = 0;

            Task[] readerTasks = new Task[5];

            for(int i = 0; i < readerTasks.Length; i++)
            {
                readerTasks[i] = new Task(() =>
                {
                    while(true)
                    {
                        rwlock.EnterReadLock();

                        Console.WriteLine("Read lock acquired - count: {0}", rwlock.CurrentReadCount);

                        Console.WriteLine("Shared data value {0}", sharedData);

                        tokenSource.Token.WaitHandle.WaitOne(1000);

                        rwlock.ExitReadLock();

                        Console.WriteLine("Read lock released - count {0}", rwlock.CurrentReadCount);

                        tokenSource.Token.ThrowIfCancellationRequested();
                    }
                }, tokenSource.Token);
                readerTasks[i].Start();
            }

            Task[] writeTasks = new Task[2];

            for(int i = 0; i < writeTasks.Length; i++)
            {
                writeTasks[i] = new Task(() =>
                {
                    while(true)
                    {
                        rwlock.EnterUpgradeableReadLock();

                        if (true)
                        {
                            rwlock.EnterWriteLock();

                            Console.WriteLine("Write lock acquired - waiting readers {0}, writers {1}, upgraders {2}",
                                rwlock.WaitingReadCount, rwlock.WaitingWriteCount, rwlock.WaitingUpgradeCount);

                            sharedData++;

                            tokenSource.Token.WaitHandle.WaitOne(1000);

                            rwlock.ExitWriteLock();
                        }

                        rwlock.ExitUpgradeableReadLock();

                        tokenSource.Token.ThrowIfCancellationRequested();
                    }
                }, tokenSource.Token);

                writeTasks[i].Start();
            }

            Console.WriteLine("Press enter to cancel tasks");

            Console.ReadLine();

            tokenSource.Cancel();

            try
            {
                Task.WaitAll(readerTasks);
            }
            catch (AggregateException agex)
            {
                agex.Handle(ex => true);
            }

            EndofProgram();
        }

        public static void WorkingWithConcurrentCollections()
        {
            Queue<int> sharedQueue = new Queue<int>();

            for(int i = 0; i < 1000; i++)
            {
                sharedQueue.Enqueue(i);
            }

            int itemCount = 0;

            Task[] tasks = new Task[10];
            for(int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                {
                    while (sharedQueue.Count > 0)
                    {
                        int item = sharedQueue.Dequeue();
                        Interlocked.Increment(ref itemCount);
                    }
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Item processed: {0}", itemCount);

            EndofProgram();
        }

        public static void UsingConcurrentQueue()
        {
            ConcurrentQueue<int> sharedQueue = new ConcurrentQueue<int>();

            for(int i = 0; i < 1000; i++)
            {
                sharedQueue.Enqueue(i);
            }

            int itemCount = 0;

            Task[] tasks = new Task[10];

            for(int i = 0; i < 10; i ++)
            {
                tasks[i] = new Task(() =>
                {
                    while (sharedQueue.Count > 0)
                    {
                        int queueElement;
                        bool gotElement = sharedQueue.TryDequeue(out queueElement);

                        if (gotElement)
                        {
                            Interlocked.Increment(ref itemCount);
                        }
                    }
                });

                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Items processed {0}", itemCount);

            EndofProgram();
        }

        public static void UsingConcurrentStack()
        {
            ConcurrentStack<int> sharedStack = new ConcurrentStack<int>();

            for(int i = 0; i < 1000; i ++)
            {
                sharedStack.Push(i);
            }

            int itemCount = 0;

            Task[] tasks = new Task[10];

            for(int i = 0; i < 1000; i++)
            {
                tasks[i] = new Task(() =>
                {
                    while(sharedStack.Count > 0)
                    {
                        int queueElement;
                        bool gotElement = sharedStack.TryPop(out queueElement);
                        if (gotElement)
                        {
                            Interlocked.Increment(ref itemCount);
                        }
                    }
                });

                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Items processed {0}", itemCount);

            EndofProgram();
        }

        public static void UsingConcurrentBag()
        {
            ConcurrentBag<int> sharedBag = new ConcurrentBag<int>();

            int itemCount = 0;

            for(int i = 0; i < 1000; i++)
            {
                sharedBag.Add(i);
            }

            Task[] tasks = new Task[10];

            for(int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                {
                    while (sharedBag.Count > 0)
                    {
                        int queueElement;
                        bool gotElement = sharedBag.TryTake(out queueElement);

                        if (gotElement)
                        {
                            Interlocked.Increment(ref itemCount);
                        }
                    }
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Item count {0}", itemCount);

            EndofProgram();
        }

        public static void UsingConcurrentDictionary()
        {
            BankAccount account = new BankAccount();

            ConcurrentDictionary<object, int> sharedDict = new ConcurrentDictionary<object, int>();

            Task<int>[] tasks = new Task<int>[10];

            for(int i = 0; i < tasks.Length; i++)
            {
                sharedDict.TryAdd(i, account.Balance);

                tasks[i] = new Task<int>((keyObj) =>
                {
                    int currentValue;
                    bool gotValue;

                    for (int j = 0; j < 1000; j++)
                    {
                        gotValue = sharedDict.TryGetValue(keyObj, out currentValue);
                        sharedDict.TryUpdate(keyObj, currentValue + 1, currentValue);
                    }

                    int result;
                    gotValue = sharedDict.TryGetValue(keyObj, out result);
                    if (gotValue)
                    {
                        return result;
                    }
                    else
                    {
                        throw new Exception(String.Format("No data item available for key {0}", keyObj));
                    }
                }, i);

                tasks[i].Start();
            }

            for (int i = 0; i < tasks.Length; i++)
            {
                account.Balance += tasks[i].Result;
            }

            Console.WriteLine("Expected value {0}, Balance {1}", 10000, account.Balance);

            EndofProgram();
        }

        public static void UsingFirstGenerationCollections()
        {
            Queue sharedQueue = Queue.Synchronized(new Queue());
            //Queue sharedQueue = new Queue();

            Task[] tasks = new Task[10];
            for(int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        sharedQueue.Enqueue(j);
                    }
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Item enqueued {0}", sharedQueue.Count);

            EndofProgram();
        }

        public static void UsingMultiOperationOnQueue()
        {
            Queue sharedQueue = new Queue();

            for(int i = 0; i < 1000; i++) {
                sharedQueue.Enqueue(i);
            }

            int itemCount = 0;

            Task[] tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                {
                    while (sharedQueue.Count > 0)
                    {
                        lock (sharedQueue.SyncRoot)
                        {
                            if (sharedQueue.Count > 0)
                            {
                                int queueElement = (int)sharedQueue.Dequeue();
                                Interlocked.Increment(ref itemCount);
                            }
                        }
                    }
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
            Console.WriteLine("item processed {0}", itemCount);

            EndofProgram();
        }

        class MyReferenceData
        {
            public double PI = 3.14;
        }

        class MyImmutableType
        {
            public readonly MyReferenceData refData = new MyReferenceData();
            public readonly int circleSize = 1;
        }

        public static void MisTakenImmutability()
        {
            MyImmutableType immutable = new MyImmutableType();

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task task1 = new Task(() =>
            {
                while(true)
                {
                    double circ = 2 * immutable.refData.PI * immutable.circleSize;
                    Console.WriteLine("Circumference: {0}", circ);
                    if (circ == 4)
                    {
                        Console.WriteLine("mutation detected");
                        break;
                    }

                    tokenSource.Token.WaitHandle.WaitOne(250);
                }
            }, tokenSource.Token);

            task1.Start();

            Task.Delay(1000);

            immutable.refData.PI = 2;
            task1.Wait();

            EndofProgram();
        }

        /*with different lock may not get the same result with same lock*/
        public static void UsingMultiLock()
        {
            BankAccount account = new BankAccount();

            object lock1 = new object();
            object lock2 = new object();

            Task[] tasks = new Task[10];

            for(int i = 0; i < 5; i++)
            {
                tasks[i] = new Task(() =>
                {
                    for(int j = 0; j < 1000; j++)
                    {
                        lock(lock1)
                        {
                            account.Balance++;
                        }
                    }
                });
            }

            for (int i = 5; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        lock (lock2)
                        {
                            account.Balance++;
                        }
                    }
                });
            }

            foreach(Task task in tasks)
            {
                task.Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Expected value {0}, Balance {1}", 10000, account.Balance);

            EndofProgram();
        }

        public static void LockAcquisitionOrder()
        {
            // this is a dead lock case.
            object lock1 = new object();
            object lock2 = new object();

            Task task1 = new Task(() =>
            {
                lock(lock1)
                {
                    Console.WriteLine("Task 1 acquired lock 1");
                    Task.Delay(500);
                    lock(lock2)
                    {
                        Console.WriteLine("Task 1 acquired lock 2");
                    }
                }
            });

            Task task2 = new Task(() =>
            {
                lock (lock2)
                {
                    Console.WriteLine("Task 2 acquired lock 2");
                    Task.Delay(500);
                    lock(lock1)
                    {
                        Console.WriteLine("Task 2 acquired lock 1");
                    }
                }
            });

            task1.Start();
            task2.Start();

            EndofProgram();
        }

        public static void OrphanedLock()
        {
            Mutex mutex = new Mutex();

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task task1 = new Task(() =>
            {
                while (true)
                {
                    mutex.WaitOne();
                    Console.WriteLine("Task 1 acquired mutex");
                    tokenSource.Token.WaitHandle.WaitOne(500);
                    mutex.ReleaseMutex();
                    Console.WriteLine("Task 1 released mutex");
                }
            }, tokenSource.Token);

            Task task2 = new Task(() =>
            {
                Console.WriteLine("Task 2 working now.");
                tokenSource.Token.WaitHandle.WaitOne(2000);
                mutex.WaitOne();
                Console.WriteLine("Task 2 acquired mutex");
                throw new Exception("Abandoning Mutex");
            }, tokenSource.Token);

            task1.Start();
            task2.Start();

            Console.WriteLine("tokenSource before wait 3000");
            tokenSource.Token.WaitHandle.WaitOne(3000);
            Console.WriteLine("tokenSource after wait 3000");
            try
            {
                task2.Wait();
            }
            catch (AggregateException ex)
            {
                ex.Handle((inner) =>
                {
                    Console.WriteLine(inner);
                    return true;
                });
            }

            EndofProgram();
        }
    }
}
