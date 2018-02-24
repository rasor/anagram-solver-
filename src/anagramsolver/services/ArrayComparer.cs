using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.services
{
    /// <summary>
    /// http://www.java2s.com/Tutorials/CSharp/System.Collections.Generic/HashSet_T_/C_HashSet_T_HashSet_T_IEqualityComparer_T_.htm
    /// </summary>
    public class ArrayComparer : EqualityComparer<int[]>
    {
        /// <summary>
        /// https://stackoverflow.com/questions/486749/compare-two-net-array-objects
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public override bool Equals(int[] a, int[] b)
        {
            // Handle identity comparison, including comparing nulls
            if (a == b)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public override int GetHashCode(int[] n)
        {
            return base.GetHashCode();
        }
    }
}
