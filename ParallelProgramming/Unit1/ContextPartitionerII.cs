using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharedData
{
    class ContextPartitionerII : OrderablePartitioner<WorkItem>
    {
        protected WorkItem[] dataItems;
        protected int targetSum;
        private long sharedStartIndex = 0;
        private object lockObj = new object();
        private EnumerableSource enumSource;

        public ContextPartitionerII(WorkItem[] data, int target)
            : base(true, false, true)
        {
            dataItems = data;
            targetSum = target;
            enumSource = new EnumerableSource(this);
        }

        public override bool SupportsDynamicPartitions
        {
            get
            {
                return true;
            }
        }

        public override IList<IEnumerator<KeyValuePair<long, WorkItem>>> GetOrderablePartitions(int partitionCount)
        {
            IList<IEnumerator<KeyValuePair<long, WorkItem>>> partitionList = new List<IEnumerator<KeyValuePair<long, WorkItem>>>();
            IEnumerable<KeyValuePair<long, WorkItem>> enumObj = GetOrderableDynamicPartitions();

            for (int i = 0; i < partitionCount; i++)
            {
                partitionList.Add(enumObj.GetEnumerator());
            }
            return partitionList;
        }

        public override IEnumerable<KeyValuePair<long, WorkItem>> GetOrderableDynamicPartitions()
        {
            return enumSource;
        }

        public Tuple<long, long> getNextChunk()
        {
            Tuple<long, long> result;
            lock (lockObj)
            {
                if (sharedStartIndex < dataItems.Length)
                {
                    int sum = 0;
                    long endIndex = sharedStartIndex;
                    while (endIndex < dataItems.Length && sum < targetSum)
                    {
                        sum += dataItems[endIndex].WorkDuration;
                        endIndex++;
                    }
                    result = new Tuple<long, long>(sharedStartIndex, endIndex);
                    sharedStartIndex = endIndex;
                }
                else
                {
                    result = new Tuple<long, long>(-1, -1);
                }
            }
            return result;
        }

        class EnumerableSource : IEnumerable<KeyValuePair<long, WorkItem>>
        {
            ContextPartitionerII parentPartitioner;

            public EnumerableSource(ContextPartitionerII parent)
            {
                parentPartitioner = parent;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<WorkItem>)this).GetEnumerator();
            }

            IEnumerator<KeyValuePair<long, WorkItem>> IEnumerable<KeyValuePair<long, WorkItem>>.GetEnumerator()
            {
                return new ChunkEnumerator(parentPartitioner).GetEnumerator();
            }
        }

        class ChunkEnumerator
        {
            private ContextPartitionerII parentPartitioner;

            public ChunkEnumerator(ContextPartitionerII parent)
            {
                parentPartitioner = parent;
            }

            public IEnumerator<KeyValuePair<long, WorkItem>> GetEnumerator()
            {
                while (true)
                {
                    Tuple<long, long> chunkIndices = parentPartitioner.getNextChunk();
                    if (chunkIndices.Item1 == -1 && chunkIndices.Item2 == -1)
                    {
                        break;
                    }
                    else
                    {
                        for (long i = chunkIndices.Item1; i < chunkIndices.Item2; i++)
                        {
                            yield return new KeyValuePair<long, WorkItem>(i, parentPartitioner.dataItems[i]);
                        }
                    }
                }
            }
        }
    }
}
