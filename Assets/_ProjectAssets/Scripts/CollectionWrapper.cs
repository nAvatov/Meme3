using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ProjectAssets.Scripts
{
    public static class CollectionWrapper
    {
        public static T[,] WrapListToTwoDimArray<T>(List<T> list, int columnsRestriction)
        {
            if (list.Count % columnsRestriction != 0)
            {
                Debug.Log("Trying to wrap potential jagged array");
                return null;
            }
            
            T[,] resultArray = new T[list.Count/columnsRestriction, columnsRestriction];
            int k = 0;

            for (int i = 0; i < list.Count / columnsRestriction; i++)
            {
                for (int j = 0; j < columnsRestriction; j++)
                {
                    resultArray[i, j] = list[k];
                    k++;
                }
            }

            return resultArray;
        }
    }
}