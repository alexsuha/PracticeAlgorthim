using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedData
{
    class ContextPartitioner : Partitioner<WorkItem>
    {
        protected WorkItem[] dataItems;
        protected int targetSum;
        private long sharedStartIndex = 0;
        private object lockObj = new object();
        private EnumerableSource enumSource;

        public ContextPartitioner(WorkItem[] data, int target)
        {
            dataItems = data;
            targetSum = target;
            enumSource = new EnumerableSource(this);
        }

        public override bool SupportsDynamicPartitions
        {
            get { return true; }
        }

        public override IList<IEnumerator<WorkItem>> GetPartitions(int partitionCount)
        {
            IList<IEnumerator<WorkItem>> partitionsList = new List<IEnumerator<WorkItem>>();
            IEnumerable<WorkItem> enumObj = GetDynamicPartitions();
            for (int i = 0; i < partitionCount; i++)
            {
                partitionsList.Add(enumObj.GetEnumerator());
            }
            return partitionsList;
        }

        public override IEnumerable<WorkItem> GetDynamicPartitions()
        {
            return enumSource;
        }

        private Tuple<long, long> getNextChunk()
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
                return result;
            }
        }

        class EnumerableSource : IEnumerable<WorkItem>
        {
            ContextPartitioner parentPartitioner;

            public EnumerableSource(ContextPartitioner parent)
            {
                parentPartitioner = parent;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<WorkItem>)this).GetEnumerator();
            }
            IEnumerator<WorkItem> IEnumerable<WorkItem>.GetEnumerator()
            {
                return new ChunkEnumerator(parentPartitioner).GetEnumerator();
            }
        }

        class ChunkEnumerator
        {
            private ContextPartitioner parentPartitioner;

            public ChunkEnumerator(ContextPartitioner parent)
            {
                parentPartitioner = parent;
            }

            public IEnumerator<WorkItem> GetEnumerator()
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
                            yield return parentPartitioner.dataItems[i];
                        }
                    }
                }
            }
        }
    }
}
