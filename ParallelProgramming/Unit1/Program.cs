using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedData
{
    class Program
    {
        static void Main(string[] args)
        {
            Listing_01Demos();
            Listing_03Demos();
            Listing_04Demos();
            Listing_05Demos();
        }

        private static void Listing_05Demos()
        {
            //Listing_05.UsingParallelLoop();
            //Listing_05.PerformActionsUsingParallelInvoke();
            //Listing_05.UsingBasicParallelLoop();
            //Listing_05.CreateASteppedLoop();
            //Listing_05.SettingOptionsForAParallelLoop();
            //Listing_05.UsingStopInParallelLoop();
            //Listing_05.UsingBreakInAParallelLoop();
            //Listing_05.UsingParallelLoopResult();
            //Listing_05.CancelParallelLoops();
            //Listing_05.UsingLocalStorageInParallelLoop();
            Listing_05.UsingMixingSynchronousAndParallelLoops();
        }

        private static void Listing_04Demos()
        {
            //Listing_04.TaskContinuation();
            //Listing_04.SimpleChildTask();
            //Listing_04.UsingBarrierClass();
            //Listing_04.ReduceParticipation();
            //Listing_04.UsingCancellationDealingWithExceptions();
            //Listing_04.UsingCountDownEvent();
            //Listing_04.UsingAutoResetEvent();
            //Listing_04.UsingSemaphoreSlim();
            //Listing_04.UsingParallelProducerConsumer();
            //Listing_04.UsingMultipleBlockingCollection();
            //Listing_04.UsingCustomScheduler();
            //Listing_04.InconsistentCancellation();
            //Listing_04.AssumingWaitAnyStatus();
            //Listing_04.TryingToTakeConcurrently();
            //Listing_04.ReusingObjectsinProducers();
            //Listing_04.UsingBlockingCollectionAsIEnum();
            //Listing_04.TestDeadlockedTaskScheduler();
        }

        private static void Listing_01Demos()
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
            //Listing_01.TaskDependencyDeadlock();
            //Listing_01.LocalVariableEvaluation();
            //Listing_01.ExcessiveSpinning();
        }

        private static void Listing_03Demos()
        {
            //Listing_03.firstSharedData();
            //Listing_03.CreateImutableData();
            //Listing_03.ExecuteInIsolation();
            //Listing_03.ExecuteInIsolationWithTLS();
            //Listing_03.ExecuteInIsolationWithTLSFactory();
            //Listing_03.AcquiringMultipleLocks();
            //Listing_03.ConfiguringInterprocessSynchronization();
            //Listing_03.UsingDeclarativeSynchronization();
            //Listing_03.UsingSpinLock();
            //Listing_03.UsingReaderWriterLock();
            //Listing_03.UsingUpgradedReadWriteLock();
            //Listing_03.WorkingWithConcurrentCollections();
            //Listing_03.UsingConcurrentQueue();
            //Listing_03.UsingConcurrentDictionary();
            //Listing_03.UsingFirstGenerationCollections();
            //Listing_03.MisTakenImmutability();
            //Listing_03.UsingMultiLock();
            //Listing_03.LockAcquisitionOrder();
            //Listing_03.OrphanedLock();
        }
    }
}
