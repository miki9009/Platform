namespace Engine
{
    public class ConcurrentInt
    {
        private static int hashcode = 0;
        private int myhashcode = 0;
        private int val;
        private readonly object safelock;

        public int Value
        {
            get
            {
                lock (safelock)
                {
                    return val;
                }
            }

            set
            {
                lock (safelock)
                {
                    val = value;
                }
            }
        }

        private int GetMyHashCode
        {
            get
            {
                lock (safelock)
                {
                    return myhashcode;
                }
            }
        }


        public ConcurrentInt()
        {
            safelock = new object();
            hashcode++;
            myhashcode = hashcode;
        }

        public static implicit operator int(ConcurrentInt val1)
        {
            return val1.Value;
        }

        public static implicit operator ConcurrentInt(int val1)
        {
            return new ConcurrentInt()
            {
                Value = val1
            };
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == typeof(int))
            {
                if (Value == (int)obj)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (obj.GetType() == typeof(ConcurrentInt))
            {
                if (Value == ((ConcurrentInt)obj).Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static bool operator ==(ConcurrentInt val1, ConcurrentInt val2)
        {
            try
            {
                return val1.Value == val2.Value ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public static bool operator !=(ConcurrentInt val1, ConcurrentInt val2)
        {
            try
            {
                return val1.Value != val2.Value ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public static bool operator ==(int val1, ConcurrentInt val2)
        {
            try
            {
                return val1 == val2.Value ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public static bool operator !=(int val1, ConcurrentInt val2)
        {
            try
            {
                return val1 == val2.Value ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return GetMyHashCode;
        }
    }
}
