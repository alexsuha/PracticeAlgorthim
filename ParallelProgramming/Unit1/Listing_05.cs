using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            EndOfProgram();
        }
    }
}
