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
            Listing_03.OrphanedLock();
        }
    }
}
