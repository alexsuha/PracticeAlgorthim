using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;

namespace SharedData
{
    class Listing_07 : SharedApp
    {
        public static void UsingStopWatchMeasureParallelLoopConcurrency()
        {
            Random rnd = new Random();
            int[] sourceData = new int[100000000];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = rnd.Next(0, int.MaxValue);
            }
            int numberOfIterations = 10;
            int maxDegreeOfConcurrency = 16;
            object lockObj = new object();
            for(int concurrency = 1; concurrency <= maxDegreeOfConcurrency; concurrency++)
            {
                // reset the stopwatch for this concurrency degree
                Stopwatch stopWatch = Stopwatch.StartNew();

                ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = concurrency };
                for (int iteration = 0; iteration < numberOfIterations; iteration++)
                {
                    double result = 0;
                    Parallel.ForEach(sourceData, options, () => 0.0,
                        (int value, ParallelLoopState loopState, long index, double localTotal) =>
                        {
                            return localTotal + Math.Pow(value, 2);
                        }, localTotal =>
                        {
                            lock (lockObj)
                            {
                                result += localTotal;
                            }
                        });
                }
                stopWatch.Stop();

                Console.WriteLine("Concurrency {0} : per-iteration time is {1} ms", concurrency, stopWatch.ElapsedMilliseconds / numberOfIterations);
            }

