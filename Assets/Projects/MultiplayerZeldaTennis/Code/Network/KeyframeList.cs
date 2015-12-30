using System.Collections.Generic;
using System;

public class KeyframeList<T> 
{
    private static readonly string version = "v00-30-12-2015";
    // Internal class containing a double for time, and a T for data
    private class Pair: IComparable<Pair>
    {
        #region Interface
        private double keyTime;
        private T keyData;

        public double KeyTime { get { return keyTime; } set { keyTime = value; } }
        public T KeyData { get { return keyData; } set { keyData = value; } }

        #endregion

        public Pair(double time, T data)
        {
            KeyTime = time;
            keyData = data;
        }

        #region Operator overloads

        // overide Equals
        public override bool Equals(object obj)
        {
            try
            {
                return (bool)(this.keyTime.Equals(((Pair)obj).keyTime) && this.keyData.Equals(((Pair)obj).keyData));
            }
            catch 
            {
                return false;
            }
        }

        public static bool operator ==(Pair a, Pair b)
        {
            return (a.keyData.Equals(b.KeyData) && a.keyTime.Equals(b.keyTime));
        }

        public static bool operator !=(Pair a, Pair b)
        {
            return !(a == b);// I'm lazy, sue me
        }

        public int CompareTo(Pair obj)
        {
            return this.keyTime.CompareTo(obj.keyTime);
        }
        
        #endregion

        #region explicit conversions

        // Conversion from pair to Keyvaluepair
        public static explicit operator KeyValuePair<double,T>(Pair p)
        {
            return new KeyValuePair<double, T>(p.keyTime, p.keyData);
        }
        // Conversion from keuvaluepair to Pair
        public static explicit operator Pair( KeyValuePair<double,T> kvPair)
        {
            return new Pair(kvPair.Key, kvPair.Value);
        }

        //Conversion from pair to string
        public static explicit operator string(Pair p)
        {
            return p.ToString();
        }

        #endregion

        public override string ToString()
        {
            return "Keyframe: " + this.keyTime + ", " + KeyData.ToString();
        }

        public override int GetHashCode()
        {
            // http://stackoverflow.com/questions/371328/why-is-it-important-to-override-gethashcode-when-equals-method-is-overridden
            // Apparently, to prevent hash collisions , prime numbers are used in the multiplication of hashes. I don't see a reason to use any other

            int hash = 13;
            hash = (hash * 7) + keyTime.GetHashCode();
            hash = (hash * 7) + keyData.GetHashCode();

            return hash;
        }
        
    }

    // Contains list
    private List<Pair> m_KeyframeList;

    //C-tor
    public KeyframeList()
    {
        m_AutoSort = true;
        m_KeyframeList = new List<Pair>();
    }

    private bool m_AutoSort;
    public bool Autosort
    {
        get { return m_AutoSort; }
        set
        {
            if (value == true)
                Sort();
            m_AutoSort = value;
        }
    }

    public int Count
    {
        get { return m_KeyframeList.Count; }
    }

    public void Sort()
    {
        m_KeyframeList.Sort();
    }

    private void Do_AutoSort()
    {
        if (m_AutoSort)
            Sort();
    }

    /// <summary>
    /// Adds item to list
    /// </summary>
    /// <param name="time"></param>
    /// <param name="data"></param>
    public void Add( double time, T data)
    {
        Pair newPair = new Pair(time, data);
        m_KeyframeList.Add(newPair);

        Do_AutoSort();
    }
    public void Add( KeyValuePair<double,T> pair)
    {
        m_KeyframeList.Add( (Pair)pair);
        Do_AutoSort();
    }


    /// <summary>
    /// Removes item from list
    /// </summary>
    /// <param name="time"></param>
    /// <param name="data"></param>
    public void Remove( double time, T data)
    {
        m_KeyframeList.Remove(new Pair(time, data));
        Do_AutoSort();
    }
    
    public int GetIndexFirstBefore(double time)
    {
        Do_AutoSort();


        for (int i = 0; i < m_KeyframeList.Count; i++)
        {
            // Compare Stored time vs Sought after time -> -1 == Stored time is BEFORE saught time
            if (m_KeyframeList[i].KeyTime.CompareTo(time) == 1 || m_KeyframeList[i].KeyTime.CompareTo(time) == 0)
            {
                return i -1;
            }
        }
        return -1;

        throw new NotImplementedException();
    }
    public int GetIndexClosestTo(double time)
    {
        int closestID = -1;
        double previousDelta = double.MaxValue;

        for (int i = 0; i < m_KeyframeList.Count; i++)
        {
            double delta = Math.Abs(m_KeyframeList[i].KeyTime - time);
            int comparisson = previousDelta.CompareTo(delta);
            if( comparisson == 1)
            {
                closestID = i;
                previousDelta = delta;
            }
                

            
        }
        return closestID;
    }
    public int GetIndexFirstAfter(double time)
    {
        Do_AutoSort();

        for (int i = 0; i < m_KeyframeList.Count; i++)
        {
            if (m_KeyframeList[i].KeyTime.CompareTo(time) == 1 )
            {
                return i;
            }
        }
        return -1;
    }

    public KeyframeList<T> FindAll(double time)
    {
        KeyframeList<T> result = new KeyframeList<T>();
        for (int i = 0; i < m_KeyframeList.Count; i++)
        {
            if (m_KeyframeList[i].KeyTime.CompareTo(time) == 0)
                result.Add( (KeyValuePair<double,T>) m_KeyframeList[i]);
        }
        return result;
    }
    public KeyframeList<T> FindAll(T data)
    {
        KeyframeList<T> result = new KeyframeList<T>();
        
        for (int i = 0; i < m_KeyframeList.Count; i++)
        {
            if (m_KeyframeList[i].KeyData.Equals(data))
                result.Add((KeyValuePair<double, T>) m_KeyframeList[i]);
        }
        return result;
    }

    public KeyframeList<T> Range(int index, int count)
    {
        KeyframeList<T> newRange = new KeyframeList<T>();

        //List<KeyValuePair<double, T>> range = new List<KeyValuePair<double, T>>();
        for (int i = index; i < index + count; i++)
        {
            newRange.Add(m_KeyframeList[(int)i].KeyTime, m_KeyframeList[(int)i].KeyData);
          //  range.Add( ( KeyValuePair<double, T>) this[i] );
        }
        return newRange;
        //return range;
    }

    // Committing some sins here

        // Array overload using indices
    public KeyValuePair<double,T> this[ int i]
    {
        get { return new KeyValuePair<double, T>(m_KeyframeList[i].KeyTime, m_KeyframeList[i].KeyData); }
        set
        {
            this.m_KeyframeList[i].KeyTime = value.Key;
            this.m_KeyframeList[i].KeyData = value.Value;

            Do_AutoSort();       
        }
    }
    
        // Array overload using timeStamps
    public T this[double d]
    {
        get
        {
            T foundData = m_KeyframeList.Find(delegate (Pair l ) { return l.KeyTime.Equals( d);}).KeyData;
            return foundData;
        }
        set
        {
            int foundIndex = m_KeyframeList.FindIndex(delegate (Pair p) { return p.KeyTime.Equals(d); });
            m_KeyframeList[foundIndex] = new Pair(d, value );
        }
    }

    public override string ToString()
    {
        string outString = "Keyframelist: [";

        for (int i = 0; i < m_KeyframeList.Count; i++)
        {
            outString = outString + " ( " + m_KeyframeList[i].ToString() + " )";
        }

        return outString + "]";
    }


}
