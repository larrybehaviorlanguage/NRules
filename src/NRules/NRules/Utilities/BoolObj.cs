using System.Collections.Generic;
using System.Text;
namespace NRules.Utilities
{
    public class BoolObj
    {
        private static int IDCtr = 0;
        private readonly static Stack<BoolObj> Pool = new Stack<BoolObj>();

        private readonly static object PoolLock = new object();

        private static readonly StringBuilder diagnosticBuilder = new StringBuilder("\n\nBoolObjDiagnostic");
        public static string Diagnostic => diagnosticBuilder.ToString();

        private readonly int ID;

        public static void ClearDiagnostic()
        {
            diagnosticBuilder.Clear();
        }

        public static BoolObj GetBoolObj(bool value)
        {
            BoolObj boolObj = null;

            // Pool needs to be locked since it can be accessed by multiple threads
            lock (PoolLock)
            {
                if (Pool.Count > 0)
                {
                    boolObj = Pool.Pop();
                    diagnosticBuilder.AppendLine("\nPop BoolObj " + boolObj.ID);
                }
            }

            if (boolObj == null)
            {

                boolObj = new BoolObj();
                diagnosticBuilder.AppendLine("\nConstruct BoolObj " + boolObj.ID);
            }


            boolObj.value = value;

            diagnosticBuilder.AppendLine("GetBoolObj " + boolObj);

            return boolObj;
        }

        private bool value;

        private BoolObj() {
            this.ID = IDCtr++;
        }

        public bool GetValueAndReturnToPool()
        {
            // Cache the value to prevent concurrent alteration after returning to the pool
            bool valueToReturn = value;

            // Pool needs to be locked since it can be accessed by multiple threads
            lock (PoolLock)
            {
                Pool.Push(this);
                diagnosticBuilder.AppendLine("Push BoolObj " + this + " and return value " + valueToReturn);
            }

            return valueToReturn;
        }

        public override string ToString()
        {
            return string.Format("BoolObj [ID {0}] [Value {1}]", ID, value);
        }
    }
}
