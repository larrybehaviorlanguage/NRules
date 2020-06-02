using System.Collections.Generic;
namespace NRules.Utilities
{
    public class BoolObj
    {
        private readonly static Stack<BoolObj> Pool = new Stack<BoolObj>();

        private readonly static object PoolLock = new object();


        public static BoolObj GetBoolObj(bool value)
        {
            BoolObj boolObj = null;
            System.Diagnostics.Debug.WriteLine("boolobj");

            // Pool needs to be locked since it can be accessed by multiple threads
            lock (PoolLock)
            {
                if (Pool.Count > 0)
                {
                    boolObj = Pool.Pop();
                }
            }

            if (boolObj == null)
            {

                boolObj = new BoolObj();
            }

            boolObj.value = value;

            return boolObj;
        }

        private bool value;

        private BoolObj() { }

        public bool GetValueAndReturnToPool()
        {
            // Cache the value to prevent concurrent alteration after returning to the pool
            bool valueToReturn = value;

            // Pool needs to be locked since it can be accessed by multiple threads
            lock (PoolLock)
            {
                Pool.Push(this);
            }

            return valueToReturn;
        }

        public override string ToString()
        {
            return "BoolObj: " + value;
        }
    }
}
