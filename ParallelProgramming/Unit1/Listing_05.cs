using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SharedData
{
    class Listing_05 : SharedApp
    {
        public static void UsingParallelLoop()
        {
            int[] dataItems = new int[100];
            double[] resultItems = new double[100];

            for (int i = 0; i < dataItems.Length; i++)
            {
                dataItems[i] = i;
            }

            Parallel.For(0, dataItems.Length, (index) =>
            {
                resultItems[index] = Math.Pow(dataItems[index], 2);
            });

            Console.WriteLine("Pow 5: {0}", Math.Pow(5, 2));
            Console.WriteLine("resultItem begin {0} / end {1}", resultItems[0], resultItems[dataItems.Length - 1]);

            EndOfProgram();
        }

        public static void PerformActionsUsingParallelInvoke()
        {
            Parallel.Invoke(() => Console.WriteLine("Action 1"),
                () => Console.WriteLine("Action 2"),
                () => Console.WriteLine("Action 3"));

            Action[] actions = new Action[3];
            actions[0] = new Action(() => Console.WriteLine("Action 4"));
            actions[1] = new Action(() => Console.WriteLine("Action 5"));
            actions[2] = new Action(() => Console.WriteLine("Action 6"));

            Parallel.Invoke(actions);

            Task parent = Task.Factory.StartNew(() =>
            {
                foreach (Action action in actions)
                {
                    Task.Factory.StartNew(action, TaskCreationOptions.AttachedToParent);
                    //Task.Factory.StartNew(action, TaskCreationOptions.None);
                }
            });

            parent.Wait();

            EndOfProgram();
        }

        public static void UsingBasicParallelLoop()
        {
            Parallel.For(0, 10, index =>
            {
                Console.WriteLine("Task ID {0} processing index: {1}", Task.CurrentId, index);
            });

            EndOfProgram();
        }

        public static void UsingBasicParallelForeachLoop()
        {
            List<string> dataList = new List<string>
            {
                "the", "quick", "brown", "fox", "jumps", "etc"  
            };

            Parallel.ForEach(dataList, item =>
            {
                Console.WriteLine("Item {0} has {1} characters", item, item.Length);
            });

            EndOfProgram();
        }

        static IEnumerable<int> SteppedIterator(int startIndex, int endIndex, int stepSize)
        {
            for (int i = startIndex; i < endIndex; i += stepSize)
            {
                yield return i;
            }
        }

        public static void CreateASteppedLoop()
        {
            Parallel.ForEach(SteppedIterator(0, 10, 2), index =>
            {
                Console.WriteLine("Index value: {0}", index);
            });

            EndOfProgram();
        }

        public static void SettingOptionsForAParallelLoop()
        {
            ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
            Parallel.For(0, 10, options, index =>
            {
                Console.WriteLine("For index {0} started", index);
                Thread.Sleep(500);
                Console.WriteLine("For Index {0} finished", index);
            });

            int[] dataElements = new int[] { 0, 2, 4, 6, 8 };
            Parallel.ForEach(dataElements, options, index =>
            {
                Console.WriteLine("Foreach Index {0} started", index);
                Thread.Sleep(500);
                Console.WriteLine("Foreach Index {0} finished", index);
            });

            EndOfProgram();
        }

        public static void UsingStopInParallelLoop()
        {
            List<string> dataItems = new List<string>() {
                "an", "apple", "a", "day", "keeps", "the", "doctor", "away"
            };

            Parallel.ForEach(dataItems, (string item, ParallelLoopState state) =>
            {
                if (item.Contains("k"))
                {
                    Console.WriteLine("Hit: {0}", item);
                    state.Stop();
                }
                else
                {
                    Console.WriteLine("Miss: {0}", item);
                }
            });

            EndOfProgram();
        }

        public static void UsingBreakInAParallelLoop()
        {
            ParallelLoopResult res = Parallel.For(0, 100, (int index, ParallelLoopState loopState) =>
            {
                double sqr = Math.Pow(index, 2);
                if (sqr > 100)
                {
                    Console.WriteLine("Breaking on index {0}", index);
                    loopState.Break();
                    //loopState.Stop();
                }
                else
                {
                    Console.WriteLine("Square value of {0} is {1}", index, sqr);
                }
            });

            EndOfProgram();
        }

        public static void UsingParallelLoopResult()
        {
            ParallelLoopResult loopResult = Parallel.For(0, 10, (int index, ParallelLoopState loopState) =>
            {
                if (index == 2)
                {
                    loopState.Stop();
                }
            });

            Console.WriteLine("Loop Result");
            Console.WriteLine("IsCompleted: {0}", loopResult.IsCompleted);
            Console.WriteLine("BreakValue: {0}", loopResult.LowestBreakIteration.HasValue);

            EndOfProgram();
        }

        public static void CancelParallelLoops()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                tokenSource.Cancel();
                Console.WriteLine("Token cancelled");
            });

            ParallelOptions loopOptions = new ParallelOptions()
            {
                CancellationToken = tokenSource.Token
            };

            try
            {
                Parallel.For(0, Int64.MaxValue, loopOptions, index =>
                {
                    double result = Math.Pow(index, 3);
                    Console.WriteLine("Index {0}, result {1}", index, result);
                    Thread.Sleep(100);
                });
            } catch (OperationCanceledException)
            {
                Console.WriteLine("Caught cancellation exception...");
            }

            EndOfProgram();
        }

        public static void UsingLocalStorageInParallelLoop()
        {
            int total = 0;

            Parallel.For(0, 100, () => 0, (int index, ParallelLoopState loopState, int tlsValue) =>
            {
                tlsValue += index;
                return tlsValue;
            }, value => Interlocked.Add(ref total, value));

            // potential data race.
            //Parallel.For(0, 100, index =>
            //{
            //    Interlocked.Add(ref total, index);
            //});

            Console.WriteLine("Total: {0}", total);

            EndOfProgram();
        }

        public static void UsingLocalStorageInParallelLoopForEach()
        {
            int matchedWords = 0;

            object lockObj = new object();  // using this can potential cause block.

            string[] dataItems = new string[] { "an", "apple", "a", "day", "keeps", "the", "doctor", "away" };

            Parallel.ForEach(
                dataItems,
                () => 0,
                (string item, ParallelLoopState loopState, int tlsValue) =>
                {
                    if (item.Contains("a"))
                    {
                        tlsValue++;
                    }
                    return tlsValue;
                },
                tlsValue =>
                {
                    lock (lockObj)
                    {
                        matchedWords += tlsValue;
                    }
                });

            Console.WriteLine("Matches: {0}", matchedWords);

            EndOfProgram();
        }

        class Transaction
        {
            public int Amount { get; set; }
        }

        public static void UsingMixingSynchronousAndParallelLoops()
        {
            Random rnd = new Random();

            int itemsPerMonth = 100000;

            Transaction[] sourceData = new Transaction[12 * itemsPerMonth];
            for (int i = 0; i < 12 * itemsPerMonth; i++)
            {
                sourceData[i] = new Transaction() { Amount = rnd.Next(-400, 500) };
            }

            int[] monthlyBalances = new int[12];

            for(int currentMonth = 0; currentMonth < 12; currentMonth++)
            {
                Parallel.For(
                    currentMonth * itemsPerMonth,
                    (currentMonth + 1) * itemsPerMonth,
                    new ParallelOptions(),
                    () => 0,
                    (index, loopstate, tlsBalance) => {
                        return tlsBalance += sourceData[index].Amount;
                    },
                    tlsBalance => monthlyBalances[currentMonth] += tlsBalance);
                if (currentMonth > 0) monthlyBalances[currentMonth] += monthlyBalances[currentMonth - 1];
            }

            for (int i = 0; i < monthlyBalances.Length; i++)
            {
                Console.WriteLine("Month {0} - Balance {1}", i, monthlyBalances[i]);
            }

            EndOfProgram();
        }

        delegate void ProcessValue(int value);
        static double[] resultData = new double[10000000];

        static void computeResultValue(int indexValue)
        {
            resultData[indexValue] = Math.Pow(indexValue, 2);
        }

        public static void UsingChunkPartitioningStrategy()
        {
            Parallel.For(0, resultData.Length, (int index) =>
            {
                resultData[index] = Math.Pow(index, 2);
            });

            Parallel.For(0, resultData.Length, delegate(int index)
            {
                resultData[index] = Math.Pow((double)index, 2);
            });

            ProcessValue pdel = new ProcessValue(computeResultValue);
            Action<int> paction = new Action<int>(pdel);
            Parallel.For(0, resultData.Length, paction);

            Console.WriteLine("result : {0}", resultData[40000]);
            EndOfProgram();
        }

        public static void UsingChunkingPartitioner()
        {
            OrderablePartitioner<Tuple<int, int>> chunkPart = Partitioner.Create(0, resultData.Length, 10000);

            Parallel.ForEach(chunkPart, chunkRange =>
            {
                for (int i = chunkRange.Item1; i < chunkRange.Item2; i++)
                {
                    resultData[i] = Math.Pow(i, 2);
                }
            });

            EndOfProgram();
        }

        public static void UsingOrderedPartitioningStrategy()
        {
            IList<string> sourceData = new List<string>() { "an", "apple", "a", "day", "keeps", "the", "doctor", "away" };

            string[] resultData = new string[sourceData.Count];

            OrderablePartitioner<string> op = Partitioner.Create(sourceData);

            Parallel.ForEach(op, (string item, ParallelLoopState loopState, long index) =>
            {
                if (item == "apple") item = "apricot";
                resultData[index] = item;
            });

            for (int i = 0; i < resultData.Length; i++)
            {
                Console.WriteLine("Item {0} is {1}", i, resultData[i]);
            }

            EndOfProgram();
        }

        public static void UsingContextPartitioner()
        {
            Random rnd = new Random();

            WorkItem[] sourceData = new WorkItem[10000];
            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = new WorkItem() { WorkDuration = rnd.Next(1, 11) };
            }

            Partitioner<WorkItem> cPartitioner = new ContextPartitioner(sourceData, 100);

            Parallel.ForEach(cPartitioner, item =>
            {
                item.performWork();
            });

            EndOfProgram();
        }

        public static void ContextTest()
        {
            Random rnd = new Random();

            WorkItem[] sourceData = new WorkItem[10000];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = new WorkItem() { WorkDuration = rnd.Next(1, 11) };
            }
            WorkItem[] resultData = new WorkItem[sourceData.Length];
            OrderablePartitioner<WorkItem> cPartitioner = new ContextPartitionerII(sourceData, 100);

            Parallel.ForEach(cPartitioner, (WorkItem item, ParallelLoopState loopState, long index) =>
            {
                item.performWork();
                resultData[index] = item;
            });
        }

        public static void SynchronizationInLoop()
        {
            double total = 0;
            object lockObj = new object();

            Parallel.For(0, 100000, item =>
            {
                lock(lockObj)
                {
                    total += Math.Pow(item, 2);
                }
            });
        }

        public static void LoopBodyDataRaces()
        {
            double total = 0;

            Parallel.For(0, 100000, item =>
            {
                total += Math.Pow(item, 2);
            });

            Console.WriteLine("Expected result: 333328333350000");
            Console.WriteLine("Actual result: {0}", total);
        }

        public static void UsingStandardCollections()
        {
            int[] sourceData = new int[10000];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }
            List<int> resultData = new List<int>();
            object obj = new object();
            long frequency;
            long lastTime;
            long currentTime;
            bool ret = Win32API.QueryPerformanceFrequency(out frequency);
            if (ret == false)
            {
                Console.WriteLine("not support to query frequency");
                return;
            }
            Win32API.QueryPerformanceCounter(out lastTime);

            Parallel.ForEach(sourceData, item =>
            {
                //lock (obj)
                {
                    resultData.Add(item);
                }
            });

            Win32API.QueryPerformanceCounter(out currentTime);

            double elapse = (currentTime - lastTime) / (double)frequency;

            Win32API.QueryPerformanceCounter(out lastTime);

            foreach(int item in sourceData)
            {
                 resultData.Add(item);
            }

            Win32API.QueryPerformanceCounter(out currentTime);
            double elapse1 = (currentTime - lastTime) / (double)frequency;

            Console.WriteLine("Result Parallel {0}", elapse);
            Console.WriteLine("Result Single {0}", elapse1);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public static void UsingChangingData()
        {
            List<int> sourceData = new List<int>();
            for(int i = 0; i < 10; i++)
            {
                sourceData.Add(i);
            }

            Task.Factory.StartNew(() =>
            {
                int counter = 10;
                while (true)
                {
                    Thread.Sleep(250);
                    Console.WriteLine("Adding item {0}", counter);
                    sourceData.Add(counter++);
                }
            });

            Parallel.ForEach(sourceData, item =>
            {
                Console.WriteLine("Processing item {0}", item);
            });

            EndOfProgram();
        }

        public static void PLINQQueries()
        {
            int[] sourceData = new int[10];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> result1 = from item in sourceData select Math.Pow(item, 2);

            foreach(double d in result1)
            {
                Console.WriteLine("Sequential result: {0}", d);
            }

            IEnumerable<double> result2 = from item in sourceData.AsParallel() select Math.Pow(item, 2);

            foreach(double d in result2)
            {
                Console.WriteLine("Parallel result: {0}", d);
            }

            EndOfProgram();
        }

        public static void UsingExtensionMethods()
        {
            int[] sourceData = new int[10];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> result1 = sourceData.Select(item => Math.Pow(item, 2));

            foreach (double d in result1)
            {
                Console.WriteLine("Sequential result: {0}", d);
            }

            var result2 = sourceData.AsParallel().Select(item => Math.Pow(item, 2));

            foreach (var d in result2)
            {
                Console.WriteLine("Parallel result: {0}", d);
            }

            EndOfProgram();
        }

        public static void UsingPLINQFilteringQuery()
        {
            int[] sourceData = new int[100000];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> results1 = from item in sourceData.AsParallel() where item % 2 == 0 select Math.Pow(item, 2);

            foreach (var d in results1)
            {
                Console.WriteLine("Result: {0}", d);
            }

            IEnumerable<double> results2 = sourceData.AsParallel().Where(item => item % 2 == 0).Select(item => Math.Pow(item, 2));

            foreach(var d in results2)
            {
                Console.WriteLine("Result: {0}", d);
            }

            EndOfProgram();
        }

        public static void UsingCustomAggregation()
        {
            int[] sourceData = new int[10000];
            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            double aggregateResult = sourceData.AsParallel().Aggregate(
                0.0,
                (subtotal, item) => subtotal += Math.Pow(item, 2),
                (total, subtotal) => total + subtotal,
                total => total/ 2);

            Console.WriteLine("Total: {0}", aggregateResult);
            
            EndOfProgram();
        }

        public static void GenerateParallelRanges()
        {
            IEnumerable<double> result1 = from e in ParallelEnumerable.Range(0, 10) where e % 2 == 0 select Math.Pow(e, 2);
            IEnumerable<double> result2 = ParallelEnumerable.Repeat(10, 100).Select(item => Math.Pow(item, 2));

            EndOfProgram();
        }
        
        public static void CreateRaceConditions()
        {
            int[] sourceData = new int[10000];
            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }
            int counter = 1000;
            var result = from e in sourceData.AsParallel() where (counter-- > 0) select e;

            EndOfProgram();
        }

        public static void ConfusingOrdering()
        {
            string[] sourceData = new string[] { "an", "apple", "a", "day", "keeps", "the", "doctor", "away" };
            var result1 = sourceData.AsParallel().AsOrdered().Select(item => item);
            foreach (var v in result1)
            {
                Console.WriteLine("AsOrdered() - {0}", v);
            }
            var result2 = sourceData.AsParallel().OrderBy(item => item).Select(item => item);
            foreach (var v in result2)
            {
                Console.WriteLine("OrderedBy() - {0}", v);
            }
            EndOfProgram();
        }

        public static void SequentialFiltering()
        {
            int[] source1 = new int[100];
            for (int i = 0; i < source1.Length; i++)
            {
                source1[i] = i;
            }
            var result = source1.Where(item => item % 2 == 0).AsParallel().Select(item => item);
            foreach (var v in result)
            {
                Console.WriteLine("SequentialFiltering() - {0}", v);
            }
            EndOfProgram();
        }
    }

    class WorkItem
    {
        public int WorkDuration { get; set; }
        public void performWork()
        {
            Console.WriteLine("performWork: {0} in task {1}", WorkDuration, Task.CurrentId);
            Thread.Sleep(WorkDuration);
        }
    }
}