            EndOfProgram();
        }

        static CountdownEvent cdEvent;
        static SemaphoreSlim semA, semB;

        public static void DebuggingProgrammeState()
        {
            semA = new SemaphoreSlim(2);
            semB = new SemaphoreSlim(2);

            int taskCount = 10;

            cdEvent = new CountdownEvent(taskCount);

            Task[] tasks = new Task[10];
            for(int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Factory.StartNew((stateObject) =>
                {
                    InitialMethod((int)stateObject);
                }, i);
            }

            cdEvent.Wait();

            throw new Exception();
        }

        static void InitialMethod(int argument)
        {
            if (argument % 2 == 0)
            {
                MethodA(argument);
            }
            else
            {
                MethodB(argument);
            }
        }

        static void MethodA(int argument)
        {
            if (argument < 5)
            {
                TerminalMethodA();
            }
            else
            {
                TerminalMethodB();
            }
        }

        static void MethodB(int argument)
        {
            if (argument < 5)
            {
                TerminalMethodA();
            }
            else
            {
                TerminalMethodB();
            }
        }

        static void TerminalMethodA()
        {
            cdEvent.Signal();
            // acquire the lock for this method
            semA.Wait();
            // perform some work
            for (int i = 0; i < 500000000; i++)
            {
                Math.Pow(i, 2);
            }
            // release the semaphore
            semA.Release();
        }

        static void TerminalMethodB()
        {
            cdEvent.Signal();
            semB.Wait();
            for (int i = 0; i < 500000000; i ++)
            {
                Math.Pow(i, 3);
            }
            semB.Release();
        }

        public static void HandleTaskExceptions()
        {
            Task[] tasks = new Task[2];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 5000000; j++)
                    {
                        if (j == 500)
                        {
                            throw new Exception("Value is 500");
                        }
                        Math.Pow(j, 2);
                    }
                });

                try
                {
                    Task.WaitAll(tasks);
                }
                catch (AggregateException ex)
                {
                    ex.Handle(innerEx =>
                    {
                        Console.WriteLine("Exception message is {0}", innerEx.Message);
                        return true;
                    });
                }
            }
        }

        public static void DetectDeadlocks()
        {
            int taskCount = 10;
            CountdownEvent cdEvent = new CountdownEvent(taskCount);

            Task[] tasks = new Task[taskCount];
            for(int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Factory.StartNew((stateObj) =>
                {
                    cdEvent.Signal();
                    tasks[(((int)stateObj) + 1) % taskCount].Wait();
                }, i);
            }

            cdEvent.Wait();

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        public static void UsingParallelSort()
        {
            Random rnd = new Random();
            int[] sourceData = new int[5000000];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = rnd.Next(1, 100);
            }

            Parallel_Sort<int>.ParallelQuickSort(sourceData, new IntComparer());
        }

        internal static Tree<int> populateTree(Tree<int> parentNode, Random rnd, int depth = 0)
        {
            parentNode.Data = rnd.Next(1, 1000);
            if (depth < 10)
            {
                parentNode.LeftNode = new Tree<int>();
                parentNode.RightNode = new Tree<int>();

                populateTree(parentNode.LeftNode, rnd, depth + 1);
                populateTree(parentNode.RightNode, rnd, depth + 1);
            }
            return parentNode;
        }

        public static void UsingParallelTraverseTree()
        {
            Tree<int> tree = populateTree(new Tree<int>(), new Random());

            TreeTraverser.TraverseTree(tree, item =>
            {
                if (item % 2 == 0)
                {
                    Console.WriteLine("Item {0}", item);
                }
            });

            EndOfProgram();
        }

        public static void UsingParallelTreeSearch()
        {
            Tree<int> tree = populateTree(new Tree<int>(), new Random(2));

            int result = TreeSearch.SearchTree(tree, item =>
            {
                if (item == 183) Console.WriteLine("Value : {0}", item);
                return item == 183;
            });

            Console.WriteLine("Search match ? {0}", result);

            EndOfProgram();
        }

        public static void UsingParallelCache()
        {
            Parallel_Cache<int, double> cache = new Parallel_Cache<int, double>(key =>
            {
                Console.WriteLine("Created value for key {0}", key);
                return Math.Pow(key, 2);
            });

            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 20; j++)
                    {
                        Console.WriteLine("Task {0} got value {1} for key {2}", Task.CurrentId, cache.GetValue(j), j);
                    }
                });
            }

            EndOfProgram();
        }

        public static void UsingParallelMap()
        {
            int[] sourceData = Enumerable.Range(0, 100).ToArray();

            Func<int, double> mapFunction = value => Math.Pow(value, 2);

            double[] resultData = Parallel_Map.ParallelMap<int, double>(mapFunction, sourceData);

            for(int i = 0; i < sourceData.Length; i++)
            {
                Console.WriteLine("Value {0} mapped to {1}", sourceData[i], resultData[i]);
            }
        }

        public static void UsingParallelReduce()
        {
            int[] sourceData = Enumerable.Range(0, 10).ToArray();

            Func<int, int, int> reduceFunction = (value1, value2) => value1 + value2;

            for (int i = 0; i < sourceData.Length; i++)
            {
                Console.WriteLine("Source {0} : {1}", i, sourceData[i]);
            }

            int result = Parallel_Reduce.Reduce<int>(sourceData, 0, reduceFunction);

            Console.WriteLine("Result: {0}", result);

            EndOfProgram();
        }

        public static void UsingParallelMapReduce()
        {
            Func<int, IEnumerable<int>> map = value => {
                IList<int> factors = new List<int>();
                for (int i = 1; i < value; i++)
                {
                    if (value % i == 0)
                    {
                        factors.Add(i);
                    }
                }
                return factors;
            };

            Func<int, int> group = value => value;

            Func<IGrouping<int, int>, KeyValuePair<int, int>> reduce = grouping =>
            {
                return new KeyValuePair<int, int>(grouping.Key, grouping.Count());
            };

            IEnumerable<int> sourceData = Enumerable.Range(1, 50);

            IEnumerable<KeyValuePair<int, int>> result = Parallel_MapReduce.MapReduce(sourceData, map, group, reduce);

            foreach(KeyValuePair<int, int> kvp in result)
            {
                Console.WriteLine("{0} is a factor {1} times", kvp.Key, kvp.Value);
            }
        }

        public static void UsingSpeculativeSuggestion()
        {
            Func<int, double> pFunction = value =>
            {
                Random rnd = new Random();
                Thread.Sleep(rnd.Next(1, 2000));
                return Math.Pow(value, 2);
            };

            Func<int, double> pFunctions2 = value =>
            {
                Random rnd = new Random();
                Thread.Sleep(rnd.Next(1, 1000));
                return Math.Pow(value, 2);
            };

            Action<long, double> callback = (index, result) =>
            {
                Console.WriteLine("Received result of {0} from function {1}", result, index);
            };

            for (int i = 0; i < 10; i++)
            {
                Speculative_Selection.Compute<int, double>(i, callback, pFunction, pFunctions2);
            }

            EndOfProgram();
        }

        public static void UsingDecoupledConsole()
        {
            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(state =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Decoupled_Console.WriteLine("Message from task {0}", Task.CurrentId);
                    }
                }, i);
            }

            EndOfProgram();
        }

        public static void UsingPipeLine()
        {
            Func<int, double> func1 = (input => Math.Pow(input, 2));
            Func<double, double> func2 = (input => input / 2);
            Func<double, bool> func3 = (input => input % 2 == 0 && input > 100);

            Action<int, bool> callback = (input, output) =>
            {
                if (output)
                {
                    Console.WriteLine("Found value {0} with result {1}", input, output);
                }
            };

            Pipeline<int, bool> pipe = new Pipeline<int, double>(func1).AddFunction(func2).AddFunction(func3);

            pipe.StartProcessing();

            for(int i = 0; i < 1000; i++)
            {
                Console.WriteLine("Added value {0}", i);
                pipe.AddValue(i, callback);
            }

            pipe.StopProcessing();

            EndOfProgram();
        }
    }

    class Pipeline<TInput, TOutput>
    {
        private class ValueCallBackWrapper
        {
            public TInput Value;
            public Action<TInput, TOutput> Callback;
        }

        private BlockingCollection<ValueCallBackWrapper> valueQueue;
        Func<TInput, TOutput> pipelineFunction;

        public Pipeline(Func<TInput, TOutput> function)
        {
            pipelineFunction = function;
        }

        public Pipeline<TInput, TNewOutput> AddFunction<TNewOutput>(Func<TOutput, TNewOutput> newfunction)
        {
            Func<TInput, TNewOutput> compositeFunction = (inputValue =>
            {
                return newfunction(pipelineFunction(inputValue));
            });

            return new Pipeline<TInput, TNewOutput>(compositeFunction);
        }

        public void AddValue(TInput value, Action<TInput, TOutput> callback)
        {
            valueQueue.Add(new ValueCallBackWrapper { 
                Value = value, Callback = callback
            });
        }

        public void StartProcessing()
        {
            valueQueue = new BlockingCollection<ValueCallBackWrapper>();

            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(
                    valueQueue.GetConsumingEnumerable(),
                    wrapper =>
                    {
                        wrapper.Callback(wrapper.Value, pipelineFunction(wrapper.Value));
                    });
            });
        }

        public void StopProcessing()
        {
            valueQueue.CompleteAdding();
        }
    }

    class Decoupled_Console
    {
        private static BlockingCollection<Action> blockingQueue;
        private static Task messageWorker;

        static Decoupled_Console()
        {
            blockingQueue = new BlockingCollection<Action>();
            messageWorker = Task.Factory.StartNew(() =>
            {
                foreach (Action aciton in blockingQueue.GetConsumingEnumerable())
                {
                    aciton.Invoke();
                }
            }, TaskCreationOptions.LongRunning);
        }

        public static void WriteLine(object value)
        {
            blockingQueue.Add(new Action(() => Console.WriteLine(value)));
        }

        public static void WriteLine(string format, params object[] values)
        {
            blockingQueue.Add(new Action(() => Console.WriteLine(format, values)));
        }
    }

    static class Speculative_Selection
    {
        public static void Compute<TInput, TOutput>(TInput value, Action<long, TOutput> callback, params Func<TInput, TOutput>[] functions)
        {
            int resultCounter = 0;
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(functions, (Func<TInput, TOutput> func, ParallelLoopState loopState, long iterationIndex) =>
                {
                    TOutput localResult = func(value);
                    if (Interlocked.Increment(ref resultCounter) == 1)
                    {
                        loopState.Stop();
                        callback(iterationIndex, localResult);
                    }
                });
            });
        }
    }

    class Parallel_MapReduce
    {
        public static IEnumerable<TOutput> MapReduce<TInput, TIntermediate, TKey, TOutput>(
            IEnumerable<TInput> sourceData, 
            Func<TInput, IEnumerable<TIntermediate>> mapFunction, 
            Func<TIntermediate, TKey> groupFunction,
            Func<IGrouping<TKey, TIntermediate>, TOutput> reduceFunction)
        {
            return sourceData.AsParallel().SelectMany(mapFunction).GroupBy(groupFunction).Select(reduceFunction);
        }
    }

    class Parallel_Reduce
    {
        public static TValue Reduce<TValue>(TValue[] sourceData, TValue seedValue, Func<TValue, TValue, TValue> reduceFunction)
        {
            return sourceData.AsParallel().Aggregate(seedValue, (localResult, value) => reduceFunction(localResult, value), (overallResult, localResult) => reduceFunction(overallResult, localResult), overallResult => overallResult);
        }
    }

    class Parallel_Map
    {
        public static TOutput[] ParallelMap<TInput, TOutput> (Func<TInput, TOutput> mapFunction, TInput[] input)
        {
            return input.AsParallel().AsOrdered().Select(value => mapFunction(value)).ToArray();
        }
    }

    class Parallel_Cache<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, Lazy<TValue>> dictionary;
        private Func<TKey, TValue> valueFactory;

        public Parallel_Cache(Func<TKey, TValue> factory)
        {
            valueFactory = factory;
            dictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        public TValue GetValue(TKey key)
        {
            return dictionary.GetOrAdd(key, new Lazy<TValue>(() => valueFactory(key))).Value;
        }
    }

    class Tree<T>
    {
        public Tree<T> LeftNode, RightNode;
        public T Data;
    }

    class TreeSearch
    {
        public static T SearchTree<T>(Tree<T> tree, Func<T, bool> searchFunction)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            TWrapper<T> result = performSearch(tree, searchFunction, tokenSource);
            return result == null ? default(T) : result.Value;
        }

        class TWrapper<T>
        {
            public T Value;
        }

        private static TWrapper<T> performSearch<T>(Tree<T> tree, Func<T, bool> searchFunction, CancellationTokenSource tokenSource)
        {
            TWrapper<T> result = null;
            if (tree != null)
            {
                if (searchFunction(tree.Data))
                {
                    tokenSource.Cancel();
                    result = new TWrapper<T>() { Value = tree.Data };
                }
                else
                {
                    if (tree.LeftNode != null && tree.RightNode != null)
                    {
                        Task<TWrapper<T>> leftTask = Task<TWrapper<T>>.Factory.StartNew(() => performSearch(tree.LeftNode, searchFunction, tokenSource), tokenSource.Token);
                        Task<TWrapper<T>> rightTask = Task<TWrapper<T>>.Factory.StartNew(() => performSearch(tree.RightNode, searchFunction, tokenSource), tokenSource.Token);

                        try
                        {
                            result = leftTask.Result != null ? leftTask.Result : rightTask.Result != null ? rightTask.Result : null;
                        }
                        catch (AggregateException) { }
                    }
                }
            }
            return result;
        }
    }

    class TreeTraverser
    {
        public static void TraverseTree<T>(Tree<T> tree, Action<T> action)
        {
            if (tree != null)
            {
                action.Invoke(tree.Data);
                if (tree.LeftNode != null && tree.RightNode != null)
                {
                    Task leftTask = Task.Factory.StartNew(() => TraverseTree(tree.LeftNode, action));
                    Task RightTask = Task.Factory.StartNew(() => TraverseTree(tree.RightNode, action));
                    Task.WaitAll(leftTask, RightTask);
                }
            }
        }
    }

    class IntComparer : IComparer<int>
    {
        public int Compare(int first, int second)
        {
            return first.CompareTo(second);
        }
    }

    class Parallel_Sort<T>
    {
        public static void ParallelQuickSort(T[] data, IComparer<T> comparer, int maxDepth = 16, int minBlockSize = 10000)
        {
            doSort(data, 0, data.Length - 1, comparer, 0, maxDepth, minBlockSize);
        }

        internal static void doSort(T[] data, int startIndex, int endIndex, IComparer<T> comparer, int depth, int maxDepth, int minBlockSize)
        {
            if (startIndex < endIndex)
            {
                if (depth > maxDepth || endIndex - startIndex < minBlockSize)
                {
                    Array.Sort(data, startIndex, endIndex - startIndex + 1, comparer);
                }
                else
                {
                    int pivotIndex = partitionBlock(data, startIndex, endIndex, comparer);
                    Task leftTask = Task.Factory.StartNew(() =>
                    {
                        doSort(data, startIndex, pivotIndex - 1, comparer, depth + 1, maxDepth, minBlockSize);
                    });

                    Task rightTask = Task.Factory.StartNew(() =>
                    {
                        doSort(data, pivotIndex + 1, endIndex, comparer, depth + 1, maxDepth, minBlockSize);
                    });

                    Task.WaitAll(leftTask, rightTask);
                }
            }
        }

        internal static int partitionBlock(T[] data, int startIndex, int endIndex, IComparer<T> comparer)
        {
            T pivot = data[startIndex];
            swapValues(data, startIndex, endIndex);
            int storeIndex = startIndex;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (comparer.Compare(data[i], pivot) <= 0)
                {
                    swapValues(data, i, storeIndex);
                    storeIndex++;
                }
            }
            swapValues(data, storeIndex, endIndex);
            return storeIndex;
        }

        internal static void swapValues(T[] data, int firstIndex, int secondIndex)
        {
            T holder = data[firstIndex];
            data[firstIndex] = data[secondIndex];
            data[secondIndex] = holder;
        }
    }
}
