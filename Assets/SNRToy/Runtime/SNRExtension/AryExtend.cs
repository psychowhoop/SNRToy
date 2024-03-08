using System;
using System.Collections.Generic;
using System.Linq;

namespace SNRAryExtend
{
    public static class AryExtend
    {
        public static bool ContainSameEleInAry<T>(this T[] selfAry, T[] destinationAry, bool useRefCheck = false)
        {
            if (useRefCheck)
            {
                return selfAry.Any(selfElement => destinationAry.Any(desElement => object.ReferenceEquals(selfElement, desElement)));
            }
            else
            {
                return selfAry.Any(selfElement => destinationAry.Any(desElement => EqualityComparer<T>.Default.Equals(selfElement, desElement)));
            }
        }


    }



}