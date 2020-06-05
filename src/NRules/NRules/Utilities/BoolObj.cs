// Comment in and set a DiagnosticLogPath to enable diagnostic logging
// #define BOOL_OBJ_DIAGNOSTIC

using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;


namespace NRules.Utilities
{
    public class BoolObj
    {
        private static int IDCtr = 0;
        private readonly static Stack<BoolObj> Pool = new Stack<BoolObj>();

        private readonly static object PoolLock = new object();

        private readonly int ID;

#if BOOL_OBJ_DIAGNOSTIC

        // To use diagnostic log, enable the #define BOOL_OBJ_DIAGNOSTIC above, and provide a log path
        private readonly static object LogFileLock = new object();
        private readonly static string DiagnosticLogPath;
        private readonly static StreamWriter diagnosticWriter;


        static BoolObj()
        {
            lock (LogFileLock)
            {
                if (!string.IsNullOrEmpty(DiagnosticLogPath))
                {
                    diagnosticWriter = File.AppendText(DiagnosticLogPath);
                }
                DiagnosticLog("BoolObj Static constructor");
            }
        }
#endif // BOOL_OBJ_DIAGNOSTIC

        [Conditional("BOOL_OBJ_DIAGNOSTIC")]
        public static void DiagnosticLog(string msg)
        {
#if BOOL_OBJ_DIAGNOSTIC
            lock (LogFileLock)
            {
                diagnosticWriter?.WriteLine("[" + DateTime.Now.ToString("mm:ss:sss") + "] " + msg);
                diagnosticWriter?.Flush();
            }
#endif // BOOL_OBJ_DIAGNOSTIC
        }

        public static BoolObj GetBoolObj(bool Value)
        {
            BoolObj boolObj = null;

            // Pool needs to be locked since it can be accessed by multiple threads
            lock (PoolLock)
            {
                if (Pool.Count > 0)
                {
                    boolObj = Pool.Pop();
                    boolObj.inPool = false;
                }
            }

            if (boolObj == null)
            {
                boolObj = new BoolObj();
                DiagnosticLog("\nConstruct BoolObj " + boolObj.ID);
            }
            else
            {
                DiagnosticLog("Pop BoolObj " + boolObj.ID);
            }


            boolObj.Value = Value;

            DiagnosticLog("GetBoolObj " + boolObj);

            return boolObj;
        }

        public bool Value { get; set; }
        private volatile bool inPool;

        private BoolObj() {
            this.ID = IDCtr++;
        }

        public bool GetValueAndReturnToPool()
        {
            // Cache the Value to prevent concurrent alteration after returning to the pool
            bool ValueToReturn = Value;

            if (inPool)
            {
                DiagnosticLog("BoolObj Value accessed while in pool: " + this);
                throw new InvalidOperationException("BoolObjValue accessed while in pool " + this);
            }

            // Pool needs to be locked since it can be accessed by multiple threads
            lock (PoolLock)
            {
                Pool.Push(this);
                inPool = true;
            }

            DiagnosticLog("Push BoolObj " + this + " and return Value " + ValueToReturn);

            return ValueToReturn;
        }

        public override string ToString()
        {
            return string.Format("BoolObj [ID {0}] [Value {1}]", ID, Value);
        }
    }
}
