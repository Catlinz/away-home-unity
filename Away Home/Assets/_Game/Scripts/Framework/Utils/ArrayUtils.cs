using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHArray
{
    /// <summary>
    /// Returns a new array with the specified index removed.  If 
    /// the index doesn't exist in the array, throws an error.
    /// </summary>
    /// <typeparam name="T">The type of element in the array.</typeparam>
    /// <param name="src">The array to remove the element from.</param>
    /// <param name="index">The index to remove from the array.</param>
    /// <returns>A new array with `index` removed from it.</returns>
    public static T[] Removed<T>(T[] src, int index) {
        // Check to make sure we have valid arguments.
        if (src == null) {
            throw (new System.ArgumentNullException("src"));
        }
        if (index < 0 || index >= src.Length) {
            throw (new System.ArgumentOutOfRangeException("index"));
        }

        // Create a new array to hold everything and copy it all over.
        T[] arr = new T[src.Length - 1];
        
        // Copy first chunch of array.
        for (int i = 0; i < index; ++i) {
            arr[i] = src[i];
        }

        // Copy last chunk of array.
        for (int i = index + 1; i < src.Length; ++i) {
            arr[i - 1] = src[i];
        }

        return arr;
    }

    /// <summary>
    /// Returns a new array that is a copy of the source array with the new element added onto the end.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="src">The array to add the element to.</param>
    /// <param name="element">The element to add to the array.</param>
    /// <returns>The new array with the element added.</returns>
    public static T[] Added<T>(T[] src, T element) {
        T[] arr = null;

        // If src is null, then just create length 1 array and add element.
        if (src == null) {
            arr = new T[1];
            arr[0] = element;
        }
        else {
            // Otherwise, copy src to new array then add element to the end.
            arr = new T[src.Length + 1];
            src.CopyTo(arr, 0);
            arr[src.Length] = element;
        }

        // return the new array.
        return arr;
    }

    /// <summary>
    /// Returns a new array that is a copy of the source array with the new element 
    /// inserted into it at the specified index.
    /// </summary>
    /// <typeparam name="T">The type of element in the array.</typeparam>
    /// <param name="src">The array to insert the element into.</param>
    /// <param name="index">The index to insert the element into.</param>
    /// <param name="element">The element to insert into the array.</param>
    /// <returns>The new array with the element inserted.</returns>
    public static T[] Added<T>(T[] src, int index, T element) {
        T[] arr = null;

        // If src is null, then just create length 1 array and add element.
        if (src == null) {
            arr = new T[1];
            arr[0] = element;
        }
        else {
            if (index < 0 || index > src.Length) {
                throw (new System.ArgumentOutOfRangeException("index"));
            }
            // Otherwise, copy the two halves of the src array, separated by index.
            arr = new T[src.Length + 1];
            for (int i = 0; i < index; ++i) {
                arr[i] = src[i];
            }
            arr[index] = element;
            for (int i = 0; i < src.Length; ++i) {
                arr[i + 1] = src[i];
            }
        }

        // Return the new array.
        return arr;
    }


}
